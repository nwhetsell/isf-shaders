/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "loicvdb <https://github.com/loicvdb>",
    "DESCRIPTION": "Fractal cloud, converted from <https://www.shadertoy.com/view/tsGSDt>",
    "INPUTS": [
        {
            "NAME": "formationSpeed",
            "LABEL": "Formation speed",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MAX": 10,
            "MIN": -10
        },
        {
            "NAME": "powerAmplitude",
            "LABEL": "Power amplitude",
            "TYPE": "float",
            "DEFAULT": 5,
            "MAX": 20,
            "MIN": -20
        },
        {
            "NAME": "absorbanceFactor",
            "LABEL": "Absorbance factor",
            "TYPE": "float",
            "DEFAULT": 10,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "outerRadius",
            "LABEL": "Outer radius",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "innerRadius",
            "LABEL": "Inner radius",
            "TYPE": "float",
            "DEFAULT": 1.2,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "rotationSpeed",
            "LABEL": "Rotation speed",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MAX": 10,
            "MIN": -10
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
            "NAME": "lightRadius",
            "LABEL": "Light distance",
            "TYPE": "float",
            "DEFAULT": 3.3166247904,
            "MIN": 0,
            "MAX": 200
        },
        {
            "NAME": "lightPhi",
            "LABEL": "Light phi (degrees)",
            "TYPE": "float",
            "DEFAULT": 72.4515993862,
            "MIN": 0,
            "MAX": 180
        },
        {
            "NAME": "lightTheta",
            "LABEL": "Light theta (degrees)",
            "TYPE": "float",
            "DEFAULT": 251.5650511771,
            "MIN": 0,
            "MAX": 360
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
            "NAME": "attenuationL",
            "LABEL": "Light attenuation",
            "TYPE": "float",
            "DEFAULT": 4,
            "MAX": 5,
            "MIN": 0.01
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
        },
        {
            "NAME": "positionRandomness",
            "LABEL": "Position randomness",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 1,
            "MIN": 0
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

// The default light parameters are:
//   lightRadius = length(vec3(-1, -3, 1))
//               = sqrt(-1 * -1 + -3 * -3 + 1 * 1)
//               = sqrt(11) ≈ 3.3166247904
//   lightPhi = acos(1 / lightRadius) ≈ 72.4515993862°
//   lightTheta = atan(-3 / -1) ≈ 251.5650511771°

// #define ISF_EDITOR_WEBSITE

#include "lygia/color/tonemap/aces.glsl"
#define RANDOM_SINLESS
#define RANDOM_HIGHER_RANGE

#define RAYMARCH_MIN_DIST max(length(rayOrigin) - outerRadius, 0.)
#define RAYMARCH_VOLUME_SAMPLES 150
#define RAYMARCH_MAX_DIST (0.03 * float(RAYMARCH_VOLUME_SAMPLES))
#define RAYMARCH_VOLUME_SAMPLES_LIGHT 7
#define RAYMARCH_VOLUME_DITHER 1.0
#define LIGHT_DIRECTION polar2cart(lightRadius, lightPhi * DEG2RAD, lightTheta * DEG2RAD)

#include "lygia/generative/random.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/mmax.glsl"
#include "lygia/math/rotate2d.glsl"
#include "lygia/math/rotate3dX.glsl"
#include "lygia/math/rotate3dY.glsl"
#include "lygia/math/rotate3dZ.glsl"
#include "lygia/space/aspect.glsl"
#include "lygia/space/cart2polar.glsl"
#include "lygia/space/center.glsl"
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

float TIME_SCALED = TIME * formationSpeed;

float distanceEstimation(vec3 position)
{
    float r = length(position);
    if (r > outerRadius)
        return r - innerRadius;

    float power = powerAmplitude * sin(TIME_SCALED * 0.1);

    vec3 z = position;
    float dr = 1.;
    for (int i = 0; i < 6; i++) {
        vec3 polar = cart2polar(z.xzy);
        r = polar.x;
        if (r > outerRadius)
            break;
        z = polar2cart(pow(r, power), polar.y * power - TIME_SCALED, polar.z * power - TIME_SCALED) + position;
        dr = pow(r, power - 1.) * power * dr + 1.;
    }

    return 0.5 * log(r) * r / dr;
}

vec3 raymarchVolumeShadowTransmittance(vec3 position, vec3 rayDirectionL, float stepSizeL)
{
    vec3 transmittanceL = vec3(1.0, 1.0, 1.0);
    float tL = 0.0;

    for (int i = 0; i < RAYMARCH_VOLUME_SAMPLES_LIGHT; i++) {
        vec3 positionL = position + rayDirectionL * tL;
        float sdfL = distanceEstimation(positionL);
        float densityL = -sdfL;
        tL -= max(sdfL, stepSizeL);
        positionL = position + rayDirectionL * tL;

        if (sdfL < stepSizeL) {
            float offset = frand() * stepSizeL * RAYMARCH_VOLUME_DITHER;
            tL += stepSizeL - offset;
            positionL = position + rayDirectionL * tL;

            if (densityL > 0.) {
                transmittanceL *= exp(-absorbanceFactor * offset);
                if (mmax(transmittanceL) < 0.1)
                    break;
            }
        }

        if (length(position) > outerRadius)
            break;
    }

    return transmittanceL;
}

// The Shadertoy shader uses the direction argument to return the color from a
// cubemap, which is impossible in an ISF shader.
vec3 backgroundColor(vec3 direction)
{
    return vec3(0);
}

vec3 raymarchVolume(vec3 rayOrigin, vec3 rayDirection, vec2 st, float minDist, vec3 background)
{
   	vec3 scatteredLuminance = vec3(0.0, 0.0, 0.0);
    vec3 transmittance = vec3(1.0, 1.0, 1.0);
    float stepSize = RAYMARCH_MAX_DIST/float(RAYMARCH_VOLUME_SAMPLES);

    float t = RAYMARCH_MIN_DIST;

    for (int i = 0; i < RAYMARCH_VOLUME_SAMPLES; i++) {
        vec3 position = rayOrigin + rayDirection * t;
        float sdf = distanceEstimation(position);
        float density = -sdf;

        t += max(sdf, stepSize);
        position = rayOrigin + rayDirection * t;

        if (length(position) < outerRadius) {
            if (sdf < stepSize) {
            float offset = frand() * stepSize * RAYMARCH_VOLUME_DITHER;
            t += -stepSize + offset;
            position = rayOrigin + rayDirection * t;

            if (density > 0.) {
                float absorbance = exp(-absorbanceFactor * offset);

                if (density < 0.0005)
                    scatteredLuminance += transmittance * highlightColor.rgb;

                if (frand() < 1. / attenuationL) {
                    float stepSizeL = 0.2; // RAYMARCH_MAX_DIST/float(RAYMARCH_VOLUME_SAMPLES_LIGHT);
                    vec3 rayDirectionL = normalize(LIGHT_DIRECTION);
                    vec3 shadow = raymarchVolumeShadowTransmittance(position, rayDirectionL, stepSizeL);
                    vec3 L = lightColor.rgb * lightIntensity;

                    scatteredLuminance += attenuationL * shadow * transmittance * volumeColor.rgb * (1. - absorbance) * L;
                }

                if (mmax(transmittance) < 0.05)
                    break;

                if (frand() > absorbance) {
                    rayDirection = vec3(1, 0, 0) * rotate3dZ(-frand() * TWO_PI) * rotate3dX(-frand() * TWO_PI); // random direction
                    transmittance *= volumeColor.rgb;
                }
            }
            }
        } else if (dot(rayDirection, position) > 0.) {
            return background * transmittance + scatteredLuminance;
        }
    }

    return scatteredLuminance;
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
        vec2 uv = gl_FragCoord.xy / RENDERSIZE;

        seed = uv * 1000. + log(vec2(FRAMEINDEX));

        uv += positionRandomness * vec2(frand(), frand()) / RENDERSIZE;
        uv = 0.5 * aspect(center(uv), RENDERSIZE);

        vec3 focalPoint = vec3(uv * cameraFocalDistance / cameraFocalLength, cameraFocalDistance);
        vec3 aperture = cameraAperture * vec3(sampleAperture(6, apertureRotation), 0.);

        float rotation = TIME_SCALED * rotationSpeed;
        mat3 cameraMatrix = rotate3dY(rotation);
        vec3 cameraPosition = vec3(0, 0, -2.5) * cameraMatrix;
        cameraMatrix = rotate3dZ(-0.5 * sin(TIME_SCALED * 0.3)) * cameraMatrix;

        vec3 rayDirection = normalize(focalPoint - aperture) * cameraMatrix;

        float minDist = 1./0.; // Not used
        gl_FragColor = vec4(raymarchVolume(cameraPosition + aperture * cameraMatrix, rayDirection, uv, minDist, backgroundColor(rayDirection)), 1);

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
