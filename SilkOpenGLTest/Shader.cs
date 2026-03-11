using System;
using System.IO;
using Silk.NET.OpenGL;

namespace SilkOpenGLTest
{
    internal class Shader : IDisposable
    {
        private uint handle;
        private GL gl;

        public Shader(GL gl, string vertexPath, string fragmentPath)
        {
            this.gl = gl;

            string vertexSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragmentPath);

            uint vertexShader = gl.CreateShader(GLEnum.VertexShader);
            gl.ShaderSource(vertexShader, vertexSource);
            gl.CompileShader(vertexShader);
            CheckShaderCompilation(vertexShader, "VERTEX");

            uint fragmentShader = gl.CreateShader(GLEnum.FragmentShader);
            gl.ShaderSource(fragmentShader, fragmentSource);
            gl.CompileShader(fragmentShader);
            CheckShaderCompilation(vertexShader, "FRAGMENT");

            handle = gl.CreateProgram();
            gl.AttachShader(handle, vertexShader);
            gl.AttachShader(handle, fragmentShader);
            gl.LinkProgram(handle);
            CheckProgramLinking(handle);

            // Shaders can be deleted after linking
            gl.DetachShader(handle, vertexShader);
            gl.DetachShader(handle, fragmentShader);
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            gl.UseProgram(handle);
        }

        public void SetBool(string name, bool value)
        {
            int location = gl.GetUniformLocation(handle, name);
            if (location != -1)
                gl.Uniform1(location, value ? 1 : 0);
        }

        public void SetInt(string name, int value)
        {
            int location = gl.GetUniformLocation(handle, name);
            if (location != -1)
                gl.Uniform1(location, value);
        }

        public void SetFloat(string name, float value)
        {
            int location = gl.GetUniformLocation(handle, name);
            if (location != -1)
                gl.Uniform1(location, value);
        }

        public float GetUniformFloatValue(string name)
        {
            
            int location = gl.GetUniformLocation(handle, name);
            if (location == -1)
            {
                Console.WriteLine($"Uniform '{name}' not found!");
                return 0.0f;
            }
            gl.GetUniform(handle, location, out float value);
            return value;

        }

        private void CheckShaderCompilation(uint shader, string type)
        {
            gl.GetShader(shader, ShaderParameterName.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = gl.GetShaderInfoLog(shader);
                Console.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type}\n{infoLog}");
            }
        }

        private void CheckProgramLinking(uint program)
        {
            gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = gl.GetProgramInfoLog(program);
                Console.WriteLine($"ERROR::PROGRAM_LINKING_ERROR\n{infoLog}");
            }
        }

        public void Dispose()
        {
            gl.DeleteProgram(handle);
        }
    }
}
