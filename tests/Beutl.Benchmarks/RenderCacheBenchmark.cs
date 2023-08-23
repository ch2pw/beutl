using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

using Beutl.Graphics;
using Beutl.Graphics.Effects;
using Beutl.Graphics.Shapes;
using Beutl.Graphics.Transformation;
using Beutl.Media;
using Beutl.Media.Immutable;
using Beutl.Media.Pixel;
using Beutl.Media.Source;
using Beutl.Rendering;
using Beutl.Rendering.Cache;

using SkiaSharp;

#nullable disable

//もともとの描画速度が速い場合、キャッシュを無効化したほうが速くなった。
//このベンチマークではテキストに縁取りをしたり、シャドウをつけるだけでキャッシュ有効の方が上回った
//(↓オンボードGPUでの計測結果↓)

/*
// * Summary *

BenchmarkDotNet v0.13.7, Windows 11 (10.0.22621.2134/22H2/2022Update/SunValley2)
Intel Core i5-8400 CPU 2.80GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK 7.0.400
  [Host]     : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2 [AttachedDebugger]
  Job-RUCFBI : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2

IterationCount=100  LaunchCount=5  RunStrategy=Monitoring

|             Method |      Mean |    Error |    StdDev |   Median |
|------------------- |----------:|---------:|----------:|---------:|
|    RenderWithCache |  9.615 us | 8.001 us | 54.049 us | 3.400 us |
| RenderWithoutCache | 10.238 us | 8.003 us | 54.059 us | 3.600 us |
 */

[SimpleJob(RunStrategy.Monitoring, launchCount: 5, iterationCount: 100)]
public class RenderCacheBenchmark
{
    private _Renderer _renderer;

    [GlobalSetup(Target = nameof(RenderWithCache))]
    public void GlobalSetup()
    {
        _renderer = new _Renderer(1920, 1080);
        _renderer.GetCacheContext()!.CacheOptions = RenderCacheOptions.Default;
    }

    [GlobalSetup(Target = nameof(RenderWithoutCache))]
    public void GlobalSetupForNoCache()
    {
        _renderer = new _Renderer(1920, 1080);
        _renderer.GetCacheContext()!.CacheOptions = RenderCacheOptions.Disabled;
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _renderer?.Dispose();
        _renderer._imageSource.Dispose();
    }

    [Benchmark]
    public void RenderWithCache()
    {
        RenderThread.Dispatcher.Invoke(() =>
        {
            for (int i = 0; i < 10000; i++)
            {
                var result = _renderer.RenderGraphics(TimeSpan.Zero);
                result.Bitmap?.Dispose();
            }
        });
    }

    [Benchmark]
    public void RenderWithoutCache()
    {
        RenderThread.Dispatcher.Invoke(() =>
        {
            for (int i = 0; i < 10000; i++)
            {
                var result = _renderer.RenderGraphics(TimeSpan.Zero);
                result.Bitmap?.Dispose();
            }
        });
    }

    private sealed class _Renderer : Renderer
    {
        internal IImageSource _imageSource;
        private SourceImage _sourceImage;
        private EllipseShape _shape;
        private TextBlock _block;
        private Blur _blur = new();
        private TranslateTransform _translateTransform = new();

        public _Renderer(int width, int height) : base(width, height)
        {
            var stream = typeof(RenderCacheBenchmark).Assembly.GetManifestResourceStream("Beutl.Benchmarks.images.Mandrill.jpg")
                ?? throw new Exception("Not found: 'Mandrill.jpg'");
            try
            {
                using var bitmap = Bitmap<Bgra8888>.FromStream(stream);
                using var skbitmap = bitmap.ToSKBitmap();
                using var resized = skbitmap.Resize(new SKSizeI(500, 500), SKFilterQuality.None);

                using (var r = Ref<IBitmap>.Create(resized.ToBitmap()))
                {
                    _imageSource = new BitmapSource(r, "TEMP");
                }
            }
            finally
            {
                stream.Dispose();
            }

            _sourceImage = new SourceImage
            {
                Source = _imageSource,
                FilterEffect = _blur
            };
            _shape = new EllipseShape
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.White,
                Transform = _translateTransform,
            };
            _block = new TextBlock
            {
                Fill = Brushes.White,
                Text = "HELLO",
                Size = 100,

                // ここのPenとFilterEffectをコメントアウトすると、キャッシュなしのほうが速い
                Pen = new ImmutablePen(
                    Brushes.Red,
                    null, 0, 10,
                    strokeCap: StrokeCap.Round,
                    strokeAlignment: StrokeAlignment.Outside),
                FilterEffect = new DropShadow
                {
                    Color = Colors.Black,
                    Sigma = new(20, 20),
                    Position = new(20, 20),
                }
            };
        }

        protected override void RenderGraphicsCore()
        {
            RenderScene[0].UpdateAll(new Renderable[] { _sourceImage });

            RenderScene[1].UpdateAll(new Renderable[] { _shape });

            RenderScene[2].UpdateAll(new Renderable[] { _block });

            base.RenderGraphicsCore();
        }
    }
}
