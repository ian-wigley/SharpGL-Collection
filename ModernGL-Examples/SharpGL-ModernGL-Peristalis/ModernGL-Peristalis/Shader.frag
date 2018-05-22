#version 330 core

uniform sampler2D colorMap;
in vec2 vs_tex_coord;

layout (location = 0) out vec4 color;

void main(void)
{
	color = texture(colorMap, vs_tex_coord);
}