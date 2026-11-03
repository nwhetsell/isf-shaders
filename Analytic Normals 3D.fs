/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "Inigo Quilez <https://iquilezles.org>",
    "DESCRIPTION": "Analytical normals of Value Noise, converted from <https://www.shadertoy.com/view/XttSz2>",
    "INPUTS": [
        {
            "NAME": "initialRotation",
            "LABEL": "Initial rotation (degrees)",
            "TYPE": "float",
            "DEFAULT": 0,
            "MIN": -180,
            "MAX": 180
        },
        {
            "NAME": "rotationSpeed",
            "LABEL": "Rotation speed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "distance",
            "LABEL": "Distance",
            "TYPE": "float",
            "DEFAULT": 3,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "mainColor",
            "LABEL": "Main color",
            "TYPE": "color",
            "DEFAULT": [1, 0.95, 0.85, 1]
        },
        {
            "NAME": "highlightColor",
            "LABEL": "Highlight color",
            "TYPE": "color",
            "DEFAULT": [0.05, 0.2, 0.3, 1]
        },
        {
            "NAME": "specularAmount",
            "LABEL": "Specular amount",
            "TYPE": "float",
            "DEFAULT": 10,
            "MIN": 0,
            "MAX": 100
        },
        {
            "NAME": "cubeSize",
            "LABEL": "Cube size",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "noiseShift",
            "LABEL": "Noise shift",
            "TYPE": "float",
            "DEFAULT": 0,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "initialFrequency",
            "LABEL": "Initial frequency",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "noiseOffset",
            "LABEL": "Noise offset",
            "TYPE": "float",
            "DEFAULT": 0.37,
            "MIN": -10,
            "MAX": 10
        },
        {
            "NAME": "approximateNormals",
            "LABEL": "Approximate normals",
            "TYPE": "bool",
            "DEFAULT": false
        },
        {
            "NAME": "aoIntensity",
            "LABEL": "Ambient occlusion intensity",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MIN": 0,
            "MAX": 1
        }
    ],
    "ISFVSN": "2"
}*/
#define STRICT_RANDOM 
int noisedRandomCount = 0;
#define NOISED_QUINTIC_INTERPOLATION 0
float noisedRandom(vec3 pos);
#define NOISED_RANDOM_FNC noisedRandom
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
contributors: Inigo Quilez
description: Returns 2D/3D gradient or value noise in the first channel and in the rest the derivatives. For more details, see https://iquilezles.org/articles/morenoise/ and https://iquilezles.org/articles/gradientnoise/
use: noised(<vec2|vec3> space)
options:
    NOISED_QUINTIC_INTERPOLATION: Quintic interpolation on/off. Default is on.
examples:
    - /shaders/generative_noised.frag
