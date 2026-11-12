using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SilkOpenGLTest.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkOpenGLTest.Core
{
    internal class GameObject
    {
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }

        public Vector3D<float> Position { get; set; } = Vector3D<float>.Zero;
        public Vector3D<float> Rotation { get; set; } = Vector3D<float>.Zero;
        public Vector3D<float> Scale { get; set; } = new Vector3D<float>(1.0f, 1.0f, 1.0f);

        public GameObject(Mesh mesh, Material material)
        {
            Mesh = mesh;
            Material = material;
        }

        public Matrix4X4<float> GetModelMatrix()
        {
            Matrix4X4<float> model = Matrix4X4<float>.Identity;
            float radX = Rotation.X * MathF.PI / 180f;
            float radY = Rotation.Y * MathF.PI / 180f;
            float radZ = Rotation.Z * MathF.PI / 180f;

            model = Matrix4X4.CreateScale(Scale) *
                    Matrix4X4.CreateRotationX(radX) *
                    Matrix4X4.CreateRotationY(radY) *
                    Matrix4X4.CreateRotationZ(radZ) *
                    Matrix4X4.CreateTranslation(Position);

            return model;
            //Vector3D<float> axis = new Vector3D<float>(1.0f, 0.3f, 0.5f);
            //axis = Vector3D.Normalize(axis);
            //Quaternion<float> rotation = Quaternion<float>.CreateFromAxisAngle(axis, angleRad);
            //Matrix4X4<float> rotationMatrix = Matrix4X4.CreateFromQuaternion(rotation);
        }

        public void Draw(Matrix4X4<float> view, Matrix4X4<float> projection)
        {
            Material.Bind();
            Material.Shader.SetMat4("view", view);
            Material.Shader.SetMat4("projection", projection);
            Material.Shader.SetMat4("model", GetModelMatrix());
            Mesh.Draw();
        }
    }
}
