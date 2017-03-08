using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Disunity.Data;
using Disunity.Rendering;
using Disunity.WorldManaging.Loading;
using Disunity.WorldManaging.StateChanging;

namespace Disunity.App.Benchmarking
{
    public static class BenchmarkFactory
    {
        private const int ViewportWidth = 800;
        private const int ViewportHeight = 800;

        private const string RootDir = @"Model";

        private const string GeometryFile = "african_head.obj";
        private const string TextureFile = "african_head_diffuse.bmp";
        private const string NormalMapFile = "african_head_nm.png";
        private const string SpecularMapFile = "african_head_spec.bmp"; 

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
            var model = ModelLoader.LoadModel(RootDir, GeometryFile, TextureFile, NormalMapFile, SpecularMapFile);
            var buffer = new Bitmap(ViewportWidth, ViewportHeight, PixelFormat.Format32bppRgb);

            using (var renderer = new Renderer(ViewportWidth, ViewportHeight))
            {
                var world = new WorldBuilder(model).BuildWorld(worldState);

                const int count = 500;

                // Heating
                for (var i = 0; i < count; i++)
                {
                    renderer.Render(world, buffer);
                }

                // Actual measuring
                var start = Stopwatch.GetTimestamp();
                for (var i = 0; i < count; i++)
                {
                    renderer.Render(world, buffer);
                }
                var end = Stopwatch.GetTimestamp();

                double runticks = end - start;
                double runtime = runticks/Stopwatch.Frequency/count;

                return new BenchmarkResult(runtime);
            }
        }
    }
}
