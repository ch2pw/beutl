using System.Numerics;
using Beutl.Media;

namespace Beutl.Graphics3D.Meshes;

public class CubeMesh : Mesh
{
    public CubeMesh()
    {
        var v1 = new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), Colors.White, Vector3.Zero, new Vector2(0, 0));
        var v2 = new Vertex(new Vector3(0.5f, 0.5f, -0.5f), Colors.White, Vector3.Zero, new Vector2(1, 0));
        var v3 = new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), Colors.White, Vector3.Zero, new Vector2(0, 1));
        var v4 = new Vertex(new Vector3(0.5f, -0.5f, -0.5f), Colors.White, Vector3.Zero, new Vector2(1, 1));

        var v5 = new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), Colors.White, Vector3.Zero, new Vector2(0, 0));
        var v6 = new Vertex(new Vector3(0.5f, 0.5f, 0.5f), Colors.White, Vector3.Zero, new Vector2(1, 0));
        var v7 = new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), Colors.White, Vector3.Zero, new Vector2(0, 1));
        var v8 = new Vertex(new Vector3(0.5f, -0.5f, 0.5f), Colors.White, Vector3.Zero, new Vector2(1, 1));

        v1 = v1 with { Color = Colors.Red };
        v2 = v2 with { Color = Colors.Green };
        v3 = v3 with { Color = Colors.Blue };
        v4 = v4 with { Color = Colors.Cyan };
        v5 = v5 with { Color = Colors.Magenta };
        v6 = v6 with { Color = Colors.Yellow };
        v7 = v7 with { Color = Colors.White };
        v8 = v8 with { Color = Colors.Black };

        AddFace(v1, v2, v3, v4);
        AddFace(v2, v6, v4, v8);
        AddFace(v6, v5, v8, v7);
        AddFace(v5, v1, v7, v3);
        AddFace(v5, v6, v1, v2);
        AddFace(v3, v4, v7, v8);

        Bounds = GenerateAABB();
    }
}
