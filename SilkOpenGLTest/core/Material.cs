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
    internal class Material : IDisposable
    {
        public Shader Shader { get; }

        private readonly List<(string name, Texture texture, TextureUnit textureUnit)> _textures = new();

        private readonly Dictionary<string, object> _uniforms = new();

        public Material(Shader shader)
        {
            Shader = shader;
        }

        public Material SetTexture(string name , Texture texture, TextureUnit textureUnit)
        {
            _textures.RemoveAll(t => t.name == name);
            _textures.Add((name, texture, textureUnit));
            return this;
        }

        public Material SetFloat(string name, float value)
        {
            _uniforms[name] = value;
            return this;
        }

        public Material SetInt(string name, int value)
        {
            _uniforms[name] = value;
            return this;
        }

        public Material SetVec3(string name, Vector3D<float> value)
        {
            _uniforms[name] = value;
            return this;
        }

        public void Bind()
        {
            Shader.Use();

            foreach(var (name, texture, textureUnit) in _textures)
            {
                texture.Bind(textureUnit);
                int samplerLocation = (int)textureUnit - (int)TextureUnit.Texture0;
                Shader.SetInt(name, samplerLocation);
            }

            foreach(var (name, value) in _uniforms)
            {
                switch(value)
                {
                    case float f:
                        Shader.SetFloat(name, f);
                        break;
                    case int i:
                        Shader.SetInt(name, i);
                        break;
                    case Vector3D<float> v3:
                        Shader.SetVec3(name, v3);
                        break;
                    default:
                        throw new NotSupportedException($"Uniform type {value.GetType()} is not supported");
                }
            }
        }

        public void Dispose()
        {
            // me bombo 
        }   
    }
}
