/*{
    "CATEGORIES": [
        "Filter",
        "Generator"
    ],
    "CREDIT": "Mykhailo Moroz <https://www.shadertoy.com/user/michael0884>",
    "DESCRIPTION": "Cell system, converted from <https://www.shadertoy.com/view/3tSfRW>",
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
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "distribution_size",
            "LABEL": "Distribution size",
            "TYPE": "float",
            "DEFAULT": 1.7,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "acceleration",
            "LABEL": "Acceleration",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "sense_ang",
            "LABEL": "Sensor angle factor",
            "TYPE": "float",
            "DEFAULT": 0.03,
            "MAX": 2,
            "MIN": 0
        },
        {
            "NAME": "sense_dis",
            "LABEL": "Sensor distance",
            "TYPE": "float",
            "DEFAULT": 15,
            "MAX": 20,
            "MIN": 0
        },
        {
            "NAME": "distance_scale",
            "LABEL": "Sensor distance scale",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "sense_oscil",
            "LABEL": "Sensor turn speed",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "oscil_scale",
            "LABEL": "Sensor turn speed scale",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "oscil_pow",
            "LABEL": "Oscillation power",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 10,
            "MIN": -10
        },
        {
            "NAME": "sense_force",
            "LABEL": "Sensor strength",
            "TYPE": "float",
            "DEFAULT": 0.38,
            "MAX": 1,
            "MIN": -1
        },
        {
            "NAME": "force_scale",
            "LABEL": "Sensor force scale",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 2,
            "MIN": 0
        },
        {
            "NAME": "density_normalization_speed",
            "LABEL": "Density normalization speed",
            "TYPE": "float",
            "DEFAULT": 0.13,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "density_target",
            "LABEL": "Density target",
            "TYPE": "float",
            "DEFAULT": 0.24,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "vorticity_confinement",
            "LABEL": "Vorticity confinement",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "massDecayFactor",
            "LABEL": "Mass decay",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 2,
            "MIN": 0
        },
        {
            "NAME": "radius",
            "LABEL": "Smoothing radius",
            "TYPE": "float",
            "DEFAULT": 2,
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
            "TARGET": "bufferB",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferC",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {

        }
    ]
}*/

#include "lygia/color/space/hsv2rgb.glsl"
#include "lygia/color/luminance.glsl"
#define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#include "lygia/generative/random.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/gaussian.glsl"
#define INV_SQRT_2 0.7071067811865475244008443621048
#include "lygia/math/rotate2d.glsl"
#include "lygia/space/polar2cart.glsl"


//
// Shadertoy Common
//

#define HALF_SENSOR_COUNT_MINUS_1 12

