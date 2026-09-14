/*{
    "CATEGORIES": [
        "Filter",
        "Generator"
    ],
    "CREDIT": "Flexi <https://www.shadertoy.com/user/Flexi>",
    "DESCRIPTION": "Reaction-diffusion system, converted from <https://www.shadertoy.com/view/4dcGW2>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "showLightWithMouse",
            "LABEL": "Show light with mouse",
            "TYPE": "bool",
            "DEFAULT": false
        },
        {
            "NAME": "mouse",
            "TYPE": "point2D",
            "DEFAULT": [0.5, 0.5],
            "MIN": [0, 0],
            "MAX": [1, 1]
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

        }
    ]
}*/

// #define ISF_EDITOR_WEBSITE
#ifdef ISF_EDITOR_WEBSITE
vec4 texture(sampler2D, vec2);
#endif

#include "lygia/generative/random.glsl"
#define SAMPLER_FNC(TEX, UV) texture(TEX, UV)
#define GAUSSIANBLUR1D_SAMPLER_FNC(TEX, UV) IMG_NORM_PIXEL(TEX, UV)
#define HORIZONTAL_COORDINATE_FNC(UV, SHIFT) fract(vec2(UV.x + (SHIFT), UV.y))
#define VERTICAL_COORDINATE_FNC(UV, SHIFT) fract(vec2(UV.x, UV.y + (SHIFT)))
#include "lygia/filter/gaussianBlur/1D.glsl"
#include "lygia/math/const.glsl"
#include "lygia/space/aspect.glsl"

// main reaction-diffusion loop

// actually the diffusion is realized as a separated two-pass Gaussian blur kernel and is stored in buffer C

void main()
{
    vec2 pixelSize = 1. / RENDERSIZE;
    vec2 uv = gl_FragCoord.xy * pixelSize;

    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        vec4 noise = random4(uv + fract(vec2(42,56)*TIME));

        // get the gradients from the blurred image
        vec2 d = pixelSize*4.;
        vec4 dx = (IMG_NORM_PIXEL(bufferC, fract(uv + vec2(1,0)*d)) - IMG_NORM_PIXEL(bufferC, fract(uv - vec2(1,0)*d))) * 0.5;
        vec4 dy = (IMG_NORM_PIXEL(bufferC, fract(uv + vec2(0,1)*d)) - IMG_NORM_PIXEL(bufferC, fract(uv - vec2(0,1)*d))) * 0.5;

        vec2 uv_red = uv + vec2(dx.x, dy.x)*pixelSize*8.; // add some diffusive expansion

        float new_red = IMG_NORM_PIXEL(bufferA, fract(uv_red)).x + (noise.x - 0.5) * 0.0025 - 0.002; // stochastic decay
        new_red -= (IMG_NORM_PIXEL(bufferC, fract(uv_red + (noise.xy-0.5)*pixelSize)).x -
                    IMG_NORM_PIXEL(bufferA, fract(uv_red + (noise.xy-0.5)*pixelSize))).x * 0.047; // reaction-diffusion

        if (FRAMEINDEX < 10) {
            gl_FragColor = noise;
        } else {
            gl_FragColor.r = clamp(new_red, 0., 1.);
        }
    }
    else if (PASSINDEX == 1) // Shadertoy Buffer B
    {
        //
        GAUSSIAN_BLUR_1D(bufferA, uv, pixelSize.x, 9, HORIZONTAL_COORDINATE_FNC)

        gl_FragColor.rgb = accumColor.rgb / accumWeight;
        gl_FragColor.a = 1.;
    }
    else if (PASSINDEX == 2) // Shadertoy Buffer C
    {
        GAUSSIAN_BLUR_1D(bufferB, uv, pixelSize.y, 9, VERTICAL_COORDINATE_FNC)

        gl_FragColor.rgb = accumColor.rgb / accumWeight;
        gl_FragColor.a = 1.;
    }
    else // Shadertoy Image
    {
        vec2 aspect = vec2(1, RENDERSIZE.y / RENDERSIZE.x);

        // add the pixel gradients
        vec2 d = pixelSize*1.;
        vec4 dx = IMG_NORM_PIXEL(bufferA, uv + vec2(1,0)*d) - IMG_NORM_PIXEL(bufferA, uv - vec2(1,0)*d);
        vec4 dy = IMG_NORM_PIXEL(bufferA, uv + vec2(0,1)*d) - IMG_NORM_PIXEL(bufferA, uv - vec2(0,1)*d);

        // recolor the red channel
        gl_FragColor = vec4(IMG_NORM_PIXEL(bufferA,uv+vec2(dx.x,dy.x)*pixelSize*8.).x)*vec4(0.7,1.5,2.0,1.0)-vec4(0.3,1.0,1.0,1.0);

        // and add the light map
        if (showLightWithMouse) {
            vec2 lightSize = vec2(4);

            vec2 displacement = vec2(dx.x, dy.x) * lightSize; // using only the red gradient as displacement vector
            float light = pow(
                max(1. - distance(0.5 + (uv - 0.5) * aspect * lightSize + displacement, 0.5 + (mouse.xy - 0.5) * aspect * lightSize), 0.),
                4.
            );

            gl_FragColor = mix(
                gl_FragColor,
                vec4(8, 6, 2, 1),
                light * 0.75 * vec4(1. - IMG_NORM_PIXEL(bufferA, uv + vec2(dx.x, dy.x) * pixelSize * 8.).x)
            );
        }

        gl_FragColor.a = 1.;
    }
}
