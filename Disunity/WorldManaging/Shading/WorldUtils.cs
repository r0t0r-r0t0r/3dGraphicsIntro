using System;
using System.Numerics;
using Disunity.Data;

namespace Disunity.WorldManaging.Shading
{
    public static class WorldUtils
    {
        public static Matrix4x4 GetTransform(this World world)
        {
            return world.ViewportTransform.Mul(world.ProjectionTransform.Mul(world.ViewTransform.Mul(world.WorldObject.ModelTransform)));
        }

        public static Matrix4x4 GetNormalTransform(this World world)
        {
            var tempTransform = world.ProjectionTransform.Mul(world.ViewTransform.Mul(world.WorldObject.ModelTransform));
            tempTransform = Matrix4x4.Transpose(tempTransform);
            Matrix4x4 transformation;
            if (!Matrix4x4.Invert(tempTransform, out transformation))
            {
                throw new ArgumentException();
            }
            return transformation;
        }
    }
}