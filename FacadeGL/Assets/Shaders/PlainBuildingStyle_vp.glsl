#version 130

precision highp float;

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

in vec4 position;
in vec4 normal;
in vec4 texcoord;
in vec4 custom;

out vec4 o_position;
out vec4 o_normal;
out vec4 o_texcoord;
out vec4 o_custom;

void main(void)
{
  //works only for orthogonal modelview
  o_normal = modelview_matrix * normal;
  o_texcoord = texcoord;
  //o_texcoord.xy = vec2(1,0);
  o_custom = custom;
  
  o_position = position;
  o_position.w = 1;

  gl_Position = projection_matrix * modelview_matrix * o_position;
}