using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SilkOpenGLTest;

public class Program
{
    private static IWindow _window;
    private static GL gl;
    private static uint VBO;
    private static uint VAO;
    private static uint EBO;
    private static Shader shader;
    private static bool isLine = false;

    private static uint texture1;
    private static uint texture2;
    private static readonly float[] Vertices =
        {
            //X    Y      Z   colors      texture coords
            0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
            0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f
        };

    private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3,
        };

    private static readonly float[] TextureCoords =
    {
        0.0f, 0.0f,
        0.0f, 1f,
        1f, 1f,
        1f, 0.0f
    };

    private static readonly int VertexSize = 8;
    public static void Main(string[] args)
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "Sanity(3/100)";
        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Update += OnUpdate;
        _window.FramebufferResize += OnFramebufferResize;
        _window.Closing += OnClose;

        _window.Run();

        _window.Dispose();
       
    }

    private static unsafe void OnLoad()
    {
        IInputContext input = _window.CreateInput();

        // Subscribing each connected keyboard to KeyDown event
        for (int i = 0; i < input.Keyboards.Count; i++)
            input.Keyboards[i].KeyDown += KeyDown;

        gl = GL.GetApi(_window);
        shader = new Shader(gl, "shaders/vertex.glsl", "shaders/fragment.glsl");
        gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        VAO = gl.GenVertexArray();
        VBO = gl.GenBuffer();
        EBO = gl.GenBuffer();

        gl.BindVertexArray(VAO);

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);

        fixed (void* v = &Vertices[0])
        {
            gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(Vertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
        }

        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);

        fixed (void* i = &Indices[0])
        {
            gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);
        }

        gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)VertexSize * sizeof(float), (void*)0);
        gl.EnableVertexAttribArray(0);

        gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, (uint)VertexSize * sizeof(float), (void*)(3 * sizeof(float)));
        gl.EnableVertexAttribArray(1);

        gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, (uint)VertexSize * sizeof(float), (void*)(6 * sizeof(float)));
        gl.EnableVertexAttribArray(2);

        texture1 = LoadTexture("img/test.jpg", texture1);
        texture2 = LoadTexture("img/thing.jpg", texture2, GLEnum.MirroredRepeat, GLEnum.MirroredRepeat, GLEnum.Nearest, GLEnum.Nearest);

        shader.Use();
        shader.SetInt("texture2", 1);
        shader.SetFloat("tex2Scale", 1.0f);
        shader.SetFloat("mixFactor", 0f);

        // note that this is allowed, the call to glVertexAttribPointer registered VBO as the vertex attribute's bound vertex buffer object so afterwards we can safely unbind
        //gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

        //gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);
    }

    private static void OnUpdate(double deltaTime)
    {
        //double timeValue = _window.Time;
        //double greenValue = (Math.Sin(timeValue) / 2.0f + 0.5f);
        //int vertexColorLocation = gl.GetUniformLocation(ShaderProgram, "timeColor");
        //gl.UseProgram(ShaderProgram);
        //gl.Uniform4(vertexColorLocation, 0.0f, (float)greenValue, 0.0f, 1.0f);
    }

    private static unsafe void OnRender(double deltaTime)
    {
        gl.Clear(ClearBufferMask.ColorBufferBit);

        gl.ActiveTexture(GLEnum.Texture0);
        gl.BindTexture(GLEnum.Texture2D, texture1);

        gl.ActiveTexture(GLEnum.Texture1);
        gl.BindTexture(GLEnum.Texture2D, texture2);

        shader.Use();

        gl.BindVertexArray(VAO);
        //gl.DrawArrays(GLEnum.Triangles, 0, 3);
        gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);
        //gl.DeleteVertexArray(VAO);
        //gl.DeleteBuffer(VBO);
        //gl.DeleteProgram(ShaderProgram);
    }

    private static void OnFramebufferResize(Vector2D<int> newSize)
    {
        gl.Viewport(newSize);
    }

    private static void OnClose()
    {
        //Remember to delete the buffers.
        gl.DeleteBuffer(VBO);
    }

    private static void ChangePolygonMode()
    {
        isLine = !isLine;
        GLEnum mode = isLine ? GLEnum.Line : GLEnum.Fill;
        gl.PolygonMode(GLEnum.FrontAndBack, mode);
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        Console.WriteLine($"{key} pressed");

        if (key == Key.Escape)
        {
            OnClose();
            _window.Close();
        }

        if (key == Key.C)
        {
            ChangePolygonMode();
        }

        if (key == Key.Up)
        {
            shader.Use();
            shader.SetFloat("tex2Scale", shader.GetUniformFloatValue("tex2Scale") + 0.5f);
        }

        if (key == Key.Down)
        {
            shader.Use();
            shader.SetFloat("tex2Scale", shader.GetUniformFloatValue("tex2Scale") - 0.5f);
        }

        if (key == Key.Left)
        {
            shader.Use();
            shader.SetFloat("mixFactor", shader.GetUniformFloatValue("mixFactor") - 0.1f);
        }

        if (key == Key.Right)
        {
            shader.Use();
            shader.SetFloat("mixFactor", shader.GetUniformFloatValue("mixFactor") + 0.1f);
        }
    }

    private static unsafe uint LoadTexture(
        string path, 
        uint texture,
        GLEnum paramWrapS = GLEnum.Repeat,
        GLEnum paramWrapT = GLEnum.Repeat,
        GLEnum paramMinFilter = GLEnum.Linear,
        GLEnum paramMaxFilter = GLEnum.Linear)
    {
        texture = gl.GenTexture();
        gl.BindTexture(GLEnum.Texture2D, texture);

        using (var img = Image.Load<Rgba32>(path))
        {
            if (img == null) throw new Exception("Failed to load image");

            // Переворачиваем по Y, если нужно (OpenGL ожидает первый пиксель снизу слева)
            img.Mutate(x => x.Flip(FlipMode.Vertical));

            // Подготавливаем массив байт для пикселей
            byte[] pixels = new byte[img.Width * img.Height * 4];

            // Копируем пиксели из ImageSharp в массив
            img.CopyPixelDataTo(pixels);

            fixed (byte* ptr = pixels)
            {
                gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba, (uint)img.Width, (uint)img.Height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
            }
        }

        // Настройки текстуры
        gl.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureWrapS, (int)paramWrapS);
        gl.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureWrapT, (int)paramWrapT);
        gl.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureMinFilter, (int)paramMinFilter);
        gl.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureMagFilter, (int)paramMaxFilter);

        return texture;
    }

}