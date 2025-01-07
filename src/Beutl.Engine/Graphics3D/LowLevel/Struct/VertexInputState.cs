namespace Beutl.Graphics3D;

public readonly struct VertexInputState : IEquatable<VertexInputState>
{
    public static readonly VertexInputState Empty = new()
    {
        VertexBufferDescriptions = [],
        VertexAttributes = []
    };

    public VertexBufferDescription[] VertexBufferDescriptions { get; init; }

    public VertexAttribute[] VertexAttributes { get; init; }

    public static VertexInputState CreateSingleBinding<T>(uint slot = 0,
        VertexInputRate inputRate = VertexInputRate.Vertex, uint stepRate = 0, uint locationOffset = 0)
        where T : unmanaged, IVertexType
    {
        var description = VertexBufferDescription.Create<T>(slot, inputRate, stepRate);
        var attributes = new VertexAttribute[T.Formats.Length];

        for (uint i = 0; i < T.Formats.Length; i++)
        {
            attributes[i] = new VertexAttribute
            {
                BufferSlot = slot,
                Location = locationOffset + i,
                Format = T.Formats[i],
                Offset = T.Offsets[i]
            };
        }

        return new VertexInputState { VertexBufferDescriptions = [description], VertexAttributes = attributes };
    }

    public bool Equals(VertexInputState other)
    {
        return VertexBufferDescriptions.SequenceEqual(other.VertexBufferDescriptions) &&
               VertexAttributes.SequenceEqual(other.VertexAttributes);
    }

    public override bool Equals(object? obj) => obj is VertexInputState other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(VertexBufferDescriptions, VertexAttributes);

    public static bool operator ==(VertexInputState left, VertexInputState right) => left.Equals(right);

    public static bool operator !=(VertexInputState left, VertexInputState right) => !left.Equals(right);
}
