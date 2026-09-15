/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "Dave Hoskins <https://www.shadertoy.com/user/Dave_Hoskins>",
    "DESCRIPTION": "Mountains, converted from <https://www.shadertoy.com/view/4slGD4>",
    "INPUTS": [
        {
            "NAME": "cameraSpeed",
            "LABEL": "Camera speed",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MAX": 50,
            "MIN": 0
        },
        {
            "NAME": "cameraRollAmplitude",
            "LABEL": "Camera roll amplitude",
            "TYPE": "float",
            "DEFAULT": 0.15,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "mountainHeight",
            "LABEL": "Mountain height",
            "TYPE": "float",
            "DEFAULT": 0.75,
            "MAX": 2,
            "MIN": 0
        },
        {
            "NAME": "mountainSize",
            "LABEL": "Mountain size",
            "TYPE": "float",
            "DEFAULT": 0.15,
            "MAX": 2,
            "MIN": 0
        },
        {
            "NAME": "terrain",
            "LABEL": "Terrain",
            "TYPE": "float",
            "DEFAULT": 0.25,
            "MAX": 2,
            "MIN": 0
        },
        {
            "NAME": "perturbationAngle",
            "LABEL": "Perturbation angle",
            "TYPE": "float",
            "DEFAULT": 52.14991,
            "MAX": 360,
            "MIN": -360
        },
        {
            "NAME": "perturbationScale",
            "LABEL": "Perturbation scale",
            "TYPE": "float",
            "DEFAULT": 2.2201844917,
            "MAX": 2.5,
            "MIN": -2.5
        },
        {
            "NAME": "perturbationShift",
            "LABEL": "Perturbation shift",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": -10
        },
        {
            "NAME": "flatness",
            "LABEL": "Flatness",
            "TYPE": "float",
            "DEFAULT": 5,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "detail",
            "LABEL": "Detail",
            "TYPE": "float",
            "DEFAULT": 66,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "cragginess",
            "LABEL": "Cragginess",
            "TYPE": "float",
            "DEFAULT": -0.4,
            "MAX": 1,
            "MIN": -1
        },
        {
            "NAME": "fogDistance",
            "LABEL": "Fog distance",
            "TYPE": "float",
            "DEFAULT": 5,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "cloudHeight",
            "LABEL": "Cloud height",
            "TYPE": "float",
            "DEFAULT": 200,
            "MAX": 1000,
            "MIN": 0
        },
        {
            "NAME": "cloudSpeed",
            "LABEL": "Cloud speed",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 100,
            "MIN": -100
        }
    ],
    "ISFVSN": "2",
    "IMPORTED":
    {
        "pebbles": { "PATH": "pebbles.png" }
    }
}*/
#define RANDOM_HIGHER_RANGE 
#define RANDOM_SINLESS 
#define FBM_NOISE_FNC(UV) gnoise(UV)
#define FBM_NOISE3_FNC(UV) snoise(UV)
#define FBM_SCALE_SCALAR 2.7
#define FBM_AMPLITUDE_INITIAL 0.7
float random_slow(vec2);
#define GNOISE_NOISE2_FNC(UV) random_slow(UV)
/*
contributors: [Stefan Gustavson, Ian McEwan]
description: modulus of 289
use: <float|vec2|vec3|vec4> mod289(<float|vec2|vec3|vec4> x)
*/
#define FNC_MOD289 
float mod289(const in float x) { return x - floor(x * (1. / 289.)) * 289.; }
vec2 mod289(const in vec2 x) { return x - floor(x * (1. / 289.)) * 289.; }
vec3 mod289(const in vec3 x) { return x - floor(x * (1. / 289.)) * 289.; }
vec4 mod289(const in vec4 x) { return x - floor(x * (1. / 289.)) * 289.; }
/*
contributors: [Stefan Gustavson, Ian McEwan]
description: permute
use: <float|vec2|vec3|vec4> permute(<float|vec2|vec3|vec4> x)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/math_functions.frag
*/
#define FNC_PERMUTE 
float permute(const in float v) { return mod289(((v * 34.0) + 1.0) * v); }
vec2 permute(const in vec2 v) { return mod289(((v * 34.0) + 1.0) * v); }
vec3 permute(const in vec3 v) { return mod289(((v * 34.0) + 1.0) * v); }
vec4 permute(const in vec4 v) { return mod289(((v * 34.0) + 1.0) * v); }
/*
contributors: [Stefan Gustavson, Ian McEwan]
description: Fast, accurate inverse square root. 
use: <float|vec2|vec3|vec4> taylorInvSqrt(<float|vec2|vec3|vec4> x)
*/
#define FNC_TAYLORINVSQRT 
float taylorInvSqrt(in float r) { return 1.79284291400159 - 0.85373472095314 * r; }
vec2 taylorInvSqrt(in vec2 r) { return 1.79284291400159 - 0.85373472095314 * r; }
vec3 taylorInvSqrt(in vec3 r) { return 1.79284291400159 - 0.85373472095314 * r; }
vec4 taylorInvSqrt(in vec4 r) { return 1.79284291400159 - 0.85373472095314 * r; }
/*
contributors: [Stefan Gustavson, Ian McEwan]
description: grad4, used for snoise(vec4 v)
use: grad4(<float> j, <vec4> ip)
*/
#define FNC_GRAD4 
vec4 grad4(float j, vec4 ip) {
    const vec4 ones = vec4(1.0, 1.0, 1.0, -1.0);
    vec4 p,s;
    p.xyz = floor( fract (vec3(j) * ip.xyz) * 7.0) * ip.z - 1.0;
    p.w = 1.5 - dot(abs(p.xyz), ones.xyz);
    s = vec4(lessThan(p, vec4(0.0)));
    p.xyz = p.xyz + (s.xyz*2.0 - 1.0) * s.www;
    return p;
}

