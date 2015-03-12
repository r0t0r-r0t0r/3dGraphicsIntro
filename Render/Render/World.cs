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
        private Matrix4x4 _normalTransform;
        private Matrix4x4 _projectionTransform;
        private Matrix4x4 _viewTransform;

        public World(WorldObject worldObject)
        {
            _worldObject = worldObject;
        }

        public FlatRenderMode RenderMode { get; set; }
        public Vector3 LightDirection { get; set; }
        public Matrix4x4 ViewportTransform { get; set; }
        public Vector3 CameraDirection { get; set; }

        public Matrix4x4 ProjectionTransform
        {
            get { return _projectionTransform; }
            set
            {
                _projectionTransform = value;
                RecalculateNormalTransform();
            }
        }

        public Matrix4x4 ViewTransform
        {
            get { return _viewTransform; }
            set
            {
                _viewTransform = value;
                RecalculateNormalTransform();
            }
        }

        public Matrix4x4 Transformation
        {
            get
            {
                return ViewportTransform.Mul(ProjectionTransform.Mul(ViewTransform.Mul(WorldObject.ModelTransform)));
            }
        }

        public Matrix4x4 NormalTransform
        {
            get { return _normalTransform; }
        }

        private void RecalculateNormalTransform()
        {
            //TODO: Consider WorldObject.NormalTransform
            var tempTransform = ProjectionTransform.Mul(ViewTransform.Mul(WorldObject.ModelTransform));
            tempTransform = Matrix4x4.Transpose(tempTransform);
            Matrix4x4 transformation;
            if (!Matrix4x4.Invert(tempTransform, out transformation))
            {
//                throw new ArgumentException();
                //TODO: HACK
                _normalTransform = Matrix4x4.Identity;
            }
            _normalTransform = transformation;
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

        public Func<Shader> ShaderFactory { get; set; }
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
            Func<Shader> shaderFactory;

            Func<Shader> innerShaderFactory;
            if (settings.RenderMode.FillMode.UseTexture)
            {
                innerShaderFactory = () => new TextureShader();
            }
            else
            {
                innerShaderFactory = () => new SolidColorShader();
            }

            switch (settings.RenderMode.LightMode)
            {
                case LightMode.None:
                    shaderFactory = innerShaderFactory;
                    break;
                case LightMode.Simple:
                    shaderFactory = () => new SimpleShader(innerShaderFactory());
                    break;
                case LightMode.Gouraud:
                    shaderFactory = () => new GouraudShader(innerShaderFactory());
                    break;
                case LightMode.Phong:
                    shaderFactory = () => new PhongShader(innerShaderFactory());
                    break;
                case LightMode.NormalMapping:
                    shaderFactory = () => new NormalMappingShader(innerShaderFactory());
                    break;
                default:
                    throw new ArgumentException();
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
            return CreateWorld(width, height, settings.ViewportScale, settings.PerspectiveProjection, shaderFactory, renderMode, settings.LightDirection);
        }

        private static World CreateWorld(int width, int height, float viewportScale, bool usePerspectiveProjection, Func<Shader> shaderFactory, FlatRenderMode renderMode, Vector3 lightDirection)
        {
            var center = new Vector3(0, 0, 0);
            var eye = new Vector3(3, 3, 10);
            var up = new Vector3(0, 1, 0);

            var v = Vector3.Normalize(Vector3.Subtract(eye, center));

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

            return CreateWorld(projection, viewport, view, shaderFactory, renderMode, v, lightDirection);
        }

        private static World CreateWorld(Matrix4x4 projection, Matrix4x4 viewport, Matrix4x4 view, Func<Shader> shaderFactory, FlatRenderMode renderMode, Vector3 v, Vector3 lightDirection)
        {
            var worldObject = new WorldObject(Model)
            {
                ModelTransform = Matrix4x4.Identity,
                ShaderFactory = shaderFactory
            };

            return new World(worldObject)
            {
                RenderMode = renderMode,
                LightDirection = Vector3.Normalize(lightDirection),
                ViewTransform = view,
                ProjectionTransform = projection,
                ViewportTransform = viewport,
                CameraDirection = v
            };
        }
    }
}
