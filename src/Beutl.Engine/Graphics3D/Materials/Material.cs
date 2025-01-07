using Beutl.Animation;

namespace Beutl.Graphics3D;

public abstract class Material : Animatable
{
    public abstract Shader CreateFragmentShader(Device device);

    public abstract Shader CreateVertexShader(Device device);
}
