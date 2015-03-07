using System;
using System.Diagnostics;
using System.Numerics;
using Render.Shaders;

namespace Render
{
    public class SolidRender: IRender
    {
        private float[,] _zBuffer;
        private int _width;
        private int _height;

        public void Init(int width, int height)
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

        unsafe public void Draw(int faceIndex, byte* data, IShader shader, ShaderState shaderState, int startY, int endY)
        {
            var faceState = shaderState.Face;
            var vertexState = shaderState.Vertex;
            var fragmentState = shaderState.Fragment;

            faceState.Clear();
            vertexState.Clear();

            shader.Face(faceState, faceIndex);
            var v04 = shader.Vertex(vertexState, faceIndex, 0);
            var v14 = shader.Vertex(vertexState, faceIndex, 1);
            var v24 = shader.Vertex(vertexState, faceIndex, 2);

            var v0 = new Vector3(v04.X / v04.W, v04.Y / v04.W, v04.Z / v04.W);
            var v1 = new Vector3(v14.X / v14.W, v14.Y / v14.W, v14.Z / v14.W);
            var v2 = new Vector3(v24.X / v24.W, v24.Y / v24.W, v24.Z / v24.W);

            var x0 = (int)Math.Round(v0.X);
            var y0 = (int)Math.Round(v0.Y);
            var x1 = (int)Math.Round(v1.X);
            var y1 = (int)Math.Round(v1.Y);
            var x2 = (int)Math.Round(v2.X);
            var y2 = (int)Math.Round(v2.Y);

            int minX = ClipX(Math3(x0, x1, x2, Math.Min));
            int minY = ClipY(Math3(y0, y1, y2, Math.Min), startY, endY);
            int maxX = ClipX(Math3(x0, x1, x2, Math.Max));
            int maxY = ClipY(Math3(y0, y1, y2, Math.Max), startY, endY);

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

            var sline1 = new MyFastLine(v0.X, v0.Y, v1.X, v1.Y, minX, minY);
            var sline2 = new MyFastLine(v1.X, v1.Y, v2.X, v2.Y, minX, minY);
            var sline3 = new MyFastLine(v2.X, v2.Y, v0.X, v0.Y, minX, minY);

            var converter = new BarycentricCoordinatesConverter(v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y);

            var line = ((int*) data) + (_height - minY - 1) * _width;

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    var curr1 = sline1.Value;
                    var curr2 = sline2.Value;
                    var curr3 = sline3.Value;

                    if (curr1 >= 0 && curr2 >= 0 && curr3 >= 0)
                    {
                        var p = converter.Convert(x, y);

                        if (p.A < 0 || p.B < 0 || p.C < 0)
                            continue;

                        var z = v0.Z*p.A + v1.Z*p.B + v2.Z*p.C;

                        if (z < _zBuffer[x, y])
                            continue;

                        fragmentState.Clear();
                        fragmentState.Intensity = faceState.Intensity;
                        for (var i = 0; i < vertexState.Varying[0].Count; i++)
                        {
                            var fragmentValue = vertexState.Varying[0][i] * p.A + vertexState.Varying[1][i] * p.B + vertexState.Varying[2][i] * p.C;
                            fragmentState.Varying.Add(fragmentValue);
                        }

                        var resColor = shader.Fragment(fragmentState);

                        if (resColor != null)
                        {
                            _zBuffer[x, y] = z;
                            line[x] = resColor.Value.ToArgb();
                        }
                    }
                    sline1.StepX();
                    sline2.StepX();
                    sline3.StepX();
                }
                sline1.StepY();
                sline2.StepY();
                sline3.StepY();
                line -= _width;
            }
        }

        private static T Math3<T>(T a, T b, T c, Func<T, T, T> func)
        {
            var result = func(a, b);
            return func(result, c);
        }

        private int ClipX(int x)
        {
            return Clip(x, 0, _width - 1);
        }

        private int ClipY(int y, int minY, int maxY)
        {
            return Clip(y, minY, maxY);
        }

        private static int Clip(int a, int minA, int maxA)
        {
            if (a < minA)
                return minA;
            if (a > maxA)
                return maxA;
            return a;
        }
    }

    public struct BarycentricCoordinatesConverter
    {
        private readonly float _ka;
        private readonly float _la;
        private readonly float _m;

        private readonly float _kb;
        private readonly float _lb;
        private readonly float _n;

        public BarycentricCoordinatesConverter(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            var a = 1/((y1 - y3)*(x2 - x3) - (x1 - x3)*(y2 - y3));
            var b = 1/((y2 - y1)*(x3 - x1) - (x2 - x1)*(y3 - y1));

            _ka = (x2 - x3)*a;
            _la = (y2 - y3)*a;
            _m = x3*_la - y3*_ka;

            _kb = (x3 - x1) * b;
            _lb = (y3 - y1) * b;
            _n = x1 * _lb - y1 * _kb;
        }

        public BarycentricCoordinates Convert(float x, float y)
        {
            var lambda1 = y*_ka - x*_la + _m;
            var lambda2 = y*_kb - x*_lb + _n;
            var lambda3 = 1 - lambda1 - lambda2;
            return new BarycentricCoordinates(lambda1, lambda2, lambda3);
        }
    }

    public struct BarycentricCoordinates
    {
        private readonly float _a;
        private readonly float _b;
        private readonly float _c;

        public BarycentricCoordinates(float a, float b, float c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public float A
        {
            get { return _a; }
        }

        public float B
        {
            get { return _b; }
        }

        public float C
        {
            get { return _c; }
        }
    }
}