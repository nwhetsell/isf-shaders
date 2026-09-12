/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "liamegan <https://www.shadertoy.com/user/liamegan>",
    "DESCRIPTION": "FBM noise domain-warped several times, converted from <https://www.shadertoy.com/view/wttXz8>",
    "INPUTS": [
        {
            "NAME": "smoothness",
            "LABEL": "smoothness",
            "TYPE": "float",
            "DEFAULT": 2,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "frequency",
            "LABEL": "Frequency",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "zoomAmount",
            "LABEL": "Zoom amount",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "redness",
            "LABEL": "Redness",
            "TYPE": "float",
            "DEFAULT": 15,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "brightness",
            "LABEL": "Brightness",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MAX": 10,
            "MIN": -10
        },
        {
            "NAME": "fbmRotation",
            "LABEL": "FBM rotation",
            "TYPE": "float",
            "DEFAULT": 28.64789,
            "MAX": 360,
            "MIN": -360
        },
        {
            "NAME": "fbmShift",
            "LABEL": "FBM shift",
            "TYPE": "float",
            "DEFAULT": 100,
            "MAX": 100,
            "MIN": -100
        }
    ],
    "ISFVSN": "2"
}*/

vec2 scaledRandom(vec2);
#define GNOISE2_NOISE2_FNC(UV) scaledRandom(UV)
#include "lygia-additions/gnoise.glsl"
#define FBM_OCTAVES 6
float gnoise1WithShift(inout vec2);
#define FBM_NOISE2_FNC(UV) gnoise1WithShift(UV)
#define FBM_SCALE_SCALAR 1.
#define FBM_AMPLITUDE_INITIAL 0.5
#define FBM_AMPLITUDE_SCALAR 0.4
#define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#include "lygia/generative/fbm.glsl"
#include "lygia/generative/random.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/rotate2d.glsl"

// The Shadertoy shader includes rotate and shift operations in the FBM noise
// generator. LYGIA doesn’t support this directly, but we can use a noise
// function with an inout argument as a workaround.
float gnoise1WithShift(inout vec2 st)
{
    float noise = gnoise1(st);
    st = rotate2d(fbmRotation * DEG2RAD) * st * 2. + fbmShift;
    return noise;
}

vec2 scaledRandom(vec2 st)
{
    vec2 t = vec2(random(st / 1023.), random(st / 1023. + 0.5));
    if (smoothness > 0.)
        t = pow(t, vec2(smoothness));
    return t * 2.5;
}

float pattern(vec2 uv, float time, inout vec2 q, inout vec2 r)
{
    q = vec2(fbm(uv * 0.1),
             fbm(uv + vec2(5.2, 1.3)));

    r = vec2(fbm(uv * 0.1 + 4. * q + vec2(1.7 - 0.5 * time, 9.2)),
             fbm(uv       + 4. * q + vec2(8.3 - 0.5 * time, 2.8)));

    vec2 s = vec2(fbm(uv        + 5. * r + vec2(21.7 - 0.5 * time, 90.2)),
                  fbm(uv * 0.05 + 5. * r + vec2(80.3 - 0.5 * time, 20.8)));

    return fbm(uv * 0.05 + s);
}

void main()
{
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / min(RENDERSIZE.y, RENDERSIZE.x);

    float time = TIME * frequency;
    uv = rotate2d(time * 0.1) * uv;
    uv *= zoomAmount * sin(time) + 3.;
    uv.x -= time * 0.2;

    vec2 q;
    vec2 r;
    vec3 color = vec3(pattern(uv, time, q, r)) * 2.;
    color.r -= dot(q, r) * redness;
    color = mix(
        color,
        vec3(
            pattern(r, time, q, r),
            // This isn’t the same as the color.r offset: q and r are inout arguments of pattern().
            dot(q, r) * redness,
            -0.1
        ),
        0.5);
    color -= q.y * 1.5;
    color = mix(color, vec3(brightness), clamp(q.x, -1., 0.) * 3.);

    gl_FragColor = vec4(abs(color) * 2. - color, 1. / length(q));
}
