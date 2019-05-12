using System;
using System.Threading.Tasks;
using RayTracing.Entities;
using System.Threading;
using System.Collections.Generic;
using RayTracing.Entities.Interfaces;
using System.Numerics;
using RayTracing.Extensions;
using RayTracing.Results;

namespace RayTracing.Services
{
    public class RayTracingService : IDisposable
    {
        private static readonly Random Random = new Random();

        private readonly int countTask;
        private readonly object lockObject = new object();
        private readonly CancellationTokenSource cancellationTokenSource;

        private readonly int width;
        private readonly int height;
        private ColorEntity[,] pixels;

        private readonly float fieldOfView = 1.57f;
        private readonly ColorEntity backgroundColor = ColorEntity.Black; 

        private readonly List<IEssence> essences;
        private readonly List<ILight> lights;

        public RayTracingService(int width, int height, int countTask)
        {
            this.width = width;
            this.height = height;
            this.countTask = countTask;

            pixels = new ColorEntity[width, height];
            cancellationTokenSource = new CancellationTokenSource();

            for (int widthIterator = 0; widthIterator < width; widthIterator++) {
                for (int heightIterator = 0; heightIterator < height; heightIterator++)
                {
                    pixels[widthIterator, heightIterator] = backgroundColor;
                }
            }

            essences = new List<IEssence>();
            lights = new List<ILight>();
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
            for (var taskIterator = 0; taskIterator < countTask; taskIterator++)
            {
                var cancellationToken = cancellationTokenSource.Token;
                Task.Run(() => RaysTrace(cancellationToken), cancellationToken);
            }
        }

        private void RaysTrace(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                lock (lockObject)
                {
                    var x = Random.Next(0, width);
                    var y = Random.Next(0, height);
                    var direction = new Vector3(
                        x + 0.5f - width * 0.5f, 
                        y + 0.5f - height * 0.5f, 
                        width / (float)Math.Tan(fieldOfView * 0.5f));

                    pixels[x, y] = CastRay(new RayEntity()
                    {
                        Origin = new Vector3(0.0f, 0.0f, 0.0f),
                        Direction = direction.Normalize()
                    });
                }
            }
        }

        private ColorEntity CastRay(RayEntity rayEntity)
        {
            var intersect = SceneIntersect(rayEntity);

            if(intersect == null)
            {
                return backgroundColor;
            }

            var diffuseLightIntensity = 0.0f;
            var specularLightIntensity = 0.0f;
            
            foreach(var light in lights)
            {
                var vectorToLight = light.Position - intersect.Point;
                var directionToLight = vectorToLight.Normalize();
                var distanceToLight = vectorToLight.Length();
                var normal = (intersect.Point - intersect.Essence.Position).Normalize();

                var rayToLight = new RayEntity()
                {
                    Origin = Vector3.Dot(directionToLight, normal) < 0 
                        ? intersect.Point - normal * 1e-5f
                        : intersect.Point + normal * 1e-5f,
                    Direction = directionToLight
                };

                if(SceneIntersect(rayToLight, distanceToLight) == null)
                {
                    diffuseLightIntensity += (light.Intensity / distanceToLight * distanceToLight) * Math.Max(0.0f, Vector3.Dot(directionToLight, normal));
                    specularLightIntensity += (float)Math.Pow(Math.Max(0.0f, -Vector3.Dot((-directionToLight).Reflect(normal), rayEntity.Direction)), intersect.Essence.Material.Specular) * light.Intensity;
                }
            } 

            return intersect.Essence.Material.Color * diffuseLightIntensity * intersect.Essence.Material.DiffuseComponent 
                   + new ColorEntity(1.0f, 1.0f, 1.0f) * specularLightIntensity * intersect.Essence.Material.SpecularComponent;
        }

        private SceneIntersectResult SceneIntersect(RayEntity rayEntity, float? distanceMax = null)
        {
            float? distanceMin = null;
            IEssence essenceRef = null;
            Vector3? point = null;

            foreach (var essence in essences)
            {
                var collisionPoint = essence.CheckCollision(rayEntity);

                if (!collisionPoint.HasValue)
                {
                    continue;
                }
                
                var distance = (rayEntity.Origin - collisionPoint.Value).Length();
                if (distanceMax != null && distanceMax < distance)
                {
                    continue;
                }

                if (distanceMin == null ^ distanceMin > distance)
                {
                    distanceMin = distance;
                    essenceRef = essence;
                    point = collisionPoint;
                }
            }

            if (essenceRef == null)
            {
                return null;
            }

            return new SceneIntersectResult()
            {
                Essence = essenceRef,
                Point = point.Value
            };
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
