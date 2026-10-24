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
            "NAME": "sdfScale",
            "LABEL": "Scale",
            "TYPE": "float",
            "DEFAULT": 2.4844999942,
            "MIN": 0,
            "MAX": 200
        },
        {
            "NAME": "sdfRadius",
            "LABEL": "Radius",
            "TYPE": "float",
            "DEFAULT": 0.0600780218,
            "MIN": 0,
            "MAX": 200
        },
        {
            "NAME": "sdfRotationFactor",
            "LABEL": "Rotation factor",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": -5,
            "MAX": 5
        },
        {
            "NAME": "spacing",
            "LABEL": "Spacing",
            "TYPE": "float",
            "DEFAULT": 2,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "repetition",
            "LABEL": "Repetitions",
            "TYPE": "float",
            "DEFAULT": 1,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "colorMix",
            "LABEL": "Color mix",
            "TYPE": "float",
            "DEFAULT": 0,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "colorBandSize",
            "LABEL": "Color band size",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0,
            "MAX": 1
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

// The defaults are:
//    sdfRadius = 0.25 + cos(15.) * 0.25
//    sdfScale = 2.25 + sin(15. * 0.5) * 0.25

// #define ISF_EDITOR_WEBSITE

#include "lygia/color/tonemap/aces.glsl"
#include "lygia/generative/random.glsl"
#include "lygia/lighting/common/henyeyGreenstein.glsl"
#define RAYMARCH_MIN_HIT_DIST 0.015
#include "lygia/lighting/raymarch/cast.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/hammersley.glsl"
#ifdef ISF_EDITOR_WEBSITE
#include "lygia/math/inverse.glsl"
#endif
#include "lygia/math/lengthSq.glsl"
#include "lygia/math/rotate3dX.glsl"
#include "lygia/math/rotate3dY.glsl"
#include "lygia/math/rotate3dZ.glsl"
#include "lygia/math/sum.glsl"
#include "lygia/sdf/sphereSDF.glsl"
#include "lygia/space/aspect.glsl"
#include "lygia/space/center.glsl"
#include "lygia/space/tbn.glsl"
#include "lygia/space/uncenter.glsl"

// #define STRICT_NORMAL
#ifndef STRICT_NORMAL
#include "lygia/lighting/raymarch/normal.glsl"
#else
vec3 raymarchNormal(vec3 pos) {
    vec2 offset = vec2(RAYMARCH_MIN_HIT_DIST, 0);
    return normalize(vec3(
        raymarchMap(pos + offset.xyy).sdf - raymarchMap(pos - offset.xyy).sdf,
        raymarchMap(pos + offset.yxy).sdf - raymarchMap(pos - offset.yxy).sdf,
        raymarchMap(pos + offset.yyx).sdf - raymarchMap(pos - offset.yyx).sdf
    ));
}
#endif

// #define STRICT_RANDOM
#ifndef STRICT_RANDOM
#include "rand/rand.glsl"
#else
float seed;
float frand(void) {
    return fract(sin(seed++) * 43758.54536156);
}
#endif


#define SDF_ITERATIONS 7

//increase this number for a better GI
#define IndirectSamples 1.

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

#define RAYMARCH_SOFTSHADOW_ITERATIONS MaxShadowSteps
#define RAYMARCH_SHADOW_MIN_DIST 0.
#define RAYMARCH_SHADOW_MAX_DIST MaxDist

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


Material raymarchMap(in vec3 pos) {
    mat3 rotation = rotationMatrix(vec3(15., 10.5, 6.) * sdfRotationFactor);

    vec4 initialPosition = vec4(pos.xyz * rotation, 1);
    vec4 p = initialPosition;
    for (int i = 0; i < SDF_ITERATIONS; i++) {
        // This performs limited domain repetition of an SDF (that is, a finite
        // number repetitions, not infinite repetitions), but it’s missing a
        // `round` call:
        //    - https://iquilezles.org/articles/sdfrepetition/#:~:text=repeated_ONLY_SYMMETRIC_SDFS
        //    - https://www.shadertoy.com/view/3syGzz
        // It’s not clear what effect this has.
        p.xyz = p.xyz - spacing * clamp(p.xyz, -repetition, repetition);

        p.xyz = -p.xyz;
       	p /= max(min(lengthSq(p.xyz), repetition), sdfRadius);
        p *= sdfScale;

        if (i < 3)
            p.xyz *= rotation;

        p += initialPosition;
    }
    p.xyz /= p.w;

    Material mat = materialNew();
    mat.albedo = vec4(1);
    mat.emissive = vec3(0);
    if (fract(initialPosition.x) < colorBandSize) {
        mat.albedo.rgb = mix(mat.albedo.rgb, vec3(0.2), colorMix);
        mat.emissive = mix(mat.emissive, uncenter(normalize(p.xyz)) * 10., colorMix);
    }
    mat.sdf = sphereSDF(
        p.xyz,
        abs(sdfScale - 1.) / p.w + pow(sdfScale, float(1 - SDF_ITERATIONS))
    );
    return mat;
}

float henyeyGreenstein(vec3 dirI, vec3 dirO) {
	return PI*PI * henyeyGreenstein(dot(dirI, dirO), Anisotropy);
}

float directLightMinDist(in vec3 ro, in vec3 rd) {
    float t = RAYMARCH_MAP_FNC(ro).sdf;
    return t + RAYMARCH_MAP_FNC(ro + rd * t).sdf;
}

DEFINE_RAYMARCH_CAST(Direct, directLightMinDist(ro, rd), 128)

float raymarchShadow(vec3 ro, vec3 rd) {
    const float mint = RAYMARCH_SHADOW_MIN_DIST;
    const float maxt = RAYMARCH_SHADOW_MAX_DIST;

    float res = 1.0;
    float t = mint;
    for (int i = 0; i < RAYMARCH_SOFTSHADOW_ITERATIONS; i++) {
        float h = RAYMARCH_MAP_FNC(ro + t * rd).sdf;
        if (h < RAYMARCH_MIN_HIT_DIST) {
            res = 0.;
            break;
        }
        if (t > maxt)
            break;
        res = min(res, asin(h / length(rd * t)));

        t -= h * 2.5; // Go 2.5 times faster since we don’t need details.
    }
    return res;
}

vec3 directLight(vec3 pos, vec3 normal) {
    float light = -dot(normal, LightDir);
    if (light < 0.)
        return vec3(0);
    float shadow = raymarchShadow(pos, LightDir);
    return LightColor * light * clamp(min(shadow, LightRadius) / LightRadius, 0., 1.);
}

DEFINE_RAYMARCH_CAST(Indirect, 0., MaxStepsIndirect)

vec3 sampleIndirectLight(vec3 pos) {
    vec3 normal = raymarchNormal(pos);
    vec3 light = vec3(0);
    vec3 absorption = vec3(1);

    for (int i = 0; i < Bounces; i++) {
        vec3 up = abs(normal.x) > abs(normal.z) ? vec3(0, 0, 1) : vec3(1, 0, 0);
        vec3 dir = tbn(normal, up) * hemisphereCosSample(vec2(frand(), 1. - frand()));
        Material res = raymarchCastIndirect(pos, dir);
        if (!res.valid) {
            light += absorption * LightColor * Density * 8. * henyeyGreenstein(-LightDir, dir);
            break;
        }

        pos += dir * res.sdf;
        normal = raymarchNormal(pos);
        pos -= (2. * RAYMARCH_MIN_HIT_DIST - raymarchMap(pos).sdf) * dir;

        light += absorption * (res.emissive + res.albedo.rgb * directLight(pos, normal));
        absorption *= res.albedo.rgb;
    }

    return light;
}

vec2 unaspect(vec2 st, vec2 s) {
    st.x = st.x * (s.y / s.x);
    return st;
}

vec4 previousSample(vec4 data) {
    vec2 previous_uv = pos2uv(getCam(TIME - TIMEDELTA), data.xyz);
    vec2 previousFragCoord = uncenter(2. * unaspect(previous_uv, RENDERSIZE)) * RENDERSIZE;

    vec2 minFragCoord;
    float minDist = MaxDist;
    for (int y = -1; y <= 1; y++)
    for (int x = -1; x <= 1; x++) {
        vec2 fragCoord = previousFragCoord + PixelCheckDistance * vec2(x, y);
        // LYGIA’s `inside` function (lygia/math/inside.glsl) tests whether a
        // point is completely inside a range. This is testing wether a point
        // is *not* completely *outside* a range (so the point can be partially
        // inside the range).
        if (!(fragCoord.x < 0. && fragCoord.x > RENDERSIZE.x &&
              fragCoord.y < 0. && fragCoord.y > RENDERSIZE.y)) {
            vec4 prevData = IMG_NORM_PIXEL(bufferA, fragCoord / RENDERSIZE);
            float dist = length(prevData - data);
            if (dist < minDist) {
                minFragCoord = fragCoord;
                minDist = dist;
            }
        }
   	}

    Camera cam = getCam(TIME);
    if (minDist < PixelAcceptance * length(data.xyz - cam.pos) / cam.focalLength / RENDERSIZE.y)
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
        bloom += gaussianFilter[abs(x)][abs(y)] * textureLod(bufferD, (fragCoord + vec2(x, y) * scale)/RENDERSIZE, logScale).rgb;

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

        Material res = raymarchCastDirect(rayOrigin, rayDirection);
        if (res.valid) {
            for (int i = 0; i < 3; i++) {
                res.sdf -= 2. * RAYMARCH_MIN_HIT_DIST - raymarchMap(rayOrigin + rayDirection * res.sdf).sdf;
            }
            gl_FragColor = vec4(rayOrigin + rayDirection * res.sdf, 1);
        } else {
            gl_FragColor = vec4(0);
        }
    }
    else if (PASSINDEX == 2) // Shadertoy Buffer C
    {
#ifndef STRICT_RANDOM
        seed = int(sum(gl_FragCoord.xy) + FRAMEINDEX);
#else
        seed = sum(gl_FragCoord.xy * vec2(0.256435865, 0.316548465)) + sin(TIME) * 16886.3158915;
#endif

        vec4 data = IMG_THIS_PIXEL(bufferB);
        bool valid = data.w > 0.;
        if (valid) {
            vec3 indirectLight = vec3(0);
            for (float i = 0; i < IndirectSamples; i++) {
                indirectLight += sampleIndirectLight(data.xyz);
            }
            indirectLight / IndirectSamples;

            gl_FragColor = previousSample(data);
            if (gl_FragColor.a <= float(SamplesLimit))
                gl_FragColor.a += IndirectSamples;
            indirectLight = clamp(indirectLight, 0., gl_FragColor.a * 0.1); // Clamp global illumination for low sample count.
            gl_FragColor.rgb = mix(gl_FragColor.rgb, indirectLight, 1. / (gl_FragColor.a / IndirectSamples));
        } else {
            vec2 uv = 0.5 * aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);
            gl_FragColor = vec4(LightColor * Density, 1);
        }
    }
    else if (PASSINDEX == 3) // Shadertoy Buffer D
    {
        Camera cam = getCam(TIME);

        vec4 col = IMG_THIS_PIXEL(bufferC);
        vec4 tx = IMG_THIS_PIXEL(bufferB);
        vec3 finalPos = tx.rgb;
        col.a = length(cam.pos - finalPos);

        vec2 uv = 0.5 * aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);
        vec3 dir = uv2dir(cam, uv);

        if (tx.a != 0.) {
            Material res = raymarchMap(finalPos);
            col.rgb = res.emissive + res.albedo.rgb * (col.rgb + directLight(finalPos, raymarchNormal(finalPos)));
        } else {
            finalPos = cam.pos + dir * 100.;
        }

        vec3 volCol = vec3(0);
        vec3 volAbs = vec3(1);
        vec3 pos = cam.pos;
        float stepDist = (tx.a == 0.) ? FogRange : min(FogRange, col.a);
        stepDist /= float(FogSteps);
        vec3 stepAbs = exp(-Density * stepDist);
        vec3 stepCol = (vec3(1) - stepAbs) * henyeyGreenstein(-LightDir, dir);
        pos += stepDist * dir * random(gl_FragCoord.xy / vec2(1024)) /*texture(iChannel2, gl_FragCoord.xy/vec2(1024)).a*/;
        for (int i = 0; i < FogSteps; i++) {
            volAbs *= stepAbs;
            volCol += stepCol * volAbs * directLight(pos, -LightDir);
            pos += dir * stepDist;
        }

        col.rgb = col.rgb * volAbs + volCol;

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
