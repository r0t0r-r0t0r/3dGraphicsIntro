using System;
using System.Collections.Generic;
using System.Numerics;
using Disunity.App.Shaders;
using Disunity.Data;
using Disunity.Data.Shaders;

namespace Disunity.App
{
    public interface IWorldStateChangeAware<out T>
    {
        T Empty();

        T ChangeRenderMode(RenderMode mode);

        T ChangeLightMode(LightMode mode);

        T ChangeFillMode(FillMode mode);

        T ChangePerspectiveProjection(bool projection);

        T ChangeViewportScale(float scale);

        T ChangeViewportSize(int width, int height);

        T ChangeLightPosition(int x, int y);

        T ChangeModelRotation(float x, float y);
    }

    public abstract class WorldStateChange
    {
        private WorldStateChange()
        {
            
        }

        public abstract T Perform<T>(IWorldStateChangeAware<T> aware);

        private class EmptyChange: WorldStateChange
        {
            public override T Perform<T>(IWorldStateChangeAware<T> aware)
            {
                return aware.Empty();
            }
        }

        private class RenderModeChange: WorldStateChange
        {
            private readonly RenderMode _renderMode;

            public RenderModeChange(RenderMode renderMode)
            {
                _renderMode = renderMode;
            }

            public override T Perform<T>(IWorldStateChangeAware<T> aware)
            {
                return aware.ChangeRenderMode(_renderMode);
            }
        }

        private class LightModeChange : WorldStateChange
        {
            private readonly LightMode _lightMode;

            public LightModeChange(LightMode lightMode)
            {
                _lightMode = lightMode;
            }

            public override T Perform<T>(IWorldStateChangeAware<T> aware)
            {
                return aware.ChangeLightMode(_lightMode);
            }
        }

        private class FillModeChange : WorldStateChange
        {
            private readonly FillMode _fillMode;

            public FillModeChange(FillMode fillMode)
            {
                _fillMode = fillMode;
            }

            public override T Perform<T>(IWorldStateChangeAware<T> aware)
            {
                return aware.ChangeFillMode(_fillMode);
            }
        }

        private class ProjectionChange: WorldStateChange
        {
            private readonly bool _projection;

            public ProjectionChange(bool projection)
            {
                _projection = projection;
            }

            public override T Perform<T>(IWorldStateChangeAware<T> aware)
            {
                return aware.ChangePerspectiveProjection(_projection);
            }
        }

        private class ViewportScaleChange: WorldStateChange
        {
            private readonly float _viewportScale;

            public ViewportScaleChange(float viewportScale)
            {
                _viewportScale = viewportScale;
            }

            public override T Perform<T>(IWorldStateChangeAware<T> aware)
            {
                return aware.ChangeViewportScale(_viewportScale);
            }
        }

        private class ViewportSizeChange: WorldStateChange
        {
            private readonly int _width;
            private readonly int _height;

            public ViewportSizeChange(int width, int height)
            {
                _width = width;
                _height = height;
            }

            public override T Perform<T>(IWorldStateChangeAware<T> aware)
            {
                return aware.ChangeViewportSize(_width, _height);
            }
        }

        private class LightPositionChange: WorldStateChange
        {
            private readonly int _x;
            private readonly int _y;

            public LightPositionChange(int x, int y)
            {
                _x = x;
                _y = y;
            }

            public override T Perform<T>(IWorldStateChangeAware<T> aware)
            {
                return aware.ChangeLightPosition(_x, _y);
            }
        }

        private class ModelRotationChange: WorldStateChange
        {
            private readonly float _x;
            private readonly float _y;

            public ModelRotationChange(float x, float y)
            {
                _x = x;
                _y = y;
            }

            public override T Perform<T>(IWorldStateChangeAware<T> aware)
            {
                return aware.ChangeModelRotation(_x, _y);
            }
        }

        public static WorldStateChange Empty = new EmptyChange();

        public static WorldStateChange ChangeRenderMode(RenderMode mode)
        {
            return new RenderModeChange(mode);
        }

        public static WorldStateChange ChangeLightMode(LightMode mode)
        {
            return new LightModeChange(mode);
        }

        public static WorldStateChange ChangeFillMode(FillMode mode)
        {
            return new FillModeChange(mode);
        }

        public static WorldStateChange ChangePerspectiveProjection(bool projection)
        {
            return new ProjectionChange(projection);
        }

        public static WorldStateChange ChangeViewportScale(float scale)
        {
            return new ViewportScaleChange(scale);
        }

        public static WorldStateChange ChangeViewportSize(int width, int height)
        {
            return new ViewportSizeChange(width, height);
        }

        public static WorldStateChange ChangeLightPosition(int x, int y)
        {
            return new LightPositionChange(x, y);
        }

        public static WorldStateChange ChangeModelRotation(float x, float y)
        {
            return new ModelRotationChange(x, y);
        }
    }
    
    public class WorldStateChangeAware: IWorldStateChangeAware<Func<WorldState, WorldState>>
    {
        public static readonly WorldStateChangeAware Instance = new WorldStateChangeAware();

        public Func<WorldState, WorldState> Empty()
        {
            return s => s;
        }

        public Func<WorldState, WorldState> ChangeRenderMode(RenderMode mode)
        {
            return s => s.Copy(renderMode: mode);
        }

        public Func<WorldState, WorldState> ChangeLightMode(LightMode mode)
        {
            return s => s.Copy(lightMode: mode);
        }

