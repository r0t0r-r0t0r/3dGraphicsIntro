using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Render.Shaders;

namespace Render
{
    public class RenderCore
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Render _render;

        public RenderCore(int width, int height)
        {
            _width = width;
            _height = height;
            _render = new Render(width, height);
        }

        public void Render(World world, Bitmap bitmap)
        {
            _render.Init(world.RenderMode);
            var data = bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            unsafe
            {
                var rawData = (int*) data.Scan0;
                var length = _width*_height;
                for (var i = 0; i < length; i++)
                {
                    rawData[i] = 0;
                }

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
                var tasks =
                    regions.Select(
                        region =>
                            new Task(() => Draw(rawData, _render, region.Item1, region.Item2, world)))
                        .ToArray();
                foreach (var task in tasks)
                {
                    task.Start();
                }
                Task.WaitAll(tasks);
            }
            bitmap.UnlockBits(data);
        }

        private unsafe static void Draw(int* data, Render render, int startY, int endY, World world)
        {
            var shaderState = new ShaderState(30, world);
            var shader = world.WorldObject.ShaderFactory();
            shader.World(world);
            
            for (var i = 0; i < world.WorldObject.Model.Geometry.Faces.Count; i++)
            {
                render.Draw(i, data, shader, shaderState, startY, endY);
            }
        }
    }
}
