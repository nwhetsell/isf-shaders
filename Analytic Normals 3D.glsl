/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "Inigo Quilez <https://iquilezles.org>",
    "DESCRIPTION": "Analytical normals of Value Noise, converted from <https://www.shadertoy.com/view/XttSz2>",
    "INPUTS": [
        {
            "NAME": "initialRotation",
            "LABEL": "Initial rotation (degrees)",
            "TYPE": "float",
            "DEFAULT": 0,
            "MIN": -180,
            "MAX": 180
        },
        {
            "NAME": "rotationSpeed",
            "LABEL": "Rotation speed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "distance",
            "LABEL": "Distance",
            "TYPE": "float",
            "DEFAULT": 3,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "mainColor",
            "LABEL": "Main color",
            "TYPE": "color",
            "DEFAULT": [1, 0.95, 0.85, 1]
        },
        {
            "NAME": "highlightColor",
            "LABEL": "Highlight color",
            "TYPE": "color",
            "DEFAULT": [0.05, 0.2, 0.3, 1]
        },
        {
            "NAME": "specularAmount",
            "LABEL": "Specular amount",
            "TYPE": "float",
            "DEFAULT": 10,
            "MIN": 0,
            "MAX": 100
        },
        {
            "NAME": "cubeSize",
            "LABEL": "Cube size",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "noiseShift",
            "LABEL": "Noise shift",
            "TYPE": "float",
            "DEFAULT": 0,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "initialFrequency",
            "LABEL": "Initial frequency",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "noiseOffset",
            "LABEL": "Noise offset",
            "TYPE": "float",
            "DEFAULT": 0.37,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "approximateNormals",
            "LABEL": "Approximate normals",
            "TYPE": "bool",
            "DEFAULT": false
        },
        {
            "NAME": "aoIntensity",
            "LABEL": "Ambient occlusion intensity",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MIN": 0,
            "MAX": 1
        }
    ],
    "ISFVSN": "2"
}*/

#define STRICT_RANDOM
#ifdef STRICT_RANDOM
int noisedRandomCount = 0;
#define NOISED_QUINTIC_INTERPOLATION 0
#else
#define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#endif
float noisedRandom(vec3 pos);
#define NOISED_RANDOM_FNC noisedRandom
#include "lygia/generative/noised.glsl"
#define FBM_OCTAVES 8
#define FBM_NOISE3_FNC(UV) vec4((n = vnoised(UV * frequency)).x, vec3(0)); value.yzw += amplitude * n.yzw * frequency; frequency *= 1.8
#define FBM_NOISE_TYPE vec4
#define FBM_SCALE_SCALAR 1.
#define FBM_AMPLITUDE_INITIAL 0.5; float frequency = initialFrequency; vec4 n
#define FBM_AMPLITUDE_SCALAR 0.5
#include "lygia/generative/fbm.glsl"
#include "lygia/geometry/aabb/aabb.glsl"
#include "lygia/geometry/aabb/intersect.glsl"
#define RAYMARCH_AO_SAMPLES 32
#define RAYMARCH_AO_MIN_DIST 0.001
#define RAYMARCH_AO_MAX_DIST 0.25
#define RAYMARCH_AO_INTENSITY aoIntensity
#define STRICT_RAYMARCH_AO
#ifndef STRICT_RAYMARCH_AO
#include "lygia/lighting/raymarch/ao.glsl"
#endif
#define RAYMARCH_SAMPLES 128
vec2 bounds;
#define RAYMARCH_MIN_DIST bounds.x
#define RAYMARCH_MAX_DIST bounds.y;
#define RAYMARCH_MIN_HIT_DIST 0.001
#define RAYMARCH_BACKGROUND
#include "lygia/lighting/raymarch/cast.glsl"
#include "lygia/lighting/raymarch/normal.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/hammersley.glsl"
#include "lygia/math/saturate.glsl"
#include "lygia/sdf/boxSDF.glsl"
#include "lygia/space/aspect.glsl"
#include "lygia/space/center.glsl"
#define LOOK_AT_RIGHT_HANDED
#include "lygia/space/lookAt.glsl"
#include "lygia/space/polar2cart.glsl"
#include "lygia/space/uncenter.glsl"


float hash(in float x) {
#ifdef STRICT_RANDOM
    return fract(sin(x) * 753.5453123);
#else
    return random(x);
#endif
}

float noisedRandom(vec3 pos) {
    float offset = 0.;
#ifdef STRICT_RANDOM
    if (noisedRandomCount == 1) {
        offset += 1.;
        pos -= vec3(1, 0, 0);
    }
    else if (noisedRandomCount == 2) {
        offset += 157.;
        pos -= vec3(0, 1, 0);
    }
    else if (noisedRandomCount == 3) {
        offset += 158.;
        pos -= vec3(1, 1, 0);
    }
    else if (noisedRandomCount == 4) {
        offset += 113.;
        pos -= vec3(0, 0, 1);
    }
    else if (noisedRandomCount == 5) {
        offset += 114.;
        pos -= vec3(1, 0, 1);
    }
    else if (noisedRandomCount == 6) {
        offset += 270.;
        pos -= vec3(0, 1, 1);
    }
    else if (noisedRandomCount == 7) {
        offset += 271.;
        pos -= vec3(1, 1, 1);
    }
    noisedRandomCount++;
#endif

    float n = pos.x + pos.y * 157. + pos.z * 113.;

    return hash(n + offset);
}

