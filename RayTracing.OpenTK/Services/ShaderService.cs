using System;
using OpenTK.Graphics.ES20;
using RayTracing.OpenTK.Entities;

namespace RayTracing.OpenTK.Services
{
    public class ShaderService
    {
        public int ShaderProgram;
        public int MatrixHandle;
        public int PointSizeHandle;

        public ShaderService()
        {
            GL.Enable((EnableCap)0x8642); // EnableCap.ProgramPointSize from OpenTK.Graphics.OpenGL
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
            PointSizeHandle = GL.GetUniformLocation(ShaderProgram, "u_pointSize");

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

            GL.ShaderSource(shader, 1, new string[] { source }, (int[])null);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var compiled);
            if (compiled == 0)
            {
                GL.GetShader(shader, ShaderParameter.InfoLogLength, out _);
                GL.DeleteShader(shader);
                throw new InvalidOperationException("Unable to compile shader of type : " + type.ToString());
            }

            return shader;
        }

        private static string vertexShaderCode =
            @"uniform mat4 u_matrix; 
            uniform float u_pointSize;
            attribute vec2 a_vertex; 
            attribute vec3 a_color;
            varying vec3 v_color; 
            void main() { 
                gl_Position = u_matrix * vec4(a_vertex, 0.0, 1.0);
                gl_PointSize = u_pointSize;
                v_color = a_color; 
            }";
        private static string fragmentShaderCode =
            @"varying vec3 v_color; 
            void main() { 
                gl_FragColor = vec4(v_color, 1.0); 
            }";
    }
}
