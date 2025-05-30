﻿using Avalonia.Controls;

using FluentAvalonia.UI.Controls;

namespace Beutl.Extensibility;

public abstract class PageExtension : Extension
{
    public abstract Control CreateControl();

    public abstract IPageContext CreateContext();

    [Obsolete]
    public abstract IconSource GetFilledIcon();

    public abstract IconSource GetRegularIcon();
}

public interface IPageContext : IDisposable
{
    PageExtension Extension { get; }

    string Header { get; }
}
