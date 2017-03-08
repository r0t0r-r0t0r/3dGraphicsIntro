using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Disunity.Data.Common;

namespace Disunity.WorldManaging.Loading
{
    internal static class TripleUtils
    {
        public static Triple<T> ToTriple<T>(this T[] values, int index1, int index2, int index3) where T : struct
        {
            return new Triple<T>(values[index1], values[index2], values[index3]);
        }

        public static Vector3 ToVector3(this IEnumerable<float> values)
        {
            var materialized = values.ToArray();
            return new Vector3(materialized[0], materialized[1], materialized[2]);
        }
    }
}