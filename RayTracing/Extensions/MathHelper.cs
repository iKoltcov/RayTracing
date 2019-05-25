using System;
using System.Numerics;
using RayTracing.Entities;

namespace RayTracing.Extensions
{
    public static class MathHelper
    {
        public static Vector3 Normalize(this Vector3 vector)
        {
            return Vector3.Normalize(vector);
        }
        
        public static Vector3 Reflect(this Vector3 vector, Vector3 normal) {
            return vector - normal * 2.0f * Vector3.Dot(vector, normal);
        }
        
        public static Vector3? IntersectTriangle(this RayEntity rayEntity, Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, Vector3 normal) 
        {
            var d = -normal.X * vertexA.X - normal.Y * vertexA.Y - normal.Z * vertexA.Z;
            var a = (Vector3.Dot(rayEntity.Origin, normal) - d) / Vector3.Dot(rayEntity.Direction, normal);
            if (a < 0)
            {
                return null;
            }

            var point = rayEntity.Origin + rayEntity.Direction * a;
            if(point.IntersectTriangle(vertexA, vertexB, vertexC))
            {
                return point;
            }

            return null;
        }
        
        private static bool IntersectTriangle(this Vector3 point, Vector3 vertexA, Vector3 vertexB, Vector3 vertexC)
        {
            var v0 = vertexC - vertexA;
            var v1 = vertexB - vertexA;
            var v2 = point - vertexA;

            var dot00 = Vector3.Dot(v0, v0);
            var dot01 = Vector3.Dot(v0, v1);
            var dot02 = Vector3.Dot(v0, v2);
            var dot11 = Vector3.Dot(v1, v1);
            var dot12 = Vector3.Dot(v1, v2);

            var invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
            var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            if (u < 0) {
                return false;
            }

            var v = (dot00 * dot12 - dot01 * dot02) * invDenom;
            return (v > 0) && (u + v < 1.0f);
        }
    }
}
