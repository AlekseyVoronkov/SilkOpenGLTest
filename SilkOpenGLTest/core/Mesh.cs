using Silk.NET.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace SilkOpenGLTest.Core;

public class Mesh : IDisposable
{
    private readonly GL _gl;
    private uint _vao;
    private uint _vbo;
    private uint _ebo;
    private uint _indexCount;

    private uint _vertexCount;

    public Mesh(GL gl, Vertex[] vertices, uint[]? indices = null)
    {
        _gl = gl;
        _vertexCount = (uint)vertices.Length;

        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);

        // VBO
        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        unsafe
        {
            fixed (Vertex* ptr = vertices)
            {
                _gl.BufferData(
                    BufferTargetARB.ArrayBuffer,
                    (nuint)(vertices.Length * Marshal.SizeOf<Vertex>()),
                    ptr,
                    BufferUsageARB.StaticDraw);
            }
        }

        // EBO (если есть индексы)
        if (indices != null && indices.Length > 0)
        {
            _ebo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);

            unsafe
            {
                fixed (uint* ptr = indices)
                {
                    _gl.BufferData(
                        BufferTargetARB.ElementArrayBuffer,
                        (nuint)(indices.Length * sizeof(uint)),
                        ptr,
                        BufferUsageARB.StaticDraw);
                }
            }
            _indexCount = (uint)indices.Length;
        }
        unsafe
        {
            // Настраиваем атрибуты
            // Position — location 0, 3 float
            _gl.VertexAttribPointer(
                0, 3, VertexAttribPointerType.Float, false,
                (uint)Marshal.SizeOf<Vertex>(),
                (void*)0);
            _gl.EnableVertexAttribArray(0);

            // TexCoord — location 1, 2 float, смещение 12 байт (3 float)
            _gl.VertexAttribPointer(
                1, 2, VertexAttribPointerType.Float, false,
                (uint)Marshal.SizeOf<Vertex>(),
                (void*)(3 * sizeof(float)));
            _gl.EnableVertexAttribArray(1);

            _gl.BindVertexArray(0);
        }
    }

    public unsafe void Draw()
    {
        _gl.BindVertexArray(_vao);

        if (_indexCount > 0)
        {
            _gl.DrawElements(
                PrimitiveType.Triangles,
                _indexCount,
                DrawElementsType.UnsignedInt,
                (void*)0);
        }
        else
        {
            _gl.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
        }
    }

    public void Dispose()
    {
        if (_vao == 0) return;

        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
        if (_ebo != 0)
            _gl.DeleteBuffer(_ebo);

        _vao = 0;
        _vbo = 0;
        _ebo = 0;
    }
}