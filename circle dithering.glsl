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
            "NAME": "L",
            "LABEL": "Search distance",
            "TYPE": "float",
            "DEFAULT": 8,
            "MAX": 30,
            "MIN": 1
        },
        {
            "NAME": "T",
            "LABEL": "Grid step",
            "TYPE": "float",
            "DEFAULT": 4,
            "MAX": 100,
            "MIN": 1
        },
        {
            "NAME": "d",
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
#define L 8.
#endif

#include "lygia/color/luminance.glsl"
#include "lygia/generative/random.glsl" // LYGIA’s functions aren’t exactly the same as the RNG in the Shadertoy shader.

// ref image: http://www.boredpanda.com/single-line-plotter-scribbles-sergej-stoppel/
// ( doing it simpler: circles instead of scribbles ;-) )

#define C(U,P,r) smoothstep(1.5, 0., abs(length(P - U) - r))                       // ring
//#define C(U,P,r) exp(-.5*dot(P-U,P-U)/(r*r)) * sin(1.5*6.28*length(P-U)/r) // Gabor

void main()
{
    gl_FragColor = vec4(1);

    for (float j = -L; j <= L; j++)    // test potential circle centers in a window around gl_FragCoord
    for (float i = -L; i <= L; i++) {
        vec2 P = floor(gl_FragCoord.xy / T + vec2(i,j)) * T; // potential circle center
        P += T * (random2(P) - 0.5);
        float v = luminance(IMG_PIXEL(inputImage, P)); // target grey value
        float r = mix(2., L * T, v); // target radius
        if (random(P) < ((1. - v) / r) * 4. * d/L * T*T) { // draw circle with probability
            gl_FragColor.rgb -= C(gl_FragCoord.xy, P, r) * 0.2;
        }
    }
}
