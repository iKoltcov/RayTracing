using System.Numerics;
using RayTracing.Entities.Interfaces;

namespace RayTracing.Entities
{
    public class PointLightEntity : ILight
    {
        public Vector3 Position { get; set; }

        public ColorEntity Color { get; set; }

        public float Intensity { get; set; }
    }
}
