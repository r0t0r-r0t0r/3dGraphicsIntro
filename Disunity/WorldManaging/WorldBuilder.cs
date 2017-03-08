using System;
using System.Numerics;
using Disunity.Data;
using Disunity.Data.Shaders;
using Disunity.WorldManaging.Shading.Shaders;
using Disunity.WorldManaging.StateChanging;

namespace Disunity.WorldManaging
{
    public class WorldBuilder
    {
        private readonly Model _model;

        public WorldBuilder(Model model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            _model = model;
        }

        public World BuildWorld(WorldState state)
        {
            var center = new Vector3(0, 0, 0);
            var eye = new Vector3(0, 0, 10);
            var up = new Vector3(0, 1, 0);

            var worldObject = new WorldObject(_model)
            {
                ShaderFactory = CreateShaderFactory(state.LightMode, state.FillMode),
                FirstPhaseShaderFactory = () => new ZBufferShader(),
                ModelTransform = Matrix4x4.CreateRotationX(state.ModelRotationY).Mul(Matrix4x4.CreateRotationY(state.ModelRotationX))
            };

            var world = new World(worldObject)
            {
                RenderMode = state.RenderMode,
                LightDirection = CreateLightDirection(state.ViewportLightX, state.ViewportLightY, state.ViewportWidth, state.ViewportHeight),

                ViewTransform = CreateViewTransform(center, eye, up),
                ProjectionTransform = CreateProjectionTransform(state.PerspectiveProjection, center, eye),
                ViewportTransform = CreateViewportTransform(state.ViewportWidth, state.ViewportHeight, state.ViewportScale),

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
}
