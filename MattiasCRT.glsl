/*{
    "CATEGORIES": [
        "Filter"
    ],
    "CREDIT": "Mattias Gustavsson <https://github.com/mattiasgustavsson>",
    "DESCRIPTION": "CRT emulation, converted from <https://www.shadertoy.com/view/Ms23DR>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "horizontalScanDensity",
            "LABEL": "Horizontal scan density",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MAX": 5,
            "MIN": 0
        },
        {
            "NAME": "verticalScanSpacing",
            "LABEL": "Vertical scan spacing",
            "TYPE": "float",
            "DEFAULT": 2,
            "MAX": 5,
            "MIN": 0
        },
        {
            "NAME": "zoomAmount",
            "LABEL": "Zoom amount",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "vignetteAmount",
            "LABEL": "Vignette amount",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MAX": 1,
            "MIN": 0
        }
    ],
    "ISFVSN": "2"
}*/

#include "lygia/space/center.glsl"
#include "lygia/space/uncenter.glsl"


void main()
{
    vec2 q = gl_FragCoord.xy / RENDERSIZE;

    // Screen curvature
    vec2 uv = q;
    uv = center(uv);
    uv *= 2. - zoomAmount;
    uv.x *= 1. + pow((abs(uv.y) / 5.), 2.);
    uv.y *= 1. + pow((abs(uv.x) / 4.), 2.);
    uv = uncenter(uv);
    uv = uv * 0.92 + 0.04;

    if (uv.x < 0. || uv.x > 1. ||
        uv.y < 0. || uv.y > 1.) {
        gl_FragColor = vec4(vec3(0), 1);
    } else {
        float x = sin(      0.30 * TIME + uv.y * 21.) *
                  sin(      0.70 * TIME + uv.y * 29.) *
                  sin(0.3 + 0.33 * TIME + uv.y * 31.) *
                  0.0017;

        vec2 northeast = uv + 0.001;
        vec2 south = vec2(uv.x, uv.y - 0.002);
        vec2 west = vec2(uv.x - 0.002, uv.y);
        vec3 color = vec3(
            IMG_NORM_PIXEL(inputImage, vec2(x, 0) + northeast).r + 0.08 * IMG_NORM_PIXEL(inputImage, 0.75 * vec2(x + 0.025, -0.027) + northeast).r,
            IMG_NORM_PIXEL(inputImage, vec2(x, 0) + south    ).g + 0.05 * IMG_NORM_PIXEL(inputImage, 0.75 * vec2(x - 0.022, -0.020) + south).g,
            IMG_NORM_PIXEL(inputImage, vec2(x, 0) + west     ).b + 0.08 * IMG_NORM_PIXEL(inputImage, 0.75 * vec2(x - 0.020, -0.018) + west).b
        ) + 0.05;

        color = clamp(color * 0.6 + 0.4 * color*color, 0., 1.);

        color *= vec3(pow(16. * uv.x * uv.y * (1. - uv.x) * (1. - uv.y), vignetteAmount));

        color *= vec3(0.95, 1.05, 0.95);
        color *= 2.8;

        float scans = clamp(0.35 + 0.35 * sin(3.5 * TIME + uv.y * RENDERSIZE.y * horizontalScanDensity), 0., 1.);
        color *= vec3(0.4 + 0.7 * pow(scans, 1.7));

        color *= 1. + 0.01 * sin(110. * TIME);

        color *= 1. - 0.65 * vec3(clamp((mod(gl_FragCoord.x, verticalScanSpacing) - 1.) * 2., 0., 1.));

        gl_FragColor = vec4(color, IMG_THIS_PIXEL(inputImage).a);
    }
}
