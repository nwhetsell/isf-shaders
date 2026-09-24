#ifndef FNC_OPREPEAT_ADDITIONS
#define FNC_OPREPEAT_ADDITIONS

float opRepeat( in float p, in float s ) {
    return mod(p+s*0.5,s)-s*0.5;
}

vec2 opRepeat( in vec2 p, in vec2 s ) {
    return mod(p+s*0.5,s)-s*0.5;
}

#endif
