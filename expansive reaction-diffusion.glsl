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

#define ISF_EDITOR_WEBSITE
#ifdef ISF_EDITOR_WEBSITE
vec4 texture(sampler2D, vec2);
#endif

#include "complex/complex.glsl"
#include "lygia/generative/random.glsl"
#define SAMPLER_FNC(TEX, UV) texture(TEX, UV)
#define GAUSSIANBLUR1D_SAMPLER_FNC(TEX, UV) IMG_NORM_PIXEL(TEX, UV)
#define HORIZONTAL_COORDINATE_FNC(UV, SHIFT) fract(vec2(UV.x + (SHIFT), UV.y))
#define VERTICAL_COORDINATE_FNC(UV, SHIFT) fract(vec2(UV.x, UV.y + (SHIFT)))
#include "lygia/filter/gaussianBlur/1D.glsl"
#include "lygia/math/const.glsl"
#define INV_HALF_PI (0.5 * INV_PI)
#include "lygia/space/aspect.glsl"

// main reaction-diffusion loop

// actually the diffusion is realized as a separated two-pass Gaussian blur kernel and is stored in buffer C

float circle(vec2 uv, vec2 aspect, float scale){
    return clamp( 1. - length((uv-0.5)*aspect*scale), 0., 1.);
}

float sigmoid(float x) {
    return 2./(1. + exp2(-x)) - 1.;
}

float conetip(vec2 uv, vec2 pos, float size, float min)
{
    vec2 aspect = vec2(1., RENDERSIZE.y / RENDERSIZE.x);
    return max( min, 1. - length((uv - pos) * aspect / size) );
}

float warpFilter(vec2 uv, vec2 pos, float size, float ramp)
{
    return 0.5 + sigmoid( conetip(uv, pos, size, -16.) * ramp) * 0.5;
}

vec2 vortex_warp(vec2 uv, vec2 pos, float size, float ramp, vec2 rot)
{
    vec2 aspect = vec2(1., RENDERSIZE.y / RENDERSIZE.x);

    vec2 pos_correct = 0.5 + (pos - 0.5);
    vec2 rot_uv = pos_correct + complex_multiply((uv - pos_correct)*aspect, rot)/aspect;
    float _filter = warpFilter(uv, pos_correct, size, ramp);
    return mix(uv, rot_uv, _filter);
}

vec2 vortex_pair_warp(vec2 uv, vec2 pos, vec2 vel)
{
    vec2 aspect = vec2(1., RENDERSIZE.y / RENDERSIZE.x);
    float ramp = 5.;

    float d = 0.2;

    float l = length(vel);
    vec2 p1 = pos;
    vec2 p2 = pos;

    if(l > 0.){
        vec2 normal = normalize(vel.yx * vec2(-1., 1.))/aspect;
        p1 = pos - normal * d / 2.;
        p2 = pos + normal * d / 2.;
    }

    float w = l / d * 2.;

    // two overlapping rotations that would annihilate when they were not displaced.
    vec2 circle1 = vortex_warp(uv, p1, d, ramp, vec2(cos(w),sin(w)));
    vec2 circle2 = vortex_warp(uv, p2, d, ramp, vec2(cos(-w),sin(-w)));
    return (circle1 + circle2) / 2.;
}

// vec2 mouseDelta(){
//     vec2 pixelSize = 1. / RENDERSIZE;
//     float eighth = 1./8.;
//     vec4 oldMouse = IMG_NORM_PIXEL(iChannel2, vec2(7.5 * eighth, 2.5 * eighth));
//     vec4 nowMouse = vec4(iMouse.xy / RENDERSIZE, iMouse.zw / RENDERSIZE);
//     if(oldMouse.z > pixelSize.x && oldMouse.w > pixelSize.y &&
//        nowMouse.z > pixelSize.x && nowMouse.w > pixelSize.y)
//     {
//         return nowMouse.xy - oldMouse.xy;
//     }
//     return vec2(0.);
// }

#define iMouse vec2(0)

void main()
{
    vec2 pixelSize = 1. / RENDERSIZE;
    vec2 uv = gl_FragCoord.xy * pixelSize;

    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        // vec2 mouseV = mouseDelta();
        vec2 aspect = vec2(1, RENDERSIZE.y / RENDERSIZE.x);
        // uv = vortex_pair_warp(uv, iMouse.xy*pixelSize, mouseV*aspect*1.4);

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

        vec4 noise = random4(uv + fract(vec2(42,56)*TIME));

        vec2 lightSize=vec2(4.);

        // add the pixel gradients
        vec2 d = pixelSize*1.;
        vec4 dx = IMG_NORM_PIXEL(bufferA, uv + vec2(1,0)*d) - IMG_NORM_PIXEL(bufferA, uv - vec2(1,0)*d);
        vec4 dy = IMG_NORM_PIXEL(bufferA, uv + vec2(0,1)*d) - IMG_NORM_PIXEL(bufferA, uv - vec2(0,1)*d);

        vec2 displacement = vec2(dx.x,dy.x)*lightSize; // using only the red gradient as displacement vector
        float light = pow(
            max(1. - distance(0.5 + (uv - 0.5) * aspect * lightSize + displacement, 0.5 + (iMouse.xy * pixelSize - 0.5) * aspect * lightSize), 0.),
            4.
        );

        // recolor the red channel
        vec4 rd = vec4(IMG_NORM_PIXEL(bufferA,uv+vec2(dx.x,dy.x)*pixelSize*8.).x)*vec4(0.7,1.5,2.0,1.0)-vec4(0.3,1.0,1.0,1.0);

        // and add the light map
        // gl_FragColor = mix(
        //     rd,
        //     vec4(8, 6, 2, 1),
        //     light * 0.75 * vec4(1. - IMG_NORM_PIXEL(bufferA, uv + vec2(dx.x, dy.x) * pixelSize * 8.).x)
        // );

        gl_FragColor = rd;

        gl_FragColor.a = 1.;
    }
}
