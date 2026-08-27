/*{
    "CATEGORIES": [
        "Filter",
        "Generator"
    ],
    "CREDIT": "Mykhailo Moroz <https://www.shadertoy.com/user/michael0884>",
    "DESCRIPTION": "Water flowing infinitely inside a looped space without gravity, converted from <https://www.shadertoy.com/view/ttBcWm>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
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
            "NAME": "col0",
            "LABEL": "Color 1",
            "TYPE": "color",
            "DEFAULT": [1, 0.5, 0, 1]
        },
        {
            "NAME": "col1",
            "LABEL": "Color 2",
            "TYPE": "color",
            "DEFAULT": [0.1, 0.4, 1, 1]
        },
        {
            "NAME": "mass",
            "LABEL": "Initial mass",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "dt",
            "LABEL": "Simulation speed",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "viscosity",
            "LABEL": "Viscosity",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "fluid_rho",
            "LABEL": "Fluid rho",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "GF",
            "LABEL": "Water pressure",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": 0
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
            "NAME": "gravity",
            "LABEL": "Gravity",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 1,
            "MIN": -1
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
            "NAME": "border_h",
            "LABEL": "Border",
            "TYPE": "float",
            "DEFAULT": 5,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "specularAmount",
            "LABEL": "Specular amount",
            "TYPE": "float",
            "DEFAULT": 3,
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

// #define ISF_EDITOR_WEBSITE
#ifdef ISF_EDITOR_WEBSITE
#define tanh(x) (2. / (1. + exp(-2. * (x))) - 1.)
#endif

#include "lygia/color/luminance.glsl"
#define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#include "lygia/generative/random.glsl" // LYGIA’s random2 isn’t exactly the same as the RNG in the Shadertoy shader.
#include "lygia/math/const.glsl"
#include "lygia/math/gaussian.glsl"
#define INV_SQRT_2 0.7071067811865475244008443621048
#include "lygia/sdf/rectSDF.glsl"
float rectSDF_without_transform(vec2 p, vec2 b) {
    // For unclear reasons, the LYGIA function shifts by 0.5 and scales by 4.2.
    return rectSDF((p + 0.5) / 4.2, b, 0.);
}
#include "lygia/space/polar2cart.glsl"


//
// Shadertoy Common
//
float Pf(vec2 rho)
{
    return mix(
        0.5 * rho.x,
        0.04 * rho.x * (rho.x / fluid_rho - 1.),
        GF // Water pressure
    );
}

float border(vec2 p)
{
    float bound = -rectSDF_without_transform(p - RENDERSIZE * 0.5, RENDERSIZE * vec2(0.5, 0.5));
    float box = rectSDF_without_transform(p - RENDERSIZE * vec2(0.5, 0.6), RENDERSIZE * vec2(0.05, 0.01));
    float drain = -rectSDF_without_transform(p - RENDERSIZE * vec2(0.5, 0.7), RENDERSIZE * vec2(1.5, 2.5));
    return max(drain, min(bound, box));
}

#define h 1.
vec3 bN(vec2 p)
{
    vec3 r = vec3( 1./h,     0, 0.25) * border(p + vec2( h,  0)) +
             vec3(-1./h,     0, 0.25) * border(p + vec2(-h,  0)) +
             vec3(    0,  1./h, 0.25) * border(p + vec2( 0,  h)) +
             vec3(    0, -1./h, 0.25) * border(p + vec2( 0, -h));
    return vec3(normalize(r.xy), r.z + EPSILON);
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


struct particle
{
    vec2 X;
    vec2 V;
    vec2 M;
};

particle getParticle(vec4 positionAndMass, vec4 velocity, vec2 pos)
{
    particle P;
    P.X = POST_UNPACK(positionAndMass.xy) + pos;
    P.V = POST_UNPACK(velocity.xy);
    P.M = positionAndMass.zw;
    return P;
}

// diffusion amount
vec3 distribution(vec2 x, vec2 p, float K)
{
    vec4 aabb0 = vec4(p - 0.5, p + 0.5);
    vec4 aabb1 = vec4(x - K * 0.5, x + K * 0.5);
    vec4 aabbX = vec4(max(aabb0.xy, aabb1.xy), min(aabb0.zw, aabb1.zw));
    vec2 center = 0.5 * (aabbX.xy + aabbX.zw); // center of mass
    vec2 size = max(aabbX.zw - aabbX.xy, 0.); // only positive
    float m = size.x * size.y / (K * K); // relative amount
    // if any of the dimensions are 0 then the mass is 0
    return vec3(center, m);
}


void main()
{
    vec2 position = gl_FragCoord.xy;

    if (PASSINDEX == 0 || PASSINDEX == 1) // Shadertoy Buffer A
    {
        particle P;
        P.X = vec2(0);
        P.V = vec2(0);
        P.M = vec2(0);

        // Diffusion and advection: basically integrate over all updated
        // neighbor distributions that fall inside of this pixel. This makes the
        // tracking conservative.
        for (int i = -2; i <= 2; i++)
        for (int j = -2; j <= 2; j++) {
            vec2 translatedPosition = position + vec2(i, j);
            vec2 wrappedPosition = mod(translatedPosition, RENDERSIZE);

            particle P0 = getParticle(
                IMG_PIXEL(bufferA_positionAndMass, wrappedPosition),
                IMG_PIXEL(bufferB, wrappedPosition),
                translatedPosition
            );

            P0.X += P0.V * dt; //integrate position

            float difR = 0.9 + 0.21 * smoothstep(fluid_rho * 0., fluid_rho / 3., P0.M.x);
            vec3 D = distribution(P0.X, position, difR);
            // the deposited mass into this cell
            float m = P0.M.x * D.z;

            // add weighted by mass
            P.X += D.xy * m;
            P.V += P0.V * m;
            P.M.y += P0.M.y * m;

            // add mass
            P.M.x += m;
        }

        // normalization
        if (P.M.x != 0.) {
            P.X /= P.M.x;
            P.V /= P.M.x;
            P.M.y /= P.M.x;
        }

        // initial condition
        if (FRAMEINDEX < 2 || restart) {
            P.X = position;

            vec3 rand = random3(position);
            if (rand.z < 0.2) {
                P.V = 0.5 * (rand.xy - 0.5) + vec2(sin(2. * position.x / RENDERSIZE.x), cos(2. * position.x / RENDERSIZE.x));
                P.M = vec2(mass, 0.5 - 0.5 * sin(10. * position.x / RENDERSIZE.x));
            } else {
                P.V = vec2(0);
                P.M = vec2(EPSILON);
            }

            P.M = mix(P.M, vec2(luminance(IMG_PIXEL(inputImage, position))), inputImageAmount);
        }

        if (PASSINDEX == 0) {
            P.X = clamp(P.X - position, vec2(-0.5), vec2(0.5));
            gl_FragColor = vec4(PRE_PACK(P.X), P.M);
        } else {
            gl_FragColor = vec4(PRE_PACK(P.V), 0, 1);
        }
    }
    else if (PASSINDEX == 2) // Shadertoy Buffer B
    {
        vec2 wrappedPosition = mod(position, RENDERSIZE);
        particle P = getParticle(
            IMG_PIXEL(bufferA_positionAndMass, wrappedPosition),
            IMG_PIXEL(bufferA_velocity, wrappedPosition),
            position
        );

        if (P.M.x != 0.) { // not vacuum
            // Compute the SPH force
            vec2 F = vec2(0);
            vec3 avgV = vec3(0);
            for (int i = -2; i <= 2; i++)
            for (int j = -2; j <= 2; j++) {
                vec2 translatedPosition = position + vec2(i, j);
                vec2 wrappedPosition = mod(translatedPosition, RENDERSIZE);
                particle P0 = getParticle(
                    IMG_PIXEL(bufferA_positionAndMass, wrappedPosition),
                    IMG_PIXEL(bufferA_velocity, wrappedPosition),
                    translatedPosition
                );
                vec2 dx = P0.X - P.X;
                float avgP = 0.5 * P0.M.x * (Pf(P.M) + Pf(P0.M));
                F -= 0.5 * gaussian(dx, INV_SQRT_2) * avgP * dx;
                avgV += P0.M.x * gaussian(dx, INV_SQRT_2) * vec3(P0.V, 1);
            }
            avgV.xy /= avgV.z;

            // viscosity
            F += viscosity * P.M.x * (avgV.xy - P.V);

            // gravity
            F += P.M.x * vec2(0, gravity);

            if (enableMouse) {
                float d = distance(mouse.xy * RENDERSIZE, P.X) / 20.;
                F += exp(-d * d);
            }

            // integrate
            P.V += F * dt / P.M.x;

            // border
            vec3 N = bN(P.X);
            float vdotN = step(N.z, border_h) * dot(-N.xy, P.V);
            P.V += 0.5 * (N.xy * vdotN + N.xy * abs(vdotN));

            if (N.z < 0.) {
                P.V = vec2(0);
            }

            // velocity limit
            float v = length(P.V);
            if (v > maxSpeed) {
                P.V /= v;
            }
        }

        gl_FragColor = vec4(PRE_PACK(P.V), 0, 1);
    }
    else if (PASSINDEX == 3) // Shadertoy Buffer C
    {
        vec2 wrappedPosition = mod(position, RENDERSIZE);
        particle P = getParticle(
            IMG_PIXEL(bufferA_positionAndMass, wrappedPosition),
            IMG_PIXEL(bufferA_velocity, wrappedPosition),
            position
        );

        // particle render
        vec4 rho = vec4(0);
        for (int i = -2; i <= 2; i++)
        for (int j = -2; j <= 2; j++) {
            vec2 translatedPosition = position + vec2(i, j);
            vec2 wrappedPosition = mod(translatedPosition, RENDERSIZE);
            particle P0 = getParticle(
                IMG_PIXEL(bufferA_positionAndMass, wrappedPosition),
                IMG_PIXEL(bufferA_velocity, wrappedPosition),
                translatedPosition
            );

            // how much mass falls into this pixel
            rho += vec4(P.V, P.M) * gaussian(position - P0.X, INV_SQRT_2);
        }

        gl_FragColor = rho;
    }
    else // Shadertoy Image
    {
        vec2 wrappedPosition = mod(position, RENDERSIZE);
        particle P = getParticle(
            IMG_PIXEL(bufferA_positionAndMass, wrappedPosition),
            IMG_PIXEL(bufferB, wrappedPosition),
            position
        );

        // border render
        vec3 Nb = bN(P.X);
        float bord = smoothstep(2. * border_h, border_h * 0.5, border(position));

        vec4 rho = IMG_PIXEL(bufferC, position);
        vec3 dx = vec3(-1, 0, 1);
        vec4 grad = -0.5 * vec4(IMG_PIXEL(bufferC, position + dx.zy).zw - IMG_PIXEL(bufferC, position + dx.xy).zw,
                                IMG_PIXEL(bufferC, position + dx.yz).zw - IMG_PIXEL(bufferC, position + dx.yx).zw);
        vec2 N = pow(length(grad.xz), 0.2) * normalize(grad.xz + EPSILON);
        vec3 n = normalize(vec3(N, 1));
        vec3 r = reflect(vec3(0, 0, 1), n);
        float specularb = gaussian(0.4 * (Nb.zz - border_h), INV_SQRT_2) *
                          pow(max(dot(Nb.xy, polar2cart(vec2(1.4, 1))), 0.), 3.);

        float a = pow(smoothstep(fluid_rho * 0., fluid_rho * 2., rho.z), 0.1);
        float b = exp(-1.7 * smoothstep(fluid_rho * 1., fluid_rho * 7.5, rho.z));

        // Output to screen
        float c = tanh(3. * (rho.w - 1.)) * 0.5 + 0.5;
        gl_FragColor = mix(col0, col1, c) * (1.5 * b + specularb * specularAmount) * a;
        gl_FragColor.rgb = tanh(gl_FragColor.rgb * gl_FragColor.rgb);
        gl_FragColor.a = 1.;
    }
}
