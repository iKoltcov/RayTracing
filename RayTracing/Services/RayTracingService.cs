using System;
using System.Threading.Tasks;
using RayTracing.Entities;
using System.Threading;
using System.Collections.Generic;
using RayTracing.Entities.Interfaces;
using System.Numerics;
using RayTracing.Extension;

namespace RayTracing.Services
{
    public class RayTracingService : IDisposable
    {
        private static Random random = new Random();

        private readonly int countTask;
        private readonly object lockObject = new object();
        private CancellationTokenSource cancellationTokenSource;

        private int width, height;
        private ColorEntity[,] pixels;

        private List<IEssence> essences;
        private List<ILight> lights;

        public RayTracingService(int width, int height, int countTask)
        {
            this.width = width;
            this.height = height;
            this.countTask = countTask;

            this.pixels = new ColorEntity[width, height];
            this.cancellationTokenSource = new CancellationTokenSource();

            for (int widthIterator = 0; widthIterator < width; widthIterator++) {
                for (int heightIterator = 0; heightIterator < height; heightIterator++)
                {
                    pixels[widthIterator, heightIterator] = ColorEntity.Black;
                }
            }

            this.essences = new List<IEssence>();
            this.lights = new List<ILight>();
        }

        public void AddLight(ILight light) 
        {
            lights.Add(light);
        }

        public void AddEssence(IEssence essence)
        {
            essences.Add(essence);
        }

        public ColorEntity[,] GetPixels()
        {
            if (pixels == null)
            {
                throw new NullReferenceException("Pixel array not initialized");
            }
            return pixels;
        }

        public void Run()
        {
            for (int taskIterator = 0; taskIterator < countTask; taskIterator++)
            {
                var cancellationToken = cancellationTokenSource.Token;
                var task = new Task(() => RaysTrace(cancellationToken), cancellationToken);
                task.Start();
            }
        }

        private void RaysTrace(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                lock (lockObject)
                {
                    var x = random.Next(0, width);
                    var y = random.Next(0, height);
                    pixels[x, y] = CastRay(x, y);
                }
            }
        }

        private ColorEntity CastRay(int x, int y)
        {
            var fov = 1.57f;
            var direction = new Vector3(
                x + 0.5f - ((float)width * 0.5f),
                y + 0.5f - ((float)height * 0.5f),
                (float)width / (float)Math.Tan(fov * 0.5f));
            direction = direction.Normalize();

            var rayEntity = new RayEntity()
            {
                Origin = new Vector3(0.0f, 0.0f, 0.0f),
                Direction = direction
            };

            IEssence essenceRef = null;
            float? distanceMin = null;
            Vector3? point = null;
            foreach (IEssence essence in essences)
            {
                var collisionPoint = essence.CheckCollision(rayEntity);

                if (collisionPoint.HasValue)
                {
                    var distance = (rayEntity.Origin - collisionPoint.Value).Length();
                    if(distanceMin == null || (distanceMin != null && distanceMin > distance))
                    {
                        distanceMin = distance;
                        essenceRef = essence;
                        point = collisionPoint;
                    }
                }
            }

            if (essenceRef != null && point.HasValue)
            {
                var diffuseLightIntensity = 0.0f;
                foreach(ILight light in lights)
                {
                    var directionToLight = (light.Position - point.Value).Normalize();
                    var normal = (point.Value - essenceRef.Position).Normalize();
                    diffuseLightIntensity += 1.5f * Math.Max(0.0f, Vector3.Dot(directionToLight, normal));
                }

                return (essenceRef as SphereEntity).Color.Copy() * diffuseLightIntensity;
            }

            return ColorEntity.Black;
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
