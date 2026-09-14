using System.Runtime.InteropServices;
using Silk.NET.Maths;

namespace SilkOpenGLTest.Core;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public Vector3D<float> Position;
    public Vector2D<float> TexCoord;

    public Vertex(Vector3D<float> position, Vector2D<float> texCoord)
    {
        Position = position;
        TexCoord = texCoord;
    }
}