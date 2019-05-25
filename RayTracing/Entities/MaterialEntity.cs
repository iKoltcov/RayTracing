namespace RayTracing.Entities
{
    public class MaterialEntity
    {
        public ColorEntity Color { get; set; }

        public float Specular { get; set; }
        
        public float DiffuseComponent { get; set; }
        
        public float SpecularComponent { get; set; }

        public float ReflectComponent { get; set; }

        public static MaterialEntity Default => new MaterialEntity()
        {
            Color = ColorEntity.Random(),
            DiffuseComponent = 0.9f,
            SpecularComponent = 0.1f,
            ReflectComponent = 0.0f,
            Specular = 50.0f
        };

        public static MaterialEntity Mirror => new MaterialEntity()
        {
            Color = ColorEntity.White,
            DiffuseComponent = 0.1f,
            SpecularComponent = 0.1f,
            ReflectComponent = 0.8f,
            Specular = 50.0f
        };
    }
}