/*
contributors: [Stefan Gustavson, Ian McEwan]
description: Simplex Noise https://github.com/stegu/webgl-noise
use: snoise(<vec2|vec3|vec4> pos)
license: |
    Copyright 2021-2023 by Stefan Gustavson and Ian McEwan.
    Published under the terms of the MIT license:
    https://opensource.org/license/mit/
examples:
    - /shaders/generative_snoise.frag
*/
#define FNC_SNOISE 
float snoise(in vec2 v) {
    const vec4 C = vec4(0.211324865405187, // (3.0-sqrt(3.0))/6.0
                        0.366025403784439, // 0.5*(sqrt(3.0)-1.0)
                        -0.577350269189626, // -1.0 + 2.0 * C.x
                        0.024390243902439); // 1.0 / 41.0
    // First corner
    vec2 i = floor(v + dot(v, C.yy) );
    vec2 x0 = v - i + dot(i, C.xx);
    // Other corners
    vec2 i1;
    //i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
    //i1.y = 1.0 - i1.x;
    i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
    // x0 = x0 - 0.0 + 0.0 * C.xx ;
    // x1 = x0 - i1 + 1.0 * C.xx ;
    // x2 = x0 - 1.0 + 2.0 * C.xx ;
    vec4 x12 = x0.xyxy + C.xxzz;
    x12.xy -= i1;
    // Permutations
    i = mod289(i); // Avoid truncation effects in permutation
    vec3 p = permute( permute( i.y + vec3(0.0, i1.y, 1.0 ))
    + i.x + vec3(0.0, i1.x, 1.0 ));
    vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
    m = m*m ;
    m = m*m ;
    // Gradients: 41 points uniformly over a line, mapped onto a diamond.
    // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
    vec3 x = 2.0 * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - 0.5;
    vec3 ox = floor(x + 0.5);
    vec3 a0 = x - ox;
    // Normalise gradients implicitly by scaling m
    // Approximation of: m *= inversesqrt( a0*a0 + h*h );
    m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
    // Compute final noise value at P
    vec3 g;
    g.x = a0.x * x0.x + h.x * x0.y;
    g.yz = a0.yz * x12.xz + h.yz * x12.yw;
    return 130.0 * dot(m, g);
}
float snoise(in vec3 v) {
    const vec2 C = vec2(1.0/6.0, 1.0/3.0) ;
    const vec4 D = vec4(0.0, 0.5, 1.0, 2.0);
    // First corner
    vec3 i = floor(v + dot(v, C.yyy) );
    vec3 x0 = v - i + dot(i, C.xxx) ;
    // Other corners
    vec3 g = step(x0.yzx, x0.xyz);
    vec3 l = 1.0 - g;
    vec3 i1 = min( g.xyz, l.zxy );
    vec3 i2 = max( g.xyz, l.zxy );
    //   x0 = x0 - 0.0 + 0.0 * C.xxx;
    //   x1 = x0 - i1  + 1.0 * C.xxx;
    //   x2 = x0 - i2  + 2.0 * C.xxx;
    //   x3 = x0 - 1.0 + 3.0 * C.xxx;
    vec3 x1 = x0 - i1 + C.xxx;
    vec3 x2 = x0 - i2 + C.yyy; // 2.0*C.x = 1/3 = C.y
    vec3 x3 = x0 - D.yyy; // -1.0+3.0*C.x = -0.5 = -D.y
    // Permutations
    i = mod289(i);
    vec4 p = permute( permute( permute(
                i.z + vec4(0.0, i1.z, i2.z, 1.0 ))
            + i.y + vec4(0.0, i1.y, i2.y, 1.0 ))
            + i.x + vec4(0.0, i1.x, i2.x, 1.0 ));
    // Gradients: 7x7 points over a square, mapped onto an octahedron.
    // The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
    float n_ = 0.142857142857; // 1.0/7.0
    vec3 ns = n_ * D.wyz - D.xzx;
    vec4 j = p - 49.0 * floor(p * ns.z * ns.z); //  mod(p,7*7)
    vec4 x_ = floor(j * ns.z);
    vec4 y_ = floor(j - 7.0 * x_ ); // mod(j,N)
    vec4 x = x_ *ns.x + ns.yyyy;
    vec4 y = y_ *ns.x + ns.yyyy;
    vec4 h = 1.0 - abs(x) - abs(y);
    vec4 b0 = vec4( x.xy, y.xy );
    vec4 b1 = vec4( x.zw, y.zw );
    //vec4 s0 = vec4(lessThan(b0,0.0))*2.0 - 1.0;
    //vec4 s1 = vec4(lessThan(b1,0.0))*2.0 - 1.0;
    vec4 s0 = floor(b0)*2.0 + 1.0;
    vec4 s1 = floor(b1)*2.0 + 1.0;
    vec4 sh = -step(h, vec4(0.0));
    vec4 a0 = b0.xzyw + s0.xzyw*sh.xxyy ;
    vec4 a1 = b1.xzyw + s1.xzyw*sh.zzww ;
    vec3 p0 = vec3(a0.xy,h.x);
    vec3 p1 = vec3(a0.zw,h.y);
    vec3 p2 = vec3(a1.xy,h.z);
    vec3 p3 = vec3(a1.zw,h.w);
    //Normalise gradients
    vec4 norm = taylorInvSqrt(vec4(dot(p0,p0), dot(p1,p1), dot(p2, p2), dot(p3,p3)));
    p0 *= norm.x;
    p1 *= norm.y;
    p2 *= norm.z;
    p3 *= norm.w;
    // Mix final noise value
    vec4 m = max(0.6 - vec4(dot(x0,x0), dot(x1,x1), dot(x2,x2), dot(x3,x3)), 0.0);
    m = m * m;
    return 42.0 * dot( m*m, vec4( dot(p0,x0), dot(p1,x1),
                                dot(p2,x2), dot(p3,x3) ) );
}
float snoise(in vec4 v) {
    const vec4 C = vec4( 0.138196601125011, // (5 - sqrt(5))/20  G4
                        0.276393202250021, // 2 * G4
                        0.414589803375032, // 3 * G4
                        -0.447213595499958); // -1 + 4 * G4
    // First corner
    vec4 i = floor(v + dot(v, vec4(.309016994374947451)) ); // (sqrt(5) - 1)/4
    vec4 x0 = v - i + dot(i, C.xxxx);
    // Other corners
    // Rank sorting originally contributed by Bill Licea-Kane, AMD (formerly ATI)
    vec4 i0;
    vec3 isX = step( x0.yzw, x0.xxx );
    vec3 isYZ = step( x0.zww, x0.yyz );
    //  i0.x = dot( isX, vec3( 1.0 ) );
    i0.x = isX.x + isX.y + isX.z;
    i0.yzw = 1.0 - isX;
    //  i0.y += dot( isYZ.xy, vec2( 1.0 ) );
    i0.y += isYZ.x + isYZ.y;
    i0.zw += 1.0 - isYZ.xy;
    i0.z += isYZ.z;
    i0.w += 1.0 - isYZ.z;
    // i0 now contains the unique values 0,1,2,3 in each channel
    vec4 i3 = clamp( i0, 0.0, 1.0 );
    vec4 i2 = clamp( i0-1.0, 0.0, 1.0 );
    vec4 i1 = clamp( i0-2.0, 0.0, 1.0 );
    //  x0 = x0 - 0.0 + 0.0 * C.xxxx
    //  x1 = x0 - i1  + 1.0 * C.xxxx
    //  x2 = x0 - i2  + 2.0 * C.xxxx
    //  x3 = x0 - i3  + 3.0 * C.xxxx
    //  x4 = x0 - 1.0 + 4.0 * C.xxxx
    vec4 x1 = x0 - i1 + C.xxxx;
    vec4 x2 = x0 - i2 + C.yyyy;
    vec4 x3 = x0 - i3 + C.zzzz;
    vec4 x4 = x0 + C.wwww;
    // Permutations
    i = mod289(i);
    float j0 = permute( permute( permute( permute(i.w) + i.z) + i.y) + i.x);
    vec4 j1 = permute( permute( permute( permute (
                i.w + vec4(i1.w, i2.w, i3.w, 1.0 ))
            + i.z + vec4(i1.z, i2.z, i3.z, 1.0 ))
            + i.y + vec4(i1.y, i2.y, i3.y, 1.0 ))
            + i.x + vec4(i1.x, i2.x, i3.x, 1.0 ));
    // Gradients: 7x7x6 points over a cube, mapped onto a 4-cross polytope
    // 7*7*6 = 294, which is close to the ring size 17*17 = 289.
    vec4 ip = vec4(1.0/294.0, 1.0/49.0, 1.0/7.0, 0.0) ;
    vec4 p0 = grad4(j0, ip);
    vec4 p1 = grad4(j1.x, ip);
    vec4 p2 = grad4(j1.y, ip);
    vec4 p3 = grad4(j1.z, ip);
    vec4 p4 = grad4(j1.w, ip);
    // Normalise gradients
    vec4 norm = taylorInvSqrt(vec4(dot(p0,p0), dot(p1,p1), dot(p2, p2), dot(p3,p3)));
    p0 *= norm.x;
    p1 *= norm.y;
    p2 *= norm.z;
    p3 *= norm.w;
    p4 *= taylorInvSqrt(dot(p4,p4));
    // Mix contributions from the five corners
    vec3 m0 = max(0.6 - vec3(dot(x0,x0), dot(x1,x1), dot(x2,x2)), 0.0);
    vec2 m1 = max(0.6 - vec2(dot(x3,x3), dot(x4,x4) ), 0.0);
    m0 = m0 * m0;
    m1 = m1 * m1;
    return 49.0 * ( dot(m0*m0, vec3( dot( p0, x0 ), dot( p1, x1 ), dot( p2, x2 )))
                + dot(m1*m1, vec2( dot( p3, x3 ), dot( p4, x4 ) ) ) ) ;
}
vec2 snoise2( vec2 x ){
    float s = snoise(vec2( x ));
    float s1 = snoise(vec2( x.y - 19.1, x.x + 47.2 ));
    return vec2( s , s1 );
}
vec3 snoise3( vec3 x ){
    float s = snoise(vec3( x ));
    float s1 = snoise(vec3( x.y - 19.1 , x.z + 33.4 , x.x + 47.2 ));
    float s2 = snoise(vec3( x.z + 74.2 , x.x - 124.5 , x.y + 99.4 ));
    return vec3( s , s1 , s2 );
}
vec3 snoise3( vec4 x ){
    float s = snoise(vec4( x ));
    float s1 = snoise(vec4( x.y - 19.1 , x.z + 33.4 , x.x + 47.2, x.w ));
    float s2 = snoise(vec4( x.z + 74.2 , x.x - 124.5 , x.y + 99.4, x.w ));
    return vec3( s , s1 , s2 );
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
description: Signed Random
use: srandomX(<vec2|vec3> x)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_SRANDOM 
float srandom(in float x) {
  return -1. + 2. * fract(sin(x) * 43758.5453);
}
float srandom(in vec2 st) {
  return -1. + 2. * fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453);
}
float srandom(in vec3 pos) {
  return -1. + 2. * fract(sin(dot(pos.xyz, vec3(70.9898, 78.233, 32.4355))) * 43758.5453123);
}
float srandom(in vec4 pos) {
    float dot_product = dot(pos, vec4(12.9898,78.233,45.164,94.673));
    return -1. + 2. * fract(sin(dot_product) * 43758.5453);
}
vec2 srandom2(in vec2 st) {
    const vec2 k = vec2(.3183099, .3678794);
    st = st * k + k.yx;
    return -1. + 2. * fract(16. * k * fract(st.x * st.y * (st.x + st.y)));
}
vec3 srandom3(in vec3 p) {
    p = vec3( dot(p, vec3(127.1, 311.7, 74.7)),
            dot(p, vec3(269.5, 183.3, 246.1)),
            dot(p, vec3(113.5, 271.9, 124.6)));
    return -1. + 2. * fract(sin(p) * 43758.5453123);
}
vec2 srandom2(in vec2 p, const in float tileLength) {
    p = mod(p, vec2(tileLength));
    return srandom2(p);
}
vec3 srandom3(in vec3 p, const in float tileLength) {
    p = mod(p, vec3(tileLength));
    return srandom3(p);
}
/*
contributors: Inigo Quiles
description: cubic polynomial https://iquilezles.org/articles/smoothsteps/
use: <float|vec2|vec3|vec4> cubic(<float|vec2|vec3|vec4> value[, <float> in, <float> out]);
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/math_functions.frag
*/
#define FNC_CUBIC 
float cubic(const in float v) { return v*v*(3.0-2.0*v); }
vec2 cubic(const in vec2 v) { return v*v*(3.0-2.0*v); }
vec3 cubic(const in vec3 v) { return v*v*(3.0-2.0*v); }
vec4 cubic(const in vec4 v) { return v*v*(3.0-2.0*v); }
float cubic(const in float v, in float slope0, in float slope1) {
    float a = slope0 + slope1 - 2.;
    float b = -2. * slope0 - slope1 + 3.;
    float c = slope0;
    float v2 = v * v;
    float v3 = v * v2;
    return a * v3 + b * v2 + c * v;
}
vec2 cubic(const in vec2 v, in float slope0, in float slope1) {
    float a = slope0 + slope1 - 2.;
    float b = -2. * slope0 - slope1 + 3.;
    float c = slope0;
    vec2 v2 = v * v;
    vec2 v3 = v * v2;
    return a * v3 + b * v2 + c * v;
}
vec3 cubic(const in vec3 v, in float slope0, in float slope1) {
    float a = slope0 + slope1 - 2.;
    float b = -2. * slope0 - slope1 + 3.;
    float c = slope0;
    vec3 v2 = v * v;
    vec3 v3 = v * v2;
    return a * v3 + b * v2 + c * v;
}
vec4 cubic(const in vec4 v, in float slope0, in float slope1) {
    float a = slope0 + slope1 - 2.;
    float b = -2. * slope0 - slope1 + 3.;
    float c = slope0;
    vec4 v2 = v * v;
    vec4 v3 = v * v2;
    return a * v3 + b * v2 + c * v;
}
/*
contributors: Inigo Quiles
description: quintic polynomial https://iquilezles.org/articles/smoothsteps/
use: <float|vec2|vec3|vec4> quintic(<float|vec2|vec3|vec4> value);
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/math_functions.frag
*/
#define FNC_QUINTIC 
float quintic(const in float v) { return v*v*v*(v*(v*6.0-15.0)+10.0); }
vec2 quintic(const in vec2 v) { return v*v*v*(v*(v*6.0-15.0)+10.0); }
vec3 quintic(const in vec3 v) { return v*v*v*(v*(v*6.0-15.0)+10.0); }
vec4 quintic(const in vec4 v) { return v*v*v*(v*(v*6.0-15.0)+10.0); }

