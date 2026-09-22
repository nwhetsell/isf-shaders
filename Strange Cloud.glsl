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
            "LABEL": "power",
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

#include "lygia/color/tonemap/aces.glsl"
#define RANDOM_SINLESS
#define RANDOM_HIGHER_RANGE
#include "lygia/generative/random.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/mmax.glsl"
#include "lygia/math/rotate2d.glsl"
#include "lygia/math/rotate3dX.glsl"
#include "lygia/math/rotate3dY.glsl"
#include "lygia/math/rotate3dZ.glsl"
#include "lygia/space/cart2polar.glsl"
#include "lygia/space/polar2cart.glsl"


vec2 seed = vec2(0);

float frand(void)
{
    seed += vec2(1.153535, -1.1231354);
    #ifdef STRICT_RANDOM
    return fract(sin(dot(seed, vec2(12.9898, 4.1414))) * 43758.5453);
    #else
    return random(seed);
    #endif
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
        #ifndef ISF_EDITOR_WEBSITE
        if (enableBloom) {
            for(int y = -1; y <= 1; y++)
            for(int x = -1; x <= 1; x++)
                bloom += textureLod(cloud, (gl_FragCoord.xy + vec2(x, y) * bloomDistance) / RENDERSIZE, 7.).rgb / color.a;
            bloom = max(bloom / 9. - 0.5, vec3(0)) * 0.25;
        }
        #endif

        color /= color.a;
        color.rgb += bloom;
        if (enableTonemap)
            color.rgb = tonemapACES(color.rgb);
        gl_FragColor = vec4(color.rgb, length(color.rgb));
    }
}
