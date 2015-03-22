using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Render.Shaders;

namespace Render
{
    public class WorldBuilder
    {
        private const string RootDir = @"Model";

        private static readonly Model Model = ModelLoader.LoadModel(
            RootDir,
            "african_head.obj",
            "african_head_diffuse.bmp",
            "african_head_nm.png",
            "african_head_spec.bmp");

        private readonly int _viewportWidth;
        private readonly int _viewportHeight;

        public WorldBuilder(int width, int height)
        {
            RenderMode = RenderMode.Fill;
            LightMode = LightMode.NormalMapping;
            FillMode = FillMode.Texture;

            PerspectiveProjection = true;

            ViewportScale = 0.9f;
            _viewportWidth = width;
            _viewportHeight = height;

            ViewportLightX = _viewportWidth/2;
            ViewportLightY = _viewportHeight/2;
        }

        public RenderMode RenderMode { get; set; }
        public LightMode LightMode { get; set; }
        public FillMode FillMode { get; set; }

        public bool PerspectiveProjection { get; set; }

        public float ViewportScale { get; set; }

        public int ViewportLightX { get; set; }
        public int ViewportLightY { get; set; }

        public World BuildWorld()
        {
            var center = new Vector3(0, 0, 0);
            var eye = new Vector3(0, 0, 10);
            var up = new Vector3(0, 1, 0);

            var worldObject = new WorldObject(Model)
            {
                ShaderFactory = CreateShaderFactory(LightMode, FillMode),
                FirstPhaseShaderFactory = () => new ZBufferShader(),
                ModelTransform = Matrix4x4.Identity
            };

            var world = new World(worldObject)
            {
                RenderMode = RenderMode,
                LightDirection = CreateLightDirection(ViewportLightX, ViewportLightY, _viewportWidth, _viewportHeight),

                ViewTransform = CreateViewTransform(center, eye, up),
                ProjectionTransform = CreateProjectionTransform(PerspectiveProjection, center, eye),
                ViewportTransform = CreateViewportTransform(_viewportWidth, _viewportHeight, ViewportScale),

                CameraDirection = CreateCameraDirection(center, eye),

                TwoPhaseRendering = true
            };

            return world;
        }

        private static Vector3 CreateLightDirection(int viewportLightX, int viewportLightY, int viewportWidth, int viewportHeight)
        {
            var x = (float)viewportLightX / viewportWidth * 2 - 1;
            var y = (float)(viewportHeight - viewportLightY) / viewportHeight * 2 - 1;

            var vect = new Vector2(x, y);
            float z;
            if (vect.Length() >= 1f)
            {
                vect = Vector2.Normalize(vect);
                x = vect.X;
                y = vect.Y;
                z = 0;
            }
            else
            {
                z = (float)Math.Sqrt(1 - x * x - y * y);
            }

            return new Vector3(x, y, z);
        }
        
        private static Func<Shader> CreateShaderFactory(LightMode lightMode, FillMode fillMode)
        {
            Func<Shader> shaderFactory;

            Func<Shader> innerShaderFactory;
            if (fillMode == FillMode.Texture)
            {
                innerShaderFactory = () => new TextureShader();
            }
            else
            {
                innerShaderFactory = () => new SolidColorShader();
            }

            switch (lightMode)
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

            return shaderFactory;
        }

        private static Matrix4x4 CreateProjectionTransform(bool usePerspectiveProjection, Vector3 center, Vector3 eye)
        {
            if (!usePerspectiveProjection)
            {
                return Matrix4x4.Identity;
            }

            var distance = new Vector3(center.X - eye.X, center.Y - eye.Y, center.Z - eye.Z).Length();
            return Matrix4x4Utils.OrthographicProjection(distance);
        }

        private static Matrix4x4 CreateViewportTransform(int width, int height, float viewportScale)
        {
            var actualWidth = (int)(width * viewportScale);
            var actualHeight = (int)(height * viewportScale);
            var actualXmin = (width - actualWidth) / 2;
            var actualYmin = (height - actualHeight) / 2;

            var viewport = Matrix4x4Utils.Viewport(actualXmin, actualYmin, actualWidth, actualHeight);
            return viewport;
        }

        private static Matrix4x4 CreateViewTransform(Vector3 center, Vector3 eye, Vector3 up)
        {
            var view = Matrix4x4Utils.LookAt(center, eye, up);
            return view;
        }

        private static Vector3 CreateCameraDirection(Vector3 center, Vector3 eye)
        {
            return Vector3.Normalize(Vector3.Subtract(eye, center));
        }

    }

    public enum RenderMode
    {
        Borders,
        Fill,
        BordersAndFill
    }

    public static class RenderModeUtils
    {
        public static bool UseFill(this RenderMode renderMode)
        {
            return renderMode == RenderMode.Fill || renderMode == RenderMode.BordersAndFill;
        }

        public static bool UseBorders(this RenderMode renderMode)
        {
            return renderMode == RenderMode.Borders || renderMode == RenderMode.BordersAndFill;
        }
    }

    public enum LightMode
    {
        None,
        Simple,
        Gouraud,
        Phong,
        NormalMapping
    }

    public enum FillMode
    {
        SolidColor,
        Texture
    }
}
