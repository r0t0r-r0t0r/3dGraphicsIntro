using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Render.Benchmarking
{
    public static class BenchmarkFactory
    {
        private const int ViewportWidth = 800;
        private const int ViewportHeight = 800;

        private static WorldBuilder CreateWorldBuilder()
        {
            return new WorldBuilder(ViewportWidth, ViewportHeight)
            {
                RenderMode = RenderMode.Fill,
                LightMode = LightMode.NormalMapping,
                FillMode = FillMode.Texture,
                PerspectiveProjection = true,
                ViewportScale = 0.9f,
                ViewportLightX = 383,
                ViewportLightY = 153,
                ModelRotationX = 0.376991123f,
                ModelRotationY = -0.0157079641f
            };
        }

        public static Benchmark Create()
        {
            return new Benchmark(CreateWorldBuilder(), () => Task.Run((Func<BenchmarkResult>) StartBenchmark));
        }

        private static BenchmarkResult StartBenchmark()
        {
            var builder = CreateWorldBuilder();

            var buffer = new Bitmap(ViewportWidth, ViewportHeight, PixelFormat.Format32bppRgb);
            using (var renderCore = new RenderCore(ViewportWidth, ViewportHeight))
            {
                var world = builder.BuildWorld();

                const int count = 500;

                // Heating
                for (var i = 0; i < count; i++)
                {
                    renderCore.Render(world, buffer);
                }

                // Actual measuring
                var start = Stopwatch.GetTimestamp();
                for (var i = 0; i < count; i++)
                {
                    renderCore.Render(world, buffer);
                }
                var end = Stopwatch.GetTimestamp();

                double runticks = end - start;
                double runtime = runticks/Stopwatch.Frequency/count;

                return new BenchmarkResult(runtime);
            }
        }
    }

    public class Benchmark
    {
        private readonly WorldBuilder _settings;
        private readonly Func<Task<BenchmarkResult>> _startBenchmark;

        public Benchmark(WorldBuilder settings, Func<Task<BenchmarkResult>> startBenchmark)
        {
            _settings = settings;
            _startBenchmark = startBenchmark;
        }

        public WorldBuilder Settings
        {
            get { return _settings; }
        }

        public Task<BenchmarkResult> Start()
        {
            return _startBenchmark();
        }
    }

    public class BenchmarkResult
    {
        private readonly double _frameRenderingDuration;

        public BenchmarkResult(double frameRenderingDuration)
        {
            _frameRenderingDuration = frameRenderingDuration;
        }

        public double FrameRenderingDuration
        {
            get { return _frameRenderingDuration; }
        }
    }
}
