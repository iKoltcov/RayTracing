using System;
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
        private readonly int colorSize = 4;
         
        private readonly RayTracingService rayTracingService;
        private readonly ShaderService shaderService;

        private Matrix4 matrix;
        private int vertexBufferHandle;
        private int colorBufferHandle;

        private float pointSize;
        private float[] arrayVertexs;
        private float[] arrayColors;

        public Window() : base(600, 600, GraphicsMode.Default, "RayTracing")
        {
            rayTracingService = new RayTracingService(cellWidth, cellHeight, countTask);
            shaderService = new ShaderService();

            arrayVertexs = new float[cellWidth * cellHeight * vertexSize];
            arrayColors = new float[cellWidth * cellHeight * colorSize];

            colorBufferHandle = GL.GenBuffer();
            vertexBufferHandle = GL.GenBuffer();

            updateVertexs();
            updateColors(rayTracingService.GetPixels());

            rayTracingService.Run();
            Run(60);
        }

        private void updateVertexs()
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
                    arrayVertexsIterator += 2;
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * arrayVertexs.Length, arrayVertexs, BufferUsageHint.DynamicDraw);
        }

        private void updateColors(ColorEntity[,] pixels)
        {
            int arrayColorsIterator = 0;

            for (int cellWidthIterator = 0; cellWidthIterator < cellWidth; cellWidthIterator++)
            {
                for (int cellHeightIterator = 0; cellHeightIterator < cellHeight; cellHeightIterator++)
                {
                    arrayColors[arrayColorsIterator + 0] = pixels[cellWidthIterator, cellHeightIterator].R;
                    arrayColors[arrayColorsIterator + 1] = pixels[cellWidthIterator, cellHeightIterator].G;
                    arrayColors[arrayColorsIterator + 2] = pixels[cellWidthIterator, cellHeightIterator].B;
                    arrayColors[arrayColorsIterator + 3] = pixels[cellWidthIterator, cellHeightIterator].A;
                    arrayColorsIterator += 4;
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
                Position = new System.Numerics.Vector3(10.0f, 10.0f, 0.0f),
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });
            rayTracingService.AddLight(new PointLightEntity()
            {
                Position = new System.Numerics.Vector3(-20.0f, 5.0f, -10.0f),
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });
            rayTracingService.AddLight(new PointLightEntity()
            {
                Position = new System.Numerics.Vector3(-20.0f, -20.0f, -10.0f),
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });

            for (int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    rayTracingService.AddEssence(new SphereEntity
                        {
                            Material = MaterialEntity.Default,
                            Position = new System.Numerics.Vector3(i, j, 4.0f),
                            Radius = i == 0 && j == 0 ? 0.75f : 0.5f
                        });
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            MakeCurrent();
            GL.Viewport(0, 0, Width, Height);
            updateVertexs();
        }

        protected override void OnUnload(EventArgs e)
        {
            rayTracingService.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            updateColors(rayTracingService.GetPixels());
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            matrix = Matrix4.CreateOrthographicOffCenter(0, Width, 0, Height, -1.0f, 1.0f);

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