        public Func<WorldState, WorldState> ChangeFillMode(FillMode mode)
        {
            return s => s.Copy(fillMode: mode);
        }

        public Func<WorldState, WorldState> ChangePerspectiveProjection(bool projection)
        {
            return s => s.Copy(perspectiveProjection: projection);
        }

        public Func<WorldState, WorldState> ChangeViewportScale(float scale)
        {
            return s => s.Copy(viewportScale: scale);
        }

        public Func<WorldState, WorldState> ChangeViewportSize(int width, int height)
        {
            return s => s.Copy(viewportWidth: width, viewportHeight: height);
        }

        public Func<WorldState, WorldState> ChangeLightPosition(int x, int y)
        {
            return s => s.Copy(viewportLightX: x, viewportLightY: y);
        }

        public Func<WorldState, WorldState> ChangeModelRotation(float x, float y)
        {
            return s => s.Copy(modelRotationX: x, modelRotationY: y);
        }
    }

    public class WorldState
    {
        private readonly RenderMode _renderMode;
        private readonly LightMode _lightMode;
        private readonly FillMode _fillMode;
        private readonly bool _perspectiveProjection;
        private readonly float _viewportScale;
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;
        private readonly int _viewportLightX;
        private readonly int _viewportLightY;
        private readonly float _modelRotationX;
        private readonly float _modelRotationY;

        public WorldState(RenderMode renderMode, LightMode lightMode, FillMode fillMode, bool perspectiveProjection, float viewportScale, int viewportWidth, int viewportHeight, int viewportLightX, int viewportLightY, float modelRotationX, float modelRotationY)
        {
            _renderMode = renderMode;
            _lightMode = lightMode;
            _fillMode = fillMode;
            _perspectiveProjection = perspectiveProjection;
            _viewportScale = viewportScale;
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            _viewportLightX = viewportLightX;
            _viewportLightY = viewportLightY;
            _modelRotationX = modelRotationX;
            _modelRotationY = modelRotationY;
        }

        public RenderMode RenderMode
        {
            get { return _renderMode; }
        }

        public LightMode LightMode
        {
            get { return _lightMode; }
        }

        public FillMode FillMode
        {
            get { return _fillMode; }
        }

        public bool PerspectiveProjection
        {
            get { return _perspectiveProjection; }
        }

        public float ViewportScale
        {
            get { return _viewportScale; }
        }

        public int ViewportWidth
        {
            get { return _viewportWidth; }
        }

        public int ViewportHeight
        {
            get { return _viewportHeight; }
        }

        public int ViewportLightX
        {
            get { return _viewportLightX; }
        }

        public int ViewportLightY
        {
            get { return _viewportLightY; }
        }

        public float ModelRotationX
        {
            get { return _modelRotationX; }
        }

        public float ModelRotationY
        {
            get { return _modelRotationY; }
        }

        public WorldState Copy(RenderMode? renderMode = null, LightMode? lightMode = null, FillMode? fillMode = null,
            bool? perspectiveProjection = null, float? viewportScale = null, int? viewportWidth = null,
            int? viewportHeight = null, int? viewportLightX = null, int? viewportLightY = null,
            float? modelRotationX = null, float? modelRotationY = null)
        {
            var newRenderMode = renderMode ?? _renderMode;
            var newLightMode = lightMode ?? _lightMode;
            var newFillMode = fillMode ?? _fillMode;
            var newPerspectiveProjection = perspectiveProjection ?? _perspectiveProjection;
            var newViewportScale = viewportScale ?? _viewportScale;
            var newViewportWidth = viewportWidth ?? _viewportWidth;
            var newVieportHeight = viewportHeight ?? _viewportHeight;
            var newViewportLightX = viewportLightX ?? _viewportLightX;
            var newViewportLightY = viewportLightY ?? _viewportLightY;
            var newModelRotationX = modelRotationX ?? _modelRotationX;
            var newModelRotationY = modelRotationY ?? _modelRotationY;

            return new WorldState(newRenderMode, newLightMode, newFillMode, newPerspectiveProjection, newViewportScale,
                newViewportWidth, newVieportHeight, newViewportLightX, newViewportLightY, newModelRotationX,
                newModelRotationY);
        }

        public IReadOnlyCollection<WorldStateChange> AsChanges()
        {
            return new List<WorldStateChange>
            {
                WorldStateChange.ChangeRenderMode(_renderMode),
                WorldStateChange.ChangeLightMode(_lightMode),
                WorldStateChange.ChangeFillMode(_fillMode),
                WorldStateChange.ChangePerspectiveProjection(_perspectiveProjection),
                WorldStateChange.ChangeViewportScale(_viewportScale),
                WorldStateChange.ChangeViewportSize(_viewportWidth, _viewportHeight),
                WorldStateChange.ChangeLightPosition(_viewportLightX, _viewportLightY),
                WorldStateChange.ChangeModelRotation(_modelRotationX, _modelRotationY)
            }.AsReadOnly();
        }
    }

    public class WorldBuilder
    {
        private const string RootDir = @"Model";

        private static readonly Model Model = ModelLoader.LoadModel(
            RootDir,
            "african_head.obj",
            "african_head_diffuse.bmp",
            "african_head_nm.png",
            "african_head_spec.bmp");

        public static World BuildWorld(WorldState state)
        {
            var center = new Vector3(0, 0, 0);
            var eye = new Vector3(0, 0, 10);
            var up = new Vector3(0, 1, 0);

            var worldObject = new WorldObject(Model)
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
