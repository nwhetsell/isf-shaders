/*{
    "CATEGORIES": [
        "Filter",
        "Generator"
    ],
    "CREDIT": "Mykhailo Moroz <https://www.shadertoy.com/user/michael0884>",
    "DESCRIPTION": "Cellular automaton molecular dynamics, converted from <https://www.shadertoy.com/view/3s3cWr>",
    "INPUTS": [
        {
            "NAME" : "inputImage",
            "TYPE" : "image"
        },
        {
            "NAME": "inputImageAmount",
            "LABEL": "Input image amount",
            "TYPE": "float",
            "DEFAULT": 0,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "restart",
            "LABEL": "Restart",
            "TYPE": "event"
        },
        {
            "NAME": "enableMouse",
            "LABEL": "Enable mouse",
            "TYPE": "bool",
            "DEFAULT": false
        },
        {
            "NAME": "mouse",
            "TYPE": "point2D",
            "DEFAULT": [0.5, 0.5],
            "MIN": [0, 0],
            "MAX": [1, 1]
        },
        {
            "NAME": "dt",
            "LABEL": "Simulation speed",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "gravityDirection",
            "LABEL": "Gravity direction",
            "TYPE": "point2D",
            "DEFAULT": [0, -1],
            "MIN": [-1, -1],
            "MAX": [1, 1]
        },
        {
            "NAME": "gravityScale",
            "LABEL": "Gravity scale",
            "TYPE": "float",
            "DEFAULT": 0.001,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "cooling",
            "LABEL": "Cooling",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MAX": 10,
            "MIN": -10
        },
        {
            "NAME": "maxSpeed",
            "LABEL": "Maximum speed",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "radius",
            "LABEL": "Smoothing radius",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 20,
            "MIN": 0
        },
        {
            "NAME": "velocityContribution",
            "LABEL": "Velocity color contribution",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 10,
            "MIN": 0
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "bufferA_positionAndMass",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferA_velocity",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferB_positionAndMass",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferB_velocity",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {

        }
    ]
}*/

#include "lygia/color/space/hsv2rgb.glsl"
#include "lygia/color/luminance.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/gaussian.glsl"
#define INV_SQRT_2 0.7071067811865475244008443621048
#include "lygia/sdf/rectSDF.glsl"
float rectSDF_without_transform(vec2 p, vec2 b) {
    // For unclear reasons, the LYGIA function shifts by 0.5 and scales by 4.2.
    return rectSDF((p + 0.5) / 4.2, b, 0.);
}


//
// Shadertoy Common
//

float scalarStep(vec2 x) // Ha in Shadertoy
{
    vec2 r = step(0., x);
    return r.x * r.y;
}

float scalarReflectedStep(vec2 x) // Hb in Shadertoy
{
    vec2 r = vec2(1) - step(x, vec2(0));
    return r.x * r.y;
}

vec3 particleDistribution(vec2 x, vec2 pos, vec2 offset)
{
    return vec3(x, 1) * scalarStep(x - (pos - offset)) * scalarReflectedStep((pos + offset) - x);
}
vec3 particleDistribution(vec2 x, vec2 pos)
{
    return particleDistribution(x, pos, vec2(0.5));
}


// The Shadertoy shader uses the functions `floatBitsToUint` and
// `uintBitsToFloat` to pack more than 4 floats (5 in this case) into a
// 4-component pixel. These functions are available in GLSL v3.30 (OpenGL v3.3)
// and later, but some ISF hosts (notably Videosync) use GLSL v1.50
// (OpenGL v3.2). We can work around this by effectively running Shadertoy
// buffers twice, but the packing operations in the Shadertoy shader also
// perform a `clamp` on the packed data. Without the `clamp` calls, this shader
// seems to blow up numerically.
#define POST_UNPACK(X) (clamp(X, 0., 1.) * 2. - 1.)
#define PRE_PACK(X) clamp(0.5 * (X) + 0.5, 0., 1.)


float border(vec2 p) // In Shadertoy buffer B
{
    float bound = -rectSDF_without_transform(p - RENDERSIZE * 0.5, RENDERSIZE * vec2(0.49, 0.49));
    // float box = rectSDF_without_transform((p - RENDERSIZE * vec2(0.5, 0.6)), RENDERSIZE * vec2(0.05, 0.01));
    // float drain = -rectSDF_without_transform(p - RENDERSIZE * vec2(0.5, 0.7), RENDERSIZE * vec2(0));
    return bound;
}


