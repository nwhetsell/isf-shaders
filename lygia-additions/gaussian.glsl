#include "../lygia/math/gaussian.glsl"
#include "const.glsl"

#ifndef FNC_GAUSSIAN_ADDITIONS
#define FNC_GAUSSIAN_ADDITIONS

float gaussian(float d) { return gaussian(d, SQRT1_2); }
float gaussian( vec2 d) { return gaussian(d, SQRT1_2); }
float gaussian( vec3 d) { return gaussian(d, SQRT1_2); }
float gaussian( vec4 d) { return gaussian(d, SQRT1_2); }

#endif
