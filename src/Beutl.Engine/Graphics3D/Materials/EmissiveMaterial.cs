using Beutl.Media;

namespace Beutl.Graphics3D;

public class EmissiveMaterial : Material
{
    public static readonly CoreProperty<Color> ColorProperty;
    public static readonly CoreProperty<IBrush?> BrushProperty;
    private Color _color = Colors.White;
    private IBrush? _brush;

    static EmissiveMaterial()
    {
        ColorProperty = ConfigureProperty<Color, EmissiveMaterial>(nameof(Color))
            .Accessor(o => o.Color, (o, v) => o.Color = v)
            .DefaultValue(Colors.White)
            .Register();

        BrushProperty = ConfigureProperty<IBrush?, EmissiveMaterial>(nameof(Brush))
            .Accessor(o => o.Brush, (o, v) => o.Brush = v)
            .Register();

        // AffectsRender<EmissiveMaterial>(ColorProperty);
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
}
