/*{
    "CATEGORIES": [
        "Filter",
        "Generator"
    ],
    "CREDIT": "Zavie <https://www.shadertoy.com/user/Zavie>",
    "DESCRIPTION": "Rain drop ripples, converted from <https://www.shadertoy.com/view/ldfyzl>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "resolution",
            "LABEL": "Resolution",
            "TYPE": "float",
            "DEFAULT": 10,
            "MAX": 20,
            "MIN": 0.000001
        },
        {
            "NAME": "frequency",
            "LABEL": "Frequency",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "decay",
            "LABEL": "Deacy",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MAX": 0.6,
            "MIN": 0
        },
        {
            "NAME": "ripples",
            "LABEL": "Ripples",
            "TYPE": "float",
            "DEFAULT": 31,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "height",
            "LABEL": "Height",
            "TYPE": "float",
            "DEFAULT": 0.001,
            "MAX": 0.1,
            "MIN": 0.000001
        },
        {
            "NAME": "specular",
            "LABEL": "Specular",
            "TYPE": "float",
            "DEFAULT": 5,
            "MAX": 50,
            "MIN": 0
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
// Maximum number of cells a ripple can cross.
#define MAX_RADIUS 2
void main()
{
    vec2 uv = gl_FragCoord.xy / min(RENDERSIZE.x, RENDERSIZE.y) * resolution;
    vec2 position = floor(uv);
    vec2 circles = vec2(0);
    int circleCount = 0;
    for (int j = -MAX_RADIUS; j <= MAX_RADIUS; ++j)
    for (int i = -MAX_RADIUS; i <= MAX_RADIUS; ++i) {
        vec2 shiftedPosition = position + vec2(i, j);
        vec2 hash = random2(shiftedPosition);
        vec2 positionPlusRandomOffset = shiftedPosition + random2(hash);
        float t = fract(frequency * TIME + random(hash));
        vec2 v = uv - positionPlusRandomOffset;
        float distance = length(v) - float(MAX_RADIUS + 1) * t;
        vec2 distances = distance + vec2(-height, height);
        vec2 ps = sin(ripples * distances) * smoothstep(-0.6, -decay, distances) * smoothstep(0., -decay, distances);
        float oneMinusT = 1. - t;
        circles += 0.25 * normalize(v) * (ps.y - ps.x) / height * oneMinusT*oneMinusT;
        circleCount++;
    }
    circles /= float(circleCount);
    float intensity = mix(0.01, 0.15, smoothstep(0.1, 0.6, abs(fract(0.05 * TIME + 0.5) * 2. - 1.)));
    vec3 n = vec3(circles, sqrt(1. - dot(circles, circles)));
    vec4 pixel = IMG_NORM_PIXEL(inputImage, gl_FragCoord.xy / RENDERSIZE - intensity * n.xy);
    vec3 color = pixel.rgb + specular * pow(clamp(dot(n, normalize(vec3(1., 0.7, 0.5))), 0., 1.), 6.);
    gl_FragColor = vec4(color, pixel.a);
}
