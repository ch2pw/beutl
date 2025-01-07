namespace Beutl.Graphics3D.Rendering;

public class ContainerRenderNode3D : RenderNode3D
{
    private readonly List<RenderNode3D> _children = [];

    public IReadOnlyList<RenderNode3D> Children => _children;

    public void AddChild(RenderNode3D item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _children.Add(item);
    }

    public void RemoveChild(RenderNode3D item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _children.Remove(item);
    }

    public void RemoveRange(int index, int count)
    {
        _children.RemoveRange(index, count);
    }

    public void SetChild(int index, RenderNode3D item)
    {
        _children[index]?.Dispose();
        _children[index] = item;
    }

    public void BringFrom(ContainerRenderNode3D containerNode)
    {
        _children.Clear();
        _children.AddRange(containerNode._children);

        containerNode._children.Clear();
    }
    
    protected override void OnDispose(bool disposing)
    {
        foreach (RenderNode3D? item in _children)
        {
            item.Dispose();
        }

        _children.Clear();
    }
}