*/
#define NOISED_RANDOM2_FNC srandom2
#define NOISED_RANDOM3_FNC srandom3
#define FNC_NOISED 
// return gradient noise (in x) and its derivatives (in yz)
vec3 noised (in vec2 p) {
    // grid
    vec2 i = floor( p );
    vec2 f = fract( p );
    vec2 u = f * f * (3. - 2. * f);
    vec2 du = 6. * f * (1. - f);
    vec2 ga = NOISED_RANDOM2_FNC(i + vec2(0., 0.));
    vec2 gb = NOISED_RANDOM2_FNC(i + vec2(1., 0.));
    vec2 gc = NOISED_RANDOM2_FNC(i + vec2(0., 1.));
    vec2 gd = NOISED_RANDOM2_FNC(i + vec2(1., 1.));
    float va = dot(ga, f - vec2(0., 0.));
    float vb = dot(gb, f - vec2(1., 0.));
    float vc = dot(gc, f - vec2(0., 1.));
    float vd = dot(gd, f - vec2(1., 1.));
    return vec3( va + u.x*(vb-va) + u.y*(vc-va) + u.x*u.y*(va-vb-vc+vd), // value
                ga + u.x*(gb-ga) + u.y*(gc-ga) + u.x*u.y*(ga-gb-gc+gd) + // derivatives
                du * (u.yx*(va-vb-vc+vd) + vec2(vb,vc) - va));
}
vec4 noised (in vec3 pos) {
    // grid
    vec3 p = floor(pos);
    vec3 w = fract(pos);
    vec3 u = w * w * (3. - 2. * w);
    vec3 du = 6. * w * (1. - w);
    // gradients
    vec3 ga = NOISED_RANDOM3_FNC(p + vec3(0., 0., 0.));
    vec3 gb = NOISED_RANDOM3_FNC(p + vec3(1., 0., 0.));
    vec3 gc = NOISED_RANDOM3_FNC(p + vec3(0., 1., 0.));
    vec3 gd = NOISED_RANDOM3_FNC(p + vec3(1., 1., 0.));
    vec3 ge = NOISED_RANDOM3_FNC(p + vec3(0., 0., 1.));
    vec3 gf = NOISED_RANDOM3_FNC(p + vec3(1., 0., 1.));
    vec3 gg = NOISED_RANDOM3_FNC(p + vec3(0., 1., 1.));
    vec3 gh = NOISED_RANDOM3_FNC(p + vec3(1., 1., 1.));
    // projections
    float va = dot(ga, w - vec3(0., 0., 0.));
    float vb = dot(gb, w - vec3(1., 0., 0.));
    float vc = dot(gc, w - vec3(0., 1., 0.));
    float vd = dot(gd, w - vec3(1., 1., 0.));
    float ve = dot(ge, w - vec3(0., 0., 1.));
    float vf = dot(gf, w - vec3(1., 0., 1.));
    float vg = dot(gg, w - vec3(0., 1., 1.));
    float vh = dot(gh, w - vec3(1., 1., 1.));
    // interpolations
    return vec4( va + u.x*(vb-va) + u.y*(vc-va) + u.z*(ve-va) + u.x*u.y*(va-vb-vc+vd) + u.y*u.z*(va-vc-ve+vg) + u.z*u.x*(va-vb-ve+vf) + (-va+vb+vc-vd+ve-vf-vg+vh)*u.x*u.y*u.z, // value
                ga + u.x*(gb-ga) + u.y*(gc-ga) + u.z*(ge-ga) + u.x*u.y*(ga-gb-gc+gd) + u.y*u.z*(ga-gc-ge+gg) + u.z*u.x*(ga-gb-ge+gf) + (-ga+gb+gc-gd+ge-gf-gg+gh)*u.x*u.y*u.z + // derivatives
                du * (vec3(vb,vc,ve) - va + u.yzx*vec3(va-vb-vc+vd,va-vc-ve+vg,va-vb-ve+vf) + u.zxy*vec3(va-vb-ve+vf,va-vb-vc+vd,va-vc-ve+vg) + u.yzx*u.zxy*(-va+vb+vc-vd+ve-vf-vg+vh) ));
}
// return value noise (in x) and its derivatives (in yzw)
vec4 vnoised( in vec3 x )
{
    vec3 p = floor(x);
    vec3 w = fract(x);
    vec3 u = w * w * (3. - 2. * w);
    vec3 du = 6. * w * (1. - w);
    float a = NOISED_RANDOM_FNC(p + vec3(0., 0., 0.));
    float b = NOISED_RANDOM_FNC(p + vec3(1., 0., 0.));
    float c = NOISED_RANDOM_FNC(p + vec3(0., 1., 0.));
    float d = NOISED_RANDOM_FNC(p + vec3(1., 1., 0.));
    float e = NOISED_RANDOM_FNC(p + vec3(0., 0., 1.));
    float f = NOISED_RANDOM_FNC(p + vec3(1., 0., 1.));
    float g = NOISED_RANDOM_FNC(p + vec3(0., 1., 1.));
    float h = NOISED_RANDOM_FNC(p + vec3(1., 1., 1.));
    float k0 = a;
    float k1 = b - a;
    float k2 = c - a;
    float k3 = e - a;
    float k4 = a - b - c + d;
    float k5 = a - c - e + g;
    float k6 = a - b - e + f;
    float k7 = - a + b + c - d + e - f - g + h;
    return vec4( k0 + k1*u.x + k2*u.y + k3*u.z + k4*u.x*u.y + k5*u.y*u.z + k6*u.z*u.x + k7*u.x*u.y*u.z,
                 du * (vec3(k1,k2,k3) + u.yzx*vec3(k4,k5,k6) + u.zxy*vec3(k6,k4,k5) + k7*u.yzx*u.zxy ));
}
#define FBM_OCTAVES 8
#define FBM_NOISE3_FNC(UV) vec4((n = vnoised(UV * frequency)).x, vec3(0)); value.yzw += amplitude * n.yzw * frequency; frequency *= 1.8
#define FBM_NOISE_TYPE vec4
#define FBM_SCALE_SCALAR 1.
#define FBM_AMPLITUDE_INITIAL 0.5; float frequency = initialFrequency; vec4 n
#define FBM_AMPLITUDE_SCALAR 0.5
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
#define RANDOM_SCALE vec4(443.897, 441.423, .0973, .1099)
#define FNC_RANDOM 
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
#define GNOISE_NOISE2_FNC(UV) GNOISE_NOISE_FNC(UV)
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
#define FBM_NOISE_FNC(UV) snoise(UV)
#define FBM_NOISE2_FNC(UV) FBM_NOISE_FNC(UV)
#define FBM_NOISE_TILABLE_FNC(UV,TILE) gnoise(UV, TILE)
#define FBM_NOISE3_TILABLE_FNC(UV,TILE) FBM_NOISE_TILABLE_FNC(UV, TILE)
#define FBM_VALUE_INITIAL 0.0
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
contributors: Patricio Gonzalez Vivo
description: This file contains the definition of the AABB struct
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define STR_AABB 
struct AABB {
    vec3 min;
    vec3 max;
};

