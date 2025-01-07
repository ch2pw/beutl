using System.Runtime.InteropServices;

namespace Beutl.Graphics3D.Meshes;

public class MeshResource : IDisposable
{
    public MeshResource(Device device, Mesh mesh)
    {
        Mesh = mesh;
        VertexBuffer = Buffer.Create<Vertex>(
            device, BufferUsageFlags.Vertex, (uint)mesh.Vertices.Count);
        VertexTransferBuffer = TransferBuffer.Create<Vertex>(
            device, TransferBufferUsage.Upload, (uint)mesh.Vertices.Count);
        if (mesh.Indices != null)
        {
            IndexBuffer = Buffer.Create<int>(
                device, BufferUsageFlags.Index, (uint)mesh.Indices.Length);
            IndexTransferBuffer = TransferBuffer.Create<int>(
                device, TransferBufferUsage.Upload, (uint)mesh.Indices.Length);
        }
    }

    public Buffer<Vertex> VertexBuffer { get; }

    public Buffer<int>? IndexBuffer { get; }

    public TransferBuffer<Vertex> VertexTransferBuffer { get; }

    public TransferBuffer<int>? IndexTransferBuffer { get; }

    public Mesh Mesh { get; }

    public void OnCopyPass(CopyPass pass)
    {
        using (MappedBuffer<Vertex> vertexBuffer = VertexTransferBuffer.Map())
        {
            CollectionsMarshal.AsSpan(Mesh.Vertices).CopyTo(vertexBuffer.Span);
        }

        pass.UploadToBuffer(VertexTransferBuffer, VertexBuffer, false);

        if (IndexBuffer != null && IndexTransferBuffer != null)
        {
            using (MappedBuffer<int> indexBuffer = IndexTransferBuffer.Map())
            {
                Mesh.Indices!.AsSpan().CopyTo(indexBuffer.Span);
            }

            pass.UploadToBuffer(IndexTransferBuffer, IndexBuffer, false);
        }
    }

    public void OnRenderPass(RenderPass pass)
    {
        pass.BindVertexBuffers(VertexBuffer);
        if (IndexBuffer != null)
        {
            pass.BindIndexBuffer<int>(IndexBuffer);
        }
    }

    public void Dispose()
    {
        VertexBuffer.Dispose();
        VertexTransferBuffer.Dispose();
        IndexBuffer?.Dispose();
        IndexTransferBuffer?.Dispose();
    }
}
