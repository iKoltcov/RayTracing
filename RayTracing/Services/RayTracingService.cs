using System;
using System.Threading.Tasks;
using RayTracing.Entities;
using System.Threading;
using System.Collections.Generic;
using RayTracing.Entities.Interfaces;
using System.Numerics;

namespace RayTracing.Services
{
    public class RayTracingService : IDisposable
    {
        private static Random random = new Random();

        private readonly int countTask;
        private readonly object lockObject = new object();

        private int width, height;
        private ColorEntity[,] pixels;
        private CancellationTokenSource cancellationTokenSource;

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
            return new ColorEntity((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
