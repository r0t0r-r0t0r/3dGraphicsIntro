using Disunity.Data;

namespace Disunity.WorldManaging.StateChanging
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
}