using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Render
{
    public static class RenderCore
    {
        public static Bitmap Render()
        {
            var b = new Bitmap(750, 750);

            using (var g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.Black, 0, 0, b.Width, b.Height);
            }

            var model = LoadModel(@"D:\Users\rotor\Documents\african_head.obj");
            Draw(model, b);

            //TestLine(b);
            //TestTriangle(b);

            b.RotateFlip(RotateFlipType.Rotate180FlipX);

            return b;
        }

        private static void TestLine(Bitmap b)
        {
            Line(50, 50, 50, 80, b, Color.White);

            Line(50, 50, 60, 80, b, Color.White);
            Line(50, 50, 80, 80, b, Color.White);
            Line(50, 50, 80, 60, b, Color.White);

            Line(50, 50, 80, 50, b, Color.White);

            Line(50, 50, 80, 40, b, Color.White);
            Line(50, 50, 80, 20, b, Color.White);
            Line(50, 50, 60, 20, b, Color.White);

            Line(50, 50, 50, 20, b, Color.White);

            Line(50, 50, 40, 20, b, Color.White);
            Line(50, 50, 20, 20, b, Color.White);
            Line(50, 50, 20, 40, b, Color.White);

            Line(50, 50, 20, 50, b, Color.White);

            Line(50, 50, 20, 60, b, Color.White);
            Line(50, 50, 20, 80, b, Color.White);
            Line(50, 50, 40, 80, b, Color.White);
        }

        private static void TestTriangle(Bitmap bmp)
        {
            Vector2[] t0 = new[] { new Vector2(10, 70), new Vector2(50, 160), new Vector2(70, 80) };
            Vector2[] t1 = new[] { new Vector2(180, 50), new Vector2(150, 1), new Vector2(70, 180) };
            Vector2[] t2 = new[] { new Vector2(180, 150), new Vector2(120, 160), new Vector2(130, 180) };
            Vector2[] t3 = new[] { new Vector2(200, 100), new Vector2(200, 150), new Vector2(220, 180) };
            Vector2[] t4 = new[] { new Vector2(200, 50), new Vector2(220, 50), new Vector2(250, 100) };

            Triangle(t0[0], t0[1], t0[2], bmp, Color.Red);
            Triangle(t1[0], t1[1], t1[2], bmp, Color.White);
            Triangle(t2[0], t2[1], t2[2], bmp, Color.Green);
            Triangle(t3[0], t3[1], t3[2], bmp, Color.Blue);
            Triangle(t4[0], t4[1], t4[2], bmp, Color.Yellow);
        }

        private static void Triangle(int x0, int y0, int x1, int y1, int x2, int y2, Bitmap bmp, Color color)
        {
            int minX = Math3(x0, x1, x2, Math.Min);
            int minY = Math3(y0, y1, y2, Math.Min);
            int maxX = Math3(x0, x1, x2, Math.Max);
            int maxY = Math3(y0, y1, y2, Math.Max);

            double xmid = ((double)x0 + x1 + x2) / 3;
            double ymid = ((double)y0 + y1 + y2) / 3;

            var line1 = MakeLineFunc(x0, y0, x1, y1);
            if (line1.Func(xmid, ymid) < 0)
                line1 = MakeLineFunc(x1, y1, x0, y0);

            var line2 = MakeLineFunc(x1, y1, x2, y2);
            if (line2.Func(xmid, ymid) < 0)
                line2 = MakeLineFunc(x2, y2, x1, y1);

            var line3 = MakeLineFunc(x2, y2, x0, y0);
            if (line3.Func(xmid, ymid) < 0)
                line3 = MakeLineFunc(x0, y0, x2, y2);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var curr1 = line1.Func(x, y);
                    var curr2 = line2.Func(x, y);
                    var curr3 = line3.Func(x, y);

                    if (curr1 >= 0 && curr2 >= 0 && curr3 >= 0)
                    {
                        bmp.SetPixel(x, y, color);
                    }
                }
            }
        }

        private static MyLine MakeLineFunc(double x0, double y0, double x1, double y1)
        {
            Func<double, double, double> func;

            if (x0 == x1)
                func = (x, _) => x * (y1 - y0) + x0 * (y0 - y1);
            else
                func = (x, y) => (y - y0) / (y1 - y0) - (x - x0) / (x1 - x0);

            var line = new MyLine
            {
                Func = func,
                DeltaX = (y1 - y0) / (x1 - x0),
                DeltaY = 0
            };
            return line;
        }

        private class MyLine
        {
            public Func<double, double, double> Func { get; set; }
            public double DeltaX { get; set; }
            public double DeltaY { get; set; }
        }

        private static int Math3(int a, int b, int c, Func<int, int, int> func)
        {
            var result = func(a, b);
            return func(result, c);
        }

        private static void Draw(Model model, Bitmap b)
        {
            var rnd = new Random();
            foreach (var face in model.Faces)
            {
                Vector2[] screenCoords = new Vector2[3];

                for (var j = 0; j < 3; j++)
                {
                    var worldCoord = model.Vertices[face[j]];
                    screenCoords[j] = new Vector2((worldCoord.X + 1f) * (b.Width - 1) / 2, (worldCoord.Y + 1f) * (b.Height - 1) / 2);
                    /*var v0 = model.Vertices[face[j]];
                    var v1 = model.Vertices[face[(j+1)%3]];
                    int x0 = (int)((v0.X+1d)*b.Width/2d);
                    int y0 = (int)((v0.Y+1d)*(b.Height - 1)/2d);
                    int x1 = (int)((v1.X+1d)*b.Width/2d);
                    int y1 = (int)((v1.Y+1d)*(b.Height - 1)/2d);
                    Line(x0, y0, x1, y1, b, Color.White);*/
                }
                Triangle(screenCoords[0], screenCoords[1], screenCoords[2], b, Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256)));
            }
        }

        private static void Line(Vector2 t0, Vector2 t1, Bitmap bmp, Color color)
        {
            Line((int)t0.X, (int)t0.Y, (int)t1.X, (int)t1.Y, bmp, color);
        }

        private static void Line(int x0, int y0, int x1, int y1, Bitmap bmp, Color color)
        {
            var vertOrientation = Math.Abs(x1 - x0) < Math.Abs(y1 - y0);

            if (vertOrientation)
            {
                int buf;

                buf = x0;
                x0 = y0;
                y0 = buf;

                buf = x1;
                x1 = y1;
                y1 = buf;
            }
            if (x0 > x1)
            {
                int buf;

                buf = x0;
                x0 = x1;
                x1 = buf;

                buf = y0;
                y0 = y1;
                y1 = buf;
            }

            int errorMul = 0;

            int sign = Math.Sign(y1 - y0);

            int kMul = 2 * (y1 - y0);
            int halfMul = x1 - x0;
            int oneMul = 2 * halfMul;

            int y = y0;
            for (var x = x0; x <= x1; x++)
            {
                errorMul += kMul;
                if (Math.Abs(errorMul) > halfMul)
                {
                    y += sign;
                    errorMul -= sign * oneMul;
                }

                if (vertOrientation)
                {
                    bmp.SetPixel(y, x, color);
                }
                else
                {
                    bmp.SetPixel(x, y, color);
                }
            }
        }

        private static void Triangle(Vector2 t0, Vector2 t1, Vector2 t2, Bitmap bmp, Color color)
        {
            Triangle((int)t0.X, (int)t0.Y, (int)t1.X, (int)t1.Y, (int)t2.X, (int)t2.Y, bmp, color);
        }

        private static Model LoadModel(string fileName)
        {
            var lines = File.ReadAllLines(fileName);

            var vertices = new List<Vector3>();
            var vertexLine = new Regex("^v ([^ ]+) ([^ ]+) ([^ ]+)$");

            var faces = new List<Face>();
            var faceLine = new Regex("^f ([^/]+).* ([^/]+).* ([^/]+).*$");

            foreach (var line in lines)
            {
                var vertMatch = vertexLine.Match(line);
                var faceMatch = faceLine.Match(line);

                if (vertMatch.Success)
                {
                    var x = float.Parse(vertMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                    var y = float.Parse(vertMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                    var z = float.Parse(vertMatch.Groups[3].Value, CultureInfo.InvariantCulture);
                    var vertex = new Vector3(x, y, z);

                    vertices.Add(vertex);
                }
                else if (faceMatch.Success)
                {
                    var a = int.Parse(faceMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                    var b = int.Parse(faceMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                    var c = int.Parse(faceMatch.Groups[3].Value, CultureInfo.InvariantCulture);
                    var face = new Face(a - 1, b - 1, c - 1);

                    faces.Add(face);
                }
            }

            return new Model(vertices, faces);
        }

        private class Face
        {
            private readonly int _a;
            private readonly int _b;
            private readonly int _c;

            public Face(int a, int b, int c)
            {
                _a = a;
                _b = b;
                _c = c;
            }

            public int A { get { return _a; } }
            public int B { get { return _b; } }
            public int C { get { return _c; } }

            public int this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0:
                            return _a;
                        case 1:
                            return _b;
                        case 2:
                            return _c;
                        default:
                            throw new IndexOutOfRangeException();
                    }
                }
            }
        }

        private class Model
        {
            private readonly List<Vector3> _vertices;
            private readonly List<Face> _faces;

            public Model(List<Vector3> vertices, List<Face> faces)
            {
                _vertices = vertices;
                _faces = faces;
            }

            public List<Vector3> Vertices
            {
                get { return _vertices; }
            }

            public List<Face> Faces
            {
                get { return _faces; }
            }
        }
    }
}
