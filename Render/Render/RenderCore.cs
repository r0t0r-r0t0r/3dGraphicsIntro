using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Render.Shaders;

namespace Render
{
    public class RenderCore
    {
        private readonly Render _render = new Render();

        public void Render(RenderSettings settings, Bitmap bitmap)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;
            var world = WorldUtils.CreateWorld(settings, width, height);

            _render.Init(width, height, world.RenderMode);
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* rawData = (byte*) data.Scan0;
                var length = width*height*4;
                for (var i = 0; i < length; i++)
                {
                    rawData[i] = 0;
                }

                var parCount = Environment.ProcessorCount;
                var vertStep = height/parCount - 1;
                var start = 0;
                var regions = new Tuple<int, int>[parCount];
                for (var i = 0; i < parCount; i++)
                {
                    var end = start + vertStep;
                    regions[i] = Tuple.Create(start, end);

                    start = end + 1;
                }
                var last = parCount - 1;
                regions[last] = Tuple.Create(regions[last].Item1, height - 1);
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

        private unsafe static void Draw(byte* data, Render render, int startY, int endY, World world)
        {
            var shaderState = new ShaderState(30, world);
            
            for (var i = 0; i < world.WorldObject.Model.Geometry.Faces.Count; i++)
            {
                render.Draw(i, data, shaderState, startY, endY);
            }
        }
    }
}
