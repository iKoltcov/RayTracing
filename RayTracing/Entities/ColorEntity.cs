using System;
namespace RayTracing.Entities
{
    public class ColorEntity
    {
        private static Random random = new Random();

        public float R { get; set; }

        public float G { get; set; }

        public float B { get; set; }

        public float A { get; set; }

        public ColorEntity(float R, float G, float B, float A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        public ColorEntity(float R, float G, float B) : this(R, G, B, 1.0f)
        {

        }

        public static ColorEntity Black => new ColorEntity(0.0f, 0.0f, 0.0f);

        public static ColorEntity White => new ColorEntity(1.0f, 1.0f, 1.0f);

        public static ColorEntity Random => new ColorEntity((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());

        public ColorEntity Set(float R, float G, float B, float A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;

            return this;
        }

        public ColorEntity Set(float R, float G, float B)
        {
            return Set(R, G, B, 1.0f);
        }

        public ColorEntity Copy()
        {
            return new ColorEntity(R, G, B, A);
        }

        public static ColorEntity operator *(ColorEntity color, float value)
        {
            return new ColorEntity(color.R * value, color.G * value, color.B * value);
        }
    }
}
