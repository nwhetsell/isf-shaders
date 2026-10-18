/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "loicvdb <https://github.com/loicvdb>",
    "DESCRIPTION": "Path tracing with global illumination, converted from <https://www.shadertoy.com/view/Wt3XRX>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "enableBloom",
            "LABEL": "Enable bloom",
            "TYPE": "bool",
            "DEFAULT": false
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "bufferA",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferB",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferC",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferD",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {

        }
    ]
}*/

// #define ISF_EDITOR_WEBSITE

#include "lygia/color/tonemap/aces.glsl"
#include "lygia/generative/random.glsl"
#include "lygia/lighting/common/henyeyGreenstein.glsl"
#define RAYMARCH_SAMPLES 128
float directLightMinDist(in vec3 ro, in vec3 rd);
#define RAYMARCH_MIN_DIST directLightMinDist(ro, rd)
#define RAYMARCH_MIN_HIT_DIST 0.015
#include "lygia/lighting/raymarch/cast.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/hammersley.glsl"
#ifdef ISF_EDITOR_WEBSITE
#include "lygia/math/inverse.glsl"
#endif
#include "lygia/math/rotate3dX.glsl"
#include "lygia/math/rotate3dY.glsl"
#include "lygia/math/rotate3dZ.glsl"
#include "lygia/math/sum.glsl"
#include "lygia/space/aspect.glsl"
#include "lygia/space/center.glsl"
#include "lygia/space/uncenter.glsl"
#include "lygia-additions/const.glsl"


//increase this number for a better GI
#define IndirectSamples 1

//increase to remove more noise, but might make the result blurrier
#define SamplesLimit 150

//GI bounces
#define Bounces 1


#define PixelAcceptance 1.5
#define PixelCheckDistance .5


#define MaxStepsDirect 128
#define MaxStepsIndirect 32
#define MaxShadowSteps 32
#define FogSteps 8

#define MaxDist 4.
#define MinDist .015

#define DoFClamping .3
#define DoFSamples 128

#define Density vec3(.0025, .0045, .006)
#define Anisotropy .4
#define FogRange 13.


vec3 LightDir = normalize(vec3(.0, .0, -1));
vec3 LightColor = vec3(1.) * 6.;
float LightRadius = .02;

struct Camera {
    vec3 pos, rot;
    float focalLength, focalDistance, aperture;
};


mat3 rotationMatrix(vec3 rotEuler) {
    return rotate3dZ(-rotEuler.z) * rotate3dX(-rotEuler.x) * rotate3dY(rotEuler.y);
}

Camera getCam(float time) {
    //time = 0.;
    vec3 rot = vec3(cos(time * 0.4) / 6., 1. + time * 0.2 + sin(time * 0.2) / 4., 0.5);
    return Camera(vec3(0, 0, -10) * rotationMatrix(rot), rot, 1., 7.5, 0.025);
}

vec3 uv2dir(Camera cam, vec2 uv) {
    return normalize(vec3(uv, cam.focalLength)) * rotationMatrix(cam.rot);
}

vec2 pos2uv(Camera cam, vec3 pos) {
    vec3 dir = normalize(pos - cam.pos) * inverse(rotationMatrix(cam.rot));
    return dir.xy * cam.focalLength / dir.z;
}


