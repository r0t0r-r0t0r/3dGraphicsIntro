using System.Collections.Generic;
using System.Numerics;

namespace Disunity.WorldManaging.Shading
{
    public static class ShaderStateUtils
    {
        public static void Push(this List<float>[] list, int vert, float value)
        {
            list[vert].Add(value);
        }

        public static void Push(this List<float> list, float value)
        {
            list.Add(value);
        }

        public static void Push(this List<float> list, Vector3 vector)
        {
            list.Add(vector.X);
            list.Add(vector.Y);
            list.Add(vector.Z);
        }

        public static Vector3 PopVector3(this List<float> list)
        {
            var xIndex = list.Count - 3;
            var x = list[xIndex];
            var y = list[xIndex + 1];
            var z = list[xIndex + 2];

            list.RemoveRange(xIndex, 3);

            return new Vector3(x, y, z);
        }

        public static float PopFloat(this List<float> list)
        {
            var valueIndex = list.Count - 1;
            var value = list[valueIndex];

            list.RemoveAt(valueIndex);

            return value;
        }
    }
}