/*
contributors: Patricio Gonzalez Vivo
description: Generic Ray Structure
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define STR_RAY 
struct Ray {
    vec3 origin;
    vec3 direction;
};

/*
contributors: Dominik Schmid 
description: |
    Compute the near and far intersections of the cube (stored in the x and y components) using the slab method
    no intersection means vec.x > vec.y (really tNear > tFar) https://gist.github.com/DomNomNom/46bb1ce47f68d255fd5d
use: <vec2> intersect(<AABB> box, <vec3> rayOrigin, <vec3> rayDir)
*/
#define FNC_AABB_INTERSECT 
vec2 intersect(const in AABB box, const in vec3 rayOrigin, const in vec3 rayDir) {
    vec3 tMin = (box.min - rayOrigin) / rayDir;
    vec3 tMax = (box.max - rayOrigin) / rayDir;
    vec3 t1 = min(tMin, tMax);
    vec3 t2 = max(tMin, tMax);
    float tNear = max(max(t1.x, t1.y), t1.z);
    float tFar = min(min(t2.x, t2.y), t2.z);
    return vec2(tNear, tFar);
}
vec2 intersect(const in AABB box, const in Ray ray) {
    vec3 tMin = (box.min - ray.origin) / ray.direction;
    vec3 tMax = (box.max - ray.origin) / ray.direction;
    vec3 t1 = min(tMin, tMax);
    vec3 t2 = max(tMin, tMax);
    float tNear = max(max(t1.x, t1.y), t1.z);
    float tFar = min(min(t2.x, t2.y), t2.z);
    return vec2(tNear, tFar);
}
#define RAYMARCH_AO_SAMPLES 32
#define RAYMARCH_AO_MIN_DIST 0.001
#define RAYMARCH_AO_MAX_DIST 0.25
#define RAYMARCH_AO_INTENSITY aoIntensity
#define STRICT_RAYMARCH_AO 
#define RAYMARCH_SAMPLES 128
vec2 bounds;
#define RAYMARCH_MIN_DIST bounds.x
#define RAYMARCH_MAX_DIST bounds.y;
#define RAYMARCH_MIN_HIT_DIST 0.001
#define RAYMARCH_BACKGROUND 
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define GAMMA 2.2
#define FNC_GAMMA2LINEAR 
float gamma2linear(const in float v) {
    return pow(v, GAMMA);
}
vec3 gamma2linear(const in vec3 v) {
    return pow(v, vec3(GAMMA));
}
vec4 gamma2linear(const in vec4 v) {
    return vec4(gamma2linear(v.rgb), v.a);
}
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define SAMPLER_FNC(TEX,UV) texture(TEX, UV)
#define SAMPLER_TYPE sampler2D

