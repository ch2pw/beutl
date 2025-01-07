using Beutl.Graphics3D.Meshes;

namespace Beutl.Graphics3D.Rendering;

public class BindMeshNode3D(Mesh mesh) : RenderNode3D
{
    private MeshResource? _meshResource;

    public Mesh Mesh { get; } = mesh;

    public bool Equals(Mesh mesh)
    {
        return Mesh == mesh;
    }

    public override void Update(Device device, CommandBuffer commandBuffer, CopyPass pass)
    {
        if (_meshResource == null)
        {
            _meshResource = Mesh.CreateResource(device, pass);
        }
    }

    public override void Render(Device device, CommandBuffer commandBuffer, RenderPass pass)
    {
        _meshResource!.Bind(pass);
    }

    protected override void OnDispose(bool disposing)
    {
        Mesh.ReleaseResource();
    }
}
