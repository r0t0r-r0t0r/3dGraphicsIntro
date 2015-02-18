using System;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render
{
    public class TextureRender: IRender
    {
        private Model _model;
        private unsafe byte* _texture;
        private double[,] zBuffer;
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
            zBuffer = new double[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    zBuffer[x, y] = double.NegativeInfinity;
                }
            }
        }

        public void Finish()
        {
//            _textureDebugBitmap.Save(_rootDir + @"debug.bmp");
        }

        unsafe public void Draw(Face face, Vector3 a, Vector3 b, Vector3 c, byte* data, byte lightLevel)
        {
            var bar1 = lightLevel;
            var screenCoords = new[] { a, b, c };
            var textureVertices = Enumerable.Range(0, 3).Select(face.GetVtIndex).Select(x => _model.TextureVertices[x]).ToArray();

            Triangle(screenCoords[0], screenCoords[1], screenCoords[2], textureVertices, data, Color.FromArgb(bar1, bar1, bar1), _texture, zBuffer);
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

            var tv0 = textureVertices[0];
            var tv1 = textureVertices[1];
            var tv2 = textureVertices[2];

            int minX = Math3(x0, x1, x2, Math.Min);
            int minY = Math3(y0, y1, y2, Math.Min);
            int maxX = Math3(x0, x1, x2, Math.Max);
            int maxY = Math3(y0, y1, y2, Math.Max);

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

                Vector3 tbuf;
                tbuf = v1;
                v1 = v2;
                v2 = tbuf;
            }

            var line1 = MakeLineFunc(x0, y0, x1, y1);
            var line2 = MakeLineFunc(x1, y1, x2, y2);
            var line3 = MakeLineFunc(x2, y2, x0, y0);

            var plain = MakePlain(x0, y0, z0, x1, y1, z1, x2, y2, z2);

            var debugColor = Color.FromArgb(_random.Next(40, 256), _random.Next(40, 256), _random.Next(40, 256));
            var tx = minTx;
            var ty = minTy;
            var deltaTx = (maxTx - minTx) / (maxX - minX);
            var deltaTy = (maxTy - minTy) / (maxY - minY);
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (!(x >= 0 && x < _width && y >= 0 && y < _height))
                        continue;

                    var curr1 = line1(x, y);
                    var curr2 = line2(x, y);
                    var curr3 = line3(x, y);

                    var z = plain(x, y);

                    if (curr1 >= 0 && curr2 >= 0 && curr3 >= 0 && z >= zBuffer[x, y])
                    {
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
                        var foo = ((_height - y - 1)*_width+x)*4;
                        data[foo + 2] = color1.R;
                        data[foo + 1] = color1.G;
                        data[foo + 0] = color1.B;
//                        bmp.SetPixel(x, y, color1);

                    }
                    ty += deltaTy;
                }
                tx += deltaTx;
                ty = minTy;
            }
        }

        private static T Math3<T>(T a, T b, T c, Func<T, T, T> func)
        {
            var result = func(a, b);
            return func(result, c);
        }

        private static Func<double, double, double> MakeLineFunc(double x0, double y0, double x1, double y1)
        {
            return (x, y) => y * (x1 - x0) - x * (y1 - y0) - y0 * (x1 - x0) + x0 * (y1 - y0);
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
    }
}