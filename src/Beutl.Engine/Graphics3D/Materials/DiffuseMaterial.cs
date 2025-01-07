using Beutl.Media;

namespace Beutl.Graphics3D;

public class DiffuseMaterial : Material
{
    public static readonly CoreProperty<Color> ColorProperty;
    public static readonly CoreProperty<Color> AmbientColorProperty;
    public static readonly CoreProperty<IBrush?> BrushProperty;
    private Color _color = Colors.White;
    private Color _ambientColor = Colors.White;
    private IBrush? _brush;

    static DiffuseMaterial()
    {
        ColorProperty = ConfigureProperty<Color, DiffuseMaterial>(nameof(Color))
            .Accessor(o => o.Color, (o, v) => o.Color = v)
            .DefaultValue(Colors.White)
            .Register();

        AmbientColorProperty = ConfigureProperty<Color, DiffuseMaterial>(nameof(AmbientColor))
            .Accessor(o => o.AmbientColor, (o, v) => o.AmbientColor = v)
            .DefaultValue(Colors.White)
            .Register();

        BrushProperty = ConfigureProperty<IBrush?, DiffuseMaterial>(nameof(Brush))
            .Accessor(o => o.Brush, (o, v) => o.Brush = v)
            .Register();

        // AffectsRender<DiffuseMaterial>(ColorProperty);
    }

    public Color Color
    {
        get => _color;
        set => SetAndRaise(ColorProperty, ref _color, value);
    }

    public Color AmbientColor
    {
        get => _ambientColor;
        set => SetAndRaise(AmbientColorProperty, ref _ambientColor, value);
    }

    public IBrush? Brush
    {
        get => _brush;
        set => SetAndRaise(BrushProperty, ref _brush, value);
    }
}
