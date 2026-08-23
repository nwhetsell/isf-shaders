/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "GPT4POWERUSER <https://www.shadertoy.com/user/GPT4POWERUSER>",
    "DESCRIPTION": "Converted from <https://www.shadertoy.com/view/DsVSRy>",
    "INPUTS": [
        {
            "NAME": "SAMPLES",
            "LABEL": "Ssamples",
            "TYPE": "float",
            "DEFAULT": 10,
            "MAX": 50,
            "MIN": 2
        },
        {
            "NAME": "FOCAL_DISTANCE",
            "LABEL": "Focal distance",
            "TYPE": "float",
            "DEFAULT": 4,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "FOCAL_RANGE",
            "LABEL": "Focal range",
            "TYPE": "float",
            "DEFAULT": 6,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "colorChangeSpeed",
            "LABEL": "Color change speed",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": 0
        }
    ],
    "ISFVSN": "2"
}*/

// #define ISF_EDITOR_WEBSITE
#ifdef ISF_EDITOR_WEBSITE
#define SAMPLES 10
#endif

#include "lygia/color/space/hsl2rgb.glsl"
#include "lygia/math/rotate2d.glsl" // The Shadertoy shader actually uses a counter-clockwise rotation.


float map(vec3 p)
{
    p.xz *= rotate2d(TIME * 0.4);
    p.xy *= rotate2d(TIME * 0.3);
    vec3 q = p * 2. + TIME;
    return length(p + vec3(sin(TIME * 0.7))) * log(length(p) + 1.) + sin(q.x + sin(q.z + sin(q.y))) * 0.5 - 1.;
}

void main()
{
    vec3 color = vec3(0);
    float depthSum = 0.;

    for (int i = 0; i < SAMPLES; i++) {
        float depth = FOCAL_DISTANCE + (float(i) / float(SAMPLES - 1)) * FOCAL_RANGE;
        float weight = 1. / (1. + abs(depth - FOCAL_DISTANCE));

        vec2 p = gl_FragCoord.xy / RENDERSIZE.y - vec2(0.9, 0.5);
        vec3 sampleColor = vec3(0);

        for (int i = 0; i <= 5; i++) {
            vec3 p = vec3(0, 0, 5) + normalize(vec3(p, -1.)) * depth;
            float rz = map(p);
            float f = clamp((rz - map(p + 0.1)) * 0.5, -0.1, 1.);

            float hue = mod(TIME * colorChangeSpeed + float(i) / 5., 1.);
            vec3 rgbColor = hsl2rgb(vec3(hue, 1, 0.5));

            vec3 l = rgbColor + vec3(5, 2.5, 3) * f;
            sampleColor = sampleColor * l + smoothstep(2.5, 0., rz) * 0.7 * l;

            depth += min(rz, 1.);
        }

        color += sampleColor * weight;
        depthSum += weight;
    }

    color /= depthSum;

    gl_FragColor = vec4(color, 1);
}
