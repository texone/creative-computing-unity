#ifndef __VOLUME_RENDERING_INCLUDED__
#define __VOLUME_RENDERING_INCLUDED__

#include "UnityCG.cginc"

#ifndef MAX_MARCHING_STEPS
#define MAX_MARCHING_STEPS 100
#endif
const float MIN_DIST = 10.0;
const float MAX_DIST = 50.0;
const float EPSILON = 0.05;

half4 _Color;
sampler3D _Volume;
half _Intensity, _Threshold;
half3 _SliceMin, _SliceMax;
float4x4 _AxisRotationMatrix;

struct Ray {
  float3 origin;
  float3 dir;
};

struct AABB {
  float3 min;
  float3 max;
};

bool intersect(Ray r, AABB aabb, out float t0, out float t1)
{
  float3 invR = 1.0 / r.dir;
  float3 tbot = invR * (aabb.min - r.origin);
  float3 ttop = invR * (aabb.max - r.origin);
  float3 tmin = min(ttop, tbot);
  float3 tmax = max(ttop, tbot);
  float2 t = max(tmin.xx, tmin.yz);
  t0 = max(t.x, t.y);
  t = min(tmax.xx, tmax.yz);
  t1 = min(t.x, t.y);
  return t0 <= t1;
}

float3 localize(float3 p) {
  return mul(unity_WorldToObject, float4(p, 1)).xyz;
}

float3 get_uv(float3 p) {
  // float3 local = localize(p);
  return (p + 0.5);
}

float sample_volume(float3 uv, float3 p)
{
  float v = tex3D(_Volume, uv).r * _Intensity;

  float3 axis = mul(_AxisRotationMatrix, float4(p, 0)).xyz;
  axis = get_uv(axis);
  float min = step(_SliceMin.x, axis.x) * step(_SliceMin.y, axis.y) * step(_SliceMin.z, axis.z);
  float max = step(axis.x, _SliceMax.x) * step(axis.y, _SliceMax.y) * step(axis.z, _SliceMax.z);

  return v * min * max;
}

bool outside(float3 uv)
{
  const float EPSILON = 0.01;
  float lower = -EPSILON;
  float upper = 1 + EPSILON;
  return (
			uv.x < lower || uv.y < lower || uv.z < lower ||
			uv.x > upper || uv.y > upper || uv.z > upper
		);
}

struct appdata
{
  float4 vertex : POSITION;
  float2 uv : TEXCOORD0;
};

struct v2f
{
  float4 vertex : SV_POSITION;
  float2 uv : TEXCOORD0;
  float3 world : TEXCOORD1;
  float3 local : TEXCOORD2;
};

v2f vert(appdata v)
{
  v2f o;
  o.vertex = UnityObjectToClipPos(v.vertex);
  o.uv = v.uv;
  o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
  o.local = v.vertex.xyz;
  return o;
}

/**
 * Signed distance function describing the scene.
 * 
 * Absolute value of the return value indicates the distance to the surface.
 * Sign indicates whether the point is inside or outside the surface,
 * negative indicating inside.
 */
float sceneSDF(float3 samplePoint) { 
	return tex3D(_Volume, samplePoint);;    
}

/**
 * Return the shortest distance from the eyepoint to the scene surface along
 * the marching direction. If no part of the surface is found between start and end,
 * return end.
 * 
 * eye: the eye point, acting as the origin of the ray
 * marchingDirection: the normalized direction to march in
 * start: the starting distance away from the eye
 * end: the max distance away from the ey to march before giving up
 */
float shortestDistanceToSurface(float3 eye, float3 marchingDirection, float start, float end) {
	float depth = start;
	for (int i = 0; i < MAX_MARCHING_STEPS; i++) {
		float dist = sceneSDF(eye + depth * marchingDirection);
		if (dist < EPSILON) {
			return depth;
		}
		depth += dist;
		if (depth >= end) {
			return end;
		}
	}
	return end;
}

float iTime;

/**
 * Using the gradient of the SDF, estimate the normal on the surface at point p.
 */
float3 estimateNormal(float3 p) {
	return normalize(float3(
		sceneSDF(float3(p.x + EPSILON, p.y, p.z)) - sceneSDF(float3(p.x - EPSILON, p.y, p.z)),
		sceneSDF(float3(p.x, p.y + EPSILON, p.z)) - sceneSDF(float3(p.x, p.y - EPSILON, p.z)),
		sceneSDF(float3(p.x, p.y, p.z  + EPSILON)) - sceneSDF(float3(p.x, p.y, p.z - EPSILON))
    ));
}