/*
contributors: Patricio Gonzalez Vivo
description: Get material BaseColor from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialAlbedo()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_MATERIAL_ALBEDO 
vec4 materialAlbedo() {
    vec4 albedo = vec4(0.5, 0.5, 0.5, 1.0);
    return albedo;
}
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material specular property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
    - MATERIAL_SPECULARMAP
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_MATERIAL_SPECULAR 
vec3 materialSpecular() {
    vec3 spec = vec3(0.04);
    return spec;
}
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material emissive property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialEmissive()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_MATERIAL_EMISSIVE 
vec3 materialEmissive() {
    vec3 emission = vec3(0.0);
    return emission;
}
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material normal property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialOcclusion()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_MATERIAL_OCCLUSION 
float materialOcclusion() {
    float occlusion = 1.0;
    return occlusion;
}

/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material normal property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialNormal()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_MATERIAL_NORMAL 
vec3 materialNormal() {
    vec3 normal = vec3(0.0, 0.0, 1.0);
    return normal;
}

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
contributors: Patricio Gonzalez Vivo
description: Convert diffuse/specular/glossiness workflow to PBR metallic factor
use: <float> toMetallic(<vec3> diffuse, <vec3> specular, <float> maxSpecular)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define TOMETALLIC_MIN_REFLECTANCE 0.04
#define FNC_TOMETALLIC 
float toMetallic(const in vec3 diffuse, const in vec3 specular, const in float maxSpecular) {
    float perceivedDiffuse = sqrt(0.299 * diffuse.r * diffuse.r + 0.587 * diffuse.g * diffuse.g + 0.114 * diffuse.b * diffuse.b);
    float perceivedSpecular = sqrt(0.299 * specular.r * specular.r + 0.587 * specular.g * specular.g + 0.114 * specular.b * specular.b);
    if (perceivedSpecular < TOMETALLIC_MIN_REFLECTANCE) {
        return 0.0;
    }
    float a = TOMETALLIC_MIN_REFLECTANCE;
    float b = perceivedDiffuse * (1.0 - maxSpecular) / (1.0 - TOMETALLIC_MIN_REFLECTANCE) + perceivedSpecular - 2.0 * TOMETALLIC_MIN_REFLECTANCE;
    float c = TOMETALLIC_MIN_REFLECTANCE - perceivedSpecular;
    float D = max(b * b - 4.0 * a * c, 0.0);
    return saturate((-b + sqrt(D)) / (2.0 * a));
}
float toMetallic(const in vec3 diffuse, const in vec3 specular) {
    float maxSpecula = max(max(specular.r, specular.g), specular.b);
    return toMetallic(diffuse, specular, maxSpecula);
}
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material BaseColor from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialAlbedo()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material specular property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
    - MATERIAL_SPECULARMAP
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material metallic property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_MATERIAL_METALLIC 
float materialMetallic() {
    float metallic = 0.0;
    vec3 diffuse = materialAlbedo().rgb;
    vec3 specular = materialSpecular();
    metallic = toMetallic(diffuse, specular);
    return metallic;
}
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material roughness property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialRoughness()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_MATERIAL_ROUGHNESS 
float materialRoughness() {
    float roughness = 0.05;
    return roughness;
}

/*
contributors: Patricio Gonzalez Vivo
description: Convertes from PBR roughness/metallic to a shininess factor (typaclly use on diffuse/specular/ambient workflow)
use: float toShininess(<float> roughness, <float> metallic)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_TOSHININESS 
float toShininess(const in float roughness, const in float metallic) {
    float s = .95 - roughness * 0.5;
    s *= s;
    s *= s;
    return s * (80.0 + 160.0 * (1.0-metallic));
}

/*
contributors: Patricio Gonzalez Vivo
description: Get material shininess property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialShininess()
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_MATERIAL_SHININESS 
float materialShininess() {
    float shininess = 15.0;
    return shininess;
}

/*
contributors: Patricio Gonzalez Vivo
description: Generic Material Structure
options:
    - SCENE_BACK_SURFACE
    - SHADING_MODEL_CLEAR_COAT
    - MATERIAL_HAS_CLEAR_COAT_NORMAL
    - SHADING_MODEL_IRIDESCENCE
    - SHADING_MODEL_SUBSURFACE
    - SHADING_MODEL_CLOTH
    - SHADING_MODEL_SPECULAR_GLOSSINESS
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define RENDER_RAYMARCHING 
#define SHADING_MODEL_CLEAR_COAT 
#define STR_MATERIAL 
struct Material {
    vec4 albedo;
    vec3 emissive;
    vec3 position; // world position of the surface
    vec3 normal; // world normal of the surface
    float sdf;
    bool valid;
    vec3 ior; // Index of Refraction
    float roughness;
    float metallic;
    float reflectance;
    float ambientOcclusion; // default 1.0
    float clearCoat;
    float clearCoatRoughness;
};
/*
contributors: Shadi El Hajj
description: Medium Structure
license: MIT License (MIT) Copyright (c) 2024 Shadi EL Hajj
*/
#define STR_MEDIUM 
struct Medium {
    vec3 scattering;
    vec3 absorption;
    float sdf;
};

