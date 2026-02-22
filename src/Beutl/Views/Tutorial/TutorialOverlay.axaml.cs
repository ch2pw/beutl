using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

using Beutl.Language;
using Beutl.Services;
using Beutl.Services.Tutorials;
using Beutl.ViewModels;

namespace Beutl.Views.Tutorial;

public partial class TutorialOverlay : UserControl
{
    private Control? _currentTarget;

    public TutorialOverlay()
    {
        InitializeComponent();
        IsVisible = false;
        OverlayBackdrop.PointerPressed += OnBackdropPointerPressed;
    }

    public void UpdateState(TutorialState? state)
    {
        if (state == null)
        {
            IsVisible = false;
            ClearHighlight();
            return;
        }

        IsVisible = true;
        TutorialStep step = state.CurrentStep;

        TipTitle.Text = step.Title;
        TipContent.Text = step.Content;
        StepCounter.Text = string.Format(TutorialStrings.TutorialStep, state.CurrentStepIndex + 1, state.TotalSteps);

        PreviousButton.IsVisible = !state.IsFirstStep;
        NextButton.IsVisible = !step.IsActionRequired || state.IsLastStep;
        NextButton.Content = state.IsLastStep ? TutorialStrings.TutorialFinish : TutorialStrings.TutorialNext;

        UpdateTargetHighlight(step);
    }

    private void UpdateTargetHighlight(TutorialStep step)
    {
        _currentTarget = ResolveTargetElement(step);

        if (_currentTarget != null)
        {
            PositionHighlight(_currentTarget);
            PositionTip(_currentTarget, step.PreferredPlacement);
        }
        else
        {
            ClearHighlight();
            // ターゲットがない場合は中央に表示
            TipContainer.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            TipContainer.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        }
    }

    private Control? ResolveTargetElement(TutorialStep step)
    {
        if (step.TargetElementResolver != null)
        {
            return step.TargetElementResolver() as Control;
        }

        if (step.TargetToolTabType != null)
        {
            return ResolveToolTabElement(step.TargetToolTabType);
        }

        if (step.TargetElementName != null)
        {
            // ビジュアルツリーを走査して Name が一致するコントロールを探す
            TopLevel? topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                return topLevel.GetVisualDescendants()
                    .OfType<Control>()
                    .FirstOrDefault(c => c.Name == step.TargetElementName);
            }
        }

        return null;
    }

    private Control? ResolveToolTabElement(Type extensionType)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
            return null;

        return topLevel.GetVisualDescendants()
            .OfType<ToolTabContent>()
            .FirstOrDefault(ttc =>
                ttc.DataContext is ToolTabViewModel vm &&
                extensionType.IsInstanceOfType(vm.Context.Extension));
    }

    private void PositionHighlight(Control target)
    {
        try
        {
            Point? pos = target.TranslatePoint(new Point(0, 0), this);
            if (pos.HasValue)
            {
                HighlightBorder.IsVisible = true;
                Canvas.SetLeft(HighlightBorder, pos.Value.X - 4);
                Canvas.SetTop(HighlightBorder, pos.Value.Y - 4);
                HighlightBorder.Width = target.Bounds.Width + 8;
                HighlightBorder.Height = target.Bounds.Height + 8;

                // オーバーレイ背景にクリッピングを適用
                UpdateOverlayClip(pos.Value, target.Bounds.Size);
            }
        }
        catch
        {
            ClearHighlight();
        }
    }

    private void UpdateOverlayClip(Point targetPos, Size targetSize)
    {
        // Boundsが0の場合はDispatcherで遅延させる（まだレイアウトが完了していない可能性があるため）
        if (Bounds.Width == 0 || Bounds.Height == 0)
        {
            Dispatcher.UIThread.Post(() => UpdateOverlayClip(targetPos, targetSize), DispatcherPriority.Render);
            return;
        }

        var fullRect = new RectangleGeometry(new Rect(0, 0, Bounds.Width, Bounds.Height));
        var cutout = new RectangleGeometry(new Rect(
            targetPos.X - 4, targetPos.Y - 4,
            targetSize.Width + 8, targetSize.Height + 8));

        OverlayBackdrop.Clip = new CombinedGeometry(
            GeometryCombineMode.Exclude, fullRect, cutout);
    }

    private void PositionTip(Control target, TutorialStepPlacement placement)
    {
        try
        {
            Point? pos = target.TranslatePoint(new Point(0, 0), this);
            if (!pos.HasValue)
                return;

            double targetLeft = pos.Value.X;
            double targetRight = pos.Value.X + target.Bounds.Width;
            double targetCenterX = pos.Value.X + target.Bounds.Width / 2;
            double targetCenterY = pos.Value.Y + target.Bounds.Height / 2;
            double targetBottom = pos.Value.Y + target.Bounds.Height;
            double targetTop = pos.Value.Y;

            TipContainer.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            TipContainer.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;

            double tipLeft = Math.Max(16, targetCenterX - 200);
            double tipTop;

            if (placement == TutorialStepPlacement.Top)
            {
                tipTop = targetTop - 16;
                TipContainer.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom;
                TipContainer.Margin = new Thickness(tipLeft, 0, 0, tipTop);
            }
            else if (placement == TutorialStepPlacement.Left)
            {
                tipLeft = target.Bounds.Width - 16;
                tipTop = targetCenterY;
                TipContainer.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
                TipContainer.Margin = new Thickness(0, tipTop, tipLeft, 0);
            }
            else if (placement == TutorialStepPlacement.Right)
            {
                tipLeft = targetRight + 16;
                tipTop = targetCenterY;
                TipContainer.Margin = new Thickness(tipLeft, tipTop, 0, 0);
            }
            else if (placement == TutorialStepPlacement.Bottom)
            {
                tipTop = targetBottom + 16;
                TipContainer.Margin = new Thickness(tipLeft, tipTop, 0, 0);
            }
        }
        catch
        {
            TipContainer.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            TipContainer.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            TipContainer.Margin = default;
        }
    }

    private void ClearHighlight()
    {
        HighlightBorder.IsVisible = false;
        OverlayBackdrop.Clip = null;
        TipContainer.Margin = default;
        _currentTarget = null;
    }

    private void OnBackdropPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // 背景クリックで何もしない（イベントを消費してオーバーレイの下にクリックが通らないようにする）
        e.Handled = true;
    }

    private void OnSkipClick(object? sender, RoutedEventArgs e)
    {
        TutorialService.Current?.CancelTutorial();
    }

    private void OnPreviousClick(object? sender, RoutedEventArgs e)
    {
        TutorialService.Current?.PreviousStep();
    }

    private void OnNextClick(object? sender, RoutedEventArgs e)
    {
        TutorialState? state = (TutorialService.Current as TutorialServiceHandler)?.GetCurrentState();
        if (state?.IsLastStep == true)
        {
            TutorialService.Current?.CancelTutorial();
        }
        else
        {
            TutorialService.Current?.AdvanceStep();
        }
    }
}
