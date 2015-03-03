using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Render
{
    public class RenderCore
    {
//        private const string RootDir = @"D:\Users\rotor\Documents\";
        private const string RootDir = @"C:\Users\p-afanasyev\Documents\";

        private readonly Model _model = ModelLoader.LoadModel(RootDir + "african_head.obj");
        private readonly Bitmap _texture = new Bitmap(Image.FromFile(RootDir + "african_head_diffuse.bmp"));

        private readonly List<IRender> _renders = new List<IRender>
        {
            new SolidRender(),
            new WireRender()
        };


        public void Render(RenderSettings settings, Bitmap bitmap)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;

            var viewportScale = settings.ViewportScale;
            var usePerspectiveProjection = settings.PerspectiveProjection;
            List<IRender> renders = new List<IRender>();
            if (settings.RenderMode.UseFilling)
            {
                renders.Add(_renders[0]);
            }
            if (settings.RenderMode.UseBorders)
            {
                renders.Add(_renders[1]);
            }

            var textureData = _texture.LockBits(new Rectangle(0, 0, _texture.Width, _texture.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            var cameraDirection = new Vector3(0, 0, 1);
            var light = new Vector3(0.6f, 0.6f, 0.75f);
            light = Vector3.Normalize(light);
            foreach (var render in renders)
            {
                render.Init(width, height);
            }
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            unsafe
            {
                IPixelShader shader;

                if (!settings.RenderMode.UseFilling)
                {
                    shader = new EmptyPixelShader();
                }
                else
                {
                    IPixelShader innerShader;
                    if (settings.RenderMode.FillMode.UseTexture)
                    {
                        innerShader = new TexturePixelShader(_model, (byte*) textureData.Scan0, _texture.Width, _texture.Height);
                    }
                    else
                    {
                        innerShader = new SolidColorPixelShader(settings.RenderMode.FillMode.Color);
                    }

                    switch (settings.RenderMode.LightMode)
                    {
                        case LightMode.None:
                            shader = innerShader;
                            break;
                        case LightMode.Simple:
                            shader = new SimplePixelShader(_model, light, innerShader);
                            break;
                        case LightMode.Gouraud:
                            shader = new GouraudPixelShader(_model, light, innerShader);
                            break;
                        case LightMode.Phong:
                            shader = new PhongPixelShader(_model, light, innerShader);
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }

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
                            new Task(() => Draw(_model, rawData, width, height, renders, viewportScale,
                                usePerspectiveProjection, cameraDirection, shader, region.Item1, region.Item2)))
                        .ToArray();
                foreach (var task in tasks)
                {
                    task.Start();
                }
                Task.WaitAll(tasks);
            }
            bitmap.UnlockBits(data);
            _texture.UnlockBits(textureData);
        }

        private unsafe static void Draw(Model model, byte* data, int width, int height, List<IRender> renders, float viewportScale, bool usePerspectiveProjection, Vector3 cameraDirection, IPixelShader shader, int startY, int endY)
        {
            var center = new Vector3(0, 0, 0);
            var eye = new Vector3(0, 0, 10);
            var up = new Vector3(0, 1, 0);

            var actualWidth = (int) (width*viewportScale);
            var actualHeight = (int) (height*viewportScale);
            var actualXmin = (width - actualWidth)/2;
            var actualYmin = (height - actualHeight)/2;

            var distance = new Vector3(center.X - eye.X, center.Y - eye.Y, center.Z - eye.Z).Length();

            var viewport = Viewport(actualXmin, actualYmin, actualWidth, actualHeight);
            var projection = usePerspectiveProjection ? Projection(distance) : Matrix4x4.Identity;
            var view = LookAt(center, eye, up);
            var modelTransform = Model();
            var transform = Mul(viewport, Mul(projection, Mul(view, modelTransform)));

            foreach (var face in model.Faces)
            {
                var screenCoords = new Vector3[3];

                for (var j = 0; j < 3; j++)
                {
                    var modelCoord = model.Vertices[face[j]];

                    var r = Mul(transform, new Vector4(modelCoord, 1));
                    screenCoords[j] = new Vector3(r.X/r.W, r.Y/r.W, r.Z/r.W);
                }

                var v0 = screenCoords[0];
                var v1 = screenCoords[1];
                var v2 = screenCoords[2];

                var foo1 = Vector3.Subtract(v1, v0);
                var foo2 = Vector3.Subtract(v2, v1);

                var normal = Vector3.Cross(foo1, foo2);
                normal = Vector3.Normalize(normal);

                var bar = Vector3.Dot(normal, cameraDirection);

                if (bar <= 0)
                    continue;

                foreach (var render in renders)
                {
                    render.Draw(face, screenCoords[0], screenCoords[1], screenCoords[2], data, shader, startY, endY);
                }
            }
        }

        private static Matrix4x4 LookAt(Vector3 center, Vector3 eye, Vector3 up)
        {
            var z = Vector3.Normalize(eye - center);
            var x = Vector3.Normalize(Vector3.Cross(up, z));
            var y = Vector3.Normalize(Vector3.Cross(z, x));

            var result = new Matrix4x4(
                x.X, x.Y, x.Z, -center.X,
                y.X, y.Y, y.Z, -center.Y,
                z.X, z.Y, z.Z, -center.Z,
                0, 0, 0, 1
                );
            return result;
        }

        private static Matrix4x4 Viewport(int x, int y, int width, int height)
        {
            const int depth = 255;

            var hw = (float)width/2;
            var hh = (float)height/2;
            var hd = (float)depth/2;

            var result = new Matrix4x4(
                hw, 0, 0, x + hw,
                0, hh, 0, y + hh,
                0, 0, hd, hd,
                0, 0, 0, 1
                );
            return result;
        }

        private static Matrix4x4 Model()
        {
            var result = Matrix4x4.Identity;
            return result;
        }

        private static Matrix4x4 Projection(float d)
        {
            var result = new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, -1/d, 1
                );
            return result;
        }

        private static Vector4 Mul(Matrix4x4 m, Vector4 v)
        {
            var result = new Vector4(
                m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z + m.M14 * v.W,
                m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z + m.M24 * v.W,
                m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z + m.M34 * v.W,
                m.M41 * v.X + m.M42 * v.Y + m.M43 * v.Z + m.M44 * v.W
                );
            return result;
        }

        private static Matrix4x4 Mul(Matrix4x4 m1, Matrix4x4 m2)
        {
            var m11 = m1.M11*m2.M11 + m1.M12*m2.M21 + m1.M13*m2.M31 + m1.M14*m2.M41;
            var m12 = m1.M11*m2.M12 + m1.M12*m2.M22 + m1.M13*m2.M32 + m1.M14*m2.M42;
            var m13 = m1.M11*m2.M13 + m1.M12*m2.M23 + m1.M13*m2.M33 + m1.M14*m2.M43;
            var m14 = m1.M11*m2.M14 + m1.M12*m2.M24 + m1.M13*m2.M34 + m1.M14*m2.M44;

            var m21 = m1.M21*m2.M11 + m1.M22*m2.M21 + m1.M23*m2.M31 + m1.M24*m2.M41;
            var m22 = m1.M21*m2.M12 + m1.M22*m2.M22 + m1.M23*m2.M32 + m1.M24*m2.M42;
            var m23 = m1.M21*m2.M13 + m1.M22*m2.M23 + m1.M23*m2.M33 + m1.M24*m2.M43;
            var m24 = m1.M21*m2.M14 + m1.M22*m2.M24 + m1.M23*m2.M34 + m1.M24*m2.M44;

            var m31 = m1.M31*m2.M11 + m1.M32*m2.M21 + m1.M33*m2.M31 + m1.M34*m2.M41;
            var m32 = m1.M31*m2.M12 + m1.M32*m2.M22 + m1.M33*m2.M32 + m1.M34*m2.M42;
            var m33 = m1.M31*m2.M13 + m1.M32*m2.M23 + m1.M33*m2.M33 + m1.M34*m2.M43;
            var m34 = m1.M31*m2.M14 + m1.M32*m2.M24 + m1.M33*m2.M34 + m1.M34*m2.M44;

            var m41 = m1.M41*m2.M11 + m1.M42*m2.M21 + m1.M43*m2.M31 + m1.M44*m2.M41;
            var m42 = m1.M41*m2.M12 + m1.M42*m2.M22 + m1.M43*m2.M32 + m1.M44*m2.M42;
            var m43 = m1.M41*m2.M13 + m1.M42*m2.M23 + m1.M43*m2.M33 + m1.M44*m2.M43;
            var m44 = m1.M41*m2.M14 + m1.M42*m2.M24 + m1.M43*m2.M34 + m1.M44*m2.M44;

            var result = new Matrix4x4(
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
                );
            return result;
        }
    }
}
