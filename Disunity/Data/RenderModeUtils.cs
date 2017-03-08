namespace Disunity.Data
{
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
}