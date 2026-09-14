using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace SilkOpenGLTest.Core;

public class Texture : IDisposable
{
    private readonly GL _gl;
    private readonly uint _handle;

    public uint Handle => _handle;

    public Texture(GL gl, string path,
        TextureWrapMode wrapS = TextureWrapMode.Repeat,
        TextureWrapMode wrapT = TextureWrapMode.Repeat,
        TextureMinFilter minFilter = TextureMinFilter.Linear,
        TextureMagFilter magFilter = TextureMagFilter.Linear)
    {
        _gl = gl;
        _handle = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _handle);

        using (var img = Image.Load<Rgba32>(path))
        {
            if (img == null) throw new Exception($"Failed to load image: {path}");

            // OpenGL ожидает первый пиксель снизу слева
            img.Mutate(x => x.Flip(FlipMode.Vertical));

            byte[] pixels = new byte[img.Width * img.Height * 4];
            img.CopyPixelDataTo(pixels);

            unsafe
            {
                fixed (byte* ptr = pixels)
                {
                    _gl.TexImage2D(
                        TextureTarget.Texture2D,
                        0,
                        InternalFormat.Rgba,
                        (uint)img.Width,
                        (uint)img.Height,
                        0,
                        PixelFormat.Rgba,
                        PixelType.UnsignedByte,
                        ptr);
                }
            }
        }

        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapS);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapT);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
    }

    public void Bind(TextureUnit unit)
    {
        _gl.ActiveTexture(unit);
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public void Dispose()
    {
        _gl.DeleteTexture(_handle);
    }
}