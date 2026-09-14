using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;

namespace SilkOpenGLTest.Core;

public class Shader : IDisposable
{
    private readonly GL _gl;
    private readonly uint _handle;
    private readonly Dictionary<string, int> _uniformLocations = new();

    public Shader(GL gl, string vertexPath, string fragmentPath)
    {
        _gl = gl;

        string vertexSource = File.ReadAllText(vertexPath);
        string fragmentSource = File.ReadAllText(fragmentPath);

        uint vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
        uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);

        _handle = _gl.CreateProgram();
        _gl.AttachShader(_handle, vertexShader);
        _gl.AttachShader(_handle, fragmentShader);
        _gl.LinkProgram(_handle);
        CheckLinkStatus(_handle);

        _gl.DetachShader(_handle, vertexShader);
        _gl.DetachShader(_handle, fragmentShader);
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        _gl.UseProgram(_handle);
    }

    public void SetBool(string name, bool value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
            _gl.Uniform1(location, value ? 1 : 0);
    }

    public void SetInt(string name, int value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
            _gl.Uniform1(location, value);
    }

    public void SetFloat(string name, float value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
            _gl.Uniform1(location, value);
    }

    public void SetVec3(string name, Vector3D<float> value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
            _gl.Uniform3(location, value.X, value.Y, value.Z);
    }

    public unsafe void SetMat4(string name, Matrix4X4<float> mat4)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
            _gl.UniformMatrix4(location, 1, false, (float*)&mat4);
    }

    public float GetUniformFloatValue(string name)
    {
        int location = GetUniformLocation(name);
        if (location == -1)
        {
            Console.WriteLine($"Uniform '{name}' not found!");
            return 0.0f;
        }
        _gl.GetUniform(_handle, location, out float value);
        return value;
    }

    private int GetUniformLocation(string name)
    {
        // Кэшируем локации — GetUniformLocation это медленный вызов
        if (_uniformLocations.TryGetValue(name, out int cached))
            return cached;

        int location = _gl.GetUniformLocation(_handle, name);
        _uniformLocations[name] = location;
        return location;
    }

    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = _gl.GetShaderInfoLog(shader);
            throw new Exception($"ERROR::SHADER_COMPILATION_ERROR of type {type}:\n{infoLog}");
        }

        return shader;
    }

    private void CheckLinkStatus(uint program)
    {
        _gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = _gl.GetProgramInfoLog(program);
            throw new Exception($"ERROR::PROGRAM_LINKING_ERROR:\n{infoLog}");
        }
    }

    public void Dispose()
    {
        _gl.DeleteProgram(_handle);
    }
}