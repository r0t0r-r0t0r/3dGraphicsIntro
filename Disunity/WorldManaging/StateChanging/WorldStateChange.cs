using Disunity.Data;

namespace Disunity.WorldManaging.StateChanging
{
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
}