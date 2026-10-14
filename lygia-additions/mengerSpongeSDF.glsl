#include "../lygia/sdf/crossSDF.glsl"
#include "../lygia/sdf/cubeSDF.glsl"
#include "../lygia/sdf/opIntersection.glsl"
#include "../lygia/sdf/opRepeat.glsl"
#include "../lygia/math/mmin.glsl"
#include "crossSDF.glsl"

/*
contributors: [Poisson](https://www.shadertoy.com/user/Poisson)
description: |
    Returns a [Menger sponge](https://en.wikipedia.org/wiki/Menger_sponge) SDF

use: mengerSpongeSDF(<vec2> st)
examples:
    - https://www.shadertoy.com/view/ctsSzH
*/

#ifndef MENGERSPONGE_ITERATIONS
#define MENGERSPONGE_ITERATIONS 6
#endif

#ifndef MENGERSPONGE_TRANSFORM_FNC
#define MENGERSPONGE_TRANSFORM_FNC(fi, st) st
#endif

#ifndef FNC_MENGERSPONGESDF
#define FNC_MENGERSPONGESDF

float mengerSpongeSDF( in vec3 st, in float scale, in float initialDistance ) {
    float dist = initialDistance;

    for (int i = 0; i < MENGERSPONGE_ITERATIONS; i++) {
        st = MENGERSPONGE_TRANSFORM(float(i), st);
        vec3 r = opRepeat(st, vec3(2. / scale)) * scale;
        scale *= 3.;
        dist = opIntersection(dist, -crossSDF(r * 3.) / scale);
    }

    return dist;
}

float mengerSpongeSDF( in vec3 st, in float scale ) {
    return mengerSpongeSDF(st, scale, cubeSDF(st, 1.));
}

float mengerSpongeSDF( in vec3 st ) {
    return mengerSpongeSDF(st, 1.);
}

#endif
