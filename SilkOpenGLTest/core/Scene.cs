using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkOpenGLTest.Core
{
    internal class Scene
    {
        public List<GameObject> GameObjects { get; } = new();
        public Camera Camera { get; set; }

        public Scene(Camera camera)
        {
            Camera = camera;
        }

        public void AddGameObject(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
        }
        public void RemoveGameObject(GameObject gameObject)
        {
            GameObjects.Remove(gameObject);
        }

        public void Draw(float aspectRatio, float fov)
        {
            Matrix4X4<float> view = Camera.GetView();
            Matrix4X4<float> projection = Matrix4X4.CreatePerspectiveFieldOfView(
                fov, aspectRatio, 0.1f, 100.0f);

            foreach (var obj in GameObjects)
            {
                obj.Draw(view, projection);
            }
        }
    }
}
