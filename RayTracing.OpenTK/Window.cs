using System;
using System.Numerics;
using OpenTK;
using OpenTK.Graphics.ES20;
using RayTracing.Services;
using RayTracing.OpenTK.Services;
using RayTracing.OpenTK.Entities;
using RayTracing.Entities;
using OpenTK.Graphics;

namespace RayTracing.OpenTK
{
    public class Window : GameWindow
    {
        private readonly int countTask = 4;
        private readonly int cellWidth = 512;
        private readonly int cellHeight = 512;

        private readonly int vertexSize = 2;
        private readonly int colorSize = 3;
         
        private readonly RayTracingService rayTracingService;
        private readonly ShaderService shaderService;

        private Matrix4 matrix;
        private int vertexBufferHandle;
        private int colorBufferHandle;

        private float pointSize;
        private float[] arrayVertexs;
        private float[] arrayColors;

        public Window() : base(800, 800, GraphicsMode.Default, "RayTracing")
        {
            rayTracingService = new RayTracingService(cellWidth, cellHeight, countTask);
            shaderService = new ShaderService();

            arrayVertexs = new float[cellWidth * cellHeight * vertexSize];
            arrayColors = new float[cellWidth * cellHeight * colorSize];

            colorBufferHandle = GL.GenBuffer();
            vertexBufferHandle = GL.GenBuffer();

            UpdateVertexs();
            UpdateColors(rayTracingService.GetPixels());
        }

        public void Start()
        {
            rayTracingService.Run();
            Run(60);
        }

        private void UpdateVertexs()
        {
            int arrayVertexsIterator = 0;

            float widthStep = Width / (float)cellWidth;
            float heightStep = Height / (float)cellHeight;
            pointSize = Math.Max(1.0f, Math.Max(widthStep, heightStep) * 2.0f);

            for (int cellWidthIterator = 0; cellWidthIterator < cellWidth; cellWidthIterator++)
            {
                for (int cellHeightIterator = 0; cellHeightIterator < cellHeight; cellHeightIterator++)
                {
                    arrayVertexs[arrayVertexsIterator + 0] = widthStep * cellWidthIterator;
                    arrayVertexs[arrayVertexsIterator + 1] = heightStep * cellHeightIterator;
                    arrayVertexsIterator += vertexSize;
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * arrayVertexs.Length, arrayVertexs, BufferUsageHint.DynamicDraw);
        }

        private void UpdateColors(PixelEntity[,] pixels)
        {
            int arrayColorsIterator = 0;

            for (int cellWidthIterator = 0; cellWidthIterator < cellWidth; cellWidthIterator++)
            {
                for (int cellHeightIterator = 0; cellHeightIterator < cellHeight; cellHeightIterator++)
                {
                    arrayColors[arrayColorsIterator + 0] = pixels[cellWidthIterator, cellHeightIterator].Color.R;
                    arrayColors[arrayColorsIterator + 1] = pixels[cellWidthIterator, cellHeightIterator].Color.G;
                    arrayColors[arrayColorsIterator + 2] = pixels[cellWidthIterator, cellHeightIterator].Color.B;
                    arrayColorsIterator += colorSize;
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * arrayColors.Length, arrayColors, BufferUsageHint.DynamicDraw);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            
            rayTracingService.AddLight(new PointLightEntity()
            {
                Position = new System.Numerics.Vector3(0.0f, 50.0f, 50.0f),
                Intensity = 0.8f,
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });
            rayTracingService.AddLight(new PointLightEntity()
            {
                Position = new System.Numerics.Vector3(-20.0f, 50.0f, 100.0f),
                Intensity = 0.8f,
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });

            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new System.Numerics.Vector3(-20.0f, 0.0f, 120.0f),
                Radius = 20.0f
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new System.Numerics.Vector3(0.0f, -5.0f, 100.0f),
                Radius = 15.0f
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new System.Numerics.Vector3(10.0f, -10.0f, 80.0f),
                Radius = 10.0f
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new System.Numerics.Vector3(15.0f, -15.0f, 70.0f),
                Radius = 5.0f
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new System.Numerics.Vector3(17.0f, -17.5f, 65.0f),
                Radius = 2.5f
            });
            
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new System.Numerics.Vector3(18.0f, -18.75f, 62.5f),
                Radius = 1.25f
            });
            
            
            rayTracingService.AddEssence(new QuadEntity()
            {
                Material = new MaterialEntity()
                {
                    Color = ColorEntity.White,
                    DiffuseComponent = 0.7f,
                    ReflectComponent = 0.3f,
                },
                Position = new System.Numerics.Vector3(0.0f, -20.0f, 100.0f),
                Normal = new System.Numerics.Vector3(0.0f, 1.0f, 0.0f),
                VertexA = new System.Numerics.Vector3(-1000.0f, 0.0f,  1000.0f),
                VertexB = new System.Numerics.Vector3(1000.0f, 0.0f,  1000.0f),
                VertexC = new System.Numerics.Vector3(1000.0f, 0.0f,  -1000.0f),
                VertexD = new System.Numerics.Vector3(-1000.0f, 0.0f,  -1000.0f)
            });
        }
        
        public string GetPixelColor(int positionX, int positionY)
        {
            try
            {                
                var pixels = rayTracingService.GetPixels();
                var x = (int) (positionX / (Width / (float) cellWidth));
                var y = (int) (positionY / (Height / (float) cellHeight));

                var color = pixels[x, y].Color;
                return $"[{x},{y}] {color.R:F3} {color.G:F3} {color.B:F3}";
            }
            catch
            {
                return null;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            MakeCurrent();
            GL.Viewport(0, 0, Width, Height);
            UpdateVertexs();
        }

        protected override void OnUnload(EventArgs e)
        {
            rayTracingService.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            UpdateColors(rayTracingService.GetPixels());
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            matrix = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, -1.0f, 1.0f);

            GL.EnableVertexAttribArray((int)ShaderAttributeEntity.Vertex);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.VertexAttribPointer((int)ShaderAttributeEntity.Vertex, vertexSize, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray((int)ShaderAttributeEntity.Color);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferHandle);
            GL.VertexAttribPointer((int)ShaderAttributeEntity.Color, colorSize, VertexAttribPointerType.Float, false, 0, 0);

            GL.UniformMatrix4(shaderService.MatrixHandle, false, ref matrix);
            GL.Uniform1(shaderService.PointSizeHandle, pointSize);
            GL.DrawArrays(PrimitiveType.Points, 0, cellWidth * cellHeight);

            GL.DisableVertexAttribArray((int)ShaderAttributeEntity.Color);
            GL.DisableVertexAttribArray((int)ShaderAttributeEntity.Vertex);

            Context.SwapBuffers();
        }
    }
}
