using System.Numerics;

namespace Disunity.App
{
    public static class Matrix4x4Utils
    {
        public static Vector4 Mul(this Matrix4x4 m, Vector4 v)
        {
            var result = new Vector4(
                m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z + m.M14 * v.W,
                m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z + m.M24 * v.W,
                m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z + m.M34 * v.W,
                m.M41 * v.X + m.M42 * v.Y + m.M43 * v.Z + m.M44 * v.W
                );
            return result;
        }

        public static Matrix4x4 Mul(this Matrix4x4 m1, Matrix4x4 m2)
        {
            var m11 = m1.M11 * m2.M11 + m1.M12 * m2.M21 + m1.M13 * m2.M31 + m1.M14 * m2.M41;
            var m12 = m1.M11 * m2.M12 + m1.M12 * m2.M22 + m1.M13 * m2.M32 + m1.M14 * m2.M42;
            var m13 = m1.M11 * m2.M13 + m1.M12 * m2.M23 + m1.M13 * m2.M33 + m1.M14 * m2.M43;
            var m14 = m1.M11 * m2.M14 + m1.M12 * m2.M24 + m1.M13 * m2.M34 + m1.M14 * m2.M44;

            var m21 = m1.M21 * m2.M11 + m1.M22 * m2.M21 + m1.M23 * m2.M31 + m1.M24 * m2.M41;
            var m22 = m1.M21 * m2.M12 + m1.M22 * m2.M22 + m1.M23 * m2.M32 + m1.M24 * m2.M42;
            var m23 = m1.M21 * m2.M13 + m1.M22 * m2.M23 + m1.M23 * m2.M33 + m1.M24 * m2.M43;
            var m24 = m1.M21 * m2.M14 + m1.M22 * m2.M24 + m1.M23 * m2.M34 + m1.M24 * m2.M44;

            var m31 = m1.M31 * m2.M11 + m1.M32 * m2.M21 + m1.M33 * m2.M31 + m1.M34 * m2.M41;
            var m32 = m1.M31 * m2.M12 + m1.M32 * m2.M22 + m1.M33 * m2.M32 + m1.M34 * m2.M42;
            var m33 = m1.M31 * m2.M13 + m1.M32 * m2.M23 + m1.M33 * m2.M33 + m1.M34 * m2.M43;
            var m34 = m1.M31 * m2.M14 + m1.M32 * m2.M24 + m1.M33 * m2.M34 + m1.M34 * m2.M44;

            var m41 = m1.M41 * m2.M11 + m1.M42 * m2.M21 + m1.M43 * m2.M31 + m1.M44 * m2.M41;
            var m42 = m1.M41 * m2.M12 + m1.M42 * m2.M22 + m1.M43 * m2.M32 + m1.M44 * m2.M42;
            var m43 = m1.M41 * m2.M13 + m1.M42 * m2.M23 + m1.M43 * m2.M33 + m1.M44 * m2.M43;
            var m44 = m1.M41 * m2.M14 + m1.M42 * m2.M24 + m1.M43 * m2.M34 + m1.M44 * m2.M44;

            var result = new Matrix4x4(
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
                );
            return result;
        }

        public static Matrix4x4 LookAt(Vector3 center, Vector3 eye, Vector3 up)
        {
            var z = Vector3.Normalize(eye - center);
            var x = Vector3.Normalize(Vector3.Cross(up, z));
            var y = Vector3.Normalize(Vector3.Cross(z, x));

            var result = new Matrix4x4(
                x.X, x.Y, x.Z, -center.X,
                y.X, y.Y, y.Z, -center.Y,
                z.X, z.Y, z.Z, -center.Z,
                0, 0, 0, 1
                );
            return result;
        }

        public static Matrix4x4 Viewport(int x, int y, int width, int height)
        {
            const int depth = 255;

            var hw = (float)width / 2;
            var hh = (float)height / 2;
            var hd = (float)depth / 2;

            var result = new Matrix4x4(
                hw, 0, 0, x + hw,
                0, hh, 0, y + hh,
                0, 0, hd, hd,
                0, 0, 0, 1
                );
            return result;
        }

        public static Matrix4x4 OrthographicProjection(float d)
        {
            var result = new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, -1 / d, 1
                );
            return result;
        }
    }
}