void main()
{
    vec2 position = gl_FragCoord.xy;

    if (PASSINDEX == 0 || PASSINDEX == 1) // Shadertoy Buffer A
    {
        vec2 X = vec2(0);
        vec2 V = vec2(0);
        float M = 0.;

        // Basically integrate over all updated neighbor distributions that fall
        // inside of this pixel. This makes the tracking conservative.
        for (int i = -2; i <= 2; i++)
        for (int j = -2; j <= 2; j++) {
            vec2 translatedPosition = position + vec2(i, j);
            vec2 wrappedPosition = mod(translatedPosition, RENDERSIZE);
            vec4 data = IMG_PIXEL(bufferB_positionAndMass, wrappedPosition);

            vec2 X0 = POST_UNPACK(data.xy) + translatedPosition;
            vec2 V0 = POST_UNPACK(IMG_PIXEL(bufferB_velocity, wrappedPosition).xy);
           	int M0 = int(data.z);

            X0 += V0 * dt; // Integrate position

            // Deposited mass into this cell
            vec3 m;
            if (M0 >= 2) {
                int halfM0 = M0 / 2;
                m = float(halfM0)      * particleDistribution(X0 + vec2(0.5, 0), position) +
                    float(M0 - halfM0) * particleDistribution(X0 - vec2(0.5, 0), position);
            } else {
                m = float(M0) * particleDistribution(X0, position);
            }

            // Add weighted by mass
            X += m.xy;
            V += V0 * m.z;

            // Add mass
            M += m.z;
        }

        // Normalization
        if (M != 0.) {
            X /= M;
            V /= M;
        }

        // Initial condition
        if (FRAMEINDEX < 1 || restart) {
            X = position;
            V = vec2(0);
            M = mix(
                particleDistribution(position, RENDERSIZE * 0.5, vec2(RENDERSIZE.x * 0.15)).z,
                luminance(IMG_PIXEL(inputImage, position)),
                inputImageAmount
            );
        }

        if (PASSINDEX == 0) {
            X = X - position;
            gl_FragColor = vec4(PRE_PACK(X), M, 1);
        } else {
            gl_FragColor = vec4(PRE_PACK(V), 0, 1);
        }
    }
    else if (PASSINDEX == 2 || PASSINDEX == 3) // Shadertoy Buffer B
    {
        vec2 wrappedPosition = mod(position, RENDERSIZE);
        vec4 data = IMG_PIXEL(bufferA_positionAndMass, wrappedPosition);
        vec2 X = POST_UNPACK(data.xy) + position;
        vec2 V = POST_UNPACK(IMG_PIXEL(bufferA_velocity, wrappedPosition).xy);
        float M = data.z;

        if (M != 0.) { // Not vacuum
            // Compute the force
            vec2 Fa = vec2(0);

            for (int i = -2; i <= 2; i++)
            for (int j = -2; j <= 2; j++) {
                vec2 translatedPosition = position + vec2(i, j);
                vec2 wrappedPosition = mod(translatedPosition, RENDERSIZE);
                vec4 data = IMG_PIXEL(bufferA_positionAndMass, wrappedPosition);

                vec2 X0 = POST_UNPACK(data.xy) + translatedPosition;
                vec2 V0 = POST_UNPACK(IMG_PIXEL(bufferA_velocity, wrappedPosition).xy);
                float M0 = data.z;
                vec2 dx = X0 - X;

                Fa += M0 * (-gaussian(0.75 * dx, INV_SQRT_2) + 0.13 * gaussian(0.4 * dx, INV_SQRT_2)) * dx;
            }

            vec2 F = vec2(0);
            if (enableMouse) {
                vec2 dx = position - mouse * RENDERSIZE;
                F -= 0.003 * dx * gaussian(dx / 30., INV_SQRT_2);
            }

           	// Gravity
            F += gravityScale * gravityDirection;

            // Integrate velocity
            V += (F + Fa) * dt / M;

            // Wyatt thermostat
            X += cooling * Fa * dt / M;

#define h 1.
            vec3 r = vec3( 1./h,     0, 0.25) * border(X + vec2( h,  0)) +
                     vec3(-1./h,     0, 0.25) * border(X + vec2(-h,  0)) +
                     vec3(    0,  1./h, 0.25) * border(X + vec2( 0,  h)) +
                     vec3(    0, -1./h, 0.25) * border(X + vec2( 0, -h));
            vec3 BORD = vec3(normalize(r.xy), r.z + EPSILON);
            V += 0.5 * smoothstep(0., 5., -BORD.z) * BORD.xy;

            // Velocity limit
            float v = length(V);
            if (v > maxSpeed) {
                V /= v;
            }
        }

        if (PASSINDEX == 2) {
            X = X - position;
            gl_FragColor = vec4(PRE_PACK(X), M, 1);
        } else {
            gl_FragColor = vec4(PRE_PACK(V), 0, 1);
        }
    }
    else // Shadertoy Image
    {
        float rho = 0.001;
        vec2 vel = vec2(0);

        // Compute the smoothed density and velocity
        for (int i = -2; i <= 2; i++)
        for (int j = -2; j <= 2; j++) {
            vec2 translatedPosition = floor(position) + vec2(i, j);
            vec2 wrappedPosition = mod(translatedPosition, RENDERSIZE);
            vec4 data = IMG_PIXEL(bufferA_positionAndMass, wrappedPosition);

            vec2 X0 = POST_UNPACK(data.xy) + translatedPosition;
            vec2 V0 = POST_UNPACK(IMG_PIXEL(bufferB_velocity, wrappedPosition).xy);
            float M0 = data.z;
            vec2 dx = X0 - position;

            float K = gaussian(dx / radius, radius * INV_SQRT_2);
            rho += M0 * K;
            vel += M0 * K * V0;
        }

        vel /= rho;
        vec3 vc = hsv2rgb(vec3(6. * atan(vel.x, vel.y) / TWO_PI, 1, rho * length(vel.xy)));
        gl_FragColor.rgb = cos(0.9 * vec3(3, 2, 1) * rho) + velocityContribution * vc;
    }
}
