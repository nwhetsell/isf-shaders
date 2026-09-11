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
#include "lygia/generative/random.glsl"


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
