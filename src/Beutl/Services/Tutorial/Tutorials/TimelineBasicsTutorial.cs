using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Beutl.Animation;
using Beutl.Controls.PropertyEditors;
using Beutl.Editor.Components.SourceOperatorsTab;
using Beutl.Engine;
using Beutl.Graphics.Shapes;
using Beutl.Language;
using Beutl.Operation;
using Beutl.Operators.Source;
using Beutl.ProjectSystem;
using Beutl.Services.PrimitiveImpls;
using Beutl.Services.Tutorials;
using Beutl.ViewModels;
using Beutl.ViewModels.Editors;

namespace Beutl.Services.Tutorials;

public static class TimelineBasicsTutorial
{
    public const string TutorialId = "timeline-basics";

    private static EditViewModel? GetEditViewModel()
    {
        return EditorService.Current.SelectedTabItem.Value?.Context.Value as EditViewModel;
    }

    private static TopLevel? GetTopLevel()
    {
        if (Application.Current?.ApplicationLifetime
            is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }

    public static TutorialDefinition Create()
    {
        IDisposable? step1Subscription = null;
        IDisposable? step2Subscription = null;
        IDisposable? step3Subscription = null;
        IDisposable? step4Subscription = null;

        return new TutorialDefinition
        {
            Id = TutorialId,
            Title = TutorialStrings.Tutorial_SceneEdit_Title,
            Description = TutorialStrings.Tutorial_SceneEdit_Description,
            Trigger = TutorialTrigger.FirstSceneOpen,
            Priority = 10,
            Category = "basics",
            Steps =
            [
                // Step 1: Add an ellipse to the timeline
                new TutorialStep
                {
                    Id = "scene-add-ellipse",
                    Title = TutorialStrings.Tutorial_SceneEdit_Step1_Title,
                    Content = TutorialStrings.Tutorial_SceneEdit_Step1_Content,
                    TargetToolTabType = typeof(TimelineTabExtension),
                    PreferredPlacement = TutorialStepPlacement.Top,
                    IsActionRequired = true,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        // Already has an ellipse element?
                        if (HasEllipseElement(editVm.Scene))
                        {
                            Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                            return;
                        }

                        Action<Element>? handler = null;
                        handler = element =>
                        {
                            if (element.Operation.Children.OfType<EllipseOperator>().Any())
                            {
                                Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                            }
                        };

                        editVm.Scene.Children.Attached += handler;
                        step1Subscription = Disposable.Create(() => editVm.Scene.Children.Attached -= handler);
                    },
                    OnDismissed = () =>
                    {
                        step1Subscription?.Dispose();
                        step1Subscription = null;
                    },
                },

                // Step 2: Select the element in the timeline
                new TutorialStep
                {
                    Id = "scene-select-element",
                    Title = TutorialStrings.Tutorial_SceneEdit_Step2_Title,
                    Content = TutorialStrings.Tutorial_SceneEdit_Step2_Content,
                    TargetToolTabType = typeof(TimelineTabExtension),
                    PreferredPlacement = TutorialStepPlacement.Top,
                    IsActionRequired = true,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        // Already selected?
                        if (editVm.SelectedObject.Value != null)
                        {
                            Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                            return;
                        }

                        IDisposable? sub = null;
                        sub = editVm.SelectedObject.Subscribe(obj =>
                        {
                            if (obj != null)
                            {
                                Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                                sub?.Dispose();
                            }
                        });

                        step2Subscription = sub;
                    },
                    OnDismissed = () =>
                    {
                        step2Subscription?.Dispose();
                        step2Subscription = null;
                    },
                },

                // Step 3: Introduce the Source Operators tab
                new TutorialStep
                {
                    Id = "scene-source-operators",
                    Title = TutorialStrings.Tutorial_SceneEdit_Step3_Title,
                    Content = TutorialStrings.Tutorial_SceneEdit_Step3_Content,
                    TargetToolTabType = typeof(SourceOperatorsTabExtension),
                    PreferredPlacement = TutorialStepPlacement.Left,
                },

                // Step 4: Highlight Width property and prompt to enable animation
                new TutorialStep
                {
                    Id = "scene-enable-animation",
                    Title = TutorialStrings.Tutorial_SceneEdit_Step4_Title,
                    Content = TutorialStrings.Tutorial_SceneEdit_Step4_Content,
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                    IsActionRequired = true,
                    TargetElementResolver = () =>
                    {
                        TopLevel? topLevel = GetTopLevel();
                        return topLevel?.GetVisualDescendants()
                            .OfType<NumberEditor<float>>()
                            .FirstOrDefault(c =>
                                c.DataContext is BaseEditorViewModel vm &&
                                vm.PropertyAdapter.GetEngineProperty() is IProperty prop &&
                                prop.GetOwnerObject() is EllipseShape &&
                                prop.Name == nameof(Shape.Width));
                    },
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        Element? element = editVm.Scene.Children
                            .FirstOrDefault(e => e.Operation.Children.OfType<EllipseOperator>().Any());
                        if (element == null) return;

                        EllipseOperator? ellipseOp =
                            element.Operation.Children.OfType<EllipseOperator>().FirstOrDefault();
                        if (ellipseOp == null) return;

                        IProperty<float> widthProp = ellipseOp.Value.Width;
                        if (widthProp.Animation != null)
                        {
                            Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                        }
                        else if (widthProp is AnimatableProperty<float> animatableProp)
                        {
                            void Handler(IAnimation<float>? anm)
                            {
                                if (anm != null)
                                {
                                    Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                                }
                            }

                            animatableProp.AnimationChanged += Handler;
                            step3Subscription = Disposable.Create(() => animatableProp.AnimationChanged -= Handler);
                        }
                    },
                    OnDismissed = () =>
                    {
                        step3Subscription?.Dispose();
                        step3Subscription = null;
                    },
                },

                // Step 5: Move current time and prompt to add a keyframe
                new TutorialStep
                {
                    Id = "scene-add-keyframe",
                    Title = TutorialStrings.Tutorial_SceneEdit_Step5_Title,
                    Content = TutorialStrings.Tutorial_SceneEdit_Step5_Content,
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                    IsActionRequired = true,
                    TargetElementResolver = () =>
                    {
                        TopLevel? topLevel = GetTopLevel();
                        return topLevel?.GetVisualDescendants()
                            .OfType<NumberEditor<float>>()
                            .FirstOrDefault(c =>
                                c.DataContext is BaseEditorViewModel vm &&
                                vm.PropertyAdapter.GetEngineProperty() is IProperty prop &&
                                prop.GetOwnerObject() is EllipseShape &&
                                prop.Name == nameof(Shape.Width));
                    },
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        Element? element = editVm.Scene.Children
                            .FirstOrDefault(e => e.Operation.Children.OfType<EllipseOperator>().Any());
                        if (element == null) return;

                        EllipseOperator? ellipseOp =
                            element.Operation.Children.OfType<EllipseOperator>().FirstOrDefault();
                        if (ellipseOp == null) return;

                        IProperty<float> widthProp = ellipseOp.Value.Width;
                        if (widthProp.Animation is not KeyFrameAnimation<float> animation) return;

                        // すでにキーフレームが2つ以上ある場合は現在時間を変更し、次のステップに進む
                        if (animation.KeyFrames.Count >= 2)
                        {
                            editVm.CurrentTime.Value = animation.KeyFrames[1].KeyTime + element.Start;
                        }
                        else
                        {
                            editVm.CurrentTime.Value = element.Start + TimeSpan.FromSeconds(2);
                            // キーフレームの追加を監視して、2つ以上になったら次のステップに進む
                            void Handler(IKeyFrame _)
                            {
                                if (animation.KeyFrames.Count >= 2)
                                {
                                    Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                                }
                            }

                            animation.KeyFrames.Attached += Handler;
                            step4Subscription = Disposable.Create(() => animation.KeyFrames.Attached -= Handler);
                        }
                    },
                    OnDismissed = () =>
                    {
                        step4Subscription?.Dispose();
                        step4Subscription = null;
                    },
                },

