using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Render
{
    public static class RenderCore
    {
//        private const string RootDir = @"D:\Users\rotor\Documents\";
        public const string RootDir = @"C:\Users\p-afanasyev\Documents\";

        public static Bitmap Render(List<IRender> renders, float cameraZPosition)
        {
            var b = new Bitmap(800, 800);

            using (var g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.Black, 0, 0, b.Width, b.Height);
            }

            var model = ModelLoader.LoadModel(RootDir + "african_head.obj");
            var texture = new Bitmap(Image.FromFile(RootDir + "african_head_diffuse.bmp"));

            foreach (var render in renders)
            {
                render.Init(model, texture, b.Width, b.Height, RootDir);
            }
            Draw(model, b, renders, cameraZPosition);
            foreach (var render in renders)
            {
                render.Finish();
            }

            b.RotateFlip(RotateFlipType.Rotate180FlipX);

            return b;
        }

        private static void Draw(Model model, Bitmap b, List<IRender> renders, float cameraZPosition)
        {
            var light = new Vector3(0, 0, 1);

            float c = cameraZPosition;

            foreach (var face in model.Faces)
            {
                var screenCoords = new Vector3[3];

                for (var j = 0; j < 3; j++)
                {
                    var worldCoord = model.Vertices[face[j]];
                    var w = 1 - worldCoord.Z/c;
                    var g = new Vector3(worldCoord.X, worldCoord.Y, worldCoord.Z);
                    screenCoords[j] = new Vector3(g.X/w, g.Y/w, g.Z/w);

                    screenCoords[j] = new Vector3((screenCoords[j].X/(cameraZPosition*0.1f))*300 + b.Width/2, (screenCoords[j].Y/(cameraZPosition*0.1f))*300 + b.Height/2, screenCoords[j].Z);
                }

                var v0 = model.Vertices[face[0]];
                var v1 = model.Vertices[face[1]];
                var v2 = model.Vertices[face[2]];

                var foo1 = Vector3.Subtract(v1, v0);
                var foo2 = Vector3.Subtract(v2, v1);

                var normal = Vector3.Cross(foo1, foo2);
                normal = Vector3.Normalize(normal);

                var bar = Vector3.Dot(normal, light);
                var bar1 = (int) (bar*255);

                if (bar1 <= 0)
                    continue;

                foreach (var render in renders)
                {
                    render.Draw(face, screenCoords[0], screenCoords[1], screenCoords[2], b, (byte)bar1);
                }
            }
        }
    }
}
