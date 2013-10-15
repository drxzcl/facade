#version 130

precision highp float;

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

in vec4 position;
in vec4 normal;
in vec4 texcoord;
flat in  int doorx;
flat in  int flags;
flat in int  wallTileNumber;
flat in  int windowTileNumber;
flat in  int altWindowTileNumber;

out vec4 o_position;
out vec4 o_normal;
out vec4 o_texcoord;
flat out  int o_doorx;
flat out  int o_flags;
flat out  int o_wallTileNumber;
flat out  int o_windowTileNumber;
flat out  int o_altWindowTileNumber;

void main(void)
{
  //works only for orthogonal modelview
  o_normal = modelview_matrix * normal;
  o_texcoord = texcoord;
  
  o_position = position;
  o_position.w = 1;

  o_doorx = doorx;
  o_flags = flags;
  o_wallTileNumber = wallTileNumber;
  o_windowTileNumber = windowTileNumber;
  o_altWindowTileNumber = altWindowTileNumber;

  gl_Position = projection_matrix * modelview_matrix * o_position;
}