float sdf(vec3 position, out vec3 diffuseColor, out vec3 emissionColor) {
    diffuseColor = vec3(1);
    emissionColor = vec3(0);
    float Scale = 2.25;
    float Radius = 0.25;

    float time = 15.;

    mat3 Rotation = rotationMatrix(vec3(time, time * 0.7, time * 0.4) * 0.2);
    Scale += sin(time * 0.5) * 0.25;
    Radius += cos(time) * 0.25;

	vec4 scalevec = vec4(Scale, Scale, Scale, abs(Scale)) / Radius;
	float C1 = abs(Scale - 1.);
	float C2 = pow(abs(Scale), float(1 - /*iterations*/7));
	vec4 p = vec4(position.xyz * Rotation, 1);
	vec4 p0 = p;
	for (int i = 0; i < 7; i++) {
    	p.xyz = clamp(p.xyz, -1., 1.) * 2. - p.xyz;
    	p.xyzw *= clamp(max(Radius/dot(p.xyz, p.xyz), Radius), 0., 1.);
        if (i < 3)
            p.xyz *= Rotation;
    	p.xyzw = p * scalevec + p0;
	}
    /*diffuseColor = fract(p0.x)<.1 ? vec3(.2) : vec3(1.);
    emissionColor = fract(p0.x)<.1 ? (normalize(p.xyz)*.5+.5)*10. : vec3(0.);*/
	return (length(p.xyz) - C1) / p.w - C2;
}

Material raymarchMap(vec3 pos)
{
    Material mat = materialNew();
    mat.position = pos;
    vec3 dc, ec;
    mat.sdf = sdf(pos, dc, ec);
    return mat;
}

vec3 normalEstimation(vec3 pos) {
  vec2 k = vec2(MinDist, 0);
  return normalize(vec3(raymarchMap(pos + k.xyy).sdf - raymarchMap(pos - k.xyy).sdf,
	  					raymarchMap(pos + k.yxy).sdf - raymarchMap(pos - k.yxy).sdf,
  						raymarchMap(pos + k.yyx).sdf - raymarchMap(pos - k.yyx).sdf));
}

float henyeyGreenstein(vec3 dirI, vec3 dirO) {
	return PI*PI * henyeyGreenstein(dot(dirI, dirO), Anisotropy);
}

vec3 directLight(vec3 pos, vec3 normal) {
    //return vec3(0.);
    float dotLight = -dot(normal, LightDir);
    if (dotLight < 0.)
        return vec3(0);
    vec3 pos0 = pos;
    float minAngle = LightRadius;
    for (int i = 0; i < MaxShadowSteps; i++) {
        float dist = raymarchMap(pos).sdf;
        if (dist > MaxDist)
            break;
        if (dist < MinDist)
            return vec3(0);
        pos -= LightDir * dist * 2.5; // goes 2.5 times faster since we don't need details
        minAngle = min(asin(dist / length(pos - pos0)), minAngle);
    }
    return LightColor * dotLight * clamp(minAngle / LightRadius, 0., 1.);
}

float directLightMinDist(in vec3 ro, in vec3 rd)
{
    float t = 0.;
    for (int i = 0; i < 2; i++) {
        t += raymarchMap(ro + rd * t).sdf;
    }
    return t;
}


float seed;

float randomFloat() {
    return fract(sin(seed++) * 43758.54536156);
}

vec3 ortho(vec3 v) {
  return abs(v.x) > abs(v.z) ? vec3(-v.y, v.x, 0)  : vec3(0, -v.z, v.y);
}

vec3 getCosineWeightedSample(vec3 dir) {
    vec3 o1 = normalize(ortho(dir));
    vec3 o2 = normalize(cross(dir, o1));
    vec3 weight = hemisphereCosSample(vec2(randomFloat(), randomFloat()));
    return weight.x * o1 + weight.y * o2 + weight.z * dir;
}


vec3 background(vec3 dir) {
    return LightColor * Density * 8. * henyeyGreenstein(-LightDir, dir);
}

vec3 directBackground(vec3 dir) {
    return LightColor * Density * 1.;
}

bool traceC(inout vec3 pos, in vec3 dir, out vec3 normal) {
    for (int i = 0; i < MaxStepsIndirect; i++) {
        float dist = raymarchMap(pos).sdf;
        if (dist > MaxDist)
            break;
        if (dist < MinDist) {
            normal = normalEstimation(pos);
            pos -= (2. * MinDist - dist) * dir;
            return true;
        }
        pos += dir * dist;
    }
    return false;
}

