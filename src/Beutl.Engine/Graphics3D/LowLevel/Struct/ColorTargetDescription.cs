namespace Beutl.Graphics3D;

public readonly struct ColorTargetDescription : IEquatable<ColorTargetDescription>
{
    public TextureFormat Format { get; init; }

    public ColorTargetBlendState BlendState { get; init; }

    public bool Equals(ColorTargetDescription other) => Format == other.Format && BlendState.Equals(other.BlendState);

    public override bool Equals(object? obj) => obj is ColorTargetDescription other && Equals(other);

    public override int GetHashCode() => HashCode.Combine((int)Format, BlendState);

    public static bool operator ==(ColorTargetDescription left, ColorTargetDescription right) => left.Equals(right);

    public static bool operator !=(ColorTargetDescription left, ColorTargetDescription right) => !left.Equals(right);
}
