#version 130

precision highp float;

//uniform sampler2D wallTexture;
uniform sampler2DArray wallTextures;
uniform sampler2DArray windowTextures;
uniform sampler2DArray interiorTextures;
uniform sampler2DArray doorTextures;

const vec3 ambient = vec3(0.1, 0.1, 0.1);
const vec3 lightVecNormalized = normalize(vec3(0.5, 0.5, 2.0));
const vec3 lightColor = vec3(0.9, 0.9, 0.7);

in vec4 o_position;
in vec4 o_normal;
in vec4 o_texcoord;
in vec4 o_custom;

out vec4 out_frag_color;

float getdecs(inout float f, int decimals)
{
	f = f*pow(float(10),float(decimals));
	float result = floor(f+0.1); // Bias to stabilize the operation
	f = fract(f);
	return result;
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 4378.5453);
}


vec4 diff(void)
{
  //out_frag_color = vec4(1,0,0,1);

  vec2 tc = vec2(o_texcoord.x,-o_texcoord.y);

  vec4 custom = o_custom;
  float doorx = round(o_custom.x);
  int nogroundfloorwindows = int(getdecs(custom.y,1));
  int altwindowsabovedoor = int(getdecs(custom.y,1));
  float wallTileNumber = getdecs(custom.z,1);
  float windowTileNumber = getdecs(custom.z,1);
  float altWindowTileNumber = getdecs(custom.z,1);

  float interiorTileNumber = rand(floor(tc.xy+vec2(custom.w,0))) * 4;

  vec4 wall = texture(wallTextures, vec3(tc,wallTileNumber));
  
  if ((floor(tc.x) == doorx) && (ceil(tc.y) == 0))
  {
	// Door time!
	vec4 door = texture(doorTextures,vec3(tc,0));
	return door.a * door + (1-door.a) * wall;
  }

  vec4 window;
  vec4 blend;
  if ((nogroundfloorwindows == 1) && (ceil(tc.y) == 0))
  {
	blend = vec4(1,0,0,1);
  }
  else if ((altwindowsabovedoor == 1) && (floor(tc.x) == doorx))
  {
	   window = texture(windowTextures, vec3(tc,altWindowTileNumber*2));
	   blend = texture(windowTextures, vec3(tc,altWindowTileNumber*2+1));
  }
  else
  {
     window = texture(windowTextures, vec3(tc,windowTileNumber*2));
     blend = texture(windowTextures, vec3(tc,windowTileNumber*2+1));
  }

  vec4 interior = texture(interiorTextures, vec3(tc,interiorTileNumber));

  vec4 diff = wall * blend.x + window * blend.y + interior * blend.z;

  return diff;

}


void main(void)
{
  float diffuse = clamp(dot(lightVecNormalized, normalize(o_normal.xyz)), 0.0, 1.0);
  vec4 light = vec4(ambient + diffuse * lightColor, 1.0);

  out_frag_color = diff() * light;// vec4(o_texcoord.xyz,1);

}