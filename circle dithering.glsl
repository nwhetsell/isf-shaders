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
        }
    ],
    "ISFVSN": "2"
}*/

#include "lygia/color/luminance.glsl"
#include "lygia/generative/random.glsl" // LYGIA’s functions aren’t exactly the same as the RNG in the Shadertoy shader.

// ref image: http://www.boredpanda.com/single-line-plotter-scribbles-sergej-stoppel/
// ( doing it simpler: circles instead of scribbles ;-) )

float L = 8.,                   // L*T = neightborhood size
      T = 4.,                   // grid step for circle centers
      d = 1.;                   // density

#define C(U,P,r) smoothstep(1.5, 0., abs(length(P - U) - r))                       // ring
//#define C(U,P,r) exp(-.5*dot(P-U,P-U)/(r*r)) * sin(1.5*6.28*length(P-U)/r) // Gabor

void main()
{
    vec2 R = RENDERSIZE.xy;
    gl_FragColor = vec4(1);

    for (float j = -L; j <= L; j++)    // test potential circle centers in a window around gl_FragCoord
    for (float i = -L; i <= L; i++) {
        vec2 P = floor(gl_FragCoord.xy / T + vec2(i,j)) * T; // potential circle center
        P += T * (random2(P) - 0.5);
        float v = luminance(IMG_PIXEL(inputImage, P)); // target grey value
        float r = mix(2., L * T, v); // target radius
        if (random(P) < ((1. - v) / r) * 4. * d/L * T*T) { // draw circle with probability
            gl_FragColor -= C(gl_FragCoord.xy, P, r) * 0.2;
        }
    }
}
