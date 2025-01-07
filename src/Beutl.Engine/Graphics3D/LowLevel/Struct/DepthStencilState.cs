using SDL;

namespace Beutl.Graphics3D;

public readonly struct DepthStencilState : IEquatable<DepthStencilState>
{
    public static readonly DepthStencilState Disable = new()
    {
        EnableDepthTest = false,
        EnableDepthWrite = false,
        EnableStencilTest = false
    };

    public CompareOp CompareOp { get; init; }

    public StencilOpState BackStencilState { get; init; }

    public StencilOpState FrontStencilState { get; init; }

    public byte CompareMask { get; init; }

    public byte WriteMask { get; init; }

    public bool EnableDepthTest { get; init; }

    public bool EnableDepthWrite { get; init; }

    public bool EnableStencilTest { get; init; }

    internal SDL_GPUDepthStencilState ToNative()
    {
        return new SDL_GPUDepthStencilState
        {
            compare_op = (SDL_GPUCompareOp)CompareOp,
            back_stencil_state = BackStencilState.ToNative(),
            front_stencil_state = FrontStencilState.ToNative(),
            compare_mask = CompareMask,
            write_mask = WriteMask,
            enable_depth_test = EnableDepthTest,
            enable_depth_write = EnableDepthWrite,
            enable_stencil_test = EnableStencilTest
        };
    }

    public bool Equals(DepthStencilState other) => CompareOp == other.CompareOp && BackStencilState.Equals(other.BackStencilState) && FrontStencilState.Equals(other.FrontStencilState) && CompareMask == other.CompareMask && WriteMask == other.WriteMask && EnableDepthTest == other.EnableDepthTest && EnableDepthWrite == other.EnableDepthWrite && EnableStencilTest == other.EnableStencilTest;

    public override bool Equals(object? obj) => obj is DepthStencilState other && Equals(other);

    public override int GetHashCode() => HashCode.Combine((int)CompareOp, BackStencilState, FrontStencilState, CompareMask, WriteMask, EnableDepthTest, EnableDepthWrite, EnableStencilTest);

    public static bool operator ==(DepthStencilState left, DepthStencilState right) => left.Equals(right);

    public static bool operator !=(DepthStencilState left, DepthStencilState right) => !left.Equals(right);
}
