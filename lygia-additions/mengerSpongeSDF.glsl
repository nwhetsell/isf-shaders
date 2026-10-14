#include "../lygia/math/mmin.glsl"

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

float mengerSpongeSDF( in vec3 st, float scale ) {
    float dist = st.y;

    float s = scale;
    for (int i = 0; i < MENGERSPONGE_ITERATIONS; i++) {
        st = MENGERSPONGE_TRANSFORM(float(i), st);

        vec3 a = mod(st * s, 2.) - 1.;
        s *= 3.;
        vec3 r = 1. - 3. * abs(a);

        r = abs(r);

        // cross distance
        float da = max(r.x, r.y);
        float db = max(r.y, r.z);
        float dc = max(r.z, r.x);

        float c = mmin(da, db, dc) - 1.;

        dist = max(dist, c / s);
    }

   return dist;
}

float mengerSpongeSDF( in vec3 st ) {
    return mengerSpongeSDF(st, 1.);
}

#endif
