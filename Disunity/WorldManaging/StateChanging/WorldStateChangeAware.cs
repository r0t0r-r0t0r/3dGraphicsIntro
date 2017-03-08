using System;
using Disunity.Data;

namespace Disunity.WorldManaging.StateChanging
{
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
}