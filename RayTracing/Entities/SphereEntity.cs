using RayTracing.Entities.Interfaces;
namespace RayTracing.Entities
{
    public class SphereEntity : IEssence, ICoordinatable
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float Radius { get; set; }

        public ColorEntity Color { get; set; }
    }
}
