using Beutl.Media;

namespace Beutl.Graphics3D;

public class SpecularMaterial : Material
{
    public static readonly CoreProperty<Color> ColorProperty;
    public static readonly CoreProperty<IBrush?> BrushProperty;
    public static readonly CoreProperty<float> SpecularPowerProperty;
    private Color _color = Colors.White;
    private IBrush? _brush;
    private float _specularPower = 40.0f;

    static SpecularMaterial()
    {
        ColorProperty = ConfigureProperty<Color, SpecularMaterial>(nameof(Color))
            .Accessor(o => o.Color, (o, v) => o.Color = v)
            .DefaultValue(Colors.White)
            .Register();

        BrushProperty = ConfigureProperty<IBrush?, SpecularMaterial>(nameof(Brush))
            .Accessor(o => o.Brush, (o, v) => o.Brush = v)
            .Register();

        SpecularPowerProperty = ConfigureProperty<float, SpecularMaterial>(nameof(SpecularPower))
            .Accessor(o => o.SpecularPower, (o, v) => o.SpecularPower = v)
            .DefaultValue(40.0f)
            .Register();

        // AffectsRender<SpecularMaterial>(ColorProperty);
    }

    public Color Color
    {
        get => _color;
        set => SetAndRaise(ColorProperty, ref _color, value);
    }

    public IBrush? Brush
    {
        get => _brush;
        set => SetAndRaise(BrushProperty, ref _brush, value);
    }

    public float SpecularPower
    {
        get => _specularPower;
        set => SetAndRaise(SpecularPowerProperty, ref _specularPower, value);
    }

    public SpecularMaterial()
    {
    }
}