                // Step 6: Change the value (auto-change if default)
                new TutorialStep
                {
                    Id = "scene-change-value",
                    Title = TutorialStrings.Tutorial_SceneEdit_Step6_Title,
                    Content = TutorialStrings.Tutorial_SceneEdit_Step6_Content,
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                    TargetElementResolver = () =>
                    {
                        TopLevel? topLevel = GetTopLevel();
                        return topLevel?.GetVisualDescendants()
                            .OfType<NumberEditor<float>>()
                            .FirstOrDefault(c =>
                                c.DataContext is BaseEditorViewModel vm &&
                                vm.PropertyAdapter.GetEngineProperty() is IProperty prop &&
                                prop.GetOwnerObject() is EllipseShape &&
                                prop.Name == nameof(Shape.Width));
                    },
                    OnDismissed = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        Element? element = editVm.Scene.Children
                            .FirstOrDefault(e => e.Operation.Children.OfType<EllipseOperator>().Any());
                        if (element == null) return;

                        EllipseOperator? ellipseOp =
                            element.Operation.Children.OfType<EllipseOperator>().FirstOrDefault();
                        if (ellipseOp is PublishOperator<EllipseShape> publishOp)
                        {
                            IProperty<float> widthProp = publishOp.Value.Width;
                            if (widthProp.Animation is KeyFrameAnimation<float> animation
                                && animation.KeyFrames.LastOrDefault() is { Value: 100f or <= 100f } lastKeyFrame)
                            {
                                lastKeyFrame.Value = 300f;
                            }
                        }
                    }
                },

                // Step 7: Highlight the Player and prompt to play
                new TutorialStep
                {
                    Id = "scene-preview-animation",
                    Title = TutorialStrings.Tutorial_SceneEdit_Step7_Title,
                    Content = TutorialStrings.Tutorial_SceneEdit_Step7_Content,
                    TargetElementName = "Player",
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        Element? element = editVm.Scene.Children
                            .FirstOrDefault(e => e.Operation.Children.OfType<EllipseOperator>().Any());
                        if (element != null)
                        {
                            editVm.Scene.Duration = element.Range.Duration + TimeSpan.FromSeconds(1);
                            editVm.CurrentTime.Value = element.Start;
                        }
                    },
                },

                // Step 8: Completion
                new TutorialStep
                {
                    Id = "scene-complete",
                    Title = TutorialStrings.Tutorial_SceneEdit_Step8_Title,
                    Content = TutorialStrings.Tutorial_SceneEdit_Step8_Content,
                    PreferredPlacement = TutorialStepPlacement.Center,
                },
            ]
        };
    }

    private static bool HasEllipseElement(Scene scene)
    {
        return scene.Children.Any(e => e.Operation.Children.OfType<EllipseOperator>().Any());
    }

    private static class Disposable
    {
        public static IDisposable Create(Action dispose) => new ActionDisposable(dispose);

        private sealed class ActionDisposable(Action dispose) : IDisposable
        {
            public void Dispose() => dispose();
        }
    }
}
