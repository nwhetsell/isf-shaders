/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "Leon Denise <https://www.shadertoy.com/user/leon>",
    "DESCRIPTION": "Tribute to Marc-Antoine Mathieu, converted from <https://www.shadertoy.com/view/XlfBR7>",
    "INPUTS": [
        {
            "NAME": "donut",
            "LABEL": "Outer radius",
            "TYPE": "float",
            "DEFAULT": 30,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "cell",
            "LABEL": "Room length",
            "TYPE": "float",
            "DEFAULT": 4,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "height",
            "LABEL": "Room depth",
            "TYPE": "float",
            "DEFAULT": 2,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "thin",
            "LABEL": "Wall thickness",
            "TYPE": "float",
            "DEFAULT": 0.04,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "radius",
            "LABEL": "Inner radius",
            "TYPE": "float",
            "DEFAULT": 15,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "speed",
            "LABEL": "Speed",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "cameraX",
            "LABEL": "Camera x",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "cameraY",
            "LABEL": "Camera y",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "cameraZ",
            "LABEL": "Camera z",
            "TYPE": "float",
            "DEFAULT": -20,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "yAxisRotation",
            "LABEL": "y-axis rotation",
            "TYPE": "float",
            "DEFAULT": 22.5,
            "MAX": 180,
            "MIN": -180
        },
        {
            "NAME": "xAxisRotation",
            "LABEL": "x-axis rotation",
            "TYPE": "float",
            "DEFAULT": 30,
            "MAX": 180,
            "MIN": -180
        },
        {
            "NAME": "boxHeight",
            "LABEL": "Box height",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "boxToroidalSeparation",
            "LABEL": "Box toroidal separation",
            "TYPE": "float",
            "DEFAULT": 0.43,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "boxPoloidalSeparation",
            "LABEL": "Box poloidal separation",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "boxProportion",
            "LABEL": "Box proportion",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "windowGrilleThickness",
            "LABEL": "Window grille thickness",
            "TYPE": "float",
            "DEFAULT": 0.008,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "windowGrilleDepth",
            "LABEL": "Window grille depth",
            "TYPE": "float",
            "DEFAULT": 0.04,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "windowFrameArea",
            "LABEL": "Window frame area",
            "TYPE": "float",
            "DEFAULT": 0.08,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "windowFrameDepth",
            "LABEL": "Window frame depth",
            "TYPE": "float",
            "DEFAULT": 0.006,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "backgroundColor",
            "LABEL": "Background color",
            "TYPE": "color",
            "DEFAULT": [0, 0, 0, 0]
        }
    ],
    "ISFVSN": "2"
}*/
// #define RANDOM_HIGHER_RANGE
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
#define RANDOM_SCALE vec4(443.897, 441.423, .0973, .1099)
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
contributors:  Inigo Quiles
description: generate the SDF of a box
use: <float> boxSDF( in <vec3> pos [, in <vec3> borders ] ) 
*/
#define FNC_BOXSDF 
float boxSDF( vec3 p ) {
    vec3 d = abs(p);
    return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));
}
float boxSDF( vec3 p, vec3 b ) {
    vec3 d = abs(p) - b;
    return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));
}
/*
contributors:  Inigo Quiles
description: generate the SDF of a sphere
use: <float> sphereSDF( in <vec3> pos[], in <float> size] ) 
*/
#define FNC_SPHERESDF 
float sphereSDF(vec3 p) { return length(p); }
float sphereSDF(vec3 p, float s) { return sphereSDF(p) - s; }
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

