using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SilkOpenGLTest.Core;
using System.Numerics;
using System.Runtime.InteropServices;

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
    private static Core.Shader cubeShader;
    private static bool isLine = false;
    private static Core.Texture _texture1;
    private static Core.Texture _texture2;
    private static bool isOnUpdateTransform = false;

    private static Mesh _cubeMesh;

    private static Vertex V(float x, float y, float z, float u, float v)
        => new(new Vector3D<float>(x, y, z), new Vector2D<float>(u, v));

    private static Vertex[] GetCubeVertices()
    {
        return new Vertex[]
        {
            V(-0.5f, -0.5f, -0.5f, 0.0f, 0.0f),
            V( 0.5f, -0.5f, -0.5f, 1.0f, 0.0f),
            V( 0.5f,  0.5f, -0.5f, 1.0f, 1.0f),
            V( 0.5f,  0.5f, -0.5f, 1.0f, 1.0f),
            V(-0.5f,  0.5f, -0.5f, 0.0f, 1.0f),
            V(-0.5f, -0.5f, -0.5f, 0.0f, 0.0f),

            V(-0.5f, -0.5f,  0.5f, 0.0f, 0.0f),
            V( 0.5f, -0.5f,  0.5f, 1.0f, 0.0f),
            V( 0.5f,  0.5f,  0.5f, 1.0f, 1.0f),
            V( 0.5f,  0.5f,  0.5f, 1.0f, 1.0f),
            V(-0.5f,  0.5f,  0.5f, 0.0f, 1.0f),
            V(-0.5f, -0.5f,  0.5f, 0.0f, 0.0f),

            V(-0.5f,  0.5f,  0.5f,  1.0f, 0.0f),
            V(-0.5f,  0.5f, -0.5f,  1.0f, 1.0f),
            V(-0.5f, -0.5f, -0.5f,  0.0f, 1.0f),
            V(-0.5f, -0.5f, -0.5f,  0.0f, 1.0f),
            V(-0.5f, -0.5f,  0.5f,  0.0f, 0.0f),
            V(-0.5f,  0.5f,  0.5f,  1.0f, 0.0f),

            V(0.5f,  0.5f,  0.5f,  1.0f, 0.0f),
            V(0.5f,  0.5f, -0.5f,  1.0f, 1.0f),
            V(0.5f, -0.5f, -0.5f,  0.0f, 1.0f),
            V(0.5f, -0.5f, -0.5f,  0.0f, 1.0f),
            V(0.5f, -0.5f,  0.5f,  0.0f, 0.0f),
            V(0.5f,  0.5f,  0.5f,  1.0f, 0.0f), 

            V(-0.5f, -0.5f, -0.5f,  0.0f, 1.0f),
            V( 0.5f, -0.5f, -0.5f,  1.0f, 1.0f),
            V( 0.5f, -0.5f,  0.5f,  1.0f, 0.0f),
            V( 0.5f, -0.5f,  0.5f,  1.0f, 0.0f),
            V(-0.5f, -0.5f,  0.5f,  0.0f, 0.0f),
            V(-0.5f, -0.5f, -0.5f,  0.0f, 1.0f),

            V(-0.5f,  0.5f, -0.5f,  0.0f, 1.0f),
            V( 0.5f,  0.5f, -0.5f,  1.0f, 1.0f),
            V( 0.5f,  0.5f,  0.5f,  1.0f, 0.0f),
            V( 0.5f,  0.5f,  0.5f,  1.0f, 0.0f),
            V(-0.5f,  0.5f,  0.5f,  0.0f, 0.0f),
            V(-0.5f,  0.5f, -0.5f,  0.0f, 1.0f)
        };
    }

    public static Vector3D<float>[] CubePositions =
    {
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
        Console.WriteLine($"Vertex size: {Marshal.SizeOf<Vertex>()}");
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
        cubeShader = new Core.Shader(gl, "shaders/cube/vertex.glsl", "shaders/cube/fragment.glsl");
        gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        _cubeMesh = new Mesh(gl, GetCubeVertices());

        _texture1 = new Core.Texture(gl, "img/test.jpg");
        _texture2 = new Core.Texture(gl, "img/osu.png", wrapS: TextureWrapMode.MirroredRepeat, wrapT: TextureWrapMode.MirroredRepeat);

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

        if (input.Keyboards[0].IsKeyPressed(Key.Up))
        {
            cubeShader.Use();
            cubeShader.SetFloat("tex2Scale", cubeShader.GetUniformFloatValue("tex2Scale") + 500.5f);
        }

        if (input.Keyboards[0].IsKeyPressed(Key.Down))
        {
            cubeShader.Use();
            cubeShader.SetFloat("tex2Scale", cubeShader.GetUniformFloatValue("tex2Scale") - 5.5f);
        }

        float timeValue = (float)_window.Time;
        if (isOnUpdateTransform)
        {
            Matrix4X4<float> trans = Matrix4X4<float>.Identity;
            trans = Matrix4X4.CreateRotationY((float)timeValue) * trans;
            //trans = Matrix4X4.CreateTranslation(new Vector3D<float>(0.0f, 0.0f, (float)Math.Sin(timeValue) )) * trans;

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

        _texture1.Bind(TextureUnit.Texture0);
        _texture2.Bind(TextureUnit.Texture1);

        cubeShader.Use();

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
            //rotationMatrix = Matrix4X4.CreateRotationX((float)timeValue) * rotationMatrix;
            //rotationMatrix = Matrix4X4.CreateRotationY((float)timeValue) * rotationMatrix;
            //rotationMatrix = Matrix4X4.CreateRotationZ((float)timeValue) * rotationMatrix;
            model = rotationMatrix * model;
            cubeShader.SetMat4("model", model);

            // Рисуем куб
            _cubeMesh.Draw();
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
        _cubeMesh.Dispose();
        _texture1?.Dispose();
        _texture2?.Dispose();
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
}