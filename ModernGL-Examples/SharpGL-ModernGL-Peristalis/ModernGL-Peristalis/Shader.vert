#version 330 core

uniform mat4 ModelWorld4x4;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

out vec2 vs_tex_coord;

layout (location = 0) in vec3 in_position;
layout (location = 1) in vec2 in_tex_coord;

void main(void)
{
	vs_tex_coord = in_tex_coord;
	gl_Position = projectionMatrix * viewMatrix * ModelWorld4x4 * vec4(in_position, 1.0);
}