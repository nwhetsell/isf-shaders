/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "loicvdb <https://github.com/loicvdb>",
    "DESCRIPTION": "Fractal cloud, converted from <https://www.shadertoy.com/view/tsGSDt>",
    "INPUTS": [
        {
            "NAME": "speed",
            "LABEL": "Speed",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "powerAmplitude",
            "LABEL": "Power amplitude",
            "TYPE": "float",
            "DEFAULT": 5,
            "MAX": 20,
            "MIN": 0
        },
        {
            "NAME": "density",
            "LABEL": "Density",
            "TYPE": "float",
            "DEFAULT": 10,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "motionBlur",
            "LABEL": "Motion blur",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "cameraFocalDistance",
            "LABEL": "Camera focal distance",
            "TYPE": "float",
            "DEFAULT": 1.6,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "cameraFocalLength",
            "LABEL": "Camera focal length",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "cameraAperture",
            "LABEL": "Camera aperture",
            "TYPE": "float",
            "DEFAULT": 0.075,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "apertureRotation",
            "LABEL": "Aperture rotation",
            "TYPE": "float",
            "DEFAULT": 0.075,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "volumeColor",
            "LABEL": "Volume color",
            "TYPE": "color",
            "DEFAULT": [0.3, 0.3, 0.3, 1]
        },
        {
            "NAME": "lightColor",
            "LABEL": "Light color",
            "TYPE": "color",
            "DEFAULT": [0.5, 0.5, 0.7, 1]
        },
        {
            "NAME": "lightIntensity",
            "LABEL": "Light intensity",
            "TYPE": "float",
            "DEFAULT": 20,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "highlightColor",
            "LABEL": "Highlight color",
            "TYPE": "color",
            "DEFAULT": [0.5, 0.1, 0.2, 1]
        },
        {
            "NAME": "enableBloom",
            "LABEL": "Enable bloom",
            "TYPE": "bool",
            "DEFAULT": true
        },
        {
            "NAME": "bloomDistance",
            "LABEL": "Bloom distance",
            "TYPE": "float",
            "DEFAULT": 32,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "enableTonemap",
            "LABEL": "Enable tonemap",
            "TYPE": "bool",
            "DEFAULT": true
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "cloud",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {

        }
    ]
}*/
// #define ISF_EDITOR_WEBSITE
/*
contributors: Patricio Gonzalez Vivo
description: clamp a value between 0 and 1
use: <float|vec2|vec3|vec4> saturation(<float|vec2|vec3|vec4> value)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/math_functions.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_SATURATE 
#define saturate(V) clamp(V, 0.0, 1.0)

/*
contributors: Narkowicz 2015
description: ACES Filmic Tone Mapping Curve. https://knarkowicz.wordpress.com/2016/01/06/aces-filmic-tone-mapping-curve/
use: <vec3|vec4> tonemapACES(<vec3|vec4> x)
*/
#define FNC_TONEMAPACES 
vec3 tonemapACES(vec3 v) {
    const float a = 2.51;
    const float b = 0.03;
    const float c = 2.43;
    const float d = 0.59;
    const float e = 0.14;
    return saturate((v*(a*v+b))/(v*(c*v+d)+e));
}
vec4 tonemapACES(in vec4 v) {
    return vec4(tonemapACES(v.rgb), v.a);
}
#define RANDOM_SINLESS 
#define RANDOM_HIGHER_RANGE 
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
contributors: Patricio Gonzalez Vivo
description: extend GLSL Max function to add more arguments
use:
    - <float> mmax(<float> A, <float> B, <float> C[, <float> D])
    - <vec2|vec3|vec4> mmax(<vec2|vec3|vec4> A)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_MMAX 
