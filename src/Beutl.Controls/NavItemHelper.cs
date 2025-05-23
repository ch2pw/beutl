﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

using FluentAvalonia.UI.Controls;

namespace Beutl.Controls;

public class NavItemHelper : Behavior<NavigationViewItem>
{
    public static readonly StyledProperty<IconSource> RegularIconProperty
        = AvaloniaProperty.Register<NavItemHelper, IconSource>("RegularIcon");

    public static readonly StyledProperty<IconSource> FilledIconProperty
        = AvaloniaProperty.Register<NavItemHelper, IconSource>("FilledIcon");
    private IDisposable _disposable;
    private IconSourceElement _regular;
    private IconSourceElement _filled;

    public IconSource RegularIcon
    {
        get => GetValue(RegularIconProperty);
        set => SetValue(RegularIconProperty, value);
    }

    public IconSource FilledIcon
    {
        get => GetValue(FilledIconProperty);
        set => SetValue(FilledIconProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        SetFontSize(RegularIcon);
        SetFontSize(FilledIcon);
        _disposable = AssociatedObject.GetPropertyChangedObservable(ListBoxItem.IsSelectedProperty)
            .Subscribe(e => SelectionChanged((NavigationViewItem)e.Sender));

        SelectionChanged(AssociatedObject);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        _regular = null;
        _filled = null;
        _disposable?.Dispose();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property.Name is nameof(RegularIcon) or nameof(FilledIcon))
        {
            if (_regular != null)
            {
                SetFontSize(RegularIcon);
                _regular.IconSource = RegularIcon;
            }

            if (_filled != null)
            {
                SetFontSize(FilledIcon);
                _filled.IconSource = FilledIcon;
            }
        }
    }

    private static void SetFontSize(IconSource iconSource)
    {
        if (iconSource is FontIconSource fontIcon)
        {
            fontIcon.FontSize = 48;
        }
        else if (iconSource is SymbolIconSource symbolIcon)
        {
            symbolIcon.FontSize = 48;
        }
    }

    private void SelectionChanged(NavigationViewItem sender)
    {
        if (sender.IsSelected)
        {
            sender.IconSource = FilledIcon;
        }
        else
        {
            sender.IconSource = RegularIcon;
        }
    }
}