vec4 boxSDFAndNormal(vec3 p, vec3 b) {
    vec3 d = abs(p) - b;
    return vec4(
        boxSDF(p, b),
        step(d.yzx, d.xyz) * step(d.zxy, d.xyz) * sign(p)
    );
}

Material raymarchMap(in vec3 pos)
{
    vec4 noiseDist = fbm(pos + noiseShift);
    noiseDist.x = (noiseDist.x - noiseOffset) * 0.7;
    noiseDist.yzw = normalize(noiseDist.yzw);

    vec4 boxDist = boxSDFAndNormal(pos, vec3(cubeSize)); // Clip to box

    vec4 dist = (noiseDist.x > boxDist.x) ? noiseDist : boxDist;

    Material mat = materialNew();
    mat.position = pos;
    mat.sdf = dist.x;
    mat.normal = dist.yzw;
    return mat;
}

#ifdef STRICT_RAYMARCH_AO
float raymarchAO(in vec3 pos, in vec3 nor) {
    float occ = 0.;
    const float samplesFactor = 1.0 / float(RAYMARCH_AO_SAMPLES);
    for (int i = 0; i < RAYMARCH_AO_SAMPLES; i++) {
        float fi = float(i);
        vec2 u = vec2(fract(fi / PHI), (2. * fi + 1.) * samplesFactor);
        // LYGIA’s hemisphereCosSample results in:
        //    float phi = 2.0 * PI * u.x;
        //    float cosTheta2 = 1.0 - u.y;
        //    float cosTheta = sqrt(cosTheta2);
        //    float sinTheta = sqrt(1.0 - cosTheta2);
        //    vec3 ap = vec3(cos(phi) * sinTheta, sin(phi) * sinTheta, cosTheta);
        // The original Shadertoy shader effectively computes:
        //    float phi = 2.0 * PI * u.x;
        //    float cosTheta = 1.0 - u.y;
        //    float sinTheta = sqrt(1.0 - cosTheta*cosTheta);
        //    vec3 ap = vec3(cos(phi) * sinTheta, sin(phi) * sinTheta, cosTheta);
        // Note the different definitions of cosTheta. To make LYGIA’s cosTheta
        // the same as the Shadertoy cosTheta, note that:
        //    (1.0 - u.y) * (1.0 - u.y) = 1.0 - 2.*u.y + u.y*u.y
        // If we set u.y to 2.*u.y - u.y*u.y, LYGIA’s function becomes:
        //    float cosTheta2 = 1.0 - (2.*u.y - u.y*u.y);
        //                 // = 1.0 - 2.*u.y + u.y*u.y;
        //                 // = (1.0 - u.y) * (1.0 - u.y)
        //    float cosTheta = sqrt(cosTheta2);
        //                // = sqrt((1.0 - u.y) * (1.0 - u.y))
        //                // = 1.0 - u.y
        // This does result in some loss of precision, unfortunately.
        u.y = 2. * u.y - u.y*u.y;
        vec3 ap = hemisphereCosSample(u);

        float d = raymarchMap(pos + RAYMARCH_AO_MIN_DIST * nor + RAYMARCH_AO_MAX_DIST * hash(float(i)) * ap * sign(dot(ap, nor))).sdf;
        occ += saturate(d * 3.);
    }
    occ *= samplesFactor;

    return saturate(occ * 5.);
}
#endif

void main()
{
    vec2 uv = aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);

    vec3 rayOrigin;
    rayOrigin.xz = polar2cart(vec2(initialRotation * DEG2RAD + rotationSpeed * TIME, 1.));
    rayOrigin.y = 0.8;
    rayOrigin *= distance;
    vec3 rayDirection = normalize(lookAt(-rayOrigin) * vec3(uv, 1.7));

    gl_FragColor = vec4(0);

    // Bounding volume
    AABB box;
    box.min = vec3(-cubeSize);
    box.max = vec3(cubeSize);
    bounds = intersect(box, rayOrigin, rayDirection);
    if (bounds.y >= 0.) {
        Material res = raymarchCast(rayOrigin, rayDirection);
        if (res.valid && res.sdf > 0.) {
            vec3 normal = approximateNormals ? raymarchNormal(res.position) : res.normal;
            gl_FragColor = mix(highlightColor, mainColor, uncenter(normal.y));

            gl_FragColor.rgb += specularAmount *
                                pow(saturate(dot(-rayDirection, normal)), 12.) *
                                (0.04 + 0.96 * pow(saturate(1. + dot(rayDirection, normal)), 5.));

            gl_FragColor.rgb *= pow(vec3(raymarchAO(res.position, normal)), vec3(1., 1.1, 1.1));

            gl_FragColor.rgb = sqrt(gl_FragColor.rgb);
            gl_FragColor.a = 1.;
        }
    }
}
