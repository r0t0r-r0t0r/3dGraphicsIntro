using System;
using Disunity.Data;

namespace Disunity.WorldManaging.Shading
{
    public static class TextureUtils
    {
        public static int ClipX(this Texture texture, int x)
        {
            return Math.Min(Math.Max(0, x), texture.Width - 1);
        }

        public static int ClipY(this Texture texture, int y)
        {
            return Math.Min(Math.Max(0, y), texture.Height - 1);
        }
    }
}