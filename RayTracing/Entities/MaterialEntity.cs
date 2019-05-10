namespace RayTracing.Entities
{
    public class MaterialEntity
    {
        public ColorEntity Color { get; set; }

        public float Specular { get; set; }

        public static MaterialEntity Default => new MaterialEntity()
        {
            Color = ColorEntity.Random(),
            Specular = 1.0f
        };
    }
}
