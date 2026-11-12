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
    private static bool isLine = false;
    private static Scene _scene;
    private static Core.Texture _texture1;
    private static Core.Texture _texture2;
    private static Core.Shader cubeShader;
    private static Mesh _cubeMesh;
    private static Material _cubeMaterial;

    private static bool isOnUpdateTransform = false;


    private static float _tex2Scale = 1.0f;
    private static float _mixFactor = 0f;
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


        _texture1 = new Core.Texture(gl, "img/test.jpg");
        _texture2 = new Core.Texture(gl, "img/osu.png", wrapS: TextureWrapMode.MirroredRepeat, wrapT: TextureWrapMode.MirroredRepeat);

        _cubeMaterial = new Material(cubeShader)
            .SetTexture("texture1", _texture1, TextureUnit.Texture0)
            .SetTexture("texture2", _texture2, TextureUnit.Texture1)
            .SetFloat("tex2Scale", 1.0f)
            .SetFloat("mixFactor", 0f);

        _cubeMesh = new Mesh(gl, GetCubeVertices());

        _scene = new Scene(camera);

        Builder.GenerateFloor(ref CubePositions);
        Builder.GenerateWalls(ref CubePositions);

        foreach (var pos in CubePositions)
        {
            _scene.AddGameObject(new GameObject(_cubeMesh, _cubeMaterial)
            {
                Position = pos
            });
        }

        gl.Enable(GLEnum.DepthTest);
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
            _tex2Scale += 0.5f;
        }

        if (input.Keyboards[0].IsKeyPressed(Key.Down))
        {
            _tex2Scale -= 0.5f;
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

        _cubeMaterial
            .SetFloat("tex2Scale", _tex2Scale)
            .SetFloat("mixFactor", _mixFactor)
            .Bind();

        if (isOnUpdateTransform)
        {
            foreach (var obj in _scene.GameObjects)
            {
                obj.Rotation = new Vector3D<float>(0, timeValue * 50f, 0);
            }
        }

        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _scene.Draw(_aspectRatio, fov);
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
            _tex2Scale += 0.5f;
        }

        if (key == Key.Down)
        {
            _tex2Scale -= 0.5f;
        }

        if (key == Key.Left)
        {
            _mixFactor -= 0.1f;
        }

        if (key == Key.Right)
        {
            _mixFactor += 0.1f;
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
            var thing = Random.Shared.Next(20);

            _scene.AddGameObject(new GameObject(_cubeMesh, _cubeMaterial)
            {
                Position = new (Random.Shared.Next(-thing, thing),
                Random.Shared.Next(-thing, thing),
                Random.Shared.Next(-thing, thing))
            });
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