/*{
    "CATEGORIES": [
        "Filter"
    ],
    "CREDIT": "Fabrice Neyret <https://www.shadertoy.com/user/FabriceNeyret2>",
    "DESCRIPTION": "Circle dithering, converted from <https://www.shadertoy.com/view/MdSfWK>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "searchDistance",
            "LABEL": "Search distance",
            "TYPE": "float",
            "DEFAULT": 8,
            "MAX": 30,
            "MIN": 0
        },
        {
            "NAME": "gridStep",
            "LABEL": "Grid step",
            "TYPE": "float",
            "DEFAULT": 4,
            "MAX": 100,
            "MIN": 1
        },
        {
            "NAME": "density",
            "LABEL": "Density",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "thickness",
            "LABEL": "Thickness",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MAX": 100,
            "MIN": 0
        },
        {
            "VALUES": [0, 1],
            "NAME": "shapeMode",
            "LABEL": "Shape",
            "TYPE": "long",
            "DEFAULT" : 0,
            "LABELS" : ["Ring", "Gabor"]
        },
        {
            "NAME": "darkness",
            "LABEL": "Darkness",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MAX": 100,
            "MIN": 0
        }
    ],
    "ISFVSN": "2"
}*/

// #define ISF_EDITOR_WEBSITE
#ifdef ISF_EDITOR_WEBSITE
#define searchDistance 8.
#endif

#include "lygia/color/luminance.glsl"
#include "lygia/generative/random.glsl" // LYGIA’s functions aren’t exactly the same as the RNG in the Shadertoy shader.
#include "lygia/math/const.glsl"
#include "lygia/math/gaussian.glsl"

// Based on https://www.boredpanda.com/single-line-plotter-scribbles-sergej-stoppel/

void main()
{
    gl_FragColor = vec4(1);

    for (float j = -searchDistance; j <= searchDistance; j++) // test potential circle centers in a window around gl_FragCoord
    for (float i = -searchDistance; i <= searchDistance; i++) {
        vec2 centerPoint = floor(gl_FragCoord.xy / gridStep + vec2(i, j)) * gridStep; // potential circle center
        centerPoint += (random2(centerPoint) - 0.5) * gridStep;

        float lum = luminance(IMG_PIXEL(inputImage, centerPoint)); // target grey value

        float radius = mix(2., searchDistance * gridStep, lum); // target radius

        // draw circle with probability
        if (random(centerPoint) < ((1. - lum) / radius) * 4. * density/searchDistance * gridStep*gridStep) {
            float shape;
            if (shapeMode == 0) { // ring
                shape = 1. - smoothstep(0., thickness, abs(length(centerPoint - gl_FragCoord.xy) - radius));
            } else { // Gabor
                shape = gaussian(centerPoint - gl_FragCoord.xy, radius) * sin(1.5 * TWO_PI * length(centerPoint - gl_FragCoord.xy) / radius);
            }
            gl_FragColor.rgb -= shape * darkness;
        }
    }
}
