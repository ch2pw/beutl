using Beutl.Graphics.Rendering;

namespace Beutl.Graphics3D.Rendering;

public abstract class RenderNode3D : INode
{
    protected RenderNode3D()
    {
    }

    ~RenderNode3D()
    {
        if (!IsDisposed)
        {
            OnDispose(false);
            IsDisposed = true;
        }
    }

    public bool IsDisposed { get; private set; }

    public virtual void Update(Device device, CommandBuffer commandBuffer, CopyPass pass)
    {
    }

    public virtual void Render(Device device, CommandBuffer commandBuffer, RenderPass pass)
    {
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            OnDispose(true);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    protected virtual void OnDispose(bool disposing)
    {
    }
}
