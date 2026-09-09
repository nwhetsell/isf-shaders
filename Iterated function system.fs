/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "",
    "DESCRIPTION": "",
    "INPUTS": [
        {
            "NAME": "decay",
            "LABEL": "Decay",
            "TYPE": "float",
            "DEFAULT": 0.999,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "numeratorFactor",
            "LABEL": "Numerator factor",
            "TYPE": "point2D",
            "DEFAULT": [0, 0],
            "MIN": [-5, -5],
            "MAX": [5, 5]
        },
        {
            "NAME": "numeratorConjugateFactor",
            "LABEL": "Numerator conjugate factor",
            "TYPE": "point2D",
            "DEFAULT": [0, 0],
            "MIN": [-5, -5],
            "MAX": [5, 5]
        },
        {
            "NAME": "numeratorOffset",
            "LABEL": "Numerator offset",
            "TYPE": "point2D",
            "DEFAULT": [1, 0],
            "MIN": [-5, -5],
            "MAX": [5, 5]
        },
        {
            "NAME": "denominatorFactor",
            "LABEL": "Denominator factor",
            "TYPE": "point2D",
            "DEFAULT": [0.075, 0.665],
            "MIN": [-5, -5],
            "MAX": [5, 5]
        },
        {
            "NAME": "denominatorConjugateFactor",
            "LABEL": "Denominator conjugate factor",
            "TYPE": "point2D",
            "DEFAULT": [-0.15, -0.01],
            "MIN": [-5, -5],
            "MAX": [5, 5]
        },
        {
            "NAME": "denominatorOffset",
            "LABEL": "Denominator offset",
            "TYPE": "point2D",
            "DEFAULT": [0.33, 0.075],
            "MIN": [-5, -5],
            "MAX": [5, 5]
        },
        {
            "NAME": "adjustmentFactor",
            "LABEL": "Adjustment factor",
            "TYPE": "point2D",
            "DEFAULT": [1, 0],
            "MIN": [0, -360],
            "MAX": [10, 360]
        },
        {
            "NAME": "contrast",
            "LABEL": "Contrast",
            "TYPE": "float",
            "DEFAULT": 1,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "brightness",
            "LABEL": "Brightness",
            "TYPE": "float",
            "DEFAULT": 100,
            "MIN": 0,
            "MAX": 200
        },
        {
            "NAME": "thickness",
            "LABEL": "Thickness",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "axesLimit",
            "LABEL": "Axes limit",
            "TYPE": "float",
            "DEFAULT": 6,
            "MIN": 1,
            "MAX": 10
        },
        {
            "NAME": "normalizedCenter",
            "LABEL": "Normalized center coordinate",
            "TYPE": "point2D",
            "DEFAULT": [0, 0],
            "MIN": [-1, -1],
            "MAX": [1, 1]
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "lastData",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {

        }
    ]
}*/

#define COMPLEX 
vec2 complex_multiply(in vec2 v1, in vec2 v2)
{
    return vec2(
        v1.x * v2.x - v1.y * v2.y,
        v1.x * v2.y + v1.y * v2.x
    );
}
float complex_magnitudeSquared(in vec2 v)
{
    return v.x * v.x + v.y * v.y;
}
float complex_magnitude(in vec2 v)
{
    return length(v);
}
vec2 complex_conjugate(in vec2 v)
{
    return vec2(v.x, -v.y);
}
// https://en.wikipedia.org/wiki/Complex_number#Complex_conjugate,_absolute_value,_argument_and_division
vec2 complex_divide(in vec2 v1, in vec2 v2)
{
    return complex_multiply(v1, complex_conjugate(v2)) / complex_magnitudeSquared(v2);
}
float complex_argument(in vec2 v)
{
    return atan(v.y, v.x);
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
// #define ISF_EDITOR_WEBSITE
#define RAND 
// Linear congruential generator for random numbers
// (https://en.wikipedia.org/wiki/Linear_congruential_generator).
int seed = 1;
// This is the multiplier (a) and increment (c) from used in Microsoft’s Visual
// C implementation; see
// https://en.wikipedia.org/wiki/Linear_congruential_generator#Parameters_in_common_use
// in the “Microsoft Visual/Quick C/C++” row. The multiplier may not be a good
// choice. Table 4 (page 258) of “Tables of Linear Congruential Generators of
// Different Sizes and Good Lattice Structure” (Pierre L’Ecuyer 1999,
// https://www.ams.org/journals/mcom/1999-68-225/S0025-5718-99-00996-5/S0025-5718-99-00996-5.pdf)
// may have better constants (the 2^31 modulus row gives 37769685, 26757677, or
// 20501397), but that article has errata that does not seem to be easily
// available, so just use the Microsoft value.
const int a = 0x343fd;
const int c = 0x269ec3;
const int randBitMask = 0x7fff;
int rand(void)
{
    seed = seed * a + c;
    // Return bits 16 to 30.
    return (seed >> 16) & randBitMask;
}
float frand(void)
{
    return float(rand()) / float(randBitMask);
}
void srand(int s)
{
    seed = s;
}
// This is a pseudo-random integer generator attributed to Hugo Elias. The
// original source for this seems to be
// http://freespace.virgin.net/hugo.elias/models/m_perlin.htm, but that website
// is defunct. This appears to be a Python translation:
// https://gist.github.com/dragon0/f70e2637e6d4e64a6ab210faf8a85a50
int hash(int n)
{
    n = (n << 13) ^ n;
    return n * (n * n * 15731 + 789221) + 1376312589;
}
// This is a heavily modified version of https://www.shadertoy.com/view/lst3zf
// by Inigo Quilez. It also uses concepts from https://github.com/profConradi/Fractals.
const int IFS_ITERATIONS = 2048;
vec2 ifs(in vec2 z)
{
    z = complex_divide(
        complex_multiply(numeratorFactor, z) + complex_multiply(numeratorConjugateFactor, complex_conjugate(z)) + numeratorOffset,
        complex_multiply(denominatorFactor, z) + complex_multiply(denominatorConjugateFactor, complex_conjugate(z)) + denominatorOffset
    );
    float p = frand();
    if (p >= 0.5) {
        z = complex_multiply(
            polar2cart(vec2(DEG2RAD * adjustmentFactor.y, adjustmentFactor.x)),
            complex_multiply(
                polar2cart(vec2(PI, 1.)),
                z
            )
        );
    }
    return z;
}
float sdf(vec2 position)
{
    vec2 z = vec2(0);
    for (int i = 0; i < 32; i++) {
        z = ifs(z);
    }
 float distance = complex_magnitude(position - z);
    for (int i = 0; i < IFS_ITERATIONS; i++) {
        z = ifs(z);
  distance = min(distance, complex_magnitude(position - z));
    }
    return distance;
}
void main()
{
    if (PASSINDEX == 0)
    {
        srand(hash(FRAMEINDEX + hash(int(gl_FragCoord.x) + hash(int(gl_FragCoord.y)))));
        vec2 position = (2. * gl_FragCoord.xy - RENDERSIZE) / RENDERSIZE.y;
        position = axesLimit * (position - normalizedCenter);
        float data = sdf(position);
        if (FRAMEINDEX > 0) {
            data = min(data, IMG_THIS_PIXEL(lastData).x);
        }
        data = min(1., (2. - decay) * data);
        gl_FragColor = vec4(data, vec3(0));
    }
    else
    {
        float data = IMG_THIS_PIXEL(lastData).x;
        float color = 1. - 1. / (contrast + brightness * data);
        color = pow(color, thickness);
        gl_FragColor = vec4(vec3(color), 1);
    }
}
