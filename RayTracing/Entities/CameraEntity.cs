using System.Numerics;
using RayTracing.Entities.Interfaces;

namespace RayTracing.Entities
{
    public class CameraEntity : ICoordinatable, ITargetable
    {
        public Vector3 Position { get; set; }

        public Vector3 Target { get; set; }

        public Vector3 UpVector { get; set; }

        public float FieldOfViewRadian { get; set; }
    }
}
