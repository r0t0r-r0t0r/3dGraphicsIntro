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
        private double[,] _zBuffer;
        private readonly Random _random = new Random(33);
//        private Bitmap _textureDebugBitmap;
        private string _rootDir;
        private int _width;
        private int _height;
        private int _textureWidth;
        private int _textureHeight;

        unsafe public void Init(Model model, byte* texture, int textureWidth, int textureHeight, int width, int height, string rootDir)
        {
            _textureWidth = textureWidth;
            _textureHeight = textureHeight;
            _width = width;
            _height = height;
            _rootDir = rootDir;
//            _textureDebugBitmap = new Bitmap(texture);
            _model = model;
            _texture = texture;
            _zBuffer = new double[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _zBuffer[x, y] = double.NegativeInfinity;
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

        unsafe public void Draw(Face face, Vector3 a, Vector3 b, Vector3 c, byte* data, byte lightLevel)
        {
            var bar1 = lightLevel;
            var screenCoords = new[] { a, b, c };
            var textureVertices = Enumerable.Range(0, 3).Select(face.GetVtIndex).Select(x => _model.TextureVertices[x]).ToArray();

            Triangle(screenCoords[0], screenCoords[1], screenCoords[2], textureVertices, data, Color.FromArgb(bar1, bar1, bar1), _texture, _zBuffer);
        }

        unsafe private void Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3[] textureVertices, byte* data, Color color, byte* texture, double[,] zBuffer)
        {
            var x0 = (int)v0.X;
            var y0 = (int)v0.Y;
            var z0 = v0.Z;
            var x1 = (int)v1.X;
            var y1 = (int)v1.Y;
            var z1 = v1.Z;
            var x2 = (int)v2.X;
            var y2 = (int)v2.Y;
            var z2 = v2.Z;

            var midX = (x0 + x1 + x2)/3f;
            var midY = (y0 + y1 + y2)/3f;
            var midZ = (z0 + z1 + z2)/3f;

            var tv0 = textureVertices[0];
            var tv1 = textureVertices[1];
            var tv2 = textureVertices[2];

            int minX = ClipX(Math3(x0, x1, x2, Math.Min));
            int minY = ClipY(Math3(y0, y1, y2, Math.Min));
            int maxX = ClipX(Math3(x0, x1, x2, Math.Max));
            int maxY = ClipY(Math3(y0, y1, y2, Math.Max));

            float minTx = Math3(tv0.X, tv1.X, tv2.X, Math.Min);
            float minTy = Math3(tv0.Y, tv1.Y, tv2.Y, Math.Min);
            float maxTx = Math3(tv0.X, tv1.X, tv2.X, Math.Max);
            float maxTy = Math3(tv0.Y, tv1.Y, tv2.Y, Math.Max);

            double xmid = ((double)x0 + x1 + x2) / 3;
            double ymid = ((double)y0 + y1 + y2) / 3;

            var dirV0 = new Vector3((float)(x0 - xmid), (float)(y0 - ymid), 0);
            var dirV1 = new Vector3((float)(x1 - xmid), (float)(y1 - ymid), 0);

            var direction = Vector3.Cross(dirV0, dirV1).Z;
            if (direction < 0)
            {
                int buf;

                buf = x1;
                x1 = x2;
                x2 = buf;

                buf = y1;
                y1 = y2;
                y2 = buf;

                var zbuf = z1;
                z1 = z2;
                z2 = zbuf;

                Vector3 tbuf;
                tbuf = v1;
                v1 = v2;
                v2 = tbuf;
            }

            var b0 = new Vector2(v0.X - midX, v0.Y - midY);
            var b1 = new Vector2(v1.X - midX, v1.Y - midY);
            var b2 = new Vector2(v2.X - midX, v2.Y - midY);

            var plain = MakePlain(v0.X, v0.Y, v0.Z, v1.X, v1.Y, v1.Z, v2.X, v2.Y, v2.Z);

            var debugColor = Color.FromArgb(_random.Next(40, 256), _random.Next(40, 256), _random.Next(40, 256));
            var tx = minTx;
            var ty = minTy;
            var deltaTx = (maxTx - minTx) / (maxX - minX);
            var deltaTy = (maxTy - minTy) / (maxY - minY);

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
                        var curr = new Vector2(x - midX, y - midY);
                        var b0p = Vector2.Dot(b0, curr);
                        var b1p = Vector2.Dot(b1, curr);
                        var b2p = Vector2.Dot(b2, curr);

                        var z = plain(x, y);

                        if (z < zBuffer[x, y])
                            continue;

                        zBuffer[x, y] = z;

                        var tx1 = (int)Math.Round(tx * (_textureWidth - 1));
                        var ty1 = (int)Math.Round(ty * (_textureHeight - 1));
                        ty1 = _textureHeight - ty1 - 1;
//                        if (tx1 == 487 && ty1 == 59)
//                        {
//                            var a = 3;
//                            a += 3;
//                        }
//                        _textureDebugBitmap.SetPixel(tx1, ty1, debugColor);
                        var tbase = (ty1*_textureWidth + tx1)*4;
                        var color1 = Color.FromArgb(texture[tbase + 2], texture[tbase + 1], texture[tbase + 0]);

                        //                        color1 = Color.FromArgb(255 - (color.R/2), color1);
                        var intense = color.R/255f;
//                        intense = 1;
                        var foo = ((_height - y - 1)*_width+x)*4;
                        data[foo + 2] = (byte) (color1.R*intense);
                        data[foo + 1] = (byte) (color1.G*intense);
                        data[foo + 0] = (byte) (color1.B*intense);
//                        bmp.SetPixel(x, y, color1);
//                        data[foo + 2] = color.R;
//                        data[foo + 1] = color.G;
//                        data[foo + 0] = color.B;


                    }
                    ty += deltaTy;
                    sline1.StepY();
                    sline2.StepY();
                    sline3.StepY();
                }
                tx += deltaTx;
                ty = minTy;
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

        private static Func<double, double, double> MakePlain(double x0, double y0, double z0, double x1, double y1, double z1, double x2, double y2, double z2)
        {
            return (x, y) =>
            {
                double k = (-y1 * z0 + y2 * z0 + y0 * z1 - y2 * z1 - y0 * z2 + y1 * z2) / (x1 * y0 - x2 * y0 - x0 * y1 + x2 * y1 + x0 * y2 - x1 * y2);
                double l = (x1 * z0 - x2 * z0 - x0 * z1 + x2 * z1 + x0 * z2 - x1 * z2) / (x1 * y0 - x2 * y0 - x0 * y1 + x2 * y1 + x0 * y2 - x1 * y2);
                double m = (x2 * y1 * z0 - x1 * y2 * z0 - x2 * y0 * z1 + x0 * y2 * z1 + x1 * y0 * z2 - x0 * y1 * z2) / (x1 * y0 - x2 * y0 - x0 * y1 + x2 * y1 + x0 * y2 - x1 * y2);

                return k * x + l * y + m;
            };
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