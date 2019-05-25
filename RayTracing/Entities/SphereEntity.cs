using RayTracing.Entities.Interfaces;
using System.Numerics;
using System;
using RayTracing.Extensions;

namespace RayTracing.Entities
{
    public class SphereEntity : IEssence
    {
        public Vector3 Position { get; set; }

        public float Radius { get; set; }

        public MaterialEntity Material { get; set; }

        public Vector3? CheckCollision(RayEntity ray)
        {
            var oc = ray.Origin - Position;
            var a = Vector3.Dot(ray.Direction, ray.Direction);
            var b = Vector3.Dot(oc, ray.Direction) * 2.0f;
            var c = Vector3.Dot(oc, oc) - Radius * Radius;

            var discriminant = b * b - 4.0f * a * c;
            if(discriminant < 0)
            {
                return null;
            }

            var sqrtDiscriminant = (float)Math.Sqrt(discriminant);
            var numerator = -b - sqrtDiscriminant;
            if(numerator > 0.0f)
            {
                return ray.Origin + ray.Direction * (numerator / (2.0f * a));
            }

            numerator = -b + sqrtDiscriminant;
            if (numerator > 0.0f)
            {
                return ray.Origin + ray.Direction * (numerator / (2.0f * a));
            }

            return null;
        }

        public Vector3 GetNormal()
        {
            throw new NotImplementedException();
        }
        
        public Vector3 GetNormal(Vector3 intersect)
        {
            return (intersect - Position).Normalize();
        }
    }
}
