using System.Runtime.InteropServices;

namespace Beutl.Graphics3D.Meshes;

public class MeshResource : GraphicsResource
{
    public MeshResource(Device device, Mesh mesh) : base(device)
    {
        Mesh = mesh;
        VertexBuffer = Buffer.Create<Vertex>(
            device, BufferUsageFlags.Vertex, (uint)mesh.Vertices.Count);
        VertexTransferBuffer = TransferBuffer.Create<Vertex>(
            device, TransferBufferUsage.Upload, (uint)mesh.Vertices.Count);
        if (mesh.Indices != null)
        {
            IndexBuffer = Buffer.Create<uint>(
                device, BufferUsageFlags.Index, (uint)mesh.Indices.Length);
            IndexTransferBuffer = TransferBuffer.Create<uint>(
                device, TransferBufferUsage.Upload, (uint)mesh.Indices.Length);
        }
    }

    public Buffer<Vertex> VertexBuffer { get; }

    public Buffer<uint>? IndexBuffer { get; }

    public TransferBuffer<Vertex> VertexTransferBuffer { get; }

    public TransferBuffer<uint>? IndexTransferBuffer { get; }

    public Mesh Mesh { get; }

    public void Update(CopyPass pass)
    {
        using (MappedBuffer<Vertex> vertexBuffer = VertexTransferBuffer.Map())
        {
            CollectionsMarshal.AsSpan(Mesh.Vertices).CopyTo(vertexBuffer.Span);
        }

        pass.UploadToBuffer(VertexTransferBuffer, VertexBuffer, false);

        if (IndexBuffer != null && IndexTransferBuffer != null)
        {
            using (MappedBuffer<uint> indexBuffer = IndexTransferBuffer.Map())
            {
                Mesh.Indices!.AsSpan().CopyTo(indexBuffer.Span);
            }

            pass.UploadToBuffer(IndexTransferBuffer, IndexBuffer, false);
        }
    }

    public void Bind(RenderPass pass)
    {
        pass.BindVertexBuffers(VertexBuffer);
        if (IndexBuffer != null)
        {
            pass.BindIndexBuffer<uint>(IndexBuffer);
        }
    }

    protected override void Dispose(bool disposing)
    {
        VertexBuffer.Dispose();
        VertexTransferBuffer.Dispose();
        IndexBuffer?.Dispose();
        IndexTransferBuffer?.Dispose();
    }
}
