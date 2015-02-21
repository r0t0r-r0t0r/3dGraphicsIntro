using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render
{
    public class SolidRender: IRender
    {
        private Model _model;
        private unsafe byte* _texture;
        private float[,] _zBuffer;
        private readonly Random _random = new Random(33);
//        private Bitmap _textureDebugBitmap;
        private string _rootDir;
        private int _width;
        private int _height;
        private int _textureWidth;
        private int _textureHeight;
        private Vector3 _light;

        unsafe public void Init(Model model, byte* texture, int textureWidth, int textureHeight, int width, int height, string rootDir, Vector3 light)
        {
            _light = light;
            _textureWidth = textureWidth;
            _textureHeight = textureHeight;
            _width = width;
            _height = height;
            _rootDir = rootDir;
//            _textureDebugBitmap = new Bitmap(texture);
            _model = model;
            _texture = texture;
            _zBuffer = new float[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _zBuffer[x, y] = float.NegativeInfinity;
                }
            }
        }

        public void Finish()
        {
//            var maxZbuff = Double.NegativeInfinity;
//            for (var i = 0; i < _width; i++)
//            {
//                for (var j = 0; j < _height; j++)
//                {
//                    if (_zBuffer[i, j] > maxZbuff)
//                        maxZbuff = _zBuffer[i, j];
//                }
//            }
//            "tralala".ToString();
//            _textureDebugBitmap.Save(_rootDir + @"debug.bmp");
        }

        unsafe public void Draw(Face face, Vector3 a, Vector3 b, Vector3 c, byte* data)
        {
            var v0 = _model.Vertices[face[0]];
            var v1 = _model.Vertices[face[1]];
            var v2 = _model.Vertices[face[2]];

            var foo1 = Vector3.Subtract(v1, v0);
            var foo2 = Vector3.Subtract(v2, v1);

            var normal = Vector3.Cross(foo1, foo2);
            normal = Vector3.Normalize(normal);

            var bar = Vector3.Dot(normal, _light);

            var screenCoords = new[] { a, b, c };
            var textureVertices = Enumerable.Range(0, 3).Select(face.GetVtIndex).Select(x => _model.TextureVertices[x]).ToArray();
            var vertexNormals = Enumerable.Range(0, 3).Select(face.GetNormalIndex).Select(x => _model.VertexNormals[x]).ToArray();

            Triangle(screenCoords[0], screenCoords[1], screenCoords[2], textureVertices, data, bar, _texture, _zBuffer, vertexNormals);
        }

        unsafe private void Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3[] textureVertices, byte* data, float intensity, byte* texture, float[,] zBuffer, Vector3[] vns)
        {
//            var l0 = Vector3.Dot(vns[0], _light);
//            var l1 = Vector3.Dot(vns[1], _light);
//            var l2 = Vector3.Dot(vns[2], _light);

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

                tbuf = textureVertices[1];
                textureVertices[1] = textureVertices[2];
                textureVertices[2] = tbuf;
            }

//            var plain = MakePlain(v0.X, v0.Y, v0.Z, v1.X, v1.Y, v1.Z, v2.X, v2.Y, v2.Z);

            var debugColor = Color.FromArgb(_random.Next(40, 256), _random.Next(40, 256), _random.Next(40, 256));

            var sline1 = new MyFastLine(v0.X, v0.Y, v1.X, v1.Y, minX, minY, maxX, maxY);
            var sline2 = new MyFastLine(v1.X, v1.Y, v2.X, v2.Y, minX, minY, maxX, maxY);
            var sline3 = new MyFastLine(v2.X, v2.Y, v0.X, v0.Y, minX, minY, maxX, maxY);

            var tx0 = textureVertices[0].X;
            var ty0 = textureVertices[0].Y;
            var tx1 = textureVertices[1].X;
            var ty1 = textureVertices[1].Y;
            var tx2 = textureVertices[2].X;
            var ty2 = textureVertices[2].Y;

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
//                        var l = l0*p.Item1 + l1*p.Item2 + l2*p.Item3;
//                        intensity = l;

//                        if (l < 0)
//                            continue;

                        if (z < zBuffer[x, y])
                            continue;

                        zBuffer[x, y] = z;

                        var nx = vns[0].X*p.Item1 + vns[1].X*p.Item2 + vns[2].X*p.Item3;
                        var ny = vns[0].Y*p.Item1 + vns[1].Y*p.Item2 + vns[2].Y*p.Item3;
                        var nz = vns[0].Z*p.Item1 + vns[1].Z*p.Item2 + vns[2].Z*p.Item3;
                        var normal = new Vector3(nx, ny, nz);
                        var l = Vector3.Dot(normal, _light);

                        intensity = l;
                        if (intensity < 0)
                            continue;

                        var tx = (int)((p.Item1*tx0 + p.Item2*tx1 + p.Item3*tx2)*(_textureWidth - 1));
                        var ty = (int)((p.Item1*ty0 + p.Item2*ty1 + p.Item3*ty2)*(_textureHeight - 1));

                        var pos = ((_textureHeight - ty - 1)*_textureWidth + tx)*4;
                        var tr = texture[pos + 2];
                        var tg = texture[pos + 1];
                        var tb = texture[pos + 0];
                        var color1 = Color.FromArgb(tr, tg, tb);

//                        var color1 = Color.WhiteSmoke;
                        var foo = ((_height - y - 1)*_width+x)*4;
//                        intensity *= 1.5f;
                        if (intensity > 1)
                            intensity = 1;
                        data[foo + 2] = (byte) (color1.R*intensity);
                        data[foo + 1] = (byte) (color1.G*intensity);
                        data[foo + 0] = (byte) (color1.B*intensity);

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