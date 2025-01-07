using System.Numerics;
using Beutl.Graphics;
using Beutl.Graphics3D.Meshes;

namespace Beutl.Graphics3D.Rendering;

public class GraphicsContext3D : IDisposable, IPopable
{
    private readonly Stack<Action> _popActions = new();
    private ContainerRenderNode3D _container = new();
    private int _drawOperationindex;

    public Matrix4x4 Transform { get; private set; } = Matrix4x4.Identity;

    private T? Next<T>() where T : RenderNode3D
    {
        return _drawOperationindex < _container.Children.Count ? _container.Children[_drawOperationindex] as T : null;
    }

    private RenderNode3D? Next()
    {
        return _drawOperationindex < _container.Children.Count ? _container.Children[_drawOperationindex] : null;
    }

    public void Dispose()
    {
        _popActions.Clear();
        Transform = Matrix4x4.Identity;
    }

    public PushedState PushTransform(Matrix4x4 mat)
    {
        Matrix4x4 old = Transform;
        Transform = mat * Transform;
        _popActions.Push(() => Transform = old);
        return new(this, _popActions.Count);
    }

    public void Pop(int count)
    {
        if (count < 0)
        {
            while (count < 0
                   && _popActions.TryPop(out Action? restorer))
            {
                restorer();
                count++;
            }
        }
        else
        {
            while (_popActions.Count >= count
                   && _popActions.TryPop(out Action? restorer))
            {
                restorer();
            }
        }
    }

    private void Add(RenderNode3D node)
    {
        if (_drawOperationindex < _container.Children.Count)
        {
            // Untracked(_container.Children[_drawOperationindex]);
            _container.SetChild(_drawOperationindex, node);
        }
        else
        {
            _container.AddChild(node);
        }
    }

    public void Bind(Mesh mesh)
    {
        BindMeshNode3D? next = Next<BindMeshNode3D>();

        if (next == null || !next.Equals(mesh))
        {
            Add(new BindMeshNode3D(mesh));
        }

        ++_drawOperationindex;
    }
}
