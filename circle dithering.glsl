/*{
    "CATEGORIES": [
        "Filter"
    ],
    "CREDIT": "Fabrice Neyret <https://www.shadertoy.com/user/FabriceNeyret2>",
    "DESCRIPTION": "Circle dithering, converted from <https://www.shadertoy.com/view/MdSfWK>",
    "INPUTS": [
        {
            "NAME" : "inputImage",
            "TYPE" : "image"
        },
        {
            "NAME": "searchDistance",
            "LABEL": "Search distance",
            "TYPE": "float",
            "DEFAULT": 8,
            "MAX": 30,
            "MIN": 1
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

// ref image: http://www.boredpanda.com/single-line-plotter-scribbles-sergej-stoppel/
// ( doing it simpler: circles instead of scribbles ;-) )

#define C(U,P,r) smoothstep(1.5, 0., abs(length(P - U) - r))                       // ring
//#define C(U,P,radius) exp(-.5*dot(P-U,P-U)/(radius*radius)) * sin(1.5*6.28*length(P-U)/radius) // Gabor

void main()
{
    gl_FragColor = vec4(1);

    for (float j = -searchDistance; j <= searchDistance; j++) // test potential circle centers in a window around gl_FragCoord
    for (float i = -searchDistance; i <= searchDistance; i++) {
        vec2 P = floor(gl_FragCoord.xy / gridStep + vec2(i, j)) * gridStep; // potential circle center
        P += gridStep * (random2(P) - 0.5);
        float lum = luminance(IMG_PIXEL(inputImage, P)); // target grey value
        float radius = mix(2., searchDistance * gridStep, lum); // target radius
        // draw circle with probability
        if (random(P) < ((1. - lum) / radius) * 4. * density/searchDistance * gridStep*gridStep) {
            gl_FragColor.rgb -= C(gl_FragCoord.xy, P, radius) * 0.2;
        }
    }
}