vec3 sampleIndirectLight(vec3 pos, vec3 normal) {
    vec3 dir;
    vec3 abso = vec3(1);
    vec3 light = vec3(0);
    vec3 dc;
    vec3 ec;
    for (int i = 0; i < Bounces; i++) {
        dir = getCosineWeightedSample(normal);
        if (!traceC(pos, dir, normal))
            return light + abso * background(dir);
        sdf(pos, dc, ec);
        light += abso * (ec + dc * directLight(pos, normal));
        abso *= dc;
    }
    return light;
}

float distancePixel(vec2 fragCoord, vec4 hit){
    if (min(RENDERSIZE, fragCoord) != fragCoord &&
        max(vec2(0.),   fragCoord) != fragCoord) {
        return MaxDist;
    }
    vec4 pos = IMG_NORM_PIXEL(bufferA, fragCoord / RENDERSIZE);
    return length(pos - hit);
}

vec2 unaspect(vec2 st, vec2 s) {
    st.x = st.x * (s.y / s.x);
    return st;
}

vec4 previousSample(vec4 hit) {
    vec2 previous_uv = pos2uv(getCam(TIME - TIMEDELTA), hit.xyz);
    vec2 previousFragCoord = uncenter(2. * unaspect(previous_uv, RENDERSIZE)) * RENDERSIZE;

    vec2 minFragCoord;
    float minDist = FLT_MAX;
    for (int y = -1; y <= 1; y++)
    for (int x = -1; x <= 1; x++) {
        vec2 fragCoord = previousFragCoord + PixelCheckDistance * vec2(x, y);
        float dist = distancePixel(fragCoord, hit);
        if (dist < minDist){
            minFragCoord = fragCoord;
            minDist = dist;
        }
   	}

    Camera cam = getCam(TIME);
    if (minDist < PixelAcceptance * length(hit.xyz - cam.pos) / cam.focalLength / RENDERSIZE.y)
        return IMG_NORM_PIXEL(bufferC, minFragCoord / RENDERSIZE);

    return vec4(0);
}

#ifndef ISF_EDITOR_WEBSITE

mat3 gaussianFilter = mat3(41, 26, 7,
                           26, 16, 4,
                           7,  4,  1) / 273.;

vec3 bloom(float scale, float threshold, vec2 fragCoord){
    float logScale = log2(scale);
    vec3 bloom = vec3(0);
    for(int y = -2; y <= 2; y++)
    for(int x = -2; x <= 2; x++)
        bloom += gaussianFilter[abs(x)][abs(y)] * textureLod(bufferD, (fragCoord+vec2(x, y)*scale)/RENDERSIZE, logScale).rgb;

    return max(bloom - vec3(threshold), vec3(0));
}

#endif


