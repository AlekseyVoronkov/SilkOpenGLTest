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

        public Vector3D<float> cameraPos = new(0.0f, 1.0f, 3.0f);
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

        private bool IsInsideCube(Vector3D<float> cameraPos)
        {
            foreach (var pos in Program.CubePositions)
            {
                if ((pos[0] - 0.55 <= cameraPos.X && cameraPos.X <= (pos[0] + 0.55)) &&
                    (pos[1] - 0.55 <= cameraPos.Y && cameraPos.Y <= (pos[1] + 0.55)) &&
                    (pos[2] - 0.55 <= cameraPos.Z && cameraPos.Z <= (pos[2] + 0.55)))
                {
                    return true;
                }
            }
            return false;
            //if ((Program.CubePositions[0][0] - 0.5 <= cameraPos.X && cameraPos.X <= (Program.CubePositions[0][0] + 0.5)) &&
            //   (Program.CubePositions[0][1] - 0.5 <= cameraPos.Y && cameraPos.Y <= (Program.CubePositions[0][1] + 0.5)) &&
            //   (Program.CubePositions[0][2] - 0.5 <= cameraPos.Z && cameraPos.Z <= (Program.CubePositions[0][2] + 0.5)))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        public Matrix4X4<float> GetView()
        {
            return view;
        }

        public void MoveLeft()
        {
            Vector3D<float> newPos = cameraPos - Vector3D.Normalize(Vector3D.Cross(cameraFront, cameraUp)) * cameraSpeed;
            
            if (!IsInsideCube(newPos))
            {
                cameraPos = newPos;
                UpdateView();
                Console.WriteLine("cameraPos: " + cameraPos.ToString());
            }
        }
        public void MoveRight()
        {
            Vector3D<float> newPos = cameraPos + Vector3D.Normalize(Vector3D.Cross(cameraFront, cameraUp)) * cameraSpeed;

            if (!IsInsideCube(newPos))
            {
                cameraPos = newPos;
                UpdateView();
                Console.WriteLine("cameraPos: " + cameraPos.ToString());
            }
        }

        public void MoveForward()
        {
            Vector3D<float> newPos = cameraPos + cameraSpeed * cameraFront;

            if (!IsInsideCube(newPos))
            {
                cameraPos = newPos;
                UpdateView();
                Console.WriteLine("cameraPos: " + cameraPos.ToString());
            }
            else { Console.WriteLine("Collision detected! Cannot move forward.");
            }
        }

        public void MoveBackwards()
        {
            Vector3D<float> newPos = cameraPos - cameraSpeed * cameraFront;

            if (!IsInsideCube(newPos))
            {
                cameraPos = newPos;
                UpdateView();
                Console.WriteLine("cameraPos: " + cameraPos.ToString());
            }
        }
    }
}
