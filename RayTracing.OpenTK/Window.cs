﻿using System;
using OpenTK;
using OpenTK.Graphics.ES20;
using RayTracing.Services;
using RayTracing.OpenTK.Services;
using RayTracing.OpenTK.Entities;
using RayTracing.Entities;

namespace RayTracing.OpenTK
{
    public class Window : GameWindow
    {
        private readonly int countTask = 4;
        private readonly int cellWidth = 100;
        private readonly int cellHeight = 100;

        private readonly int vertexSize = 2;
        private readonly int colorSize = 4;
         
        private readonly RayTracingService rayTracingService;
        private readonly ShaderService shaderService;

        private Matrix4 matrix;
        private int vertexBufferHandle;
        private int colorBufferHandle;

        private float[] arrayVertexs;
        private float[] arrayColors;

        public Window()
        {
            rayTracingService = new RayTracingService(cellWidth, cellHeight, countTask);
            shaderService = new ShaderService();

            arrayVertexs = new float[cellWidth * cellHeight * 8 * vertexSize];
            arrayColors = new float[cellWidth * cellHeight * 16 * colorSize];

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

            for (int cellWidthIterator = 0; cellWidthIterator < cellWidth; cellWidthIterator++)
            {
                for (int cellHeightIterator = 0; cellHeightIterator < cellHeight; cellHeightIterator++)
                {
                    arrayVertexs[arrayVertexsIterator + 0] = widthStep * cellWidthIterator;
                    arrayVertexs[arrayVertexsIterator + 1] = heightStep * cellHeightIterator;
                    arrayVertexs[arrayVertexsIterator + 2] = widthStep * cellWidthIterator + widthStep;
                    arrayVertexs[arrayVertexsIterator + 3] = heightStep * cellHeightIterator;
                    arrayVertexs[arrayVertexsIterator + 4] = widthStep * cellWidthIterator;
                    arrayVertexs[arrayVertexsIterator + 5] = heightStep * cellHeightIterator + heightStep;

                    arrayVertexs[arrayVertexsIterator + 6] = widthStep * cellWidthIterator + widthStep;
                    arrayVertexs[arrayVertexsIterator + 7] = heightStep * cellHeightIterator;
                    arrayVertexs[arrayVertexsIterator + 8] = widthStep * cellWidthIterator;
                    arrayVertexs[arrayVertexsIterator + 9] = heightStep * cellHeightIterator + heightStep;
                    arrayVertexs[arrayVertexsIterator + 10] = widthStep * cellWidthIterator + widthStep;
                    arrayVertexs[arrayVertexsIterator + 11] = heightStep * cellHeightIterator + heightStep;
                    arrayVertexsIterator += 12;
                }
            }

            vertexBufferHandle = GL.GenBuffer();
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

                    arrayColors[arrayColorsIterator + 4] = pixels[cellWidthIterator, cellHeightIterator].R;
                    arrayColors[arrayColorsIterator + 5] = pixels[cellWidthIterator, cellHeightIterator].G;
                    arrayColors[arrayColorsIterator + 6] = pixels[cellWidthIterator, cellHeightIterator].B;
                    arrayColors[arrayColorsIterator + 7] = pixels[cellWidthIterator, cellHeightIterator].A;

                    arrayColors[arrayColorsIterator + 8] = pixels[cellWidthIterator, cellHeightIterator].R;
                    arrayColors[arrayColorsIterator + 9] = pixels[cellWidthIterator, cellHeightIterator].G;
                    arrayColors[arrayColorsIterator + 10] = pixels[cellWidthIterator, cellHeightIterator].B;
                    arrayColors[arrayColorsIterator + 11] = pixels[cellWidthIterator, cellHeightIterator].A;

                    arrayColors[arrayColorsIterator + 12] = pixels[cellWidthIterator, cellHeightIterator].R;
                    arrayColors[arrayColorsIterator + 13] = pixels[cellWidthIterator, cellHeightIterator].G;
                    arrayColors[arrayColorsIterator + 14] = pixels[cellWidthIterator, cellHeightIterator].B;
                    arrayColors[arrayColorsIterator + 15] = pixels[cellWidthIterator, cellHeightIterator].A;

                    arrayColors[arrayColorsIterator + 16] = pixels[cellWidthIterator, cellHeightIterator].R;
                    arrayColors[arrayColorsIterator + 17] = pixels[cellWidthIterator, cellHeightIterator].G;
                    arrayColors[arrayColorsIterator + 18] = pixels[cellWidthIterator, cellHeightIterator].B;
                    arrayColors[arrayColorsIterator + 19] = pixels[cellWidthIterator, cellHeightIterator].A;
                    
                    arrayColors[arrayColorsIterator + 20] = pixels[cellWidthIterator, cellHeightIterator].R;
                    arrayColors[arrayColorsIterator + 21] = pixels[cellWidthIterator, cellHeightIterator].G;
                    arrayColors[arrayColorsIterator + 22] = pixels[cellWidthIterator, cellHeightIterator].B;
                    arrayColors[arrayColorsIterator + 23] = pixels[cellWidthIterator, cellHeightIterator].A;
                    arrayColorsIterator += 24;
                }
            }

            colorBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * arrayColors.Length, arrayColors, BufferUsageHint.DynamicDraw);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(1.0f, 1.0f, 0.8f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            rayTracingService.AddLight(new PointLightEntity()
            {
                X = 0.0f,
                Y = 0.0f,
                Z = 10.0f,
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });
            rayTracingService.AddEssence(new SphereEntity { 
                X = 0.0f,
                Y = 5.0f,
                Z = 1.0f,
                Radius = 1.0f,
                Color = new ColorEntity(1.0f, 0.0f, 0.0f)
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                X = 5.0f,
                Y = 0.0f,
                Z = 1.0f,
                Radius = 1.0f,
                Color = new ColorEntity(0.0f, 1.0f, 0.0f)
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                X = 0.0f,
                Y = -5.0f,
                Z = 1.0f,
                Radius = 1.0f,
                Color = new ColorEntity(0.0f, 0.0f, 1.0f)
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                X = -5.0f,
                Y = 0.0f,
                Z = 1.0f,
                Radius = 1.0f,
                Color = new ColorEntity(1.0f, 1.0f, 0.0f)
            });
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
            matrix = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, -1.0f, 1.0f);

            GL.EnableVertexAttribArray((int)ShaderAttributeEntity.Vertex);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.VertexAttribPointer((int)ShaderAttributeEntity.Vertex, vertexSize, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray((int)ShaderAttributeEntity.Color);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferHandle);
            GL.VertexAttribPointer((int)ShaderAttributeEntity.Color, colorSize, VertexAttribPointerType.Float, false, 0, 0);

            GL.UniformMatrix4(shaderService.MatrixHandle, false, ref matrix);
            GL.DrawArrays(PrimitiveType.Triangles, 0, cellWidth * cellHeight * 6);

            GL.DisableVertexAttribArray((int)ShaderAttributeEntity.Color);
            GL.DisableVertexAttribArray((int)ShaderAttributeEntity.Vertex);

            Context.SwapBuffers();
        }
    }
}