void main()
{
    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        gl_FragColor = IMG_THIS_PIXEL(bufferB);
    }
    else if (PASSINDEX == 1) // Shadertoy Buffer B
    {
        vec2 uv = 0.5 * aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);

        Camera cam = getCam(TIME);

        vec3 rayOrigin = cam.pos;
        vec3 rayDirection = uv2dir(cam, uv);

        Material res = raymarchCast(rayOrigin, rayDirection);
        if (res.valid) {
            float twiceMinHitDist = 2. * RAYMARCH_MIN_HIT_DIST;
            for (int i = 0; i < 3; i++) {
                res.sdf -= twiceMinHitDist - raymarchMap(rayOrigin + rayDirection * res.sdf).sdf;
            }

            gl_FragColor = vec4(rayOrigin + rayDirection * res.sdf, 1);
        } else {
            gl_FragColor = vec4(0);
        }
    }
    else if (PASSINDEX == 2) // Shadertoy Buffer C
    {
        seed = sum(gl_FragCoord.xy * vec2(0.256435865, 0.316548465)) + sin(TIME) * 16886.3158915;

        Camera cam = getCam(TIME);

        vec4 hit = IMG_THIS_PIXEL(bufferB);
        bool valid = hit.a > 0.;
        if (valid) {
            #if 0
            gl_FragColor = previousSample(hit);
            gl_FragColor.rgb = gl_FragColor.a == 0. ? vec3(1, 0, 0) : vec3(0, 1, 0);
            gl_FragColor.a = 1.;
            #else
            vec3 normal = normalEstimation(hit.xyz);

            vec3 iLight = vec3(0);
            for (int i = 0; i < IndirectSamples; i++) {
                iLight += sampleIndirectLight(hit.xyz, normal) / float(IndirectSamples);
            }

            gl_FragColor = previousSample(hit);
            if (gl_FragColor.a <= float(SamplesLimit))
                gl_FragColor.a += float(IndirectSamples);
            iLight = clamp(iLight, 0., gl_FragColor.a / 10.); // clamp gi for low sample count
            gl_FragColor.rgb = mix(gl_FragColor.rgb, iLight, 1. / (gl_FragColor.a / float(IndirectSamples)));
            #endif
        } else {
            vec2 uv = 0.5 * aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);
            gl_FragColor = vec4(directBackground(uv2dir(cam, uv)), 1);
        }
    }
    else if (PASSINDEX == 3) // Shadertoy Buffer D
    {
        Camera cam = getCam(TIME);

        vec4 col = IMG_THIS_PIXEL(bufferC);
        vec4 tx = IMG_THIS_PIXEL(bufferB);
        vec3 finalPos = tx.rgb;
        col.a = length(cam.pos-finalPos);

        vec2 uv = (gl_FragCoord.xy-RENDERSIZE/2.0) / RENDERSIZE.y;
        vec3 dir = uv2dir(cam, uv);

        if(tx.a != 0.) {
            vec3 dc, ec;
            sdf(finalPos, dc, ec);
            col.rgb = ec + dc * (col.rgb + directLight(finalPos, normalEstimation(finalPos)));
        } else {
            finalPos = cam.pos + dir * 100.;
        }

        vec3 volCol = vec3(0.), volAbs = vec3(1.), pos = cam.pos;
        float stepDist = (tx.a == 0. ? FogRange : min(FogRange, col.a))/float(FogSteps);
        vec3 stepAbs = exp(-Density*stepDist);
        vec3 stepCol = (vec3(1.) - stepAbs) * henyeyGreenstein(-LightDir, dir);
        pos += stepDist * dir * random(gl_FragCoord.xy/vec2(1024)) /*texture(iChannel2, gl_FragCoord.xy/vec2(1024)).a*/;
        for(int i = 0; i < FogSteps; i++){
            volAbs *= stepAbs;
            volCol += stepCol*volAbs*directLight(pos, -LightDir);
            pos += dir * stepDist;
        }

        col.rgb = col.rgb*volAbs + volCol;

        gl_FragColor = col;
    }
    else // Shadertoy Image
    {
        vec4 col = vec4(0.);

#ifndef ISF_EDITOR_WEBSITE
        Camera cam = getCam(TIME);
        float frd = RENDERSIZE.y*cam.aperture*DoFClamping;
        int rd = int(ceil(frd - .5));
        for(int y = -rd; y <= rd; y++){
            int ln = int(ceil(sqrt(frd*frd-float(y*y)) - .5));
            for(int x = -ln; x <= ln; x++){
                vec4 p = IMG_PIXEL(bufferD, ivec2(clamp(gl_FragCoord.xy + vec2(x, y), vec2(0), RENDERSIZE-1.)));
                float dof = min(abs(p.a-cam.focalDistance)/p.a, DoFClamping) * RENDERSIZE.y*cam.aperture;
                p.a = 1.;
                p *= clamp((dof - length(vec2(x, y))) + .5, 0.0, 1.0) / (dof*dof+.5);
                col += p;
           	}
        }

        col /= col.a;

        if (enableBloom)
            col.rgb += bloom(.07 * RENDERSIZE.y, .0, gl_FragCoord.xy) * .06;
#endif

        gl_FragColor = vec4(tonemapACES(col.rgb), 1);
    }
}
