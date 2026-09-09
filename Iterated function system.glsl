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

#include "complex/complex.glsl"
#include "lygia/math/const.glsl"
#include "lygia/space/polar2cart.glsl"
#include "rand/rand.glsl"


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
