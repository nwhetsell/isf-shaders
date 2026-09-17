/*{
    "CATEGORIES": [
        "Filter"
    ],
    "CREDIT": "Florian Berger <https://www.shadertoy.com/user/flockaroo>",
    "DESCRIPTION": "Hand drawing effect, converted from <https://www.shadertoy.com/view/XtVGD1>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "rollAmplitude",
            "LABEL": "Roll amplitude",
            "TYPE": "float",
            "DEFAULT": 4,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "lineDefinition",
            "LABEL": "Line definition",
            "TYPE": "float",
            "DEFAULT": 400,
            "MAX": 1000,
            "MIN": 1
        },
        {
            "NAME": "lineAngle",
            "LABEL": "Line angle",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "lineThinness",
            "LABEL": "Line thinness",
            "TYPE": "float",
            "DEFAULT": 0.75,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "lineAmount",
            "LABEL": "Line amount",
            "TYPE": "float",
            "DEFAULT": 3,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "lineDistance",
            "LABEL": "Line distance",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "lineDensity",
            "LABEL": "Line density",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "saturation",
            "LABEL": "Saturation",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MAX": 1,
            "MIN": 0
        }
    ],
    "ISFVSN": "2"
}*/
#define RANDOM_HIGHER_RANGE 
#define RANDOM_SINLESS 
/*
contributor: nan
description: |
    Computes the luminance of the specified linear RGB color using the luminance coefficients from Rec. 709.
    Note, ThreeJS seems to inject this in all their shaders. Which could lead to issues
use: luminance(<vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_LUMINANCE 
float luminance(in vec3 linear) { return dot(linear, vec3(0.21250175, 0.71537574, 0.07212251)); }
float luminance(in vec4 linear) { return luminance( linear.rgb ); }
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
#define RANDOM_SCALE vec4(.1031, .1030, .0973, .1099)
#define FNC_RANDOM 
float random(in float x) {
    x = fract(x * RANDOM_SCALE.x);
    x *= x + 33.33;
    x *= x + x;
    return fract(x);
}
float random(in vec2 st) {
    vec3 p3 = fract(vec3(st.xyx) * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}
float random(in vec3 pos) {
    pos = fract(pos * RANDOM_SCALE.xyz);
    pos += dot(pos, pos.zyx + 31.32);
    return fract((pos.x + pos.y) * pos.z);
}
float random(in vec4 pos) {
    pos = fract(pos * RANDOM_SCALE);
    pos += dot(pos, pos.wzxy + 33.33);
    return fract((pos.x + pos.y) * (pos.z + pos.w));
}
vec2 random2(float p) {
    vec3 p3 = fract(vec3(p) * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xx + p3.yz) * p3.zy);
}
vec2 random2(vec2 p) {
    vec3 p3 = fract(p.xyx * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xx + p3.yz) * p3.zy);
}
vec2 random2(vec3 p3) {
    p3 = fract(p3 * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xx + p3.yz) * p3.zy);
}
vec3 random3(float p) {
    vec3 p3 = fract(vec3(p) * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xxy + p3.yzz) * p3.zyx);
}
vec3 random3(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yxz + 19.19);
    return fract((p3.xxy + p3.yzz) * p3.zyx);
}
vec3 random3(vec3 p) {
    p = fract(p * RANDOM_SCALE.xyz);
    p += dot(p, p.yxz + 19.19);
    return fract((p.xxy + p.yzz) * p.zyx);
}
vec4 random4(float p) {
    vec4 p4 = fract(p * RANDOM_SCALE);
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
vec4 random4(vec2 p) {
    vec4 p4 = fract(p.xyxy * RANDOM_SCALE);
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
vec4 random4(vec3 p) {
    vec4 p4 = fract(p.xyzx * RANDOM_SCALE);
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
vec4 random4(vec4 p4) {
    p4 = fract(p4 * RANDOM_SCALE);
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
/*
contributors: Patricio Gonzalez Vivo
description: some useful math constants
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define EIGHTH_PI 0.39269908169
#define QTR_PI 0.78539816339
#define HALF_PI 1.5707963267948966192313216916398
#define PI 3.1415926535897932384626433832795
#define TWO_PI 6.2831853071795864769252867665590
#define TAU 6.2831853071795864769252867665590
#define INV_PI 0.31830988618379067153776752674503
#define INV_SQRT_TAU 0.39894228040143267793994605993439
#define SQRT_HALF_PI 1.25331413732
#define PHI 1.618033988749894848204586834
#define EPSILON 0.0000001
#define GOLDEN_RATIO 1.6180339887
#define GOLDEN_RATIO_CONJUGATE 0.61803398875
#define GOLDEN_ANGLE 2.39996323
#define DEG2RAD (PI / 180.0)
#define RAD2DEG (180.0 / PI)
/*
contributors: [Ivan Dianov, Shadi El Hajj]
description: polar to cartesian conversion.
use: polar2cart(<vec2> polar)
*/
#define FNC_POLAR2CART 
vec2 polar2cart(in vec2 polar) {
    return vec2(cos(polar.x), sin(polar.x)) * polar.y;
}
// https://mathworld.wolfram.com/SphericalCoordinates.html
vec3 polar2cart( in float r, in float phi, in float theta) {
    float x = r * cos(theta) * sin(phi);
    float y = r * sin(theta) * sin(phi);
    float z = r * cos(phi);
    return vec3(x, y, z);
}
vec2 sampleDerivative(vec2 st, float pixel)
{
    return vec2(
        luminance(IMG_PIXEL(inputImage, st + vec2(pixel,0.0))) - luminance(IMG_PIXEL(inputImage, st - vec2(pixel,0.0))),
        luminance(IMG_PIXEL(inputImage, st + vec2(0.0,pixel))) - luminance(IMG_PIXEL(inputImage, st - vec2(0.0,pixel)))
    );
}
const int angleCount = 3;
const int sampleCount = 16;
void main()
{
    float scaleFactor = RENDERSIZE.y / lineDefinition;
    vec2 position = gl_FragCoord.xy + rollAmplitude * sin(TIME * vec2(1, 1.7)) * scaleFactor;
    vec3 color1 = vec3(0);
    vec3 color2 = vec3(0);
    float sum = 0.;
    for (int i = 0; i < angleCount; i++) {
        float angle = TWO_PI / float(angleCount) * (float(i) + lineAngle);
        vec2 vector = polar2cart(vec2(angle, 1));
        vec2 perpendicularVector = vector.yx * vec2(1, -1);
        for (int j = 0; j < sampleCount; j++) {
            vec2 deltaPosition1 = perpendicularVector * float(j) * scaleFactor;
            vec2 deltaPosition2 = vector * float(j * j) / float(sampleCount) * 0.5 * scaleFactor;
            for (float sgn = -1.; sgn <= 1.; sgn += 2.) {
                vec2 displacementVector = sgn * deltaPosition1 + deltaPosition2;
                float derivativePixel = max(lineDistance, EPSILON);
                vec2 gradient = sampleDerivative(position + displacementVector, derivativePixel);
                gradient /= derivativePixel * 2.;
                color1 += clamp(dot(gradient, vector) - 0.5 * abs(dot(gradient, perpendicularVector)), 0., 0.05)
                          * (1. - float(j) / float(sampleCount));
                float factor = abs(dot(normalize(gradient + vec2(EPSILON)), perpendicularVector));
                vec2 colorPosition = position + displacementVector.yx * vec2(1, -1) * 2.;
                color2 += factor * smoothstep(0.95, 1.05, IMG_PIXEL(inputImage, colorPosition) * saturation + (1. - saturation) + random4(colorPosition * 0.7)).rgb;
                sum += factor;
            }
        }
    }
    color1 /= float(angleCount * sampleCount) * lineThinness / sqrt(RENDERSIZE.y);
    color1.r *= lineDensity + 0.8 * random4(position * 0.7).r;
    color1.r = 1. - color1.r;
    color1.r = pow(color1.r, lineAmount);
    color2 /= sum;
    gl_FragColor = vec4(color1.r * color2, IMG_PIXEL(inputImage, position).a);
}