/*
contributors:  Inigo Quiles
description: Map of SDF functions to be declare
use: <vec4> raymarchMap( in <vec3> pos ) 
examples:
    - /shaders/lighting_raymarching.frag
*/
#define RAYMARCH_MAP_FNC raymarchMap
#define RAYMARCH_VOLUME_MAP_FNC raymarchVolumeMap
#define FNC_RAYMARCH_MAP 
Material RAYMARCH_MAP_FNC( in vec3 pos );
Medium RAYMARCH_VOLUME_MAP_FNC( in vec3 pos );
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material BaseColor from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialAlbedo()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material specular property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
    - MATERIAL_SPECULARMAP
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material emissive property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialEmissive()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material normal property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialOcclusion()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material normal property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialNormal()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
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
/*
contributors: Patricio Gonzalez Vivo
description: Convert diffuse/specular/glossiness workflow to PBR metallic factor
use: <float> toMetallic(<vec3> diffuse, <vec3> specular, <float> maxSpecular)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material BaseColor from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialAlbedo()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material specular property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
    - MATERIAL_SPECULARMAP
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material metallic property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material roughness property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialRoughness()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: Get material shininess property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialShininess()
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material BaseColor from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialAlbedo()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material specular property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
    - MATERIAL_SPECULARMAP
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material emissive property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialEmissive()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material normal property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialOcclusion()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material normal property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialNormal()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

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
/*
contributors: Patricio Gonzalez Vivo
description: Convert diffuse/specular/glossiness workflow to PBR metallic factor
use: <float> toMetallic(<vec3> diffuse, <vec3> specular, <float> maxSpecular)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material BaseColor from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialAlbedo()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material specular property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
    - MATERIAL_SPECULARMAP
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material metallic property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material roughness property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialRoughness()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material shininess property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialShininess()
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Generic Material Structure
options:
    - SCENE_BACK_SURFACE
    - SHADING_MODEL_CLEAR_COAT
    - MATERIAL_HAS_CLEAR_COAT_NORMAL
    - SHADING_MODEL_IRIDESCENCE
    - SHADING_MODEL_SUBSURFACE
    - SHADING_MODEL_CLOTH
    - SHADING_MODEL_SPECULAR_GLOSSINESS
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define RENDER_RAYMARCHING 
#define SHADING_MODEL_CLEAR_COAT 
/*
contributors: Patricio Gonzalez Vivo
description: Refractive index of different materials based on https://en.wikipedia.org/wiki/Refractive_index
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define IOR_AIR 1.000293
#define IOR_ICE 1.31
#define IOR_WATER 1.333
#define IOR_WATER_RGB vec3(1.337, 1.333, 1.331)
#define IOR_GLYCERING 1.473
#define IOR_OIL 1.515
#define IOR_OIL_RGB vec3(1.530, 1.520, 1.516)
#define IOR_GLASS 1.5168
#define IOR_GLASS_RGB vec3(1.524, 1.517, 1.515)
#define IOR_GLASS_FLINT 1.69
#define IOR_GLASS_FLINT_RGB vec3(1.639, 1.627, 1.622)
#define IOR_SAPPHIRE 1.77
#define IOR_DIAMONG 2.42
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: |
    Material Constructor. Designed to integrate with GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use:
    - void materialNew(out <material> _mat)
    - <material> materialNew()
options:
    - SURFACE_POSITION
    - SCENE_BACK_SURFACE
    - SHADING_MODEL_CLEAR_COAT
    - MATERIAL_HAS_CLEAR_COAT_NORMAL
    - SHADING_MODEL_IRIDESCENCE
    - SHADING_MODEL_SUBSURFACE
    - SHADING_MODEL_CLOTH
    - SHADING_MODEL_SPECULAR_GLOSSINESS
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define SURFACE_POSITION vec3(0.0, 0.0, 0.0)
#define FNC_MATERIAL_NEW 
void materialNew(out Material _mat) {
    // Surface data
    _mat.position = (SURFACE_POSITION).xyz;
    _mat.normal = materialNormal();
    _mat.sdf = RAYMARCH_MAX_DIST;
    _mat.valid = true;
    // PBR Properties
    _mat.albedo = materialAlbedo();
    _mat.emissive = materialEmissive();
    _mat.roughness = materialRoughness();
    _mat.metallic = materialMetallic();
    _mat.reflectance = 0.5;
    _mat.ior = vec3(IOR_GLASS_RGB); // Index of Refraction
    _mat.ambientOcclusion = materialOcclusion();
    _mat.clearCoat = 0.0;
    _mat.clearCoatRoughness = 0.01;
}
Material materialNew() {
    Material mat;
    materialNew(mat);
    return mat;
}
Material materialNew(vec3 albedo, float sdf) {
    Material mat = materialNew();
    mat.albedo.rgb = albedo;
    mat.sdf = sdf;
    return mat;
}
Material materialNew(vec3 albedo, float roughness, float metallic, float sdf) {
    Material mat = materialNew();
    mat.albedo.rgb = albedo;
    mat.metallic = metallic;
    mat.roughness = roughness;
    mat.sdf = sdf;
    return mat;
}

/*
contributors:  Inigo Quiles
description: Cast a ray
use: <float> castRay( in <vec3> pos, in <vec3> nor ) 
*/
#define FNC_RAYMARCH_CAST 
Material raymarchCast( in vec3 ro, in vec3 rd ) {
    float tmin = RAYMARCH_MIN_DIST;
    float tmax = RAYMARCH_MAX_DIST;
    float t = tmin;
    Material m = materialNew();
    m.valid = false;
    for (int i = 0; i < RAYMARCH_SAMPLES; i++) {
        Material res = RAYMARCH_MAP_FNC(ro + rd * t);
        float dist = res.sdf;
        m = res;
        if (dist < RAYMARCH_MIN_HIT_DIST || t > tmax)
            break;
        t += res.sdf;
    }
    if ( t > tmax )
        m.valid = false;
    m.sdf = t;
    return m;
}
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material BaseColor from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialAlbedo()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material specular property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
    - MATERIAL_SPECULARMAP
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material emissive property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialEmissive()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material normal property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialOcclusion()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material normal property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialNormal()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

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
/*
contributors: Patricio Gonzalez Vivo
description: Convert diffuse/specular/glossiness workflow to PBR metallic factor
use: <float> toMetallic(<vec3> diffuse, <vec3> specular, <float> maxSpecular)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material BaseColor from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialAlbedo()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material specular property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
    - MATERIAL_SPECULARMAP
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material metallic property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialMetallic()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: It defines the default sampler type and function for the shader based on the version of GLSL.
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material roughness property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialRoughness()
options:
    - SAMPLER_FNC(TEX, UV): optional depending the target version of GLSL (texture2D(...) or texture(...))
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Get material shininess property from GlslViewer's defines https://github.com/patriciogonzalezvivo/glslViewer/wiki/GlslViewer-DEFINES#material-defines
use: vec4 materialShininess()
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Patricio Gonzalez Vivo
description: Generic Material Structure
options:
    - SCENE_BACK_SURFACE
    - SHADING_MODEL_CLEAR_COAT
    - MATERIAL_HAS_CLEAR_COAT_NORMAL
    - SHADING_MODEL_IRIDESCENCE
    - SHADING_MODEL_SUBSURFACE
    - SHADING_MODEL_CLOTH
    - SHADING_MODEL_SPECULAR_GLOSSINESS
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define RENDER_RAYMARCHING 
#define SHADING_MODEL_CLEAR_COAT 
/*
contributors:  Inigo Quiles
description: Map of SDF functions to be declare
use: <vec4> raymarchMap( in <vec3> pos ) 
examples:
    - /shaders/lighting_raymarching.frag
*/

