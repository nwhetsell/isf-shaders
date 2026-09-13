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
vec4 texture(sampler2D, vec2);
vec2 complex_multiply(in vec2 v1, in vec2 v2)
{
    return vec2(
        v1.x * v2.x - v1.y * v2.y,
        v1.x * v2.y + v1.y * v2.x
    );
}
float complex_magnitudeSquared(in vec2 v)
{
    return v.x * v.x + v.y * v.y;
}
float complex_magnitude(in vec2 v)
{
    return length(v);
}
vec2 complex_conjugate(in vec2 v)
{
    return vec2(v.x, -v.y);
}
// https://en.wikipedia.org/wiki/Complex_number#Complex_conjugate,_absolute_value,_argument_and_division
vec2 complex_divide(in vec2 v1, in vec2 v2)
{
    return complex_multiply(v1, complex_conjugate(v2)) / complex_magnitudeSquared(v2);
}
float complex_argument(in vec2 v)
{
    return atan(v.y, v.x);
}
/*
contributors: ["Patricio Gonzalez Vivo", "David Hoskins", "Inigo Quilez"]
description: Pass a value and get some random normalize value between 0 and 1
use: float random[2|3](<float|vec2|vec3> value)
options:
    - RANDOM_HIGHER_RANGE: for working with a range over 0 and 1
    - RANDOM_SINLESS: Use sin-less random, which tolerates bigger values before producing pattern. From https://www.shadertoy.com/view/4djSRW
    - RANDOM_SCALE: by default this scale if for number with a big range. For producing good random between 0 and 1 use bigger range
examples:
    - /shaders/generative_random.frag
license:
    - MIT License (MIT) Copyright 2014, David Hoskins
*/
float random(in float x) {
    return fract(sin(x) * 43758.5453);
}
float random(in vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453);
}
float random(in vec3 pos) {
    return fract(sin(dot(pos.xyz, vec3(70.9898, 78.233, 32.4355))) * 43758.5453123);
}
float random(in vec4 pos) {
    float dot_product = dot(pos, vec4(12.9898,78.233,45.164,94.673));
    return fract(sin(dot_product) * 43758.5453);
}
vec2 random2(float p) {
    vec3 p3 = fract(vec3(p) * vec4(443.897, 441.423, .0973, .1099).xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xx + p3.yz) * p3.zy);
}
vec2 random2(vec2 p) {
    vec3 p3 = fract(p.xyx * vec4(443.897, 441.423, .0973, .1099).xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xx + p3.yz) * p3.zy);
}
vec2 random2(vec3 p3) {
    p3 = fract(p3 * vec4(443.897, 441.423, .0973, .1099).xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xx + p3.yz) * p3.zy);
}
vec3 random3(float p) {
    vec3 p3 = fract(vec3(p) * vec4(443.897, 441.423, .0973, .1099).xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xxy + p3.yzz) * p3.zyx);
}
vec3 random3(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec4(443.897, 441.423, .0973, .1099).xyz);
    p3 += dot(p3, p3.yxz + 19.19);
    return fract((p3.xxy + p3.yzz) * p3.zyx);
}
vec3 random3(vec3 p) {
    p = fract(p * vec4(443.897, 441.423, .0973, .1099).xyz);
    p += dot(p, p.yxz + 19.19);
    return fract((p.xxy + p.yzz) * p.zyx);
}
vec4 random4(float p) {
    vec4 p4 = fract(p * vec4(443.897, 441.423, .0973, .1099));
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
vec4 random4(vec2 p) {
    vec4 p4 = fract(p.xyxy * vec4(443.897, 441.423, .0973, .1099));
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
vec4 random4(vec3 p) {
    vec4 p4 = fract(p.xyzx * vec4(443.897, 441.423, .0973, .1099));
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
vec4 random4(vec4 p4) {
    p4 = fract(p4 * vec4(443.897, 441.423, .0973, .1099));
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
/*
contributors: Patricio Gonzalez Vivo
description: gaussian coefficient
use: <vec4|vec3|vec2|float> gaussian(<float> sigma, <vec4|vec3|vec2|float> d)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/math_gaussian.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
float gaussian(float d, float s) { return exp(-(d*d) / (2.0 * s*s)); }
float gaussian( vec2 d, float s) { return exp(-( d.x*d.x + d.y*d.y) / (2.0 * s*s)); }
float gaussian( vec3 d, float s) { return exp(-( d.x*d.x + d.y*d.y + d.z*d.z ) / (2.0 * s*s)); }
float gaussian( vec4 d, float s) { return exp(-( d.x*d.x + d.y*d.y + d.z*d.z + d.w*d.w ) / (2.0 * s*s)); }
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: fakes a clamp to edge texture
use: <vec4> sampleClamp2edge(<SAMPLER_TYPE> tex, <vec2> st [, <vec2> texResolution]);
options:
    - SAMPLER_FNC(TEX, UV)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
vec4 sampleClamp2edge(sampler2D tex, vec2 st, vec2 texResolution) {
    vec2 pixel = 1.0/texResolution;
    return texture(tex, clamp(st, pixel, 1.0-pixel));
}
vec4 sampleClamp2edge(sampler2D tex, vec2 st) {
    return texture(tex, clamp(st, vec2(0.01), vec2(0.99) ));
}
vec4 sampleClamp2edge(sampler2D tex, vec2 st, float edge) {
    return texture(tex, clamp(st, vec2(edge), vec2(1.0 - edge) ));
}

/*
contributors: Patricio Gonzalez Vivo
description: One dimension Gaussian Blur to be applied in two passes
use: gaussianBlur1D(<SAMPLER_TYPE> texture, <vec2> st, <vec2> pixel_direction , const int kernelSize)
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
    - GAUSSIANBLUR1D_TYPE: null
    - GAUSSIANBLUR1D_SAMPLER_FNC(TEX, UV): null
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: some useful math constants
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: 'Fix the aspect ratio of a space keeping things squared for you.'
use: <vec2> aspect(<vec2> st, <vec2> st_size)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/draw_shapes.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
vec2 aspect(vec2 st, vec2 s) {
    st.x = st.x * (s.x / s.y);
    return st;
}

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
        vec4 accumColor=vec4(0.); float kernelSizef = float(9); float accumWeight = 0.0; const float k = 1.44; for (int i = 0; i < 9; i++) { float x = -0.5 * ( kernelSizef -1.0) + float(i); float weight = (k / kernelSizef) * gaussian(x, kernelSizef * 0.2913965934); vec4 tex = IMG_NORM_PIXEL(bufferA, fract(vec2(uv.x + (x * (pixelSize.x)), uv.y))); accumColor += weight * tex; accumWeight += weight; }
        gl_FragColor.rgb = accumColor.rgb / accumWeight;
        gl_FragColor.a = 1.;
    }
    else if (PASSINDEX == 2) // Shadertoy Buffer C
    {
        vec4 accumColor=vec4(0.); float kernelSizef = float(9); float accumWeight = 0.0; const float k = 1.44; for (int i = 0; i < 9; i++) { float x = -0.5 * ( kernelSizef -1.0) + float(i); float weight = (k / kernelSizef) * gaussian(x, kernelSizef * 0.2913965934); vec4 tex = IMG_NORM_PIXEL(bufferB, fract(vec2(uv.x, uv.y + (x * (pixelSize.y))))); accumColor += weight * tex; accumWeight += weight; }
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
            max(1. - distance(0.5 + (uv - 0.5) * aspect * lightSize + displacement, 0.5 + (vec2(0).xy * pixelSize - 0.5) * aspect * lightSize), 0.),
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
