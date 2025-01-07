namespace Beutl.Graphics3D;

public readonly struct GraphicsPipelineCreateInfo : IEquatable<GraphicsPipelineCreateInfo>
{
    public Shader VertexShader { get; init; }

    public Shader FragmentShader { get; init; }

    public VertexInputState VertexInputState { get; init; }

    public PrimitiveType PrimitiveType { get; init; }

    public RasterizerState RasterizerState { get; init; }

    public MultisampleState MultisampleState { get; init; }

    public DepthStencilState DepthStencilState { get; init; }

    public GraphicsPipelineTargetInfo TargetInfo { get; init; }

    public uint Props { get; init; }

    public bool Equals(GraphicsPipelineCreateInfo other)
    {
        return VertexShader.Equals(other.VertexShader) && FragmentShader.Equals(other.FragmentShader) &&
               VertexInputState.Equals(other.VertexInputState) && PrimitiveType == other.PrimitiveType &&
               RasterizerState.Equals(other.RasterizerState) && MultisampleState.Equals(other.MultisampleState) &&
               DepthStencilState.Equals(other.DepthStencilState) && TargetInfo.Equals(other.TargetInfo) &&
               Props == other.Props;
    }

    public override bool Equals(object? obj) => obj is GraphicsPipelineCreateInfo other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(VertexShader);
        hashCode.Add(FragmentShader);
        hashCode.Add(VertexInputState);
        hashCode.Add((int)PrimitiveType);
        hashCode.Add(RasterizerState);
        hashCode.Add(MultisampleState);
        hashCode.Add(DepthStencilState);
        hashCode.Add(TargetInfo);
        hashCode.Add(Props);
        return hashCode.ToHashCode();
    }

    public static bool operator ==(GraphicsPipelineCreateInfo left, GraphicsPipelineCreateInfo right) => left.Equals(right);

    public static bool operator !=(GraphicsPipelineCreateInfo left, GraphicsPipelineCreateInfo right) => !left.Equals(right);
}