/**
 * Lighting contribution of a single point light source via Phong illumination.
 * 
 * The float3 returned is the RGB color of the light's contribution.
 *
 * k_a: Ambient color
 * k_d: Diffuse color
 * k_s: Specular color
 * alpha: Shininess coefficient
 * p: position of point being lit
 * eye: the position of the camera
 * lightPos: the position of the light
 * lightIntensity: color/intensity of the light
 *
 * See https://en.wikipedia.org/wiki/Phong_reflection_model#Description
 */
float3 phongContribForLight(float3 k_d, float3 k_s, float alpha, float3 p, float3 eye,
                          float3 lightPos, float3 lightIntensity) {
    float3 N = estimateNormal(p);
    float3 L = normalize(lightPos - p);
    float3 V = normalize(eye - p);
    float3 R = normalize(reflect(-L, N));
    
    float dotLN = dot(L, N);
    float dotRV = dot(R, V);
    
    if (dotLN < 0.0) {
        // Light not visible from this point on the surface
        return float3(0.0, 0.0, 0.0);
    } 
    
    if (dotRV < 0.0) {
        // Light reflection in opposite direction as viewer, apply only diffuse
        // component
        return lightIntensity * (k_d * dotLN);
    }
    return lightIntensity * (k_d * dotLN + k_s * pow(dotRV, alpha));
}

/**
 * Lighting via Phong illumination.
 * 
 * The float3 returned is the RGB color of that point after lighting is applied.
 * k_a: Ambient color
 * k_d: Diffuse color
 * k_s: Specular color
 * alpha: Shininess coefficient
 * p: position of point being lit
 * eye: the position of the camera
 *
 * See https://en.wikipedia.org/wiki/Phong_reflection_model#Description
 */
float3 phongIllumination(float3 k_a, float3 k_d, float3 k_s, float alpha, float3 p, float3 eye) {
    const float3 ambientLight = 0.5 * float3(1.0, 1.0, 1.0);
    float3 color = ambientLight * k_a;
    
    float3 light1Pos = float3(4.0 * sin(iTime),
                          2.0,
                          4.0 * cos(iTime));
    float3 light1Intensity = float3(0.8, 0.4, 0.4);
    
    color += phongContribForLight(k_d, k_s, alpha, p, eye,
                                  light1Pos,
                                  light1Intensity);
    
    float3 light2Pos = float3(20.0 * sin(0.37 * iTime),
                          20.0 * cos(0.37 * iTime),
                          2.0);
    float3 light2Intensity = float3(0.5,0.5,0.5);
    
    color += phongContribForLight(k_d, k_s, alpha, p, eye,
                                  light2Pos,
                                  light2Intensity);    
                                  
    return color; 
}

fixed4 frag(v2f i) : SV_Target
{
  Ray ray;
  // ray.origin = localize(i.world);
  ray.origin = i.local;

  // world space direction to object space
  float3 dir = (i.world - _WorldSpaceCameraPos);
  ray.dir = normalize(mul(unity_WorldToObject, dir));

  AABB aabb;
  aabb.min = float3(-0.5, -0.5, -0.5);
  aabb.max = float3(0.5, 0.5, 0.5);

  float tnear;
  float tfar;
  intersect(ray, aabb, tnear, tfar);

  tnear = max(0.0, tnear);

  // float3 start = ray.origin + ray.dir * tnear;
  float3 start = ray.origin;
  float3 end = ray.origin + ray.dir * tfar;
  float dist = abs(tfar - tnear); // float dist = distance(start, end);
  float step_size = dist / float(ITERATIONS);
  float3 ds = normalize(end - start) * step_size;

  float4 dst = float4(0, 0, 0, 0);
  float3 p = start;

  [unroll]
  /*
  for (int iter = 0; iter < MAX_MARCHING_STEPS; iter++)
  {
    float3 uv = get_uv(p);
    float v = (sample_volume(uv, p) ) ;
    float4 src = float4(v, v, v, v);
    src.a *= 0.01;
    src.rgb *= src.a;

    // blend
    dst = (1.0 - dst.a) * src + dst;
    p += ds;

    if (dst.a > _Threshold) break;
  }*/
  float depth = 0;
  float density = 0;
  for (int iter = 0; iter < MAX_MARCHING_STEPS; iter++)
  {
    float3 uv = get_uv(p);
    float v = sceneSDF(uv);
    if(v > 0.5){
    density += v;
       // break;
    }
	depth += dist;
  }
  depth *= 0.002;
  
  density *= 0.0025;
  
  float3 pos = start + depth * normalize(end - start);
  
  float3 K_a = float3(0.0, 0.0, 1.0);
    float3 K_d = float3(1.5, 0.9, 1.);
    float3 K_s = float3(1.0, 1.0, 1.0); 
    float shininess = 10.0;
    
    float3 color = phongIllumination(K_a, K_d, K_s, shininess, pos, start);

  return float4(density,density, density,1);
}

#endif 
