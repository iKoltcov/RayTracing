using RayTracing.Entities.Interfaces;

namespace RayTracing.Entities
{
    public class PointLightEntity : ILight, ICoordinatable
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public ColorEntity Color { get; set; }
    }
}
