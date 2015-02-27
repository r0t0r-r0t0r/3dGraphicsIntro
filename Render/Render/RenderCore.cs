using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Render
{
    public class RenderCore
    {
//        private const string RootDir = @"D:\Users\rotor\Documents\";
        private const string RootDir = @"C:\Users\p-afanasyev\Documents\";

        private readonly Bitmap _bitmap = new Bitmap(800, 800, PixelFormat.Format32bppRgb);

        private readonly Model _model = ModelLoader.LoadModel(RootDir + "african_head.obj");
        private readonly Bitmap _texture = new Bitmap(Image.FromFile(RootDir + "african_head_diffuse.bmp"));

        private readonly List<IRender> _renders = new List<IRender>
        {
            new SolidRender(),
            new WireRender()
        };


        public Bitmap Render(RenderSettings settings)
        {
            var cameraZPosition = settings.CameraZPosition;
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
                render.Init(_bitmap.Width, _bitmap.Height);
            }
            var data = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
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
                var length = _bitmap.Width*_bitmap.Height*4;
                for (var i = 0; i < length; i++)
                {
                    rawData[i] = 0;
                }
                Draw(_model, rawData, _bitmap.Width, _bitmap.Height, renders, cameraZPosition, usePerspectiveProjection, cameraDirection, shader);
            }
            _bitmap.UnlockBits(data);
            _texture.UnlockBits(textureData);

            return _bitmap;
        }

        private unsafe static void Draw(Model model, byte* data, int width, int height, List<IRender> renders, float cameraZPosition, bool usePerspectiveProjection, Vector3 cameraDirection, IPixelShader shader)
        {
            float c = cameraZPosition;

            foreach (var face in model.Faces)
            {
                var screenCoords = new Vector3[3];

                for (var j = 0; j < 3; j++)
                {
                    var worldCoord = model.Vertices[face[j]];
                    var w = 1 - worldCoord.Z/c;
                    var g = new Vector3(worldCoord.X*300, worldCoord.Y*300, worldCoord.Z*300);
                    if (usePerspectiveProjection)
                    {
                        screenCoords[j] = new Vector3(g.X/w, g.Y/w, g.Z/w);
                    }
                    else
                    {
                        screenCoords[j] = new Vector3(g.X, g.Y, g.Z);
                    }

                    screenCoords[j] = new Vector3(screenCoords[j].X/(cameraZPosition*0.1f) + width/2,
                        screenCoords[j].Y/(cameraZPosition*0.1f) + height/2, screenCoords[j].Z);
                }

                var v0 = model.Vertices[face[0]];
                var v1 = model.Vertices[face[1]];
                var v2 = model.Vertices[face[2]];

                var foo1 = Vector3.Subtract(v1, v0);
                var foo2 = Vector3.Subtract(v2, v1);

                var normal = Vector3.Cross(foo1, foo2);
                normal = Vector3.Normalize(normal);

                var bar = Vector3.Dot(normal, cameraDirection);

                if (bar <= 0)
                    continue;

                foreach (var render in renders)
                {
                    render.Draw(face, screenCoords[0], screenCoords[1], screenCoords[2], data, shader);
                }
            }
        }
    }
}
