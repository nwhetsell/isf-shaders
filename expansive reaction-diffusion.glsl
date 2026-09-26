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
            "NAME": "gradientDistance",
            "LABEL": "Gradient distance",
            "TYPE": "float",
            "DEFAULT": 4,
            "MIN": 1,
            "MAX": 10
        },
        {
            "NAME": "diffusionFactor",
            "LABEL": "Diffusion factor",
            "TYPE": "float",
            "DEFAULT": 1,
            "MIN": 0,
            "MAX": 2
        },
        {
            "NAME": "reactionDiffusionFactor",
            "LABEL": "Reaction-diffusion factor",
            "TYPE": "float",
            "DEFAULT": 0.047,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "decayFactor",
            "LABEL": "Decay factor",
            "TYPE": "float",
            "DEFAULT": 0.0025,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "gaussianKernelScale",
            "LABEL": "Gaussian kernel scale",
            "TYPE": "float",
            "DEFAULT": 1.44,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "standardDeviationScale",
            "LABEL": "Standard deviation scale",
            "TYPE": "float",
            "DEFAULT": 0.2913965934,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "showLightWithInputImage",
            "LABEL": "Show light with input image",
            "TYPE": "bool",
            "DEFAULT": false
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
        },
        {
            "NAME": "inverseLightSize",
            "LABEL": "Light size",
            "TYPE": "float",
            "DEFAULT": 0.25,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "radiance",
            "LABEL": "Radiance",
            "TYPE": "float",
            "DEFAULT": 4,
            "MIN": 0,
            "MAX": 10
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

#include "lygia/color/luminance.glsl"
#include "lygia/generative/random.glsl"
#define SAMPLER_FNC(TEX, UV) texture(TEX, UV)
#define GAUSSIANBLUR1D_SAMPLER_FNC(TEX, UV) IMG_NORM_PIXEL(TEX, UV)
#define HORIZONTAL_COORDINATE_FNC(UV, SHIFT) fract(vec2(UV.x + (SHIFT), UV.y))
#define VERTICAL_COORDINATE_FNC(UV, SHIFT) fract(vec2(UV.x, UV.y + (SHIFT)))
#include "lygia/filter/gaussianBlur/1D.glsl"
#include "lygia/math/const.glsl"
#include "lygia/space/aspect.glsl"

#define GAUSSIANBLUR_KERNEL_SIZE 9

void main()
{
    vec2 pixelSize = 1. / RENDERSIZE;
    vec2 uv = gl_FragCoord.xy * pixelSize;

    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        vec4 noise = random4(uv + fract(vec2(42, 56) * TIME));

        // Get the gradients from the blurred image.
        vec2 d = pixelSize * gradientDistance;
        vec4 dx = IMG_NORM_PIXEL(bufferC, fract(uv + vec2(1, 0) * d)) - IMG_NORM_PIXEL(bufferC, fract(uv - vec2(1, 0) * d));
        vec4 dy = IMG_NORM_PIXEL(bufferC, fract(uv + vec2(0, 1) * d)) - IMG_NORM_PIXEL(bufferC, fract(uv - vec2(0, 1) * d));

        // Add some diffusive expansion.
        vec2 red = uv + vec2(dx.r, dy.r) * d * diffusionFactor;

        vec2 noise2 = noise.xy - 0.5;
        float newRed = IMG_NORM_PIXEL(bufferA, fract(red)).x + noise2.x * decayFactor - 0.002; // stochastic decay
        newRed -= (IMG_NORM_PIXEL(bufferC, fract(red + noise2 * pixelSize)).x -
                    IMG_NORM_PIXEL(bufferA, fract(red + noise2 * pixelSize)).x) * reactionDiffusionFactor; // reaction-diffusion

        if (FRAMEINDEX < 10) {
            gl_FragColor = noise;
        } else {
            gl_FragColor.r = clamp(newRed, 0., 1.);
        }
    }
    else if (PASSINDEX == 1) // Shadertoy Buffer B
    {
        // A limitation of ISF shaders is that the texture look-ups are
        // performed by macros like IMG_NORM_PIXEL, not functions like
        // texture(), and IMG_NORM_PIXEL expands to a function call (like
        // VVSAMPLER_2DBYNORM) that includes variables that are seemingly only
        // defined in main():
        //    https://github.com/search?q=owner%3AmrRay+VVSAMPLER_2DBYNORM&type=code
        // As a workaround, we define LYGIA’s Gaussian blur function as a macro
        // and completely expand it here.
        GAUSSIAN_BLUR_1D(bufferA, uv, pixelSize.x, GAUSSIANBLUR_KERNEL_SIZE, HORIZONTAL_COORDINATE_FNC)

        gl_FragColor.rgb = accumColor.rgb / accumWeight;
        gl_FragColor.a = 1.;
    }
    else if (PASSINDEX == 2) // Shadertoy Buffer C
    {
        GAUSSIAN_BLUR_1D(bufferB, uv, pixelSize.y, GAUSSIANBLUR_KERNEL_SIZE, VERTICAL_COORDINATE_FNC)

        gl_FragColor.rgb = accumColor.rgb / accumWeight;
        gl_FragColor.a = 1.;
    }
    else // Shadertoy Image
    {
        // Add the pixel gradients.
        vec2 d = pixelSize;
        vec4 dx = IMG_NORM_PIXEL(bufferA, uv + vec2(1, 0) * d) - IMG_NORM_PIXEL(bufferA, uv - vec2(1, 0) * d);
        vec4 dy = IMG_NORM_PIXEL(bufferA, uv + vec2(0, 1) * d) - IMG_NORM_PIXEL(bufferA, uv - vec2(0, 1) * d);

        // Recolor the red channel.
        gl_FragColor = vec4(IMG_NORM_PIXEL(bufferA, uv + vec2(dx.x, dy.x) * pixelSize * 8.).x) * vec4(0.7, 1.5, 2.0, 1.0) - vec4(0.3, 1.0, 1.0, 1.0);

        // Add the light map.
        vec2 lightSize = vec2(1. / inverseLightSize);
        // Use only the red gradient as displacement vector.
        vec2 displacement = vec2(dx.r, dy.r) * lightSize;
        lightSize = aspect(lightSize.yx, RENDERSIZE.yx).yx;

        float light = 0.;

        if (showLightWithMouse) {
            light += pow(
                max(1. - distance(0.5 + (uv - 0.5) * lightSize + displacement, 0.5 + (mouse.xy - 0.5) * lightSize), 0.),
                radiance
            );
        }

        if (showLightWithInputImage) {
            light += pow(
                distance(displacement, luminance(IMG_NORM_PIXEL(inputImage, uv)) * lightSize),
                radiance
            );
        }

        gl_FragColor = mix(
            gl_FragColor,
            vec4(8, 6, 2, 1),
            light * 0.75 * vec4(1. - IMG_NORM_PIXEL(bufferA, uv + vec2(dx.x, dy.x) * pixelSize * 8.).x)
        );

        gl_FragColor.a = 1.;
    }
}
