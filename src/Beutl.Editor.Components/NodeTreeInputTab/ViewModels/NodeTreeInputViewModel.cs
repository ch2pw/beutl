using Beutl.Editor.Components.NodeTreeTab.ViewModels;
using Beutl.NodeTree;
using Beutl.NodeTree.Nodes;
using Beutl.Operation;
using Beutl.ProjectSystem;

using Microsoft.Extensions.DependencyInjection;

using Reactive.Bindings;

namespace Beutl.Editor.Components.NodeTreeInputTab.ViewModels;

public sealed class NodeTreeInputViewModel : IDisposable, IServiceProvider
{
    private readonly CompositeDisposable _disposables = [];
    private NodeTreeInputTabViewModel _parent;
    private NodeTreeModel? _nodeTreeModel;

    public NodeTreeInputViewModel(Element element, NodeTreeInputTabViewModel parent)
    {
        Model = element;
        _parent = parent;
        _nodeTreeModel = NodeTreeTabViewModel.FindNodeTreeModel(element);

        if (_nodeTreeModel != null)
        {
            _nodeTreeModel.Nodes.ForEachItem(
                (originalIdx, item) =>
                {
                    if (item is LayerInputNode layerInput)
                    {
                        int idx = ConvertFromOriginalIndex(originalIdx);
                        Items.Insert(idx, new NodeInputViewModel(layerInput, originalIdx, this));

                        for (int i = idx; i < Items.Count; i++)
                        {
                            Items[i].OriginalIndex = _nodeTreeModel.Nodes.IndexOf(Items[i].Node);
                        }
                    }
                },
                (originalIdx, item) =>
                {
                    if (item is LayerInputNode layerInput)
                    {
                        int idx = ConvertFromOriginalIndex(originalIdx);
                        Items[idx].Dispose();
                        Items.RemoveAt(idx);

                        for (int i = idx; i < Items.Count; i++)
                        {
                            Items[i].OriginalIndex = _nodeTreeModel.Nodes.IndexOf(Items[i].Node);
                        }
                    }
                },
                () =>
                {
                    foreach (NodeInputViewModel item in Items.GetMarshal().Value)
                    {
                        item.Dispose();
                    }

                    Items.Clear();
                })
                .DisposeWith(_disposables);
        }
    }

    public Element Model { get; }

    public CoreList<NodeInputViewModel> Items { get; } = [];

    // NodesのIndexから、ItemsのIndexに変換。
    public int ConvertFromOriginalIndex(int originalIndex)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].OriginalIndex == originalIndex)
            {
                return i;
            }
        }

        if (Items.Count > 0)
        {
            int lastIdx = Items[^1].OriginalIndex;
            if (lastIdx < originalIndex)
            {
                return Items.Count;
            }
            else
            {
                for (int i = 1; i < Items.Count; i++)
                {
                    if (Items[i - 1].OriginalIndex < originalIndex
                        && originalIndex <= Items[i].OriginalIndex)
                    {
                        return i;
                    }
                }
            }
        }

        return 0;
    }

    // ItemsのIndexから、NodesのIndexに変換。
    public int ConvertToOriginalIndex(int index)
    {
        return Items[index].OriginalIndex;
    }

    public void Dispose()
    {
        foreach (NodeInputViewModel item in Items.GetMarshal().Value)
        {
            item.Dispose();
        }

        Items.Clear();
        _disposables.Dispose();
        _parent = null!;
    }

    public object? GetService(Type serviceType)
    {
        if (serviceType.IsAssignableTo(typeof(NodeTreeModel)))
            return _nodeTreeModel;

        return _parent.GetService(serviceType);
    }
}
