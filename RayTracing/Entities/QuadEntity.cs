using System;
using System.Numerics;
using RayTracing.Entities.Interfaces;
using RayTracing.Extensions;

namespace RayTracing.Entities
{
    public class QuadEntity : IEssence
    {
        public Vector3 Position { get; set; }
        
        public MaterialEntity Material { get; set; }
        
        public Vector3 VertexA { get; set; }
        
        public Vector3 VertexB { get; set; }
        
        public Vector3 VertexC { get; set; }

        public Vector3 VertexD { get; set; }

        public Vector3 Normal { get; set; }
        
        public Vector3? CheckCollision(RayEntity ray)
        {
            var collision = ray.IntersectTriangle(
                                Position + VertexA,
                                Position + VertexB,
                                Position + VertexC,
                                Normal)
                            ?? ray.IntersectTriangle(
                                Position + VertexA,
                                Position + VertexC,
                                Position + VertexD,
                                Normal);
            var inverseCollision = ray.IntersectTriangle(
                                       Position + VertexA,
                                       Position + VertexB,
                                       Position + VertexC,
                                       -Normal)
                                   ?? ray.IntersectTriangle(
                                       Position + VertexA,
                                       Position + VertexC,
                                       Position + VertexD,
                                       -Normal);
            
            return collision ?? inverseCollision;
        }

        public Vector3 GetNormal()
        {
            throw new NotImplementedException();
        }

        public Vector3 GetNormal(Vector3 intersect)
        {
            return Normal;
        }
    }
}