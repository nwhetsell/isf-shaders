#include "../lygia/math/cubic.glsl"

#ifndef GNOISE2_NOISE2_FNC
#define GNOISE2_NOISE2_FNC(UV) random2(UV)
#endif

#ifndef FNC_GNOISE2
#define FNC_GNOISE2

// https://www.shadertoy.com/view/XdXGW8
float gnoise1(in vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);
    vec2 a = GNOISE2_NOISE2_FNC(i);
    vec2 b = GNOISE2_NOISE2_FNC(i + vec2(1.0, 0.0));
    vec2 c = GNOISE2_NOISE2_FNC(i + vec2(0.0, 1.0));
    vec2 d = GNOISE2_NOISE2_FNC(i + vec2(1.0, 1.0));
    vec2 u = cubic(f);
    return mix(mix(dot(a, f),
                   dot(b, f - vec2(1.0, 0.0)), u.x),
               mix(dot(c, f - vec2(0.0, 1.0)),
                   dot(d, f - vec2(1.0, 1.0)), u.x), u.y);
}

vec2 gnoise2(in vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);
    vec2 a = GNOISE2_NOISE2_FNC(i);
    vec2 b = GNOISE2_NOISE2_FNC(i + vec2(1.0, 0.0));
    vec2 c = GNOISE2_NOISE2_FNC(i + vec2(0.0, 1.0));
    vec2 d = GNOISE2_NOISE2_FNC(i + vec2(1.0, 1.0));
    vec2 u = cubic(f);
    return mix(a, b, u.x) +
           (c - a) * u.y * (1.0 - u.x) +
           (d - b) * u.x * u.y;
}

#endif
