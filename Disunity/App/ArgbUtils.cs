namespace Disunity.App
{
    public static class ArgbUtils
    {
        public static byte GetRed(this int argb)
        {
            return (byte)((argb & 0x00ff0000) >> 16);
        }
    }
}