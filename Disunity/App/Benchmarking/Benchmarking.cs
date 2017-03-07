using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Disunity.App.Benchmarking
{
    public static class BenchmarkFactory
    {
        private const int ViewportWidth = 800;
        private const int ViewportHeight = 800;

        private static WorldState CreateWorldState()
        {
            return new WorldState(renderMode: RenderMode.Fill, lightMode: LightMode.NormalMapping,
                fillMode: FillMode.Texture, perspectiveProjection: true, viewportScale: 0.9f,
                viewportWidth: ViewportWidth, viewportHeight: ViewportHeight, viewportLightX: 383,
                viewportLightY: 153, modelRotationX: 0.376991123f, modelRotationY: -0.0157079641f);
        }

        public static Benchmark Create()
        {
            return new Benchmark(CreateWorldState(), () => Task.Run((Func<BenchmarkResult>) StartBenchmark));
        }

        private static BenchmarkResult StartBenchmark()
        {
            var worldState = CreateWorldState();

            var buffer = new Bitmap(ViewportWidth, ViewportHeight, PixelFormat.Format32bppRgb);
            using (var renderCore = new RenderCore(ViewportWidth, ViewportHeight))
            {
                var world = WorldBuilder.BuildWorld(worldState);

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
        private readonly WorldState _state;
        private readonly Func<Task<BenchmarkResult>> _startBenchmark;

        public Benchmark(WorldState state, Func<Task<BenchmarkResult>> startBenchmark)
        {
            _state = state;
            _startBenchmark = startBenchmark;
        }

        public WorldState State
        {
            get { return _state; }
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
