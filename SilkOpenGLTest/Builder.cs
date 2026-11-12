using Silk.NET.Maths;
using SilkOpenGLTest.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkOpenGLTest
{
    internal class Builder
    {
        public static void GenerateFloor(ref Vector3D<float>[] CubePositions)
        {
            for (int x = -10; x <= 10; x++)
            {
                for (int z = -10; z <= 10; z++)
                {
                    CubePositions = CubePositions.Append(new Vector3D<float>(x, -0.5f, z)).ToArray();
                }
            }
        }

        public static void GenerateWalls(ref Vector3D<float>[] CubePositions)
        {
            for (int x = -5; x <= 5; x++)
            {
                for (int y = 0; y <= 3; y++)
                {
                    CubePositions = CubePositions.Append(new Vector3D<float>(x, y, -5f)).ToArray();
                }
            }
        }

        public static void GenerateRandomSlop(Scene scene)
        {
            for (int i = 0; i <=10; i++)
            {
                var thing = Random.Shared.Next(20);

            }
        }
    }
}
//private static readonly Vector3D<float>[] CubePositions =
//{
//        new Vector3D<float>( 0.0f,  0.0f,  0.0f),
//        new Vector3D<float>( 2.0f,  5.0f, -15.0f),
//        new Vector3D<float>(-1.5f, -2.2f, -2.5f),
//        new Vector3D<float>(-3.8f, -2.0f, -12.3f),
//        new Vector3D<float>( 2.4f, -0.4f, -3.5f),
//        new Vector3D<float>(-1.7f,  3.0f, -7.5f),
//        new Vector3D<float>( 1.3f, -2.0f, -2.5f),
//        new Vector3D<float>( 1.5f,  2.0f, -2.5f),
//        new Vector3D<float>( 1.5f,  0.2f, -1.5f),
//        new Vector3D<float>(-1.3f,  1.0f, -1.5f),
//        new Vector3D<float>(-5.0f,  5.0f, -5.0f),
//        new Vector3D<float>(-6.0f,  5.0f, -5.0f),
//        new Vector3D<float>(-7.0f,  5.0f, -5.0f),
//        new Vector3D<float>(-8.0f,  5.0f, -5.0f)

//    };