/*
contributors:  Inigo Quiles
description: Calculate normals http://iquilezles.org/www/articles/normalsSDF/normalsSDF.htm
use: <float> raymarchNormal( in <vec3> pos ) 
examples:
    - /shaders/lighting_raymarching.frag
*/
#define RAYMARCH_NORMAL_OFFSET 0.0001
#define FNC_RAYMARCH_NORMAL 
vec3 raymarchNormal(vec3 pos, vec2 pixel) {
   vec2 offset = vec2(1.0, -1.0) * pixel;
   return normalize( offset.xyy * RAYMARCH_MAP_FNC( pos + offset.xyy ).sdf +
                     offset.yyx * RAYMARCH_MAP_FNC( pos + offset.yyx ).sdf +
                     offset.yxy * RAYMARCH_MAP_FNC( pos + offset.yxy ).sdf +
                     offset.xxx * RAYMARCH_MAP_FNC( pos + offset.xxx ).sdf );
}
vec3 raymarchNormal(vec3 pos, float e) {
   vec2 offset = vec2(1.0, -1.0) * e;
   return normalize( offset.xyy * RAYMARCH_MAP_FNC( pos + offset.xyy ).sdf +
                     offset.yyx * RAYMARCH_MAP_FNC( pos + offset.yyx ).sdf +
                     offset.yxy * RAYMARCH_MAP_FNC( pos + offset.yxy ).sdf +
                     offset.xxx * RAYMARCH_MAP_FNC( pos + offset.xxx ).sdf );
}
vec3 raymarchNormal( in vec3 pos ) {
   return raymarchNormal(pos, RAYMARCH_NORMAL_OFFSET);
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
description: some useful math constants
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/

/*
contributors: Holger Dammertz
description: Return a Hammersley point
use: hammersley(uint index)
license: CC BY 3.0 Copyright (c) 2012 Holger Dammertz
*/
#define FNC_HAMMERSLEY 
vec2 hammersley(uint index, int numSamples) {
    const float tof = 0.5 / float(0x80000000U);
    uint bits = index;
    bits = (bits << 16u) | (bits >> 16u);
    bits = ((bits & 0x55555555u) << 1u) | ((bits & 0xAAAAAAAAu) >> 1u);
    bits = ((bits & 0x33333333u) << 2u) | ((bits & 0xCCCCCCCCu) >> 2u);
    bits = ((bits & 0x0F0F0F0Fu) << 4u) | ((bits & 0xF0F0F0F0u) >> 4u);
    bits = ((bits & 0x00FF00FFu) << 8u) | ((bits & 0xFF00FF00u) >> 8u);
    return vec2(float(index) / float(numSamples), float(bits) * tof);
}
vec3 hemisphereCosSample(vec2 u) {
    float phi = 2.0 * PI * u.x;
    float cosTheta2 = 1.0 - u.y;
    float cosTheta = sqrt(cosTheta2);
    float sinTheta = sqrt(1.0 - cosTheta2);
    return vec3(cos(phi) * sinTheta, sin(phi) * sinTheta, cosTheta);
}
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
contributors: Patricio Gonzalez Vivo
description: 'Fix the aspect ratio of a space keeping things squared for you.'
use: <vec2> aspect(<vec2> st, <vec2> st_size)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/draw_shapes.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_ASPECT 
vec2 aspect(vec2 st, vec2 s) {
    st.x = st.x * (s.x / s.y);
    return st;
}
/*
contributors: Patricio Gonzalez Vivo
description: "It center the coordinates from 0 to 1 to -1 to 1\nSo the center goes\
    \ from 0.5 to 0.0. \n"
use: <float|vec2|vec3> center(<float|vec2|vec3> st)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/draw_shapes.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_CENTER 
float center(float x) { return x * 2.0 - 1.0; }
vec2 center(vec2 v) { return v * 2.0 - 1.0; }
vec3 center(vec3 v) { return v * 2.0 - 1.0; }
#define LOOK_AT_RIGHT_HANDED 
/*
contributors: Patricio Gonzalez Vivo
description: create a look at matrix. Right handed by default.
use:
    - <mat3> lookAt(<vec3> forward, <vec3> up)
    - <mat3> lookAt(<vec3> eye, <vec3> target, <vec3> up)
    - <mat3> lookAt(<vec3> eye, <vec3> target, <float> roll)
    - <mat3> lookAt(<vec3> forward)
options:
    - LOOK_AT_LEFT_HANDED: assume a left-handed coordinate system
    - LOOK_AT_RIGHT_HANDED: assume a right-handed coordinate system
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_LOOKAT 
mat3 lookAt(vec3 forward, vec3 up) {
    vec3 zaxis = normalize(forward);
    vec3 xaxis = normalize(cross(zaxis, up));
    vec3 yaxis = cross(xaxis, zaxis);
    return mat3(xaxis, yaxis, zaxis);
}
mat3 lookAt(vec3 eye, vec3 target, vec3 up) {
    vec3 forward = normalize(target - eye);
    return lookAt(forward, up);
}
mat3 lookAt(vec3 eye, vec3 target, float roll) {
    vec3 up = vec3(sin(roll), cos(roll), 0.0);
    return lookAt(eye, target, up);
}
mat3 lookAt(vec3 forward) {
    return lookAt(forward, vec3(0.0, 1.0, 0.0));
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
/*
contributors: Patricio Gonzalez Vivo
description: 'Moves the center from 0.0 to 0.5'
use: <float|vec2|vec3> uncenter(<float|vec2|vec3> st)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/draw_shapes.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_UNCENTER 
float uncenter(float v) { return v * 0.5 + 0.5; }
vec2 uncenter(vec2 v) { return v * 0.5 + 0.5; }
vec3 uncenter(vec3 v) { return v * 0.5 + 0.5; }
float hash(in float x) {
    return fract(sin(x) * 753.5453123);
}
float noisedRandom(vec3 pos) {
    float offset = 0.;
    if (noisedRandomCount == 1) {
        offset += 1.;
        pos -= vec3(1, 0, 0);
    }
    else if (noisedRandomCount == 2) {
        offset += 157.;
        pos -= vec3(0, 1, 0);
    }
    else if (noisedRandomCount == 3) {
        offset += 158.;
        pos -= vec3(1, 1, 0);
    }
    else if (noisedRandomCount == 4) {
        offset += 113.;
        pos -= vec3(0, 0, 1);
    }
    else if (noisedRandomCount == 5) {
        offset += 114.;
        pos -= vec3(1, 0, 1);
    }
    else if (noisedRandomCount == 6) {
        offset += 270.;
        pos -= vec3(0, 1, 1);
    }
    else if (noisedRandomCount == 7) {
        offset += 271.;
        pos -= vec3(1, 1, 1);
    }
    noisedRandomCount++;
    float n = pos.x + pos.y * 157. + pos.z * 113.;
    return hash(n + offset);
}
vec4 boxSDFAndNormal(vec3 p, vec3 b) {
    vec3 d = abs(p) - b;
    return vec4(
        boxSDF(p, b),
        step(d.yzx, d.xyz) * step(d.zxy, d.xyz) * sign(p)
    );
}
Material raymarchMap(in vec3 pos)
{
    vec4 noiseDist = fbm(pos + noiseShift);
    noiseDist.x = (noiseDist.x - noiseOffset) * 0.7;
    noiseDist.yzw = normalize(noiseDist.yzw);
    vec4 boxDist = boxSDFAndNormal(pos, vec3(cubeSize)); // Clip to box
    vec4 dist = (noiseDist.x > boxDist.x) ? noiseDist : boxDist;
    Material mat = materialNew();
    mat.position = pos;
    mat.sdf = dist.x;
    mat.normal = dist.yzw;
    return mat;
}
float raymarchAO(in vec3 pos, in vec3 nor) {
    float occ = 0.;
    const float samplesFactor = 1.0 / float(RAYMARCH_AO_SAMPLES);
    for (int i = 0; i < RAYMARCH_AO_SAMPLES; i++) {
        float fi = float(i);
        vec2 u = vec2(fract(fi / PHI), (2. * fi + 1.) * samplesFactor);
        // LYGIA’s hemisphereCosSample results in:
        //    float phi = 2.0 * PI * u.x;
        //    float cosTheta2 = 1.0 - u.y;
        //    float cosTheta = sqrt(cosTheta2);
        //    float sinTheta = sqrt(1.0 - cosTheta2);
        //    vec3 ap = vec3(cos(phi) * sinTheta, sin(phi) * sinTheta, cosTheta);
        // The original Shadertoy shader effectively computes:
        //    float phi = 2.0 * PI * u.x;
        //    float cosTheta = 1.0 - u.y;
        //    float sinTheta = sqrt(1.0 - cosTheta*cosTheta);
        //    vec3 ap = vec3(cos(phi) * sinTheta, sin(phi) * sinTheta, cosTheta);
        // Note the different definitions of cosTheta. To make LYGIA’s cosTheta
        // the same as the Shadertoy cosTheta, note that:
        //    (1.0 - u.y) * (1.0 - u.y) = 1.0 - 2.*u.y + u.y*u.y
        // If we set u.y to 2.*u.y - u.y*u.y, LYGIA’s function becomes:
        //    float cosTheta2 = 1.0 - (2.*u.y - u.y*u.y);
        //                 // = 1.0 - 2.*u.y + u.y*u.y;
        //                 // = (1.0 - u.y) * (1.0 - u.y)
        //    float cosTheta = sqrt(cosTheta2);
        //                // = sqrt((1.0 - u.y) * (1.0 - u.y))
        //                // = 1.0 - u.y
        // This does result in some loss of precision, unfortunately.
        u.y = 2. * u.y - u.y*u.y;
        vec3 ap = hemisphereCosSample(u);
        float d = raymarchMap(pos + RAYMARCH_AO_MIN_DIST * nor + RAYMARCH_AO_MAX_DIST * hash(float(i)) * ap * sign(dot(ap, nor))).sdf;
        occ += saturate(d * 3.);
    }
    occ *= samplesFactor;
    return saturate(occ * 5.);
}
void main()
{
    vec2 uv = aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);
    vec3 rayOrigin;
    rayOrigin.xz = polar2cart(vec2(initialRotation * DEG2RAD + rotationSpeed * TIME, 1.));
    rayOrigin.y = 0.8;
    rayOrigin *= distance;
    vec3 rayDirection = normalize(lookAt(-rayOrigin) * vec3(uv, 1.7));
    gl_FragColor = vec4(0);
    // Bounding volume
    AABB box;
    box.min = vec3(-cubeSize);
    box.max = vec3(cubeSize);
    bounds = intersect(box, rayOrigin, rayDirection);
    if (bounds.y >= 0.) {
        Material res = raymarchCast(rayOrigin, rayDirection);
        if (res.valid && res.sdf > 0.) {
            vec3 normal = approximateNormals ? raymarchNormal(res.position) : res.normal;
            gl_FragColor = mix(highlightColor, mainColor, uncenter(normal.y));
            gl_FragColor.rgb += specularAmount *
                                pow(saturate(dot(-rayDirection, normal)), 12.) *
                                (0.04 + 0.96 * pow(saturate(1. + dot(rayDirection, normal)), 5.));
            gl_FragColor.rgb *= pow(vec3(raymarchAO(res.position, normal)), vec3(1., 1.1, 1.1));
            gl_FragColor.rgb = sqrt(gl_FragColor.rgb);
            gl_FragColor.a = 1.;
        }
    }
}
