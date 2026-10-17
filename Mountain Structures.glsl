/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "Poisson <https://www.shadertoy.com/user/Poisson>",
    "DESCRIPTION": "Path-traced Menger sponge, converted from <https://www.shadertoy.com/view/ctsSzH>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "scale",
            "LABEL": "Scale",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MAX": 50,
            "MIN": -50
        },
        {
            "NAME": "xRotation",
            "LABEL": "x-axis rotation",
            "TYPE": "float",
            "DEFAULT": 114.5915590262,
            "MAX": 360,
            "MIN": 0
        },
        {
            "NAME": "yRotation",
            "LABEL": "y-axis rotation",
            "TYPE": "float",
            "DEFAULT": 57.2957795131,
            "MAX": 360,
            "MIN": 0
        },
        {
            "NAME": "maxDistance",
            "LABEL": "Maximum distance",
            "TYPE": "float",
            "DEFAULT": 12,
            "MAX": 50,
            "MIN": 12
        },
        {
            "NAME": "feedback",
            "LABEL": "Feedback amount",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "cameraX",
            "LABEL": "Camera x-coordinate",
            "TYPE": "float",
            "DEFAULT": -1,
            "MAX": 5,
            "MIN": -5
        },
        {
            "NAME": "cameraY",
            "LABEL": "Camera y-coordinate",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MAX": 5,
            "MIN": -5
        },
        {
            "NAME": "cameraZ",
            "LABEL": "Camera z-coordinate",
            "TYPE": "float",
            "DEFAULT": 1.6,
            "MAX": 5,
            "MIN": -5
        },
        {
            "NAME": "fogDensity",
            "LABEL": "Fog density",
            "TYPE": "float",
            "DEFAULT": 0.001,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "mainColor",
            "LABEL": "Main color",
            "TYPE": "color",
            "DEFAULT": [0.72, 0.81, 0.9, 1]
        },
        {
            "NAME": "backgroundColor",
            "LABEL": "Background color",
            "TYPE": "color",
            "DEFAULT": [0.6, 0.8, 1, 1]
        },
        {
            "NAME": "lightRadius",
            "LABEL": "Light distance",
            "TYPE": "float",
            "DEFAULT": 2.4494897428,
            "MIN": 0,
            "MAX": 200
        },
        {
            "NAME": "lightPhi",
            "LABEL": "Light phi (degrees)",
            "TYPE": "float",
            "DEFAULT": 65.9051574479,
            "MIN": 0,
            "MAX": 180
        },
        {
            "NAME": "lightTheta",
            "LABEL": "Light theta (degrees)",
            "TYPE": "float",
            "DEFAULT": 26.5650511771,
            "MIN": 0,
            "MAX": 360
        },
        {
            "NAME": "lightDirectionRandomization",
            "LABEL": "Light direction randomization",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "darkenAmount",
            "LABEL": "Darken amount",
            "TYPE": "float",
            "DEFAULT": 1.1,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "enableBloom",
            "LABEL": "Enable bloom",
            "TYPE": "bool",
            "DEFAULT": false
        },
        {
            "NAME": "bloomSpread",
            "LABEL": "Bloom spread",
            "TYPE": "float",
            "DEFAULT": 4,
            "MAX": 100,
            "MIN": 0
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "lastData",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {

        }
    ]
}*/

// The default light parameters are:
//   lightRadius = length(vec3(2, 1, 1))
//               = sqrt(2 * 2 + 1 * 1 + 1 * 1)
//               = sqrt(6) ≈ 2.4494897428
//   lightPhi = acos(1 / lightRadius) ≈ 65.9051574479°
//   lightTheta = atan(1 / 2) ≈ 26.5650511771°

// #define ISF_EDITOR_WEBSITE

#include "lygia/color/blend/darken.glsl"
#define GAMMA 0.4545
#include "lygia/color/space/gamma2linear.glsl"
#include "lygia/color/tonemap/aces.glsl"
#define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#include "lygia/generative/random.glsl"
#define RAYMARCH_BACKGROUND vec3(0)
#define RAYMARCH_SAMPLES 256
#define RAYMARCH_MIN_DIST 0.
#define RAYMARCH_MAX_DIST maxDistance
#define RAYMARCH_MIN_HIT_DIST 0.001
#include "lygia/lighting/raymarch/cast.glsl"
#define FOG_DENSITY fogDensity
#define FOG_COLOR_COOL backgroundColor.rgb
#include "lygia/lighting/raymarch/fog.glsl"
#include "lygia/lighting/raymarch/normal.glsl"
#define RAYMARCH_SHADOW_MIN_DIST 0.
#define RAYMARCH_SHADOW_MAX_DIST 4.
#define RAYMARCH_SHADOW_SOLID_ANGLE 0.
#include "lygia/lighting/raymarch/softShadow.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/cubic.glsl"
#include "lygia/math/pow3.glsl"
#include "lygia/math/rotate3dX.glsl"
#include "lygia/math/rotate3dY.glsl"
#include "lygia/math/saturate.glsl"
#include "lygia/space/aspect.glsl"
#include "lygia/space/center.glsl"
#define LOOK_AT_RIGHT_HANDED
#include "lygia/space/lookAt.glsl"
#include "lygia/space/polar2cart.glsl"
#define BLOOM_QUALITY 8
#include "lygia-additions/bloom.glsl" // This isn’t exactly the same as the bloom function in the Shadertoy shader.
#define MENGERSPONGE_TRANSFORM(fi, st) rotate3dY(-fi * yRotation * DEG2RAD) * rotate3dX(fi * xRotation * DEG2RAD) * st;
#include "lygia-additions/mengerSpongeSDF.glsl"


