using System.Collections.Generic;
using Disunity.Data;

namespace Disunity.WorldManaging.StateChanging
{
    public class WorldState
    {
        public WorldState(RenderMode renderMode, LightMode lightMode, FillMode fillMode, bool perspectiveProjection, float viewportScale, int viewportWidth, int viewportHeight, int viewportLightX, int viewportLightY, float modelRotationX, float modelRotationY)
        {
            RenderMode = renderMode;
            LightMode = lightMode;
            FillMode = fillMode;
            PerspectiveProjection = perspectiveProjection;
            ViewportScale = viewportScale;
            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;
            ViewportLightX = viewportLightX;
            ViewportLightY = viewportLightY;
            ModelRotationX = modelRotationX;
            ModelRotationY = modelRotationY;
        }

        public RenderMode RenderMode { get; }
        public LightMode LightMode { get; }
        public FillMode FillMode { get; }
        public bool PerspectiveProjection { get; }
        public float ViewportScale { get; }
        public int ViewportWidth { get; }
        public int ViewportHeight { get; }
        public int ViewportLightX { get; }
        public int ViewportLightY { get; }
        public float ModelRotationX { get; }
        public float ModelRotationY { get; }

        public WorldState Copy(RenderMode? renderMode = null, LightMode? lightMode = null, FillMode? fillMode = null,
            bool? perspectiveProjection = null, float? viewportScale = null, int? viewportWidth = null,
            int? viewportHeight = null, int? viewportLightX = null, int? viewportLightY = null,
            float? modelRotationX = null, float? modelRotationY = null)
        {
            var newRenderMode = renderMode ?? RenderMode;
            var newLightMode = lightMode ?? LightMode;
            var newFillMode = fillMode ?? FillMode;
            var newPerspectiveProjection = perspectiveProjection ?? PerspectiveProjection;
            var newViewportScale = viewportScale ?? ViewportScale;
            var newViewportWidth = viewportWidth ?? ViewportWidth;
            var newVieportHeight = viewportHeight ?? ViewportHeight;
            var newViewportLightX = viewportLightX ?? ViewportLightX;
            var newViewportLightY = viewportLightY ?? ViewportLightY;
            var newModelRotationX = modelRotationX ?? ModelRotationX;
            var newModelRotationY = modelRotationY ?? ModelRotationY;

            return new WorldState(newRenderMode, newLightMode, newFillMode, newPerspectiveProjection, newViewportScale,
                newViewportWidth, newVieportHeight, newViewportLightX, newViewportLightY, newModelRotationX,
                newModelRotationY);
        }

        public IReadOnlyCollection<WorldStateChange> AsChanges()
        {
            return new List<WorldStateChange>
            {
                WorldStateChange.ChangeRenderMode(RenderMode),
                WorldStateChange.ChangeLightMode(LightMode),
                WorldStateChange.ChangeFillMode(FillMode),
                WorldStateChange.ChangePerspectiveProjection(PerspectiveProjection),
                WorldStateChange.ChangeViewportScale(ViewportScale),
                WorldStateChange.ChangeViewportSize(ViewportWidth, ViewportHeight),
                WorldStateChange.ChangeLightPosition(ViewportLightX, ViewportLightY),
                WorldStateChange.ChangeModelRotation(ModelRotationX, ModelRotationY)
            }.AsReadOnly();
        }
    }
}