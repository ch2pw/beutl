using Beutl.Media;

namespace Beutl.Graphics3D;

public class StandardMaterial : Material
{
    public static readonly CoreProperty<Color> ColorProperty;
    public static readonly CoreProperty<float> SpecularPowerProperty;
    public static readonly CoreProperty<float> ReflectivityProperty;
    public static readonly CoreProperty<IBrush?> BrushProperty;
    private Color _color = Colors.White;
    private float _specularPower = 40.0f;
    private float _reflectivity;
    private IBrush? _brush;

    static StandardMaterial()
    {
        ColorProperty = ConfigureProperty<Color, StandardMaterial>(nameof(Color))
            .Accessor(o => o.Color, (o, v) => o.Color = v)
            .DefaultValue(Colors.White)
            .Register();

        SpecularPowerProperty = ConfigureProperty<float, StandardMaterial>(nameof(SpecularPower))
            .Accessor(o => o.SpecularPower, (o, v) => o.SpecularPower = v)
            .DefaultValue(40.0f)
            .Register();

        ReflectivityProperty = ConfigureProperty<float, StandardMaterial>(nameof(Reflectivity))
            .Accessor(o => o.Reflectivity, (o, v) => o.Reflectivity = v)
            .Register();

        BrushProperty = ConfigureProperty<IBrush?, StandardMaterial>(nameof(Brush))
            .Accessor(o => o.Brush, (o, v) => o.Brush = v)
            .Register();

        // AffectsRender<SpecularMaterial>(ColorProperty);
    }

    public Color Color
    {
        get => _color;
        set => SetAndRaise(ColorProperty, ref _color, value);
    }

    public float SpecularPower
    {
        get => _specularPower;
        set => SetAndRaise(SpecularPowerProperty, ref _specularPower, value);
    }

    public float Reflectivity
    {
        get => _reflectivity;
        set => SetAndRaise(ReflectivityProperty, ref _reflectivity, value);
    }

    public IBrush? Brush
    {
        get => _brush;
        set => SetAndRaise(BrushProperty, ref _brush, value);
    }

    public StandardMaterial()
    {
    }
}
