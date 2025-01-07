using SDL;

namespace Beutl.Graphics3D;

public readonly struct VertexAttribute : IEquatable<VertexAttribute>
{
    public uint Location { get; init; }

    public uint BufferSlot { get; init; }

    public VertexElementFormat Format { get; init; }

    public uint Offset { get; init; }

    internal unsafe SDL_GPUVertexAttribute ToNative()
    {
        return new SDL_GPUVertexAttribute
        {
            location = Location,
            buffer_slot = BufferSlot,
            format = (SDL_GPUVertexElementFormat)Format,
            offset = Offset
        };
    }

    public bool Equals(VertexAttribute other)
    {
        return Location == other.Location && BufferSlot == other.BufferSlot && Format == other.Format &&
               Offset == other.Offset;
    }

    public override bool Equals(object? obj) => obj is VertexAttribute other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Location, BufferSlot, (int)Format, Offset);

    public static bool operator ==(VertexAttribute left, VertexAttribute right) => left.Equals(right);

    public static bool operator !=(VertexAttribute left, VertexAttribute right) => !left.Equals(right);
}
