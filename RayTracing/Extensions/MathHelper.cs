using System;
using System.Numerics;

namespace RayTracing.Extensions
{
    public static class MathHelper
    {
        /// <summary>Transform a Vector by the given Matrix using right-handed notation</summary>
        /// <param name="matrix">The desired transformation</param>
        /// <param name="vector">The vector to transform</param>
        public static Vector4 Transform(this Vector4 vector, Matrix4x4 matrix)
        {
            return new Vector4(
                matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14 * vector.W,
                matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24 * vector.W,
                matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34 * vector.W,
                matrix.M41 * vector.X + matrix.M42 * vector.Y + matrix.M43 * vector.Z + matrix.M44 * vector.W);
        }

        public static Matrix4x4 Invert(this Matrix4x4 matrix)
        {
            if(!Matrix4x4.Invert(matrix, out var matrixInverted))
            {
                throw new ArithmeticException("Could not invert matrix");
            }
            return matrixInverted;
        }

        public static Vector3 Normalize(this Vector3 vector)
        {
            return Vector3.Normalize(vector);
        }
        
        public static Vector3 Reflect(this Vector3 vector, Vector3 normal) {
            return vector - normal * 2.0f * Vector3.Dot(vector, normal);
        }

        public static Matrix4x4 CreatePerspectiveFieldOfView(float fovy, float aspect, float zNear, float zFar)
        {
            if (fovy <= 0 || fovy > Math.PI)
                throw new ArgumentOutOfRangeException("fovy");
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException("aspect");
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            float yMax = zNear * (float)Math.Tan(0.5f * fovy);
            float yMin = -yMax;
            float xMin = yMin * aspect;
            float xMax = yMax * aspect;
            float x = (2.0f * zNear) / (xMax - xMin);
            float y = (2.0f * zNear) / (yMax - yMin);
            float a = (xMax + xMin) / (xMax - xMin);
            float b = (yMax + yMin) / (yMax - yMin);
            float c = -(zFar + zNear) / (zFar - zNear);
            float d = -(2.0f * zFar * zNear) / (zFar - zNear);

            return new Matrix4x4(x, 0, 0, 0,
                                 0, y, 0, 0,
                                 a, b, c, -1,
                                 0, 0, d, 0);
        }
    }
}
