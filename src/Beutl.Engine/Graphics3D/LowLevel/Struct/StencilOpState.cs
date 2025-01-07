using SDL;

namespace Beutl.Graphics3D;

public readonly struct StencilOpState : IEquatable<StencilOpState>
{
    public StencilOp FailOp { get; init; }

    public StencilOp PassOp { get; init; }

    public StencilOp DepthFailOp { get; init; }

    public CompareOp CompareOp { get; init; }

    internal SDL_GPUStencilOpState ToNative()
    {
        return new SDL_GPUStencilOpState
        {
            fail_op = (SDL_GPUStencilOp)FailOp,
            pass_op = (SDL_GPUStencilOp)PassOp,
            depth_fail_op = (SDL_GPUStencilOp)DepthFailOp,
            compare_op = (SDL_GPUCompareOp)CompareOp
        };
    }

    public bool Equals(StencilOpState other) => FailOp == other.FailOp && PassOp == other.PassOp && DepthFailOp == other.DepthFailOp && CompareOp == other.CompareOp;

    public override bool Equals(object? obj) => obj is StencilOpState other && Equals(other);

    public override int GetHashCode() => HashCode.Combine((int)FailOp, (int)PassOp, (int)DepthFailOp, (int)CompareOp);

    public static bool operator ==(StencilOpState left, StencilOpState right) => left.Equals(right);

    public static bool operator !=(StencilOpState left, StencilOpState right) => !left.Equals(right);
}
