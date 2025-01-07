namespace Beutl.Graphics3D;

public readonly struct GraphicsPipelineTargetInfo : IEquatable<GraphicsPipelineTargetInfo>
{
    public ColorTargetDescription[] ColorTargetDescriptions { get; init; }

    public TextureFormat DepthStencilFormat { get; init; }

    public bool HasDepthStencilTarget { get; init; }

    public bool Equals(GraphicsPipelineTargetInfo other)
    {
        return (ColorTargetDescriptions ?? []).SequenceEqual(other.ColorTargetDescriptions ?? []) &&
               DepthStencilFormat == other.DepthStencilFormat && HasDepthStencilTarget == other.HasDepthStencilTarget;
    }

    public override bool Equals(object? obj)
    {
        return obj is GraphicsPipelineTargetInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ColorTargetDescriptions, (int)DepthStencilFormat, HasDepthStencilTarget);
    }

    public static bool operator ==(GraphicsPipelineTargetInfo left, GraphicsPipelineTargetInfo right) =>
        left.Equals(right);

    public static bool operator !=(GraphicsPipelineTargetInfo left, GraphicsPipelineTargetInfo right) =>
        !left.Equals(right);
}
