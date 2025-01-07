using System.Numerics;
using System.Runtime.InteropServices;
using Beutl.Media;

namespace Beutl.Graphics3D;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Vertex : IVertexType
{
    public Vertex(Vector3 pos) : this(pos, Colors.White, Vector3.Zero, Vector2.Zero) { }

    public Vertex(Vector3 pos, Color col) : this(pos, col, Vector3.Zero, Vector2.Zero) { }

    public Vertex(Vector3 pos, Color col, Vector3 normal) : this(pos, col, normal, Vector2.Zero) { }

    public Vertex(Vector3 pos, Color col, Vector3 normal, Vector2 texcoord)
    {
        Position = pos;
        Color = col;
        Normal = normal;
        TexCoord = texcoord;
        Tangent = normal;
        Bitangent = normal;
    }

    public Vector3 Position { get; init; }

    public Vector2 TexCoord { get; init; }

    public Color Color { get; init; }

    public Vector3 Normal { get; init; }

    public Vector3 Tangent { get; init; }

    public Vector3 Bitangent { get; init; }

    public static VertexElementFormat[] Formats { get; } =
    [
        VertexElementFormat.Float3,
        VertexElementFormat.Float2,
        VertexElementFormat.UByte4Norm,
        VertexElementFormat.Float3,
        VertexElementFormat.Float3,
        VertexElementFormat.Float3,
    ];

    public static uint[] Offsets { get; } =
    [
        0,
        12,
        20,
        24,
        36,
        48,
    ];
}
