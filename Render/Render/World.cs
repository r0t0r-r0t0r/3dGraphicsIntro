using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Render.Shaders;

namespace Render
{
    public class World
    {
        private readonly WorldObject _worldObject;

        public World(WorldObject worldObject)
        {
            _worldObject = worldObject;
        }

        public FlatRenderMode RenderMode { get; set; }
        public Vector3 LightDirection { get; set; }
        public Matrix4x4 ViewportTransform { get; set; }
        public Matrix4x4 ProjectionTransform { get; set; }
        public Matrix4x4 ViewTransform { get; set; }

        public Matrix4x4 Transformation
        {
            get
            {
                return ViewportTransform.Mul(ProjectionTransform.Mul(ViewTransform.Mul(WorldObject.ModelTransform)));
            }
        }

        public Matrix4x4 NormalTransform
        {
            get
            {
                var tempTransform = ProjectionTransform.Mul(ViewTransform.Mul(WorldObject.ModelTransform));
                tempTransform = Matrix4x4.Transpose(tempTransform);
                Matrix4x4 transformation;
                if (!Matrix4x4.Invert(tempTransform, out transformation))
                {
                    throw new ArgumentException();
                }

                return transformation;
            }
        }

        public WorldObject WorldObject
        {
            get { return _worldObject; }
        }
    }

    public class WorldObject
    {
        private readonly Model _model;

        public WorldObject(Model model)
        {
            _model = model;
        }

        public Model Model
        {
            get { return _model; }
        }

        public IShader Shader { get; set; }
        public Matrix4x4 ModelTransform { get; set; }
    }

    public static class WorldUtils
    {
        private const string RootDir = @"Model";

        private static readonly Model Model = ModelLoader.LoadModel(
            RootDir,
            "african_head.obj",
            "african_head_diffuse.bmp",
            "african_head_nm.png",
            "african_head_spec.bmp");

        public static World CreateWorld(RenderSettings settings, int width, int height)
        {
            IShader shader;

            if (!settings.RenderMode.UseFilling)
            {
                shader = new EmptyShader();
            }
            else
            {
                IShader innerShader;
                if (settings.RenderMode.FillMode.UseTexture)
                {
                    innerShader = new TextureShader();
                }
                else
                {
                    innerShader = new SolidColorShader();
                }

                switch (settings.RenderMode.LightMode)
                {
                    case LightMode.None:
                        shader = innerShader;
                        break;
                    case LightMode.Simple:
                        shader = new SimpleShader(innerShader);
                        break;
                    case LightMode.Gouraud:
                        shader = new GouraudShader(innerShader);
                        break;
                    case LightMode.Phong:
                        shader = new PhongShader(innerShader);
                        break;
                    case LightMode.NormalMapping:
                        shader = new NormalMappingShader(innerShader);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            FlatRenderMode renderMode;
            if (settings.RenderMode.UseBorders && settings.RenderMode.UseFilling)
            {
                renderMode = FlatRenderMode.BordersAndFill;
            }
            else if (settings.RenderMode.UseBorders)
            {
                renderMode = FlatRenderMode.Borders;
            }
            else
            {
                renderMode = FlatRenderMode.Fill;
            }
            return CreateWorld(width, height, settings.ViewportScale, settings.PerspectiveProjection, shader, renderMode);
        }

        private static World CreateWorld(int width, int height, float viewportScale, bool usePerspectiveProjection, IShader shader, FlatRenderMode renderMode)
        {
            var center = new Vector3(0, 0, 0);
            var eye = new Vector3(3, 3, 10);
            var up = new Vector3(0, 1, 0);

            var actualWidth = (int) (width*viewportScale);
            var actualHeight = (int) (height*viewportScale);
            var actualXmin = (width - actualWidth)/2;
            var actualYmin = (height - actualHeight)/2;

            var distance = new Vector3(center.X - eye.X, center.Y - eye.Y, center.Z - eye.Z).Length();

            var viewport = Matrix4x4Utils.Viewport(actualXmin, actualYmin, actualWidth, actualHeight);
            var projection = usePerspectiveProjection
                ? Matrix4x4Utils.OrthographicProjection(distance)
                : Matrix4x4.Identity;
            var view = Matrix4x4Utils.LookAt(center, eye, up);

            return CreateWorld(projection, viewport, view, shader, renderMode);
        }

        private static World CreateWorld(Matrix4x4 projection, Matrix4x4 viewport, Matrix4x4 view, IShader shader, FlatRenderMode renderMode)
        {
            var worldObject = new WorldObject(Model)
            {
                ModelTransform = Matrix4x4.Identity,
                Shader = shader
            };

            return new World(worldObject)
            {
                RenderMode = renderMode,
                LightDirection = Vector3.Normalize(new Vector3(8f, 6f, 10f)),
                ViewTransform = view,
                ProjectionTransform = projection,
                ViewportTransform = viewport,
            };
        }
    }
}
