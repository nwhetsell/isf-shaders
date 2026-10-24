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
            "NAME": "anisotropy",
            "LABEL": "Anisotropy",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "lightColor",
            "LABEL": "Light color",
            "TYPE": "color",
            "DEFAULT": [1, 1, 1, 1]
        },
        {
            "NAME": "lightGain",
            "LABEL": "Light gain",
            "TYPE": "float",
            "DEFAULT": 6,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "lightRadius",
            "LABEL": "Light radius",
            "TYPE": "float",
            "DEFAULT": 0.02,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "lightDistance",
            "LABEL": "Light distance",
            "TYPE": "float",
            "DEFAULT": 1,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "lightPhi",
            "LABEL": "Light phi (degrees)",
            "TYPE": "float",
            "DEFAULT": 180,
            "MIN": 0,
            "MAX": 180
        },
        {
            "NAME": "lightTheta",
            "LABEL": "Light theta (degrees)",
            "TYPE": "float",
            "DEFAULT": 0,
            "MIN": 0,
            "MAX": 360
        },
        {
            "NAME": "depthOfFieldMinimum",
            "LABEL": "Depth of field minimum",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "previousPixelDistance",
            "LABEL": "Previous pixel distance",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "previousPixelAcceptance",
            "LABEL": "Previous pixel acceptance",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MIN": 0,
            "MAX": 10
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

// The default SDF parameters are:
//    sdfRadius = 0.25 + cos(15.) * 0.25
//    sdfScale = 2.25 + sin(15. * 0.5) * 0.25
// The default light parameters are:
//   lightDistance = length(vec3(0, 0, -1))
//                 = 1
//   lightPhi = acos(-1 / lightDistance) = 180°
//   lightTheta = atan(0 / 0) … arbitrarily choose 0°

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
#include "lygia/space/polar2cart.glsl"
#include "lygia/space/tbn.glsl"
#include "lygia/space/uncenter.glsl"

// #define STRICT_NORMAL
#ifndef STRICT_NORMAL
#include "lygia/lighting/raymarch/normal.glsl"
#else
vec3 raymarchNormal(vec3 pos) {
    // https://iquilezles.org/articles/normalsSDF/
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

#define GLOBAL_ILLUMINATION_MAX_BOUNCES 1

// Increase for better global illumination
#define GLOBAL_ILLUMINATION_SAMPLES 1.
// Increase to blur noise
#define GLOBAL_ILLUMINATION_SAMPLES_MAX 150.

#define RAYMARCH_SOFTSHADOW_ITERATIONS 32
#define RAYMARCH_SHADOW_MIN_DIST 0.
#define RAYMARCH_SHADOW_MAX_DIST RAYMARCH_MAX_DIST

#define FOG_STEP_COUNT 8

#define Density vec3(.0025, .0045, .006)
#define FogRange 13.


struct FocusedCamera {
    vec3 pos;
    vec3 rot;

    mat3 rotationMatrix;

    float focalLength;
    float focalDistance;
    float aperture;
};

void cameraSetRotation(inout FocusedCamera camera, vec3 rot) {
    camera.rot = rot;
    camera.rotationMatrix = rotate3dZ(rot.z) * rotate3dX(rot.x) * rotate3dY(rot.y);
}

FocusedCamera cameraAtTime(float time) {
    FocusedCamera camera;
    cameraSetRotation(camera, vec3(-cos(time * 0.4) / 6., 1. + time * 0.2 + sin(time * 0.2) / 4., -0.5));
    camera.pos = vec3(0, 0, -10) * camera.rotationMatrix;
    camera.focalLength = 1.;
    camera.focalDistance = 7.5;
    camera.aperture = 0.025;
    return camera;
}

vec3 camera_uv2dir(FocusedCamera camera, vec2 uv) {
    return normalize(vec3(uv, camera.focalLength)) * camera.rotationMatrix;
}

vec2 camera_pos2uv(FocusedCamera camera, vec3 pos) {
    vec3 dir = normalize(pos - camera.pos) * inverse(camera.rotationMatrix);
    return dir.xy * camera.focalLength / dir.z;
}


Material raymarchMap(in vec3 pos) {
    vec3 rotationAngles = vec3(-15., 10.5, -6.) * sdfRotationFactor;
    mat3 rotationMatrix = rotate3dZ(rotationAngles.z) * rotate3dX(rotationAngles.x) * rotate3dY(rotationAngles.y);

    vec4 initialPosition = vec4(pos.xyz * rotationMatrix, 1);
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
            p.xyz *= rotationMatrix;

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

vec3 lightDirection;
vec3 directLight(vec3 pos, vec3 normal) {
    float light = -dot(normal, lightDirection);
    if (light < 0.)
        return vec3(0);
    float shadow = raymarchShadow(pos, lightDirection);
    return lightColor.rgb * lightGain * light * clamp(min(shadow, lightRadius) / lightRadius, 0., 1.);
}

DEFINE_RAYMARCH_CAST(Indirect, 0., 32)

vec3 sampleIndirectLight(vec3 pos) {
    vec3 normal = raymarchNormal(pos);
    vec3 light = vec3(0);
    vec3 absorption = vec3(1);

    for (int i = 0; i < GLOBAL_ILLUMINATION_MAX_BOUNCES; i++) {
        vec3 up = (abs(normal.x) > abs(normal.z)) ? vec3(0, 0, 1) : vec3(1, 0, 0);
        vec3 dir = tbn(normal, up) * hemisphereCosSample(vec2(frand(), 1. - frand()));
        Material res = raymarchCastIndirect(pos, dir);
        if (!res.valid) {
            light += absorption * lightColor.rgb * lightGain * Density * 8. * PI*PI * henyeyGreenstein(dot(dir, -lightDirection), anisotropy);
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
    vec2 previous_uv = camera_pos2uv(cameraAtTime(TIME - TIMEDELTA), data.xyz);
    vec2 previousFragCoord = uncenter(2. * unaspect(previous_uv, RENDERSIZE)) * RENDERSIZE;

    vec2 minFragCoord;
    float minDist = RAYMARCH_MAX_DIST;
    for (int y = -1; y <= 1; y++)
    for (int x = -1; x <= 1; x++) {
        vec2 fragCoord = previousFragCoord + previousPixelDistance * vec2(x, y);
        // LYGIA’s `inside` function (lygia/math/inside.glsl) tests whether a
        // point is completely inside a range. This is testing whether a point
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

    FocusedCamera camera = cameraAtTime(TIME);
    if (minDist < previousPixelAcceptance / RENDERSIZE.y * length(data.xyz - camera.pos) / camera.focalLength)
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
    lightDirection = normalize(polar2cart(lightDistance, lightPhi * DEG2RAD, lightTheta * DEG2RAD));

    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        gl_FragColor = IMG_THIS_PIXEL(bufferB);
    }
    else if (PASSINDEX == 1) // Shadertoy Buffer B
    {
        vec2 uv = 0.5 * aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);

        FocusedCamera camera = cameraAtTime(TIME);
        vec3 rayOrigin = camera.pos;
        vec3 rayDirection = camera_uv2dir(camera, uv);

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
            for (float i = 0; i < GLOBAL_ILLUMINATION_SAMPLES; i++) {
                indirectLight += sampleIndirectLight(data.xyz);
            }
            indirectLight /= GLOBAL_ILLUMINATION_SAMPLES;

            gl_FragColor = previousSample(data);

            if (gl_FragColor.a <= GLOBAL_ILLUMINATION_SAMPLES_MAX)
                gl_FragColor.a += GLOBAL_ILLUMINATION_SAMPLES;

            // Clamp global illumination for low sample count.
            indirectLight = clamp(indirectLight, 0., gl_FragColor.a * 0.1);

            gl_FragColor.rgb = mix(gl_FragColor.rgb, indirectLight, GLOBAL_ILLUMINATION_SAMPLES / gl_FragColor.a);
        } else {
            gl_FragColor = vec4(lightColor.rgb * lightGain * Density, 1);
        }
    }
    else if (PASSINDEX == 3) // Shadertoy Buffer D
    {
        FocusedCamera camera = cameraAtTime(TIME);

        vec4 data = IMG_THIS_PIXEL(bufferB);
        vec4 color = IMG_THIS_PIXEL(bufferC);

        color.a = length(camera.pos - data.xyz);

        float fogStepDistance;
        bool valid = data.w > 0.;
        if (valid) {
            Material res = raymarchMap(data.xyz);
            color.rgb = res.emissive + res.albedo.rgb * (color.rgb + directLight(data.xyz, raymarchNormal(data.xyz)));

            fogStepDistance = min(FogRange, color.a);
        } else {
            fogStepDistance = FogRange;
        }
        fogStepDistance /= float(FOG_STEP_COUNT);

        vec2 uv = 0.5 * aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);
        vec3 dir = camera_uv2dir(camera, uv);

        vec3 fogStepAbsorption = exp(-Density * fogStepDistance);
        vec3 fogStepColor = (1. - fogStepAbsorption) * PI*PI * henyeyGreenstein(dot(dir, -lightDirection), anisotropy);

        vec3 volumeAbsorption = vec3(1);
        vec3 volumeColor = vec3(0);
        float t = fogStepDistance * random(gl_FragCoord.xy /  1024.);
        for (int i = 0; i < FOG_STEP_COUNT; i++) {
            volumeAbsorption *= fogStepAbsorption;
            volumeColor += fogStepColor * volumeAbsorption * directLight(camera.pos + dir * t, -lightDirection);
            t += fogStepDistance;
        }

        color.rgb = color.rgb * volumeAbsorption + volumeColor;

        gl_FragColor = color;
    }
    else // Shadertoy Image
    {
        vec4 color = vec4(0);

#ifndef ISF_EDITOR_WEBSITE
        FocusedCamera camera = cameraAtTime(TIME);
        float scaledAperture = RENDERSIZE.y * camera.aperture;
        float rd = scaledAperture * depthOfFieldMinimum;
        int ird = int(ceil(rd - 0.5));
        for (int y = -ird; y <= ird; y++) {
            int ln = int(ceil(sqrt(rd*rd - float(y*y)) - 0.5));
            for (int x = -ln; x <= ln; x++) {
                vec4 p = IMG_PIXEL(bufferD, ivec2(clamp(gl_FragCoord.xy + vec2(x, y), vec2(0), RENDERSIZE - 1.)));
                float dof = min(abs(p.a - camera.focalDistance) / p.a, depthOfFieldMinimum) * scaledAperture;
                p.a = 1.;
                p *= clamp((dof - length(vec2(x, y))) + 0.5, 0., 1.) / (dof*dof + 0.5);
                color += p;
           	}
        }
        color /= color.a;

        if (enableBloom)
            color.rgb += bloom(0.07 * RENDERSIZE.y, 0., gl_FragCoord.xy) * 0.06;
#endif

        gl_FragColor = vec4(tonemapACES(color.rgb), 1);
    }
}
