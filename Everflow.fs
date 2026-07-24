/*{
    "CATEGORIES": [
        "Filter",
        "Generator"
    ],
    "CREDIT": "Mykhailo Moroz <https://www.shadertoy.com/user/michael0884>",
    "DESCRIPTION": "Water flowing infinitely inside a looped space without gravity, converted from <https://www.shadertoy.com/view/ttBcWm>",
    "INPUTS": [
        {
            "NAME" : "inputImage",
            "TYPE" : "image"
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

#define INV_SQRT_2 0.7071067811865475244008443621048

// Constants and functions from LYGIA <https://github.com/patriciogonzalezvivo/lygia>
#define PI 3.1415926535897932384626433832795

float gaussian( vec2 d, float s) { return exp(-( d.x*d.x + d.y*d.y) / (2.0 * s*s)); }

vec2 polar2cart(in vec2 polar) {
    return vec2(cos(polar.x), sin(polar.x)) * polar.y;
}

float rectSDF(vec2 p, vec2 b, float r) {
    vec2 d = abs(p - 0.5) * 4.2 - b + vec2(r);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - r;
}
float rectSDF(vec2 p, vec2 b) {
    // Why the LYGIA function shifts by 0.5 and scales by 4.2 is a complete mystery.
    return rectSDF((p + 0.5) / 4.2, b, 0.);
}



// Hash function from <https://www.shadertoy.com/view/4djSRW>, MIT-licensed:
//
// Copyright © 2014 David Hoskins.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
vec3 hash32(vec2 p)
{
	vec3 p3 = fract(vec3(p.xyx) * vec3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yxz+33.33);
    return fract((p3.xxy+p3.yzz)*p3.zyx);
}


//
// ShaderToy Common
//
#define dt 1.5
#define border_h 5.
#define mass 1.
#define fluid_rho 0.5

float Pf(vec2 rho)
{
    // Water pressure
    float GF = 1.;
    return mix(0.5 * rho.x, 0.04 * rho.x * (rho.x / fluid_rho - 1.), GF);
}

float border(vec2 p)
{
    float bound = -rectSDF(p - RENDERSIZE * 0.5, RENDERSIZE * vec2(0.5, 0.5));
    float box = rectSDF(p - RENDERSIZE * vec2(0.5, 0.6), RENDERSIZE * vec2(0.05, 0.01));
    float drain = -rectSDF(p - RENDERSIZE * vec2(0.5, 0.7), RENDERSIZE * vec2(1.5, 2.5));
    return max(drain, min(bound, box));
}

#define h 1.
vec3 bN(vec2 p)
{
    vec3 r = vec3( 1./h,     0, 0.25) * border(p + vec2( h,  0)) +
             vec3(-1./h,     0, 0.25) * border(p + vec2(-h,  0)) +
             vec3(    0,  1./h, 0.25) * border(p + vec2( 0,  h)) +
             vec3(    0, -1./h, 0.25) * border(p + vec2( 0, -h));
    return vec3(normalize(r.xy), r.z + 1e-4);
}


// The ShaderToy shader uses the functions `floatBitsToUint` and
// `uintBitsToFloat` to pack more than 4 floats (5 in this case) into a
// 4-component pixel. These functions are available in GLSL v3.30 (OpenGL v3.3)
// and later, but some ISF hosts (notably Videosync) use GLSL v1.50
// (OpenGL v3.2). We can work around this by effectively running ShaderToy
// buffers twice, but the packing operations in the ShaderToy shader also
// perform a `clamp` on the packed data. Without the `clamp` calls, this shader
// seems to blow up numerically.
#define POST_UNPACK(X) (clamp(X, 0., 1.) * 2. - 1.)
#define PRE_PACK(X) clamp(0.5 * X + 0.5, 0., 1.)


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


vec4 V(vec2 p)
{
    return IMG_NORM_PIXEL(bufferC, p/RENDERSIZE);
}


void main()
{
    vec2 position = gl_FragCoord.xy;

    if (PASSINDEX == 0 || PASSINDEX == 1) // ShaderToy Buffer A
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

            float difR = 0.9 + 0.21*smoothstep(fluid_rho * 0., fluid_rho * 0.333, P0.M.x);
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
        if (FRAMEINDEX < 1) {
            P.X = position;

            // random
            vec3 rand = hash32(position);
            if(rand.z < 0.2) {
                P.V = 0.5 * (rand.xy - 0.5) + vec2(sin(2. * position.x / RENDERSIZE.x), cos(2. * position.x / RENDERSIZE.x));
                P.M = vec2(mass, 0.5 - 0.5 * sin(10. * position.x / RENDERSIZE.x));
            }
            else {
                P.V = vec2(0);
                P.M = vec2(1e-6);
            }
        }

        if (PASSINDEX == 0) {
            P.X = clamp(P.X - position, vec2(-0.5), vec2(0.5));
            gl_FragColor = vec4(PRE_PACK(P.X), P.M);
        } else {
            gl_FragColor = vec4(PRE_PACK(P.V), 0, 1);
        }
    }
    else if (PASSINDEX == 2) // ShaderToy Buffer B
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

            // if (Mouse.z > 0.) {
            //     vec2 dm =(Mouse.xy - Mouse.zw) / 10.;
            //     float d = distance(Mouse.xy, P.X) / 20.;
            //     F += 0.001 * dm * exp(-d * d);
            // }

            //integrate
            P.V += F * dt / P.M.x;

            //border
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
    else if (PASSINDEX == 3) // ShaderToy Buffer C
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
    else // ShaderToy Image
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

        vec4 rho = V(position);
        vec3 dx = vec3(-1, 0, 1);
        vec4 grad = -0.5 * vec4(V(position + dx.zy).zw - V(position + dx.xy).zw,
                                V(position + dx.yz).zw - V(position + dx.yx).zw);
        vec2 N = pow(length(grad.xz), 0.2) * normalize(grad.xz + 1e-5);
        vec3 n = normalize(vec3(N, 1));
        vec3 r = reflect(vec3(0, 0, 1), n);
        float specularb = gaussian(0.4 * (Nb.zz - border_h), INV_SQRT_2) *
                          pow(max(dot(Nb.xy, polar2cart(vec2(1.4, 1))), 0.), 3.);

        float a = pow(smoothstep(fluid_rho * 0., fluid_rho * 2., rho.z), 0.1);
        float b = exp(-1.7 * smoothstep(fluid_rho * 1., fluid_rho * 7.5, rho.z));
        vec3 col0 = vec3(1, 0.5, 0);
        vec3 col1 = vec3(0.1, 0.4, 1);

        // Output to screen
#ifndef VIDEOSYNC
#define tanh(x) (2. / (1. + exp(-2. * (x))) - 1.)
#endif
        float c = tanh(3. * (rho.w - 1.)) * 0.5 + 0.5;
        gl_FragColor.xyz = mix(col0, col1, c) * (1.5 * b + specularb * specularAmount) * a;
        gl_FragColor.xyz = tanh(gl_FragColor.xyz * gl_FragColor.xyz);
        gl_FragColor.a = 1.;
    }
}
