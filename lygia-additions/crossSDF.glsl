#ifndef FNC_CROSSSDF_ADDITIONS
#define FNC_CROSSSDF_ADDITIONS

float crossSDF(vec3 st) {
    vec3 d = abs(st);
    return mmin(max(d.x, d.y), max(d.y, d.z), max(d.x, d.z)) - 1.;
}

#endif