float seed;

// #define STRICT_RANDOM

vec2 random2(void)
{
#ifdef STRICT_RANDOM
    return fract(sin(seed += 0.1) * vec2(8472.5636, 9854.4213));
#else
    return random2(seed += 0.1);
#endif
}

vec3 random3(void)
{
#ifdef STRICT_RANDOM
    return fract(sin(seed += 0.1) * vec3(7653.1285, 6912.8512, 5914.7134));
#else
    return random3(seed += 0.1);
#endif
}

vec3 randomUnitVector(void)
{
    return normalize(center(random3()));
}

Material raymarchMap(vec3 pos)
{
    Material mat = materialNew();
    mat.position = pos;
    mat.sdf = min(mengerSpongeSDF(pos, scale, pos.y), RAYMARCH_MAX_DIST);
    return mat;
}

vec3 render(vec3 rayOrigin, vec3 rayDirection)
{
    vec3 lightDirection = normalize(polar2cart(lightRadius, lightPhi * DEG2RAD, lightTheta * DEG2RAD) + lightDirectionRandomization * randomUnitVector());

    Material res = raymarchCast(rayOrigin, rayDirection);
    float dist = res.sdf;

    vec3 color;

    if (res.valid) {
        color = vec3(0);
        vec3 emit = vec3(1); // color mask

        vec3 ro = rayOrigin;
        vec3 rd = rayDirection;
        for (int i = 0; i < 5; i++) { // 5 bounces of global illumination
            float t = res.sdf;
            vec3 p = ro + rd * t; // hit point

// #define STRICT_NORMAL
#ifdef STRICT_NORMAL
            float dist = raymarchMap(p).sdf;
            vec2 e = vec2(dist, 0);
            vec3 n = normalize(dist - vec3(raymarchMap(p - e.xyy).sdf,
                                           raymarchMap(p - e.yxy).sdf,
                                           raymarchMap(p - e.yyx).sdf));
#else
            vec3 n = raymarchNormal(p);
#endif

            ro = p + n * 0.002; // new ray origin
            rd = normalize(n + randomUnitVector()); // new ray direction, cheap diffuse BRDF

            emit *= mainColor.rgb; // color the object

            float dif = saturate(dot(n, lightDirection)); // diffuse light
            dif *= raymarchSoftShadow(ro, lightDirection);

            color += emit * dif;

            res = raymarchCast(ro, rd);
            if (!res.valid)
                break;
        }
    } else {
        color = backgroundColor.rgb;
    }

// #define STRICT_FOG
#ifdef STRICT_FOG
    color = raymarchFog(color, pow3(dist));
    color += 3. * pow(saturate(dot(rayDirection, lightDirection)), 3.5); // sun glare
#else
    color = raymarchColorFog(color, pow3(dist), rayDirection, lightDirection);
#endif

    return color;
}

void main()
{
    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        seed = TIME + dot(sin(gl_FragCoord.xy), vec2(453.712, 983.234));

        vec2 antiAliasingOffset = random2() - 0.5;
        vec2 pos = 0.5 * aspect(center((gl_FragCoord.xy + antiAliasingOffset) / RENDERSIZE), RENDERSIZE);

        vec3 rayOrigin = vec3(cameraX, cameraY, cameraZ);
        mat3 lookAtMatrix = lookAt(rayOrigin, vec3(0), vec3(0, 1, 0));

        vec3 rayDirection = lookAtMatrix * normalize(vec3(pos, 1.5));

        // Depth of field
        float focalDistance = raymarchCast(rayOrigin, lookAtMatrix[2]).sdf;
        vec3 focalPlane = rayOrigin + rayDirection * focalDistance;
        rayOrigin += randomUnitVector() * 0.05;
        rayDirection = normalize(focalPlane - rayOrigin);

        gl_FragColor = vec4(render(rayOrigin, rayDirection), 1);

        if (FRAMEINDEX > 0)
            gl_FragColor += IMG_THIS_PIXEL(lastData) * feedback;
    }
    else // Shadertoy Image
    {
        vec4 data = IMG_THIS_PIXEL(lastData);
        vec3 color = data.rgb / data.a;

        color = gamma2linear(color);
        color = blendDarken(vec3(1), color, darkenAmount);
        if (enableBloom)
            color += 0.4 * bloom(lastData, bloomSpread, 4.);
        color = cubic(saturate(color));
        color = tonemapACES(color);

        gl_FragColor = vec4(color, 1);
    }
}
