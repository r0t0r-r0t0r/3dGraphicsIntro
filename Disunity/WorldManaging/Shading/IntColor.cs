using System.Runtime.InteropServices;

namespace Disunity.WorldManaging.Shading
{
    [StructLayout(LayoutKind.Explicit)]
    public struct IntColor
    {
        [FieldOffset(0)]
        public int Color;
        
        [FieldOffset(0)]
        public byte Blue;

        [FieldOffset(1)]
        public byte Green;

        [FieldOffset(2)]
        public byte Red;

        [FieldOffset(3)]
        public byte Alpha;
    }
}