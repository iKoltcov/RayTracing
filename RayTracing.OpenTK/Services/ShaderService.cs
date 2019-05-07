using System;
using OpenTK.Graphics.ES20;
using RayTracing.OpenTK.Entities;

namespace RayTracing.OpenTK.Services
{
    public class ShaderService
    {
        public int ShaderProgram;
        public int MatrixHandle;

        public ShaderService()
        {
            Compile();
        }

        private void Compile()
        {
            ShaderProgram = GL.CreateProgram();

            var vertexShader = LoadShader(ShaderType.VertexShader, vertexShaderCode);
            var fragmentShader = LoadShader(ShaderType.FragmentShader, fragmentShaderCode);

            GL.AttachShader(ShaderProgram, vertexShader);
            GL.AttachShader(ShaderProgram, fragmentShader);
            GL.BindAttribLocation(ShaderProgram, (int)ShaderAttributeEntity.Vertex, "a_vertex");
            GL.BindAttribLocation(ShaderProgram, (int)ShaderAttributeEntity.Color, "a_color");
            GL.LinkProgram(ShaderProgram);

            MatrixHandle = GL.GetUniformLocation(ShaderProgram, "u_matrix");

            if (vertexShader != 0)
            {
                GL.DetachShader(ShaderProgram, vertexShader);
                GL.DeleteShader(vertexShader);
            }

            if (fragmentShader != 0)
            {
                GL.DetachShader(ShaderProgram, fragmentShader);
                GL.DeleteShader(fragmentShader);
            }

            GL.UseProgram(ShaderProgram);
        }

        private static int LoadShader(ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            if (shader == 0)
                throw new InvalidOperationException("Unable to create shader");

            int length = 0;
            GL.ShaderSource(shader, 1, new string[] { source }, (int[])null);
            GL.CompileShader(shader);

            int compiled = 0;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out compiled);
            if (compiled == 0)
            {
                length = 0;
                GL.GetShader(shader, ShaderParameter.InfoLogLength, out length);
                GL.DeleteShader(shader);
                throw new InvalidOperationException("Unable to compile shader of type : " + type.ToString());
            }

            return shader;
        }

        private static string vertexShaderCode =
            @"uniform mat4 u_matrix; 
            attribute vec2 a_vertex; 
            attribute vec4 a_color;
            varying vec4 v_color; 
            void main() { 
                gl_Position = u_matrix * vec4(a_vertex, 0.0, 1.0);
                v_color = a_color; 
            }";
        private static string fragmentShaderCode =
            @"varying vec4 v_color; 
            void main() { 
                gl_FragColor = v_color; 
            }";
    }
}
