using System;
using System.Diagnostics;
using System.Numerics;

namespace Render
{
    public class SolidRender: IRender
    {
        private float[,] _zBuffer;
        private int _width;
        private int _height;

        unsafe public void Init(int width, int height)
        {
            _width = width;
            _height = height;
            _zBuffer = new float[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _zBuffer[x, y] = float.NegativeInfinity;
                }
            }
        }

        unsafe public void Draw(Face face, Vector3 a, Vector3 b, Vector3 c, byte* data, IPixelShader shader)
        {
            var screenCoords = new[] { a, b, c };

            var state = shader.OnFace(face);
            Triangle(screenCoords[0], screenCoords[1], screenCoords[2], data, _zBuffer, shader, state);
        }

        unsafe private void Triangle(Vector3 v0, Vector3 v1, Vector3 v2, byte* data, float[,] zBuffer, IPixelShader shader, object state)
        {
            var x0 = (int)Math.Round(v0.X);
            var y0 = (int)Math.Round(v0.Y);
            var x1 = (int)Math.Round(v1.X);
            var y1 = (int)Math.Round(v1.Y);
            var x2 = (int)Math.Round(v2.X);
            var y2 = (int)Math.Round(v2.Y);

            int minX = ClipX(Math3(x0, x1, x2, Math.Min));
            int minY = ClipY(Math3(y0, y1, y2, Math.Min));
            int maxX = ClipX(Math3(x0, x1, x2, Math.Max));
            int maxY = ClipY(Math3(y0, y1, y2, Math.Max));

            float midX = (v0.X + v1.X + v2.X)/3;
            float midY = (v0.Y + v1.Y + v2.Y)/3;

            var dirV0 = new Vector3(v0.X - midX, v0.Y - midY, 0);
            var dirV1 = new Vector3(v1.X - midX, v1.Y - midY, 0);

            var direction = Vector3.Cross(dirV0, dirV1).Z;
            if (direction < 0)
            {
                Vector3 tbuf;
                tbuf = v1;
                v1 = v2;
                v2 = tbuf;
            }

            var sline1 = new MyFastLine(v0.X, v0.Y, v1.X, v1.Y, minX, minY, maxX, maxY);
            var sline2 = new MyFastLine(v1.X, v1.Y, v2.X, v2.Y, minX, minY, maxX, maxY);
            var sline3 = new MyFastLine(v2.X, v2.Y, v0.X, v0.Y, minX, minY, maxX, maxY);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var curr1 = sline1.Value;
                    var curr2 = sline2.Value;
                    var curr3 = sline3.Value;

                    if (curr1 >= 0 && curr2 >= 0 && curr3 >= 0)
                    {
                        var p = GetBarycentricCoordinates(x, y, v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y);

                        if (p.Item1 < 0 || p.Item2 < 0 || p.Item3 < 0)
                            continue;

                        var z = v0.Z*p.Item1 + v1.Z*p.Item2 + v2.Z*p.Item3;

                        if (z < zBuffer[x, y])
                            continue;

                        var foo = ((_height - y - 1)*_width+x)*4;

                        var resColor = shader.OnPixel(state, p.Item1, p.Item2, p.Item3);

                        if (resColor != null)
                        {
                            zBuffer[x, y] = z;
                            data[foo + 2] = resColor.Value.R;
                            data[foo + 1] = resColor.Value.G;
                            data[foo + 0] = resColor.Value.B;
                        }


                    }
                    sline1.StepY();
                    sline2.StepY();
                    sline3.StepY();
                }
                sline1.StepX();
                sline2.StepX();
                sline3.StepX();
            }
        }

        private static T Math3<T>(T a, T b, T c, Func<T, T, T> func)
        {
            var result = func(a, b);
            return func(result, c);
        }

        private static Tuple<float, float, float> GetBarycentricCoordinates(float x, float y, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            var lambda1 = ((y - y3)*(x2 - x3) - (x - x3)*(y2 - y3))/((y1 - y3)*(x2 - x3) - (x1 - x3)*(y2 - y3));
            var lambda2 = ((y - y1)*(x3 - x1) - (x - x1)*(y3 - y1))/((y2 - y1)*(x3 - x1) - (x2 - x1)*(y3 - y1));
            var lambda3 = 1 - lambda1 - lambda2;

            return Tuple.Create(lambda1, lambda2, lambda3);
        }

        private int ClipX(int x)
        {
            return Clip(x, _width);
        }

        private int ClipY(int y)
        {
            return Clip(y, _height);
        }

        private static int Clip(int a, int length)
        {
            if (a < 0)
                return 0;
            if (a >= length)
                return length - 1;
            return a;
        }
    }
}