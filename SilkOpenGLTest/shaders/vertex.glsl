#version 410 core //Using version GLSL version 3.3
layout (location = 0) in vec3 aPos;
//layout (location = 1) in vec3 aColor;
layout (location = 1) in vec2 aTextureCoords;

//out vec3 ourColor; // output a color to the fragment shader
out vec2 TextureCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 transform;

void main()
{
    gl_Position = transform * projection * view * model * vec4(aPos, 1.0);
    //ourColor = aColor; // set ourColor to the input color we got from the vertex data 
    TextureCoords = aTextureCoords;
}