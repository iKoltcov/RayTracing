using RayTracing.Entities.Interfaces;
using System.Numerics;
using System;
namespace RayTracing.Entities
{
    public class SphereEntity : IEssence
    {
        public Vector3 Position { get; set; }

        public float Radius { get; set; }

        public ColorEntity Color { get; set; }

        public Vector3? CheckCollision(RayEntity ray)
        {
            Vector3 oc = ray.Origin - Position;
            float a = Vector3.Dot(ray.Direction, ray.Direction);
            float b = Vector3.Dot(oc, ray.Direction) * 2.0f;
            float c = Vector3.Dot(oc, oc) - Radius * Radius;

            float discriminant = b * b - 4.0f * a * c;
            if(discriminant < 0)
            {
                return null;
            }

            float numerator = -b - (float)Math.Sqrt(discriminant);
            if(numerator > 0.0f)
            {
                return ray.Origin + ray.Direction * (numerator / (2.0f * a));
            }

            numerator = -b + (float)Math.Sqrt(discriminant);
            if (numerator > 0.0f)
            {
                return ray.Origin + ray.Direction * (numerator / (2.0f * a));
            }

            return null;
        }
    }
}
