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
using System.Numerics;
using System.Collections.Generic;

namespace SilkOpenGLTest;

public class Program
{
    private static IWindow _window;
    private static IInputContext input;
    private static int _width = 800;
    private static int _height = 600;
    private static float _aspectRatio = (float)_width/(float)_height;
    static float fov = MathF.PI / 180f * 45.0f;

    private static GL gl;
    private static uint VBO;
    private static uint VAO;
    private static uint lightVAO;
    private static uint EBO;
    private static Shader cubeShader;
    private static Shader lightsourceShader;
    private static bool isLine = false;
    private static uint texture1;
    private static uint texture2;
    private static bool isOnUpdateTransform = false;
    private static readonly float[] Vertices =
        {
            //X    Y      Z   colors      texture coords
            //0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
            //0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
            //-0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            //-0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };

    public static Vector3D<float>[] CubePositions =
    {
        new Vector3D<float>( 0.0f,  0.0f,  0.0f),
        new Vector3D<float>( 2.0f,  5.0f, -15.0f),
        new Vector3D<float>(-1.5f, -2.2f, -2.5f),
        new Vector3D<float>(-3.8f, -2.0f, -12.3f),
        new Vector3D<float>( 2.4f, -0.4f, -3.5f),
        new Vector3D<float>(-1.7f,  3.0f, -7.5f),
        new Vector3D<float>( 1.3f, -2.0f, -2.5f),
        new Vector3D<float>( 1.5f,  2.0f, -2.5f),
        new Vector3D<float>( 1.5f,  0.2f, -1.5f),
        new Vector3D<float>(-1.3f,  1.0f, -1.5f),
        new Vector3D<float>(-5.0f,  5.0f, -5.0f),
        new Vector3D<float>(-6.0f,  5.0f, -5.0f),
        new Vector3D<float>(-7.0f,  5.0f, -5.0f),
        new Vector3D<float>(-8.0f,  5.0f, -5.0f)

    };

    private static Camera camera = new Camera();

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

    private static readonly int VertexSize = 5;
    public static void Main(string[] args)
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(_width, _height);
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
        input = _window.CreateInput();

        // Subscribing each connected keyboard to KeyDown event
        for (int i = 0; i < input.Keyboards.Count; i++)
        {
            input.Keyboards[i].KeyDown += KeyDown;
        }


        for (int i = 0; i < input.Mice.Count; i++)
        {
            input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
            input.Mice[i].Cursor.IsConfined = true;
            input.Mice[i].Scroll += OnMouseWheelScrole;
            input.Mice[i].MouseMove += OnMouseMove;
        }

        gl = GL.GetApi(_window);
        lightsourceShader = new Shader(gl, "shaders/lightsource/vertex.glsl", "shaders/lightsource/fragment.glsl");
        cubeShader = new Shader(gl, "shaders/cube/vertex.glsl", "shaders/cube/fragment.glsl");
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

        gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)VertexSize * sizeof(float), (void*)(3 * sizeof(float)));
        gl.EnableVertexAttribArray(1);

        //gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, (uint)VertexSize * sizeof(float), (void*)(6 * sizeof(float)));
        //gl.EnableVertexAttribArray(2);

        texture1 = LoadTexture("img/igor2.png", texture1);
        texture2 = LoadTexture("img/thing.jpg", texture2, GLEnum.MirroredRepeat, GLEnum.MirroredRepeat);

        cubeShader.Use();
        cubeShader.SetInt("texture2", 1);
        cubeShader.SetFloat("tex2Scale", 1.0f);
        cubeShader.SetFloat("mixFactor", 0f);

        gl.Enable(GLEnum.DepthTest);
        Builder.GenerateFloor(ref CubePositions);
        Builder.GenerateWalls(ref CubePositions);
    }

    private static unsafe void OnUpdate(double deltaTime)
    {
        ///Builder.GenerateRandomSlop();
        if (input.Keyboards[0].IsKeyPressed(Key.W))
            camera.MoveForward();

        if (input.Keyboards[0].IsKeyPressed(Key.S))
            camera.MoveBackwards();

        if (input.Keyboards[0].IsKeyPressed(Key.A))
            camera.MoveLeft();

        if (input.Keyboards[0].IsKeyPressed(Key.D))
            camera.MoveRight();

        float timeValue = (float)_window.Time;
        if (isOnUpdateTransform)
        {
            Matrix4X4<float> trans = Matrix4X4<float>.Identity;
            //trans = Matrix4X4.CreateRotationZ((float)timeValue) * trans;
            trans = Matrix4X4.CreateTranslation(new Vector3D<float>(0.0f, 0.0f, (float)Math.Sin(timeValue) )) * trans;

            cubeShader.SetMat4("transform", trans);
        }
        else
        {
            Matrix4X4<float> trans = Matrix4X4.CreateTranslation(new Vector3D<float>(0f, 0f, 0f));
            cubeShader.SetMat4("transform", trans);
        }
    }

    private static unsafe void OnRender(double deltaTime)
    {
        float timeValue = (float)_window.Time;

        camera.cameraSpeed = 2.5f * (float)deltaTime;

        gl.Clear(ClearBufferMask.ColorBufferBit);
        gl.Clear(ClearBufferMask.DepthBufferBit);

        gl.ActiveTexture(GLEnum.Texture0);
        gl.BindTexture(GLEnum.Texture2D, texture1);

        gl.ActiveTexture(GLEnum.Texture1);
        gl.BindTexture(GLEnum.Texture2D, texture2);

        cubeShader.Use();

        gl.BindVertexArray(VAO);
        //gl.DrawArrays(GLEnum.Triangles, 0, 3);

        cubeShader.SetMat4("view", camera.GetView());

        Matrix4X4<float> projection = Matrix4X4<float>.Identity;
        projection = Matrix4X4.CreatePerspectiveFieldOfView(fov, _aspectRatio, 0.1f, 100.0f) * projection;
        cubeShader.SetMat4("projection", projection);

        for (uint i = 0; i < CubePositions.Length; i++)
        {
            Matrix4X4<float> model = Matrix4X4<float>.Identity;
            model = Matrix4X4.CreateTranslation(CubePositions[ (int) i ]) * model;

            float angle = 0 * i; // в градусах
            float angleRad = angle * MathF.PI / 180f;
            Vector3D<float> axis = new Vector3D<float>(1.0f, 0.3f, 0.5f);
            axis = Vector3D.Normalize(axis);

            Quaternion<float> rotation = Quaternion<float>.CreateFromAxisAngle(axis, angleRad);
            Matrix4X4<float> rotationMatrix = Matrix4X4.CreateFromQuaternion(rotation);
            model = rotationMatrix * model;

            cubeShader.SetMat4("model", model);

            // Рисуем куб
            gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
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
            cubeShader.Use();
            cubeShader.SetFloat("tex2Scale", cubeShader.GetUniformFloatValue("tex2Scale") + 0.5f);
        }

        if (key == Key.Down)
        {
            cubeShader.Use();
            cubeShader.SetFloat("tex2Scale", cubeShader.GetUniformFloatValue("tex2Scale") - 0.5f);
        }

        if (key == Key.Left)
        {
            cubeShader.Use();
            cubeShader.SetFloat("mixFactor", cubeShader.GetUniformFloatValue("mixFactor") - 0.1f);
        }

        if (key == Key.Right)
        {
            cubeShader.Use();
            cubeShader.SetFloat("mixFactor", cubeShader.GetUniformFloatValue("mixFactor") + 0.1f);
        }

        if (key == Key.A)
        {
            camera.MoveLeft();
        }

        if (key == Key.D)
        {
            camera.MoveRight();
        }

        if (key == Key.W)
        {
            camera.MoveForward();
        }

        if (key == Key.S)
        {
            camera.MoveBackwards();
        }

        if (key == Key.R)
        {
            isOnUpdateTransform = !isOnUpdateTransform;
        }

        if (key == Key.Z)
        {
            Builder.GenerateRandomSlop(ref CubePositions);
        }

    }

    private static void OnMouseMove(IMouse mouse, Vector2 position)
    {
        camera.ProcessMouseMovement(position.X, position.Y);
    }
    private static void OnMouseWheelScrole(IMouse mouse, ScrollWheel scrollWheel)
    {
        camera.ProcessMouseScroll(scrollWheel.Y, ref fov);
        Console.WriteLine($"Wheel scrole, fov: {fov}");
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