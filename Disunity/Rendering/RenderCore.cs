using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Disunity.Data;
using Disunity.Data.Shaders;

namespace Disunity.Rendering
{
    public class RenderCore: IDisposable
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Render _render;
        private readonly WritableTexture _firstPhaseScreen;

        public RenderCore(int width, int height)
        {
            _width = width;
            _height = height;
            _render = new Render(width, height);
            _firstPhaseScreen = new WritableTexture(new Bitmap(width, height, PixelFormat.Format32bppRgb), true);
        }

        public void Render(World world, Bitmap bitmap)
        {
            using (var screen = new WritableTexture(bitmap, false))
            {
                screen.Clear();

                var parCount = Environment.ProcessorCount;
                var vertStep = _height/parCount - 1;
                var start = 0;
                var regions = new Tuple<int, int>[parCount];
                for (var i = 0; i < parCount; i++)
                {
                    var end = start + vertStep;
                    regions[i] = Tuple.Create(start, end);

                    start = end + 1;
                }
                var last = parCount - 1;
                regions[last] = Tuple.Create(regions[last].Item1, _height - 1);
                if (world.TwoPhaseRendering)
                {
                    var task = TwoPhaseDraw(world, regions, screen);
                    task.Wait();
                }
                else
                {
                    _render.Init(world.RenderMode);
                    var tasks = regions.Select(
                        region =>
                            new Task(() => Draw(screen, world.WorldObject.ShaderFactory, _render, region.Item1, region.Item2, world)))
                        .ToArray();
                    foreach (var task in tasks)
                    {
                        task.Start();
                    }
                    Task.WaitAll(tasks);
                }
            }
        }

        private async Task TwoPhaseDraw(World world, Tuple<int, int>[] regions, WritableTexture screen)
        {
            _firstPhaseScreen.Clear();
            _render.Init(RenderMode.Fill);

            var firstPhaseTasks = regions.Select(
                region =>
                    new Task(
                        () =>
                            Draw(_firstPhaseScreen, world.WorldObject.FirstPhaseShaderFactory, _render, region.Item1,
                                region.Item2, world)))
                .ToArray();
            foreach (var task in firstPhaseTasks)
            {
                task.Start();
            }
            await Task.WhenAll(firstPhaseTasks).ConfigureAwait(false);

            _render.Init(world.RenderMode);

            var secondPhaseTasks = regions.Select(
                region =>
                    new Task(() =>
                    {
                        var shaderState = new ShaderState(30, world);
                        var shader = world.WorldObject.ShaderFactory();
                        shader.World(world, _firstPhaseScreen);

                        for (var i = 0; i < world.WorldObject.Model.Geometry.Faces.Count; i++)
                        {
                            _render.Draw(i, screen, shader, shaderState, region.Item1, region.Item2);
                        }
                    }))
                .ToArray();
            foreach (var task in secondPhaseTasks)
            {
                task.Start();
            }

            await Task.WhenAll(secondPhaseTasks).ConfigureAwait(false);
        }

        private static void Draw(WritableTexture screen, Func<Shader> shaderFactory, Render render, int startY, int endY, World world)
        {
            var shaderState = new ShaderState(30, world);
            var shader = shaderFactory();
            shader.World(world);
            
            for (var i = 0; i < world.WorldObject.Model.Geometry.Faces.Count; i++)
            {
                render.Draw(i, screen, shader, shaderState, startY, endY);
            }
        }

        public void Dispose()
        {
            _firstPhaseScreen.Dispose();
        }
    }
}
