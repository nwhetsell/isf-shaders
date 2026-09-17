/*{
    "CATEGORIES": [
        "Filter"
    ],
    "CREDIT": "Florian Berger <https://www.shadertoy.com/user/flockaroo>",
    "DESCRIPTION": "Hand drawing effect, converted from <https://www.shadertoy.com/view/XtVGD1>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "rollAmplitude",
            "LABEL": "Roll amplitude",
            "TYPE": "float",
            "DEFAULT": 4,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "lineDefinition",
            "LABEL": "Line definition",
            "TYPE": "float",
            "DEFAULT": 400,
            "MAX": 1000,
            "MIN": 1
        },
        {
            "NAME": "lineAngle",
            "LABEL": "Line angle",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "lineThinness",
            "LABEL": "Line thinness",
            "TYPE": "float",
            "DEFAULT": 0.75,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "lineAmount",
            "LABEL": "Line amount",
            "TYPE": "float",
            "DEFAULT": 3,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "lineDistance",
            "LABEL": "Line distance",
            "TYPE": "float",
            "DEFAULT": 0.4,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "lineDensity",
            "LABEL": "Line density",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "saturation",
            "LABEL": "Saturation",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MAX": 1,
            "MIN": 0
        }
    ],
    "ISFVSN": "2"
}*/

#define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#include "lygia/color/luminance.glsl"
#include "lygia/generative/random.glsl"
#include "lygia/math/const.glsl"
#include "lygia/space/polar2cart.glsl"


vec2 sampleDerivative(vec2 st, float pixel)
{
    return vec2(
        luminance(IMG_PIXEL(inputImage, st + vec2(pixel,0.0))) - luminance(IMG_PIXEL(inputImage, st - vec2(pixel,0.0))),
        luminance(IMG_PIXEL(inputImage, st + vec2(0.0,pixel))) - luminance(IMG_PIXEL(inputImage, st - vec2(0.0,pixel)))
    );
}

const int angleCount = 3;
const int sampleCount = 16;

void main()
{
    float scaleFactor = RENDERSIZE.y / lineDefinition;

    vec2 position = gl_FragCoord.xy + rollAmplitude * sin(TIME * vec2(1, 1.7)) * scaleFactor;
    float lineColor = 0.;
    vec3 mainColor = vec3(0);
    float sum = 0.;

    for (int i = 0; i < angleCount; i++) {
        float angle = TWO_PI / float(angleCount) * (float(i) + lineAngle);
        vec2 vector = polar2cart(vec2(angle, 1));
        vec2 perpendicularVector = vector.yx * vec2(1, -1);

        for (int j = 0; j < sampleCount; j++) {
            vec2 deltaPosition1 = perpendicularVector * float(j) * scaleFactor;
            vec2 deltaPosition2 = vector * float(j * j) / float(sampleCount) * 0.5 * scaleFactor;

            for (float sgn = -1.; sgn <= 1.; sgn += 2.) {
                vec2 displacementVector = sgn * deltaPosition1 + deltaPosition2;
                float derivativePixel = max(lineDistance, EPSILON);
               	vec2 gradient = sampleDerivative(position + displacementVector, derivativePixel);
                gradient /= derivativePixel * 2.;

               	lineColor += clamp(dot(gradient, vector) - 0.5 * abs(dot(gradient, perpendicularVector)), 0., 0.05)
                          * (1. - float(j) / float(sampleCount));

               	float factor = abs(dot(normalize(gradient + vec2(EPSILON)), perpendicularVector));
                vec2 colorPosition = position + displacementVector.yx * vec2(1, -1) * 2.;
               	mainColor += factor * smoothstep(0.95, 1.05, IMG_PIXEL(inputImage, colorPosition) * saturation + (1. - saturation) + random4(colorPosition * 0.7)).rgb;
               	sum += factor;
            }
        }
    }

    lineColor /= float(angleCount * sampleCount) * lineThinness / sqrt(RENDERSIZE.y);
    lineColor *= lineDensity + 0.8 * random4(position * 0.7).x;
    lineColor = 1. - lineColor;
    lineColor = pow(lineColor, lineAmount);

    mainColor /= sum;

    gl_FragColor = vec4(lineColor * mainColor, IMG_PIXEL(inputImage, position).a);
}