#define FNC_OPREPEAT_ADDITIONS 
float opRepeat( in float p, in float s ) {
    return mod(p+s*0.5,s)-s*0.5;
}
vec2 opRepeat( in vec2 p, in vec2 s ) {
    return mod(p+s*0.5,s)-s*0.5;
}
// Raymarching sketch inspired by the work of Marc-Antoine Mathieu
// Leon 2017-11-21
// using code from IQ, Mercury, LJ, Duke, Koltes
#define STEPS 250.
#define VOLUME 0.001
float map(vec3);
float getShadow(vec3 pos, vec3 at, float k)
{
    vec3 dir = normalize(at - pos);
    float maxt = length(at - pos);
    float f = 1.;
    float t = VOLUME * 50.;
    for (float i = 0.; i <= 1.; i += 1. / 15.) {
        float dist = map(pos + dir * t);
        if (dist < VOLUME) {
            return 0.;
        }
        f = min(f, k * dist / t);
        t += dist;
        if (t >= maxt) {
            break;
        }
    }
    return f;
}
vec3 getNormal(vec3 p)
{
    return normalize(vec3(
        map(p + vec3(EPSILON,0,0)) - map(p - vec3(EPSILON,0,0)),
        map(p + vec3(0,EPSILON,0)) - map(p - vec3(0,EPSILON,0)),
        map(p + vec3(0,0,EPSILON)) - map(p - vec3(0,0,EPSILON))
    ));
}
void camera(inout vec3 p)
{
    p.xz *= rotate2d(-yAxisRotation * DEG2RAD);
    p.yz *= rotate2d(-xAxisRotation * DEG2RAD);
}
float windowGrille(vec3 pos, float height, float width, vec2 indexes)
{
    float randomness = random(indexes);
    vec3 p = pos;
    p.xy = opRepeat(p.xy, vec2(height * (0.6 + randomness * 0.4), width * (0.3 + randomness * 0.7)));
    float scene = boxSDF(p, vec3(windowGrilleThickness, width, windowGrilleDepth) * 2.);
    scene = min(scene, boxSDF(p, vec3(height, windowGrilleThickness, windowGrilleDepth) * 2.));
    scene = max(scene, boxSDF(pos, vec3(height, width, windowGrilleDepth)));
    return scene;
}
float window(vec3 pos, float height, float width, vec2 indexes)
{
    float frame = boxSDF(pos, vec3(height, width, windowFrameDepth));
    frame = max(frame, -boxSDF(pos, vec3(height - windowFrameArea, width - windowFrameArea, windowFrameDepth * 2.)));
    float scene = windowGrille(pos, height, width, indexes);
    scene = min(scene, frame);
    return scene;
}
float boxes(vec3 pos, vec2 indexes)
{
    float randomness1 = random(indexes);
    vec2 separation = vec2(
        cell * boxToroidalSeparation * (0.3 + randomness1),
        cell * boxPoloidalSeparation * (0.5 + randomness1)
    );
    float randomness2 = random(vec2(floor(pos.y / separation.x), floor(pos.z / separation.y)));
    vec3 p = pos;
    p.y = opRepeat(p.y - separation.x * 0.5, separation.x);
    p.z = opRepeat(p.z - separation.y * 0.5, separation.y);
    float randomizedHeight = boxHeight + 0.8 * randomness1 + randomness2;
    float scene = boxSDF(p, vec3(randomizedHeight, 0.1 + 0.2 * vec2(randomness1, randomness2)));
    scene = max(scene, boxSDF(pos, vec3(randomizedHeight, 0, 0) + cell * boxProportion));
    return scene;
}
float getCellIndexX(inout vec3 p)
{
    p.xz = cart2polar(p.xz);
    float angleIncrement = 1. / radius;
    float angle = p.x + angleIncrement;
    float x = floor(angle / (2. * angleIncrement));
    if (abs(x) < HALF_PI * radius)
        x = abs(x);
    angle = mod(angle, 2. * angleIncrement) - angleIncrement;
    p.xz = polar2cart(vec2(angle, p.z));
    p.x -= radius;
    return x;
}
float cellSize = cell + thin;
float getCellIndexY(inout vec3 p)
{
    p.y += TIME * speed;
    float y = floor(p.y / cellSize);
    p.y = opRepeat(p.y - cellSize * 0.5, cellSize);
    return y;
}
vec2 getCellIndexes(inout vec3 p)
{
    return vec2(getCellIndexX(p), getCellIndexY(p));
}
float map(vec3 pos)
{
    vec3 cameraOffset = vec3(-4, 0, 0);
    // donut distortion
    vec3 pDonut = pos + cameraOffset;
    pDonut.xy += vec2(donut, radius);
    pDonut.xz = cart2polar(pDonut.xz);
    pDonut.x *= donut;
    pDonut.z -= donut;
    pDonut.zy *= rotate2d(-TIME * 0.05 * speed);
    pDonut.xyz = pDonut.zxy;
    // ground
    vec3 p = pDonut;
    float scene = min(1000., sphereSDF(vec3(p.x, 0, p.z), radius - height));
    // walls
    p = pDonut;
    getCellIndexY(p);
    scene = min(scene, max(abs(p.y) - thin, sphereSDF(vec3(p.x, 0, p.z), radius)));
    getCellIndexX(p);
    scene = min(scene, max(abs(p.z) - thin, p.x));
    // horizontal window
    p = pDonut;
    p.xz *= rotate2d(-1. / radius);
    vec2 indexes = getCellIndexes(p);
    float windowHeight = 0.75;
    p.x += windowHeight * 1.5;
    float windowWidth = 0.5;
    scene = max(scene, -boxSDF(p, vec3(windowHeight, thin + 0.01, windowWidth)));
    scene = min(scene, window(p.xzy, windowHeight, windowWidth, indexes));
    // vertical window
    p = pDonut;
    p.y += cell * 0.5;
    indexes = getCellIndexes(p);
    windowHeight = 0.75;
    p.x += windowHeight * 1.25;
    windowWidth = 1.5;
    scene = max(scene, -boxSDF(p, vec3(windowHeight, windowWidth, thin + 0.01)));
    scene = min(scene, window(p, windowHeight, windowWidth, indexes));
    // boxes
    p = pDonut;
    p.xz *= rotate2d(-1. / radius);
    p.y += cell * 0.5;
    indexes = getCellIndexes(p);
    p.x += height;
    scene = min(scene, boxes(p, indexes));
    return scene;
}
void main()
{
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / RENDERSIZE.y;
    vec3 eye = vec3(cameraX, cameraY, cameraZ);
    vec3 ray = normalize(vec3(uv, 1.3));
    camera(eye);
    camera(ray);
    float dither = random(uv + fract(TIME));
    vec3 pos = eye;
    float shade = 0.;
    bool isTorus = false;
    for (float i = 0.; i <= 1.; i += 1. / STEPS) {
        float dist = map(pos);
        if (dist < VOLUME) {
            shade = 1. - i;
            isTorus = true;
            break;
        }
        dist *= 0.5 + 0.1 * dither;
        pos += ray * dist;
    }
    if (isTorus) {
        vec3 light = vec3(40, 100, -10);
        float shadow = getShadow(pos, light, 4.);
        gl_FragColor.rgb = vec3(sqrt(smoothstep(0., 0.5, shade * shadow)));
        gl_FragColor.a = 1.;
    } else {
        gl_FragColor = backgroundColor;
    }
}
