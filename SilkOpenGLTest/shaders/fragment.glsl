#version 410 core
out vec4 FragColor;

in vec3 ourColor; // the input variable from the vertex shader (same name and same type)
in vec2 TextureCoords;

uniform sampler2D texture1;
uniform sampler2D texture2;
uniform float tex2Scale;
uniform float mixFactor;
void main()
{
    vec4 color1 = texture(texture1, TextureCoords);
    vec4 color2 = texture(texture2, TextureCoords * tex2Scale);
    FragColor = mix(color1, color2, mixFactor);    
}