float mmax(in float a, in float b) { return max(a, b); }
float mmax(in float a, in float b, in float c) { return max(a, max(b, c)); }
float mmax(in float a, in float b, in float c, in float d) { return max(max(a, b), max(c, d)); }
float mmax(const vec2 v) { return max(v.x, v.y); }
float mmax(const vec3 v) { return mmax(v.x, v.y, v.z); }
float mmax(const vec4 v) { return mmax(v.x, v.y, v.z, v.w); }
/*
contributors: Patricio Gonzalez Vivo
description: returns a 2x2 rotation matrix
use: <mat2> rotate2d(<float> radians)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_ROTATE2D 
mat2 rotate2d(const in float r){
    float c = cos(r);
    float s = sin(r);
    return mat2(c, s, -s, c);
}
/*
contributors: Patricio Gonzalez Vivo
description: returns a 3x3 rotation matrix
use: <mat3> rotate3dX(<float> radians)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_ROTATE3DX 
mat3 rotate3dX(const in float r){
    float c = cos(r);
    float s = sin(r);
    return mat3(vec3(1.0,0.0,0.0),
                vec3(0.0,c,s),
                vec3(0.0,-s,c));
}
/*
contributors: Patricio Gonzalez Vivo
description: returns a 3x3 rotation matrix
use: <mat3> rotate3dY(<float> radians)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_ROTATE3DY 
mat3 rotate3dY(const in float r){
    float c = cos(r);
    float s = sin(r);
    return mat3(vec3(c,0.,-s),
                vec3(0.,1.,0.),
                vec3(s,0.,c));
}
/*
contributors: Patricio Gonzalez Vivo
description: returns a 3x3 rotation matrix
use: <mat3> rotate3dZ(<float> radians)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_ROTATE3DZ 
mat3 rotate3dZ(const in float r){
    float c = cos(r);
    float s = sin(r);
    return mat3(vec3(c,s,0.),
                vec3(-s,c,0.),
                vec3(0.,0.,1.));
}
/*
contributors: [Ivan Dianov, Kathy McGuiness]
description: cartesian to polar transformation.
use: <vec2|vec3> cart2polar(<vec2|vec3> st)
*/
#define FNC_CART2POLAR 
vec2 cart2polar(in vec2 st) {
    return vec2(atan(st.y, st.x), length(st));
}
// https://mathworld.wolfram.com/SphericalCoordinates.html
vec3 cart2polar( in vec3 st ) {
    float r = length(st);
    float phi = acos(st.z/r);
    float theta = atan(st.y, st.x);
    return vec3(r, phi, theta);
}
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
vec2 seed = vec2(0);
float frand(void)
{
    seed += vec2(1.153535, -1.1231354);
    return random(seed);
}
mat3 rotationMatrix(vec3 r)
{
    return rotate3dZ(-r.z) * rotate3dX(-r.x) * rotate3dY(r.y);
}
float TIME_SCALED = TIME * speed;
float distanceEstimation(vec3 position)
{
    const float maxDistance = 1.5;
    float r = length(position);
    if (r > maxDistance)
        return r - 1.2;
    float power = powerAmplitude * sin(TIME_SCALED * 0.1);
    vec3 z = position;
    float dr = 1.;
    for (int i = 0; i < 6; i++) {
        vec3 polar = cart2polar(z.xzy);
        r = polar.x;
        if (r > maxDistance)
            break;
        z = polar2cart(pow(r, power), polar.y * power - TIME_SCALED, polar.z * power - TIME_SCALED) + position;
        dr = pow(r, power - 1.) * power * dr + 1.;
    }
    return 0.5 * log(r) * r / dr;
}
#define StepSize 0.03
#define ShadowStepSize 0.2
#define ShadowRaysPerStep 0.25
vec3 directLight(in vec3 position)
{
    const vec3 lightDirection = normalize(vec3(-1, -3, 1));
    vec3 absorption = vec3(1);
    for (int i = 0; i < 7; i++) {
        float distance = distanceEstimation(position);
        position -= lightDirection * max(distance, ShadowStepSize);
        if (distance < ShadowStepSize) {
            float abStep = ShadowStepSize * frand();
            position -= lightDirection * (abStep - ShadowStepSize);
            if (distance < 0.) {
                absorption *= exp(-density * abStep);
                if (mmax(absorption) < 0.1)
                    break;
            }
        }
        if (length(position) > 1.5)
            break;
    }
    return lightColor.rgb * lightIntensity * absorption;
}
// The Shadertoy shader uses the direction argument to return the color from a
// cubemap, which is impossible in an ISF shader.
vec3 backgroundColor(vec3 direction)
{
    return vec3(0);
}
vec3 pathTrace(vec3 rayPosition, vec3 rayDirection)
{
    rayPosition += rayDirection * max(length(rayPosition) - 1.5, 0.);
    vec3 absorption = vec3(1);
    vec3 color = vec3(0);
    for (int i = 0; i < 150; i++) {
        float distance = distanceEstimation(rayPosition);
        rayPosition += rayDirection * max(distance, StepSize);
        if (distance < StepSize && length(rayPosition) < 1.5) {
            float abStep = StepSize * frand();
            rayPosition += rayDirection * (abStep - StepSize);
            if (distance < 0.) {
                float absorbance = exp(-density * abStep);
                float transmittance = 1. - absorbance;
                if (distance > -0.0005)
                    color += absorption * highlightColor.rgb;
                if (frand() < ShadowRaysPerStep)
                    color += 1. / ShadowRaysPerStep * absorption * volumeColor.rgb * transmittance * directLight(rayPosition);
                if (mmax(absorption) < 0.05)
                    break;
                if (frand() > absorbance) {
                    rayDirection = vec3(1, 0, 0) * rotationMatrix(vec3(frand() * TWO_PI, 0, frand() * TWO_PI)); // random direction
                    absorption *= volumeColor.rgb;
                }
            }
        }
        if (length(rayPosition) > 1.5 && dot(rayDirection, rayPosition) > 0.)
            return color + backgroundColor(rayDirection) * absorption;
    }
    return color;
}
vec2 sampleAperture(int nbBlades, float rotation)
{
    float alpha = TWO_PI / float(nbBlades);
    float side = sin(alpha * 0.5);
    int blade = int(frand() * float(nbBlades));
    vec2 tri = vec2(frand(), -frand());
    if (tri.x + tri.y > 0.)
        tri = vec2(tri.x - 1., -1. - tri.y);
    tri.x *= side;
    tri.y *= sqrt(1. - side*side);
    return tri * rotate2d(rotation * DEG2RAD + float(blade) / float(nbBlades) * TWO_PI);
}
void main()
{
    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        vec2 uv = (gl_FragCoord.xy + vec2(frand(), frand()) - RENDERSIZE * 0.5) / RENDERSIZE.y;
        seed = gl_FragCoord.xy / RENDERSIZE * 1000. + log(vec2(FRAMEINDEX));
        vec3 focalPoint = vec3(uv * cameraFocalDistance / cameraFocalLength, cameraFocalDistance);
        vec3 aperture = cameraAperture * vec3(sampleAperture(6, apertureRotation), 0.);
        vec3 cameraPosition = vec3(0, 0, -2.5) * rotationMatrix(vec3(0, TIME_SCALED * 0.2, 0));
        mat3 cameraMatrix = rotationMatrix(vec3(0, TIME_SCALED * 0.2, 0.5 * sin(TIME_SCALED * 0.3)));
        vec3 rayDirection = normalize(focalPoint - aperture) * cameraMatrix;
        gl_FragColor = vec4(pathTrace(cameraPosition + aperture * cameraMatrix, rayDirection), 1);
        if (FRAMEINDEX > 0)
            gl_FragColor += IMG_THIS_PIXEL(cloud) * motionBlur;
    }
    else // Shadertoy Image
    {
        vec4 color = IMG_THIS_PIXEL(cloud);
        vec3 bloom = vec3(0);
        if (enableBloom) {
            for(int y = -1; y <= 1; y++)
            for(int x = -1; x <= 1; x++)
                bloom += textureLod(cloud, (gl_FragCoord.xy + vec2(x, y) * bloomDistance) / RENDERSIZE, 7.).rgb / color.a;
            bloom = max(bloom / 9. - 0.5, vec3(0)) * 0.25;
        }
        color /= color.a;
        color.rgb += bloom;
        if (enableTonemap)
            color.rgb = tonemapACES(color.rgb);
        gl_FragColor = vec4(color.rgb, length(color.rgb));
    }
}