// The Shadertoy shader uses the functions `floatBitsToUint` and
// `uintBitsToFloat` to pack more than 4 floats (5 in this case) into a
// 4-component pixel. These functions are available in GLSL v3.30 (OpenGL v3.3)
// and later, but some ISF hosts (notably Videosync) use GLSL v1.50
// (OpenGL v3.2). We can work around this by effectively running one of the
// Shadertoy buffers twice, but the packing operations in the Shadertoy shader
// also perform a `clamp` on the packed data. Without the `clamp` calls, this
// shader seems to blow up numerically.
#define POST_UNPACK(X) (clamp(X, 0., 1.) * 2. - 1.)
#define PRE_PACK(X) clamp(0.5 * (X) + 0.5, 0., 1.)


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
            vec4 data = IMG_PIXEL(bufferA_positionAndMass, wrappedPosition);

            vec2 X0 = POST_UNPACK(data.xy) + translatedPosition;
            vec2 V0 = POST_UNPACK(IMG_PIXEL(bufferB, wrappedPosition).xy);
            float M0 = data.z;

            X0 += V0 * dt; // Integrate position

            // Overlap aabb
            vec4 aabbX = vec4(
                max(position - 0.5, X0 - 0.5 * distribution_size),
                min(position + 0.5, X0 + 0.5 * distribution_size)
            );
            vec2 center = 0.5 * (aabbX.xy + aabbX.zw); // Center of mass
            vec2 size = max(aabbX.zw - aabbX.xy, 0.); // Only positive

            // Deposited mass into this cell
            float m = M0 * size.x * size.y / (distribution_size * distribution_size);

            // Add weighted by mass
            X += center * m;
            V += V0 * m;

            // Add mass
            M += m;
        }

        // Normalization
        if (M != 0.) {
            X /= M;
            V /= M;
        }

        float inputLuminance = luminance(IMG_PIXEL(inputImage, position));
        M += inputImageAmount * inputLuminance;
        V += inputImageAmount * inputLuminance;

        // Mass renormalization
        float prevM = M;
        M = mix(M, density_target, density_normalization_speed);
        V = V * prevM / M;

        // Mass decay
        M *= massDecayFactor;

        // Initial condition
        if (FRAMEINDEX < 1 || restart) {
            X = position;

            vec2 dx0 = position - 0.3 * RENDERSIZE;
            vec2 dx1 = position - 0.7 * RENDERSIZE;
            V = 0.5 * rotate2d(HALF_PI) * (dx0 * gaussian(dx0 / 30., INV_SQRT_2) - dx1 * gaussian(dx1 / 30., INV_SQRT_2));
            V += polar2cart(vec2(
                TWO_PI * random(floor(position.x / 10.) + RENDERSIZE.x * floor(position.y / 20.)),
                1
            ));

            M = 0.1 + 0.01 * (position.x + position.y) / RENDERSIZE.x;
        }

        if (PASSINDEX == 0) {
            X = clamp(X - position, vec2(-0.5), vec2(0.5));
            gl_FragColor = vec4(PRE_PACK(X), M, 1);
        } else {
            gl_FragColor = vec4(PRE_PACK(V), 0, 1);
        }
    }
    else if (PASSINDEX == 2) // Shadertoy Buffer B
    {
        vec2 wrappedPosition = mod(position, RENDERSIZE);

        vec4 data = IMG_PIXEL(bufferA_positionAndMass, wrappedPosition);
        vec2 X = POST_UNPACK(data.xy) + position;
        vec2 V = POST_UNPACK(IMG_PIXEL(bufferA_velocity, wrappedPosition).xy);
        float M = data.z;

        if (M != 0.) { // Not vacuum
            // Compute the force
            vec2 F = vec2(0);

            // Get neighbor data
            const vec2 dx = vec2(0, 1);
            wrappedPosition = mod(position + dx.xy, RENDERSIZE);
            vec4 d_u = IMG_PIXEL(bufferA_positionAndMass, wrappedPosition);
            vec2 v_u = POST_UNPACK(IMG_PIXEL(bufferA_velocity, wrappedPosition).xy);
            wrappedPosition = mod(position - dx.xy, RENDERSIZE);
            vec4 d_d = IMG_PIXEL(bufferA_positionAndMass, wrappedPosition);
            vec2 v_d = POST_UNPACK(IMG_PIXEL(bufferA_velocity, wrappedPosition).xy);
            wrappedPosition = mod(position + dx.yx, RENDERSIZE);
            vec4 d_r = IMG_PIXEL(bufferA_positionAndMass, wrappedPosition);
            vec2 v_r = POST_UNPACK(IMG_PIXEL(bufferA_velocity, wrappedPosition).xy);
            wrappedPosition = mod(position - dx.yx, RENDERSIZE);
            vec4 d_l = IMG_PIXEL(bufferA_positionAndMass, wrappedPosition);
            vec2 v_l = POST_UNPACK(IMG_PIXEL(bufferA_velocity, wrappedPosition).xy);

            // Pressure gradient
            vec2 p = 0.5 * vec2(d_r.z - d_l.z, d_u.z - d_d.z);

            // Velocity operators
            float curl = v_r.y - v_l.y - v_u.x + v_d.x;

            F -= M * p;

            float ang = atan(V.y, V.x);
            float dang = sense_ang * PI / float(HALF_SENSOR_COUNT_MINUS_1);
            vec2 slimeF = vec2(0);
            // Slime mold sensors
            for (int i = -HALF_SENSOR_COUNT_MINUS_1; i <= HALF_SENSOR_COUNT_MINUS_1; i++) {
                float cang = ang + float(i) * dang;
                vec2 dir = polar2cart(vec2(cang, 1. + sense_dis * pow(M, distance_scale)));
                vec2 sensedPosition = mod(X + dir, RENDERSIZE);
                vec4 s0 = IMG_NORM_PIXEL(bufferC, sensedPosition / RENDERSIZE);
                slimeF += rotate2d(oscil_scale * pow(s0.z - M, oscil_pow)) * s0.xy * sense_oscil +
                          polar2cart(vec2(ang + sign(float(i)) * HALF_PI, sense_force * pow(s0.z, force_scale)));
            }

            // Remove acceleration component from slime force and leave rotation only
            slimeF -= dot(slimeF, normalize(V)) * normalize(V);
            F += slimeF / float(2 * HALF_SENSOR_COUNT_MINUS_1);

            if (enableMouse) {
                vec2 dx = position - mouse * RENDERSIZE;
                F += 0.1 * rotate2d(HALF_PI) * dx * gaussian(dx / 30., INV_SQRT_2);
            }

            // Integrate velocity
            V += rotate2d(-vorticity_confinement * curl) * F * dt / M;

            // Acceleration for fun effects
            V *= 1. + acceleration;

            // Velocity limit
            float v = length(V);
            if (v > 1.) {
                V /= v;
            }
        }

        gl_FragColor = vec4(PRE_PACK(V), 0, 1);
    }
    else if (PASSINDEX == 3) // Shadertoy Buffer C
    {
        float rho = 0.001;
        vec2 vel = vec2(0);

        // Compute the smoothed density and velocity
        for (int i = -2; i <= 2; i++)
        for (int j = -2; j <= 2; j++) {
            vec2 translatedPosition = position + vec2(i, j);
            vec2 wrappedPosition = mod(translatedPosition, RENDERSIZE);
            vec4 data = IMG_PIXEL(bufferA_positionAndMass, wrappedPosition);

            vec2 X0 = POST_UNPACK(data.xy) + translatedPosition;
            vec2 V0 = POST_UNPACK(IMG_PIXEL(bufferB, wrappedPosition).xy);
            float M0 = data.z;
            vec2 dx = X0 - position;

            float K = gaussian(dx, radius * INV_SQRT_2) / radius;
            rho += M0 * K;
            vel += M0 * K * V0;
        }

        vel /= rho;

        gl_FragColor = vec4(vel, rho, 1);
    }
    else // Shadertoy Image
    {
        vec2 wrappedPosition = mod(position.xy, RENDERSIZE);
        float rho = IMG_NORM_PIXEL(bufferA_positionAndMass, wrappedPosition / RENDERSIZE).z;

        gl_FragColor = vec4(sin(rho * 1.2 * vec3(1, 2, 3)), 1);
    }
}
