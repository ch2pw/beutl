using SDL;

namespace Beutl.Graphics3D;

public readonly struct MultisampleState : IEquatable<MultisampleState>
{
    public static readonly MultisampleState None = new() { SampleCount = SampleCount.One };

    public SampleCount SampleCount { get; init; }

    public uint SampleMask { get; init; }

    public bool EnableMask { get; init; }

    internal SDL_GPUMultisampleState ToNative()
    {
        return new SDL_GPUMultisampleState
        {
            sample_count = (SDL_GPUSampleCount)SampleCount,
            sample_mask = SampleMask,
            enable_mask = EnableMask
        };
    }

    public bool Equals(MultisampleState other) => SampleCount == other.SampleCount && SampleMask == other.SampleMask && EnableMask == other.EnableMask;

    public override bool Equals(object? obj) => obj is MultisampleState other && Equals(other);

    public override int GetHashCode() => HashCode.Combine((int)SampleCount, SampleMask, EnableMask);

    public static bool operator ==(MultisampleState left, MultisampleState right) => left.Equals(right);

    public static bool operator !=(MultisampleState left, MultisampleState right) => !left.Equals(right);
}
