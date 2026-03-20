using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkOpenGLTest
{
    internal class Camera
    {
        public float yaw = -90.0f;
        public float pitch = 0.0f;
        private bool firstMouse = true;
        private float lastX, lastY;

        public Vector3D<float> cameraPos = new(0.0f, 0.0f, 3.0f);
        public Vector3D<float> cameraFront = new(0.0f, 0.0f, -1.0f);
        public Vector3D<float> cameraUp = new(0.0f, 1.0f, 0.0f);
        private Matrix4X4<float> view;

        public float cameraSpeed = 2.5f;

        public Camera() 
        { 
        }

        public void ProcessMouseScroll(float yoffset, ref float fov)
        {
            fov -= (float)yoffset * 0.1f;

            if (fov < 0.1f)
            {
                fov = 0.1f;
            }

            if (fov > 2.0f)
            {
                fov = 2.0f;
            }

            UpdateView();
        }

        public void ProcessMouseMovement(float xpos, float ypos)
        {
            if (firstMouse)
            {
                lastX = xpos;
                lastY = ypos;
                firstMouse = false;
            }

            float xoffset = xpos - lastX;
            float yoffset = lastY - ypos;
            lastX = xpos;
            lastY = ypos;

            float sensitivity = 0.1f;
            xoffset *= sensitivity;
            yoffset *= sensitivity;

            yaw += xoffset;
            pitch += yoffset;

            if (pitch > 89.0f) pitch = 89.0f;
            if (pitch < -89.0f) pitch = -89.0f;

            Vector3D<float> direction;
            direction.X = MathF.Cos(yaw * MathF.PI / 180f) * MathF.Cos(pitch * MathF.PI / 180f);
            direction.Y = MathF.Sin(pitch * MathF.PI / 180f);
            direction.Z = MathF.Sin(yaw * MathF.PI / 180f) * MathF.Cos(pitch * MathF.PI / 180f);

            cameraFront = Vector3D.Normalize(direction);
            UpdateView();
        }

        private void UpdateView()
        {
            view = Matrix4X4.CreateLookAt(cameraPos, cameraPos + cameraFront, cameraUp);
        }

        public Matrix4X4<float> GetView()
        {
            return view;
        }

        public void MoveLeft()
        {
            cameraPos -= Vector3D.Normalize(Vector3D.Cross(cameraFront, cameraUp)) * cameraSpeed;
            UpdateView();
            Console.WriteLine("cameraPos: " + cameraPos.ToString());
        }
        public void MoveRight()
        {
            cameraPos += Vector3D.Normalize(Vector3D.Cross(cameraFront, cameraUp)) * cameraSpeed;
            UpdateView();
            Console.WriteLine("cameraPos: " + cameraPos.ToString());
        }

        public void MoveForward()
        {
            cameraPos += cameraSpeed * cameraFront;
            UpdateView();
            Console.WriteLine("cameraPos: " + cameraPos.ToString());
        }

        public void MoveBackwards()
        {
            cameraPos -= cameraSpeed * cameraFront;
            UpdateView();
            Console.WriteLine("cameraPos: " + cameraPos.ToString());
        }
    }
}
