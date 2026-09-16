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
        }
    ],
    "ISFVSN": "2"
}*/
#define RANDOM_HIGHER_RANGE 
#define RANDOM_SINLESS 
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
vec4 getCol(vec2 pos)
{
    // take aspect ratio into account
    vec2 uv = ((pos - RENDERSIZE.xy * 0.5) / RENDERSIZE.y * RENDERSIZE.y) / RENDERSIZE.xy + 0.5;
    vec4 c1 = texture(inputImage, uv);
    vec4 e = smoothstep(vec4(-0.05), vec4(0), vec4(uv, vec2(1) - uv));
    // c1 = mix(vec4(1, 1, 1, 0), c1, e.x * e.y * e.z * e.w);
    float d = clamp(dot(c1.xyz, vec3(-0.5, 1., -0.5)), 0., 1.);
    vec4 c2 = vec4(0.7);
    return c1; // min(mix(c1, c2, 1.8 * d), 0.7);
}
vec4 getColHT(vec2 pos)
{
  return smoothstep(0.95, 1.05, getCol(pos) * 0.8 + 0.2 + random4(pos * 0.7));
}
float getVal(vec2 pos)
{
    vec4 c = getCol(pos);
  return pow(dot(c.xyz, vec3(0.333)), 1.) * 1.; // TODO
}
vec2 getGrad(vec2 pos, float eps)
{
    vec2 d = vec2(eps, 0);
    return vec2(
        getVal(pos + d.xy) - getVal(pos - d.xy),
        getVal(pos + d.yx) - getVal(pos - d.yx)
    ) / eps / 2.;
}
#define AngleNum 3
#define SampNum 16
void main()
{
    vec2 pos = gl_FragCoord.xy + 4. * sin(TIME * vec2(1, 1.7)) * RENDERSIZE.y / 400.;
    vec3 col = vec3(0);
    vec3 col2 = vec3(0);
    float sum = 0.;
    for (int i = 0; i < AngleNum; i++) {
        float ang = TWO_PI / float(AngleNum) * (float(i) + 0.8);
        vec2 v = polar2cart(vec2(ang, 1));
        for (int j = 0; j < SampNum; j++) {
            vec2 dpos = v.yx * vec2(1, -1) * float(j) * RENDERSIZE.y / 400.;
            vec2 dpos2 = v.xy * float(j * j) / float(SampNum) * 0.5 * RENDERSIZE.y / 400.;
            for (float s = -1.; s <= 1.; s += 2.) {
                vec2 pos2 = pos + s * dpos + dpos2;
                vec2 pos3 = pos + (s * dpos + dpos2).yx * vec2(1, -1) * 2.;
                vec2 g = getGrad(pos2, 0.4);
                float fact = dot(g, v) - 0.5 * abs(dot(g, v.yx * vec2(1, -1)));
                float fact2 = dot(normalize(g + vec2(EPSILON)), v.yx * vec2(1, -1));
                fact = clamp(fact, 0., 0.05);
                fact2 = abs(fact2);
                fact *= 1. - float(j) / float(SampNum);
                col += fact;
                col2 += fact2 * getColHT(pos3).xyz;
                sum += fact2;
            }
        }
    }
    col /= float(SampNum * AngleNum) * 0.75 / sqrt(RENDERSIZE.y);
    col2 /= sum;
    col.x *= 0.6 + 0.8 * random4(pos * 0.7).x;
    col.x = 1. - col.x;
    col.x *= col.x * col.x;
    vec2 s = sin(pos.xy * 0.1 / sqrt(RENDERSIZE.y / 400.));
    vec3 karo = vec3(1);
    karo -= 0.5 * vec3(0.25, 0.1, 0.1) * dot(exp(-s*s * 80.), vec2(1));
    float r = length(pos - RENDERSIZE.xy * 0.5) / RENDERSIZE.x;
    float vign = 1. - r*r*r;
    gl_FragColor = vec4(col.x * col2, 1);
}