/*
contributors: Patricio Gonzalez Vivo
description: Gradient Noise
use: gnoise(<float> x)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define GNOISE_NOISE_FNC(UV) random(UV)
#define GNOISE_NOISE3_FNC(UV) GNOISE_NOISE_FNC(UV)
#define GNOISE_NOISE_TILABLE_FNC(UV,TILE) srandom3(UV, TILE)
#define FNC_GNOISE 
float gnoise(float x) {
    float i = floor(x); // integer
    float f = fract(x); // fraction
    return mix(GNOISE_NOISE_FNC(i), GNOISE_NOISE_FNC(i + 1.0), smoothstep(0.,1.,f));
}
float gnoise(vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);
    float a = GNOISE_NOISE2_FNC(i);
    float b = GNOISE_NOISE2_FNC(i + vec2(1.0, 0.0));
    float c = GNOISE_NOISE2_FNC(i + vec2(0.0, 1.0));
    float d = GNOISE_NOISE2_FNC(i + vec2(1.0, 1.0));
    vec2 u = cubic(f);
    return mix( a, b, u.x) +
                (c - a)* u.y * (1.0 - u.x) +
                (d - b) * u.x * u.y;
}
float gnoise(vec3 p) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    vec3 u = quintic(f);
    return -1.0 + 2.0 * mix( mix( mix( GNOISE_NOISE3_FNC(i + vec3(0.0,0.0,0.0)),
                                        GNOISE_NOISE3_FNC(i + vec3(1.0,0.0,0.0)), u.x),
                                mix( GNOISE_NOISE3_FNC(i + vec3(0.0,1.0,0.0)),
                                        GNOISE_NOISE3_FNC(i + vec3(1.0,1.0,0.0)), u.x), u.y),
                            mix( mix( GNOISE_NOISE3_FNC(i + vec3(0.0,0.0,1.0)),
                                        GNOISE_NOISE3_FNC(i + vec3(1.0,0.0,1.0)), u.x),
                                mix( GNOISE_NOISE3_FNC(i + vec3(0.0,1.0,1.0)),
                                        GNOISE_NOISE3_FNC(i + vec3(1.0,1.0,1.0)), u.x), u.y), u.z );
}
float gnoise(vec3 p, float tileLength) {
    vec3 i = floor(p);
    vec3 f = fract(p);
    vec3 u = quintic(f);
    return mix( mix( mix( dot( GNOISE_NOISE_TILABLE_FNC(i + vec3(0.0,0.0,0.0), tileLength), f - vec3(0.0,0.0,0.0)),
                            dot( GNOISE_NOISE_TILABLE_FNC(i + vec3(1.0,0.0,0.0), tileLength), f - vec3(1.0,0.0,0.0)), u.x),
                    mix( dot( GNOISE_NOISE_TILABLE_FNC(i + vec3(0.0,1.0,0.0), tileLength), f - vec3(0.0,1.0,0.0)),
                            dot( GNOISE_NOISE_TILABLE_FNC(i + vec3(1.0,1.0,0.0), tileLength), f - vec3(1.0,1.0,0.0)), u.x), u.y),
                mix( mix( dot( GNOISE_NOISE_TILABLE_FNC(i + vec3(0.0,0.0,1.0), tileLength), f - vec3(0.0,0.0,1.0)),
                            dot( GNOISE_NOISE_TILABLE_FNC(i + vec3(1.0,0.0,1.0), tileLength), f - vec3(1.0,0.0,1.0)), u.x),
                    mix( dot( GNOISE_NOISE_TILABLE_FNC(i + vec3(0.0,1.0,1.0), tileLength), f - vec3(0.0,1.0,1.0)),
                            dot( GNOISE_NOISE_TILABLE_FNC(i + vec3(1.0,1.0,1.0), tileLength), f - vec3(1.0,1.0,1.0)), u.x), u.y), u.z );
}
vec3 gnoise3(vec3 x) {
    return vec3(gnoise(x+vec3(123.456, 0.567, 0.37)),
                gnoise(x+vec3(0.11, 47.43, 19.17)),
                gnoise(x) );
}
/*
contributors: Patricio Gonzalez Vivo
description: Fractal Brownian Motion
use: fbm(<vec2> pos)
options:
    FBM_OCTAVES: numbers of octaves. Default is 4.
    FBM_NOISE_FNC(UV): noise function to use Default 'snoise(UV)' (simplex noise)
    FBM_VALUE_INITIAL: initial value. Default is 0.
    FBM_SCALE_SCALAR: scalar. Default is 2.
    FBM_AMPLITUDE_INITIAL: initial amplitude value. Default is 0.5
    FBM_AMPLITUDE_SCALAR: amplitude scalar. Default is 0.5
examples:
    - /shaders/generative_fbm.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FBM_OCTAVES 4
#define FBM_NOISE2_FNC(UV) FBM_NOISE_FNC(UV)
#define FBM_NOISE_TILABLE_FNC(UV,TILE) gnoise(UV, TILE)
#define FBM_NOISE3_TILABLE_FNC(UV,TILE) FBM_NOISE_TILABLE_FNC(UV, TILE)
#define FBM_NOISE_TYPE float
#define FBM_VALUE_INITIAL 0.0
#define FBM_AMPLITUDE_SCALAR 0.5
#define FNC_FBM 
FBM_NOISE_TYPE fbm(in vec2 st) {
    // Initial values
    FBM_NOISE_TYPE value = FBM_NOISE_TYPE(FBM_VALUE_INITIAL);
    float amplitude = FBM_AMPLITUDE_INITIAL;
    // Loop of octaves
    for (int i = 0; i < FBM_OCTAVES; i++) {
        value += amplitude * FBM_NOISE2_FNC(st);
        st *= FBM_SCALE_SCALAR;
        amplitude *= FBM_AMPLITUDE_SCALAR;
    }
    return value;
}
FBM_NOISE_TYPE fbm(in vec3 pos) {
    // Initial values
    FBM_NOISE_TYPE value = FBM_NOISE_TYPE(FBM_VALUE_INITIAL);
    float amplitude = FBM_AMPLITUDE_INITIAL;
    // Loop of octaves
    for (int i = 0; i < FBM_OCTAVES; i++) {
        value += amplitude * FBM_NOISE3_FNC(pos);
        pos *= FBM_SCALE_SCALAR;
        amplitude *= FBM_AMPLITUDE_SCALAR;
    }
    return value;
}
FBM_NOISE_TYPE fbm(vec3 p, float tileLength) {
    const float persistence = 0.5;
    const float lacunarity = 2.0;
    float amplitude = 0.5;
    FBM_NOISE_TYPE total = FBM_NOISE_TYPE(0.0);
    float normalization = 0.0;
    for (int i = 0; i < FBM_OCTAVES; ++i) {
        float noiseValue = FBM_NOISE3_TILABLE_FNC(p, tileLength * lacunarity * 0.5) * 0.5 + 0.5;
        total += noiseValue * amplitude;
        normalization += amplitude;
        amplitude *= persistence;
        p = p * lacunarity;
    }
    return total / normalization;
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
/*
contributors: Patricio Gonzalez Vivo
description: Gradient Noise
use: gnoise(<float> x)
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
#define GNOISE2_NOISE2_FNC(UV) random2(UV)
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
float random_slow(in vec2 p) {
    return random2(vec3(p.x, RANDOM_SCALE.x * p.yx / RANDOM_SCALE.yz)).x;
}
// Stereo version code thanks to Croqueteer :)
//#define STEREO
float treeLine = 0.0;
float treeCol = 0.0;
vec3 sunLight = normalize(vec3(0.4, 0.4, 0.48));
vec3 sunColour = vec3(1, 0.9, 0.83);
float specular = 0.;
vec3 cameraPos;
float ambient;
float Trees(vec2 p)
{
    return gnoise(p * 13.) * treeLine;
}
// Low def version for ray-marching through the height field...
float Terrain(in vec2 p, out vec2 pos, out float w)
{
    // There's some real magic numbers in here!
    // The gnoise calls add large mountain ranges for more variation over distances...
    pos = p * 0.05;
    w = gnoise(pos * terrain) * mountainHeight + mountainSize;
    w = detail * w*w;
    // This peturbs the fractal positions for each iteration down...
    // Helps make nice twisted landscapes...
    mat2 rotate2D = perturbationScale * rotate2d(perturbationAngle * DEG2RAD) + perturbationShift * mat2(0., 0., 0.04, 0.1);
    float f = 0.;
    for (int i = 0; i < 5; i++) {
        f += gnoise(pos) * w;
        pos = rotate2D * pos; // Non-commutative matrix multiplication
        w *= cragginess;
    }
    f += pow(abs(gnoise(pos * 0.002)), flatness) * 275. - flatness;
    return f;
}
float Terrain(in vec2 p)
{
    vec2 pos;
    float w;
    return Terrain(p, pos, w);
}
// Map to lower resolution for height field mapping for Scene function...
float Map(in vec3 p)
{
    float h = Terrain(p.xz);
    float ff = gnoise(p.xz * 0.3) + gnoise(p.xz * 3.3) * 0.5;
    treeLine = smoothstep(ff, 0. + ff * 2., h) * smoothstep(1. + ff * 3., 0.4 + ff, h);
    treeCol = Trees(p.xz);
    h += treeCol;
    return p.y - h;
}
// High def version only used for grabbing normal information.
float Terrain2(in vec2 p)
{
    vec2 pos;
    float w;
    float f = Terrain(p, pos, w);
    treeCol = Trees(p);
    f += treeCol;
    if (treeCol > 0.)
        return f;
    mat2 rotate2D = perturbationScale * rotate2d(perturbationAngle * DEG2RAD) + perturbationShift * mat2(0., 0., 0.04, 0.1);
    // That's the last of the low resolution, now go down further for the Normal data...
    for (int i = 0; i < 6; i++) {
        f += gnoise(pos) * w;
        pos = rotate2D * pos; // Non-commutative matrix multiplication
        w *= cragginess;
    }
    return f;
}
// Simply Perlin clouds that fade to the horizon...
vec3 GetClouds(in vec3 sky, in vec3 rd)
{
    if (rd.y < 0.01)
        return sky;
    float v = (cloudHeight - cameraPos.y) / rd.y;
    rd.xz *= v;
    rd.xz += cameraPos.xz;
    rd.xz *= 0.01;
    float f = (fbm(rd.xz + cloudSpeed * TIME) - 0.55) * 5.;
    // Uses the ray's y component for horizon fade of fixed colour clouds...
    sky = mix(sky, vec3(0.55, 0.55, 0.52), clamp(f * rd.y - 0.1, 0., 1.));
    return sky;
}
// Grab all sky information for a given ray from camera
vec3 GetSky(in vec3 rd)
{
    float sunAmount = max(dot(rd, sunLight), 0.);
    float v = pow(1. - max(rd.y, 0.), 5.) * 0.5;
    vec3 sky = v * sunColour * 0.4 + vec3(0.11, 0.22, 0.5);
    // Wide glare effect...
    sky += sunColour * pow(sunAmount, 6.5) * 0.32;
    // Actual sun...
    sky += sunColour * min(pow(sunAmount, 1150.), 0.3) * 0.65;
    return sky;
}
// Merge mountains into the sky background for correct disappearance...
vec3 ApplyFog(in vec3 rgb, in float distance, in vec3 dir)
{
    float fogAmount = exp(-distance * fogDistance * 1e-5);
    return mix(GetSky(dir), rgb, fogAmount);
}
// Calculate sun light...
void DoLighting(inout vec3 mat, in vec3 pos, in vec3 normal, in vec3 eyeDir, in float dis)
{
    float h = dot(sunLight, normal);
    float c = max(h, 0.) + ambient;
    mat = mat * sunColour * c;
    // Specular...
    if (h > 0.) {
        vec3 R = reflect(sunLight, normal);
        float specAmount = pow(max(dot(R, normalize(eyeDir)), 0.), 3.) * specular;
        mat = mix(mat, sunColour, specAmount);
    }
}
// Hack the height, position, and normal data to create the coloured landscape
vec3 TerrainColour(vec3 pos, vec3 normal, float distance)
{
    specular = 0.;
    ambient = 0.1;
    vec3 dir = normalize(pos - cameraPos);
    // I had to change scale halfway though, this lazy multiply allows me to
    // keep the graphic scales I had.
    vec3 matPos = pos * 2.;
    float f = clamp(gnoise(matPos.xz * 0.05), 0., 1.);
    f += gnoise(matPos.xz * 0.1 + normal.yz * 1.08) * 0.85;
    f *= 0.55;
    vec3 m = mix(
        vec3(0.63 * f + 0.2, 0.7 * f + 0.1, 0.7 * f + 0.1),
        vec3(f * 0.43 + 0.1, f * 0.3 + 0.2, f * 0.35 + 0.1),
        f * 0.65
    );
    vec3 mat = m * (f * m + vec3(0.36, 0.3, 0.28));
    // Should have used smoothstep to add colours, but left it using 'if' for sanity...
    if (normal.y < 0.5) {
        float v = normal.y;
        float c = (0.5 - normal.y) * 4.;
        c = clamp(c*c, 0.1, 1.);
        f = gnoise(vec2(matPos.x * 0.09, matPos.z * 0.095 + matPos.yy * 0.15));
        f += gnoise(vec2(matPos.x * 2.233, matPos.z * 2.23)) * 0.5;
        mat = mix(mat, vec3(0.4 * f), c);
        specular += 0.1;
    }
    // Grass. Use the normal to decide when to plonk grass down...
    if (matPos.y < 45.35 && normal.y > 0.65) {
        m = vec3(gnoise(matPos.xz * 0.023) * 0.5 + 0.15, gnoise(matPos.xz * 0.03) * 0.6 + 0.25, 0);
        m *= (normal.y - 0.65) * 0.6;
        mat = mix(mat, m, clamp((normal.y - 0.65) * 1.3 * (45.35 - matPos.y) * 0.1, 0., 1.));
    }
    if (treeCol > 0.) {
        mat = vec3(0.02 + gnoise(matPos.xz * 5.) * 0.03, 0.05, 0);
        normal = normalize(normal + vec3(gnoise(matPos.xz * 33.) * 1. - 0.5, 0, gnoise(matPos.xz * 33.) * 1. - 0.5));
        specular = 0.;
    }
    // Snow topped mountains...
    if (matPos.y > 80. && normal.y > 0.42) {
        float snow = clamp((matPos.y - 80. - gnoise(matPos.xz * 0.1) * 28.) * 0.035, 0., 1.);
        mat = mix(mat, vec3(0.7, 0.7, 0.8), snow);
        specular += snow;
        ambient += snow * 0.3;
    }
    // Beach effect...
    if (matPos.y < 1.45) {
        if (normal.y > 0.4) {
            f = gnoise(matPos.xz * 0.084) * 1.5;
            f = clamp((1.45 - f - matPos.y) * 1.34, 0., 0.67);
            float t = normal.y - 0.4;
            t = t*t;
            mat = mix(mat, t + vec3(0.09, 0.07, 0.03), f);
        }
        // Cheap under water darkening...it's wet after all...
        if (matPos.y < 0.) {
            mat *= 0.2;
        }
    }
    float distanceSquared = distance * distance; // Squaring it gives better distance scales.
    DoLighting(mat, pos, normal, dir, distanceSquared);
    // Do the water...
    if (matPos.y < 0.) {
        // Pull back along the ray direction to get water surface point at y = 0.0 ...
        float time = TIME * 0.03;
        vec3 watPos = matPos;
        watPos += -dir * (watPos.y / dir.y);
        // Make some dodgy waves...
        float tx = cos(watPos.x * 0.052) * 4.5;
        float tz = sin(watPos.z * 0.072) * 4.5;
        vec2 co = gnoise2(vec2(watPos.x * 4.7 + 1.3 + tz, watPos.z * 4.69 + time * 35. - tx));
        co += gnoise2(vec2(watPos.z * 8.6 + time * 13. - tx, watPos.x * 8.712 + tz)) * 0.4;
        vec3 nor = normalize(vec3(co.x, 20., co.y));
        nor = normalize(reflect(dir, nor));
        // Mix it in at depth transparancy to give beach cues..
        tx = watPos.y - matPos.y;
        mat = mix(mat, GetClouds(GetSky(nor) * vec3(0.3, 0.3, 0.5), nor) * 0.1 + vec3(0.0, 0.02, 0.03), clamp((tx) * 0.4, 0.6, 1.));
        // Add some extra water glint...
        mat += vec3(0.1) * clamp(1. - pow(tx + 0.5, 3.) * IMG_NORM_PIXEL(pebbles, watPos.xz * 0.1).x, 0., 1.);
        float sunAmount = max(dot(nor, sunLight), 0.);
        mat = mat + sunColour * pow(sunAmount, 228.5) * 0.6;
        vec3 temp = (watPos - cameraPos * 2.) * 0.5;
        distanceSquared = dot(temp, temp);
    }
    mat = ApplyFog(mat, distanceSquared, dir);
    return mat;
}
float BinarySubdivision(in vec3 rO, in vec3 rD, vec2 t)
{
    // Home in on the surface by dividing by two and split...
    float halfwayT;
    for (int i = 0; i < 5; i++) {
        halfwayT = dot(t, vec2(0.5));
        float d = Map(rO + halfwayT * rD);
        t = mix(vec2(t.x, halfwayT), vec2(halfwayT, t.y), step(0.5, d));
    }
    return halfwayT;
}
bool Scene(in vec3 rO, in vec3 rD, out float resT, in vec2 fragCoord)
{
    float t = 1. + random_slow(fragCoord.xy) * 1.;
    float oldT = 0.;
    float delta = 0.;
    bool fin = false;
    bool res = false;
    vec2 distances;
    for(int j = 0; j < 150; j++) {
        if (fin || t > 240.)
            break;
        vec3 p = rO + t * rD;
        float h = Map(p); // ...Get this positions height mapping.
        // Are we inside, and close enough to fudge a hit?...
        if (h < 0.5) {
            fin = true;
            distances = vec2(oldT, t);
            break;
        }
        // Delta ray advance - a fudge between the height returned
        // and the distance already travelled.
        // It's a really fiddly compromise between speed and accuracy
        // Too large a step and the tops of ridges get missed.
        delta = max(0.01, 0.3 * h) + t * 0.0065;
        oldT = t;
        t += delta;
    }
    if (fin)
        resT = BinarySubdivision(rO, rD, distances);
    return fin;
}
vec3 CameraPath(float t)
{
    t += (TIME * cameraSpeed + 658.) * 0.006;
    vec2 p = 476. * vec2(sin(3.5 * t), cos(1.5 * t));
    return vec3(35. - p.x, 0.6, 4108. + p.y);
}
// Some would say, most of the magic is done in post! :D
vec3 PostEffects(vec3 rgb, vec2 uv)
{
    return (1. - exp(-rgb * 6.)) * 1.0024;
}
void main()
{
    vec2 xy = -1. + 2. * gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 uv = xy * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.);
    // Use several forward heights, of decreasing influence with distance from the camera.
    float h = 0.;
    float f = 1.;
    for (int i = 0; i < 7; i++) {
        h += Terrain(CameraPath((0.6 - f) * 0.008).xz) * f;
        f -= 0.1;
    }
    cameraPos.xz = CameraPath(0.).xz;
    cameraPos.y = max(h * 0.25 + 3.5, 1.5 + sin(TIME * 5.) * 0.5);
    vec3 cameraTarget;
    cameraTarget.xz = CameraPath(0.1).xz;
    cameraTarget.y = cameraPos.y - smoothstep(60., 300., cameraPos.y) * 150.;
    float roll = cameraRollAmplitude * sin(TIME * 0.2);
    vec3 cw = normalize(cameraTarget - cameraPos);
    vec3 cp = vec3(polar2cart(-vec2(roll + 0.5 * PI, 1.)), 0);
    vec3 cu = normalize(cross(cw, cp));
    vec3 cv = normalize(cross(cu, cw));
    vec3 rd = normalize(uv.x * cu + uv.y * cv + 1.5 * cw);
    vec3 col;
    float distance;
    if (!Scene(cameraPos, rd, distance, gl_FragCoord.xy)) {
        // Missed scene, now just get the sky value...
        col = GetSky(rd);
        col = GetClouds(col, rd);
    } else {
        // Get world coordinate of landscape...
        vec3 pos = cameraPos + distance * rd;
        // Get normal from sampling the high definition height map
        // Use the distance to sample larger gaps to help stop aliasing...
        float p = 0.02 + 0.00005 * distance*distance;
        vec3 nor = vec3(0, Terrain2(pos.xz), 0);
        vec3 v2 = nor - vec3(p, Terrain2(pos.xz + vec2(p, 0)), 0);
        vec3 v3 = nor - vec3(0, Terrain2(pos.xz + vec2(0, -p)), -p);
        nor = cross(v2, v3);
        nor = normalize(nor);
        // Get the colour using all available data...
        col = TerrainColour(pos, nor, distance);
    }
    col = PostEffects(col, uv);
    gl_FragColor = vec4(col, 1.);
}
