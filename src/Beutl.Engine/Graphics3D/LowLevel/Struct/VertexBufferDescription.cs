using SDL;

namespace Beutl.Graphics3D;

public readonly struct VertexBufferDescription : IEquatable<VertexBufferDescription>
{
    public uint Slot { get; init; }

    public uint Pitch { get; init; }

    public VertexInputRate InputRate { get; init; }

    public uint InstanceStepRate { get; init; }

    public static unsafe VertexBufferDescription Create<T>(
        uint slot = 0,
        VertexInputRate inputRate = VertexInputRate.Vertex,
        uint stepRate = 0)
        where T : unmanaged
    {
        return new VertexBufferDescription
        {
            Slot = slot,
            Pitch = (uint)sizeof(T),
            InputRate = inputRate,
            InstanceStepRate = stepRate
        };
    }

    internal SDL_GPUVertexBufferDescription ToNative()
    {
        return new SDL_GPUVertexBufferDescription
        {
            slot = Slot,
            pitch = Pitch,
            input_rate = (SDL_GPUVertexInputRate)InputRate,
            instance_step_rate = InstanceStepRate
        };
    }

    public bool Equals(VertexBufferDescription other)
    {
        return Slot == other.Slot && Pitch == other.Pitch && InputRate == other.InputRate &&
               InstanceStepRate == other.InstanceStepRate;
    }

    public override bool Equals(object? obj) => obj is VertexBufferDescription other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Slot, Pitch, (int)InputRate, InstanceStepRate);

    public static bool operator ==(VertexBufferDescription left, VertexBufferDescription right) => left.Equals(right);

    public static bool operator !=(VertexBufferDescription left, VertexBufferDescription right) => !left.Equals(right);
}
