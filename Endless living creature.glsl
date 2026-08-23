/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "Leon Denise <https://www.shadertoy.com/user/leon>",
    "DESCRIPTION": "Weird endless living creature, converted from <https://www.shadertoy.com/view/tljXWy>",
    "INPUTS": [
        {
            "NAME": "sphereCount",
            "LABEL": "Sphere count",
            "TYPE": "float",
            "DEFAULT": 15,
            "MAX": 100,
            "MIN": 1
        },
        {
            "NAME": "speed",
            "LABEL": "Speed",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "scrollSpeed",
            "LABEL": "Scroll speed",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 2,
            "MIN": 0
        },
        {
            "NAME": "balance",
            "LABEL": "Balance",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "range",
            "LABEL": "Range",
            "TYPE": "float",
            "DEFAULT": 1.4,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "radius",
            "LABEL": "Radius",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "blend",
            "LABEL": "Blend",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "falloff",
            "LABEL": "Fall-off",
            "TYPE": "float",
            "DEFAULT": 1.2,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "enableMouse",
            "LABEL": "Enable mouse",
            "TYPE": "bool",
            "DEFAULT": false
        },
        {
            "NAME": "mouse",
            "TYPE": "point2D",
            "DEFAULT": [0, 0],
            "MIN": [-1, -1],
            "MAX": [1, 1]
        },
        {
            "NAME": "motion_frames",
            "LABEL": "Motion frames",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": 1
        }
    ],
    "ISFVSN": "2"
}*/

// #define ISF_EDITOR_WEBSITE
#ifdef ISF_EDITOR_WEBSITE
#define sphereCount 15.
#define motion_frames 1.
#endif

#include "lygia/math/const.glsl"
#include "lygia/math/rotate2d.glsl"
#include "lygia/sdf/sphereSDF.glsl"
#include "lygia/space/lookAt.glsl"


// Weird endless living creature
// inspired by Inigo Quilez live stream shader deconstruction
// Leon Denise (ponk) 2019.08.28
// Licensed under hippie love conspiracy

// Using code from
// Inigo Quilez
// Morgan McGuire

// toolbox
#define repeat(p,r) (mod(p,r)-r/2.)
float random(vec2 p) { return fract(1e4 * sin(17.0 * p.x + p.y * 0.1) * (0.1 + abs(sin(p.y * 13.0 + p.x)))); }

mat2 rotate2dCounterclockwise(const in float r) {
    return rotate2d(-r);
}

float smoothmin (float a, float b, float r) { float h = clamp(.5+.5*(b-a)/r, 0., 1.); return mix(b, a, h)-r*h*(1.-h); }

float geometry (vec3 pos, float time) {
    float scene = 1., a = 1.;
    float t = time * .5 + pos.x / 30.;
    t = floor(t)+smoothstep(0.0,.9,pow(fract(t),2.));
    pos.x = repeat(pos.x+TIME*scrollSpeed, 5.);
    for (int i = int(sphereCount); i > 0; --i) {
        pos.x = abs(pos.x)-range*a;
        pos.xy *= rotate2dCounterclockwise(cos(t)*balance/a+a*2.);
        pos.zy *= rotate2dCounterclockwise(sin(t)*balance/a+a*2.);
        scene = smoothmin(scene, sphereSDF(pos,(radius*a)), blend*a);
        a /= falloff;
    }
    return scene;
}

float raymarch ( vec3 eye, vec3 ray, float time, out float total ) {
    float dither = random(ray.xy+fract(time));
    total = 0.0;
    const int count = 20;
    for (int index = count; index > 0; --index) {
        float dist = geometry(eye+total*ray,time);
        dist *= 0.9+0.1*dither;
        total += dist;
        if (dist < 0.001 * total) {
            return float(index)/float(count);
        }
    }
    return 0.;
}

vec3 camera (vec3 eye) {
    if (enableMouse) {
        eye.yz *= rotate2dCounterclockwise(mouse.y*PI);
        eye.xz *= rotate2dCounterclockwise(mouse.x*PI);
    }
    return eye;
}

void main()
{
    vec2 uv = 2.*(gl_FragCoord.xy-0.5*RENDERSIZE)/RENDERSIZE.y;
    vec3 eye = camera(vec3(0,0,4));
    mat3 lookMatrix = lookAt(eye, vec3(0), vec3(0, 1, 0));
    vec3 ray = normalize(lookMatrix[2] + lookMatrix[0] * uv.x + lookMatrix[1] * uv.y);

    float total = 0.0;
    gl_FragColor = vec4(0);
    for (float index = motion_frames; index > 0.; --index) {
        float dither = random(ray.xy+fract(TIME+index));
        float time = TIME*speed+(dither+index)/10./motion_frames;
        gl_FragColor += vec4(raymarch(eye, ray, time, total))/motion_frames;
    }

    // extra color
    gl_FragColor.rgb *= vec3(.7,.8,.9);
    float d = smoothstep(7.,0.,total);
    gl_FragColor.rgb += vec3(0.8,.6,.5) * d;

    if (any(greaterThan(gl_FragColor.rgb, vec3(0)))) {
        gl_FragColor.a = 1.;
    }
}
