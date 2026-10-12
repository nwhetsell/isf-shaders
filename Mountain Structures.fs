/*{
    "CATEGORIES": [
        "Filter",
        "Generator"
    ],
    "CREDIT": "Poisson <https://www.shadertoy.com/user/Poisson>",
    "DESCRIPTION": "Path-traced Menger sponge, converted from <https://www.shadertoy.com/view/ctsSzH>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "bufferA",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {

        }
    ]
}*/
#define GAMMA 0.4545
/*
contributors: Patricio Gonzalez Vivo
description: Convert from gamma to linear color space.
use: gamma2linear(<float|vec3|vec4> color)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_GAMMA2LINEAR 
float gamma2linear(const in float v) {
    return pow(v, GAMMA);
}
vec3 gamma2linear(const in vec3 v) {
    return pow(v, vec3(GAMMA));
}
vec4 gamma2linear(const in vec4 v) {
    return vec4(gamma2linear(v.rgb), v.a);
}
/*
contributors: Patricio Gonzalez Vivo
description: clamp a value between 0 and 1
use: <float|vec2|vec3|vec4> saturation(<float|vec2|vec3|vec4> value)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/math_functions.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_SATURATE 
#define saturate(V) clamp(V, 0.0, 1.0)

/*
contributors: Narkowicz 2015
description: ACES Filmic Tone Mapping Curve. https://knarkowicz.wordpress.com/2016/01/06/aces-filmic-tone-mapping-curve/
use: <vec3|vec4> tonemapACES(<vec3|vec4> x)
*/
#define FNC_TONEMAPACES 
vec3 tonemapACES(vec3 v) {
    const float a = 2.51;
    const float b = 0.03;
    const float c = 2.43;
    const float d = 0.59;
    const float e = 0.14;
    return saturate((v*(a*v+b))/(v*(c*v+d)+e));
}
vec4 tonemapACES(in vec4 v) {
    return vec4(tonemapACES(v.rgb), v.a);
}
#define RANDOM_HIGHER_RANGE 
#define RANDOM_SINLESS 
/*
contributors: ["Patricio Gonzalez Vivo", "David Hoskins", "Inigo Quilez"]
description: Pass a value and get some random normalize value between 0 and 1
use: float random[2|3](<float|vec2|vec3> value)
options:
    - RANDOM_HIGHER_RANGE: for working with a range over 0 and 1
    - RANDOM_SINLESS: Use sin-less random, which tolerates bigger values before producing pattern. From https://www.shadertoy.com/view/4djSRW
    - RANDOM_SCALE: by default this scale if for number with a big range. For producing good random between 0 and 1 use bigger range
examples:
    - /shaders/generative_random.frag
license:
    - MIT License (MIT) Copyright 2014, David Hoskins
*/
#define RANDOM_SCALE vec4(.1031, .1030, .0973, .1099)
#define FNC_RANDOM 
float random(in float x) {
    x = fract(x * RANDOM_SCALE.x);
    x *= x + 33.33;
    x *= x + x;
    return fract(x);
}
float random(in vec2 st) {
    vec3 p3 = fract(vec3(st.xyx) * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}
float random(in vec3 pos) {
    pos = fract(pos * RANDOM_SCALE.xyz);
    pos += dot(pos, pos.zyx + 31.32);
    return fract((pos.x + pos.y) * pos.z);
}
float random(in vec4 pos) {
    pos = fract(pos * RANDOM_SCALE);
    pos += dot(pos, pos.wzxy + 33.33);
    return fract((pos.x + pos.y) * (pos.z + pos.w));
}
vec2 random2(float p) {
    vec3 p3 = fract(vec3(p) * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xx + p3.yz) * p3.zy);
}
vec2 random2(vec2 p) {
    vec3 p3 = fract(p.xyx * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xx + p3.yz) * p3.zy);
}
vec2 random2(vec3 p3) {
    p3 = fract(p3 * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xx + p3.yz) * p3.zy);
}
vec3 random3(float p) {
    vec3 p3 = fract(vec3(p) * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.xxy + p3.yzz) * p3.zyx);
}
vec3 random3(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * RANDOM_SCALE.xyz);
    p3 += dot(p3, p3.yxz + 19.19);
    return fract((p3.xxy + p3.yzz) * p3.zyx);
}
vec3 random3(vec3 p) {
    p = fract(p * RANDOM_SCALE.xyz);
    p += dot(p, p.yxz + 19.19);
    return fract((p.xxy + p.yzz) * p.zyx);
}
vec4 random4(float p) {
    vec4 p4 = fract(p * RANDOM_SCALE);
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
vec4 random4(vec2 p) {
    vec4 p4 = fract(p.xyxy * RANDOM_SCALE);
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
vec4 random4(vec3 p) {
    vec4 p4 = fract(p.xyzx * RANDOM_SCALE);
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
vec4 random4(vec4 p4) {
    p4 = fract(p4 * RANDOM_SCALE);
    p4 += dot(p4, p4.wzxy + 19.19);
    return fract((p4.xxyz + p4.yzzw) * p4.zywx);
}
/*
contributors: Patricio Gonzalez Vivo
description: clamp a value between 0 and 1
use: <float|vec2|vec3|vec4> saturation(<float|vec2|vec3|vec4> value)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/math_functions.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
/*
contributors: Patricio Gonzalez Vivo
description: returns a 2x2 rotation matrix
use: <mat2> rotate2d(<float> radians)
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_ROTATE2D 
mat2 rotate2d(const in float r){
    float c = cos(r);
    float s = sin(r);
    return mat2(c, s, -s, c);
}
/*
contributors: Patricio Gonzalez Vivo
description: 'Fix the aspect ratio of a space keeping things squared for you.'
use: <vec2> aspect(<vec2> st, <vec2> st_size)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/draw_shapes.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_ASPECT 
vec2 aspect(vec2 st, vec2 s) {
    st.x = st.x * (s.x / s.y);
    return st;
}
/*
contributors: Patricio Gonzalez Vivo
description: "It center the coordinates from 0 to 1 to -1 to 1\nSo the center goes\
    \ from 0.5 to 0.0. \n"
use: <float|vec2|vec3> center(<float|vec2|vec3> st)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/draw_shapes.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_CENTER 
float center(float x) { return x * 2.0 - 1.0; }
vec2 center(vec2 v) { return v * 2.0 - 1.0; }
vec3 center(vec3 v) { return v * 2.0 - 1.0; }
#define LOOK_AT_RIGHT_HANDED 
/*
contributors: Patricio Gonzalez Vivo
description: create a look at matrix. Right handed by default.
use:
    - <mat3> lookAt(<vec3> forward, <vec3> up)
    - <mat3> lookAt(<vec3> eye, <vec3> target, <vec3> up)
    - <mat3> lookAt(<vec3> eye, <vec3> target, <float> roll)
    - <mat3> lookAt(<vec3> forward)
options:
    - LOOK_AT_LEFT_HANDED: assume a left-handed coordinate system
    - LOOK_AT_RIGHT_HANDED: assume a right-handed coordinate system
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_LOOKAT 
mat3 lookAt(vec3 forward, vec3 up) {
    vec3 zaxis = normalize(forward);
    vec3 xaxis = normalize(cross(zaxis, up));
    vec3 yaxis = cross(xaxis, zaxis);
    return mat3(xaxis, yaxis, zaxis);
}
mat3 lookAt(vec3 eye, vec3 target, vec3 up) {
    vec3 forward = normalize(target - eye);
    return lookAt(forward, up);
}
mat3 lookAt(vec3 eye, vec3 target, float roll) {
    vec3 up = vec3(sin(roll), cos(roll), 0.0);
    return lookAt(eye, target, up);
}
mat3 lookAt(vec3 forward) {
    return lookAt(forward, vec3(0.0, 1.0, 0.0));
}

float seed;
vec2 random2(void)
{
    return random2(seed += 0.1);
}
vec3 random3(void)
{
    return random3(seed += 0.1);
}
// normalized random 3d vector
vec3 uniformVector() {
    return normalize(center(random3()));
}
// scene sdf (menger sponge)
float map(vec3 p) {
    vec3 q = abs(p);
    float d = p.y; // distance
    float s = .6; // scale
    for (int i=0; i<6; i++) { // 6 iteration
        // transformation
        p.yz *= rotate2d(-float(i)*2.);
        p.xz *= rotate2d(-float(i));
        vec3 a = mod(p*s, 2.) - 1.;
        s *= 3.;
        vec3 r = 1. - 3.*abs(a);
        r = abs(r);
        // cross distance
        float da = max(r.x,r.y);
        float db = max(r.y,r.z);
        float dc = max(r.z,r.x);
        float c = min(da,min(db,dc)) - 1.;
        d = max(d, c/s);
    }
    // return the distance
    return min(d,12.);
}
// raymarching function
float intersect(vec3 ro, vec3 rd) {
    float t = 0.; // distance travelled
    for (int i=0; i<256; i++) { // marching loop
        vec3 p = ro + rd*t; // current point
        float h = map(p); // distance to the scene
        if (h<.001) break; // we hit the surface
        t += h; // march
        if (t>12.) return -1.;
    }
    // return the distance
    return t;
}
// normal estimation
vec3 calcNormal(vec3 p) {
    float h = map(p);
    vec2 e = vec2(h,0); // epsilon
    return normalize(h - vec3(map(p-e.xyy),
                              map(p-e.yxy),
                              map(p-e.yyx)));
}
// shadow function
float shadow(vec3 ro, vec3 rd) {
    for (float t=0.; t<4.;) {
        vec3 p = ro + rd*t;
        float h = map(p);
        if (h<.001) return 0.;
        t += h;
    }
    return 1.;
}
// rendering
vec3 render(vec3 ro, vec3 rd) {
    // light (sun) direction
    vec3 lig = normalize(vec3(2,1,1)+.1*uniformVector());
    vec3 bgCol = vec3(.6,.8,1); // background color
    float fdis = 0.; // final distance
    vec3 frd; // final ray direction
    vec3 col = vec3(0); // accumulated color
    vec3 emit = vec3(1); // color mask
    for (int i=0; i<5; i++) { // 5 bounces of gi
        float t = intersect(ro, rd);
        if (i==0) {fdis = t; frd = rd;}
        if (t<0.) { // we don't hit the surface
            if (i==0) col = bgCol;
            break;
        }
        vec3 p = ro + rd*t; // hit point
        vec3 n = calcNormal(p); // surface normal
        ro = p+n*.002; // new ray origin
        rd = normalize(n+uniformVector()); // new ray direction, cheap diffuse BRDF
        emit *= vec3(.8,.9,1)*.9; // color the object
        float dif = clamp(dot(n,lig), 0., 1.); // diffuse light
        float sha = shadow(ro, lig); // shadow
        // lighting
        col += emit*dif*sha;
    }
    col = mix(col, bgCol, 1.-exp(-fdis*fdis*fdis*.001)); // fog
    col += 3.*pow(clamp(dot(frd,lig),0.,1.),3.5); // sun glare
    return col;
}
// bloom function
vec4 bloom(sampler2D tex) {
    // normalized pixel coordinates
    vec2 p = gl_FragCoord.xy / RENDERSIZE;
    vec4 col = vec4(0); // accumulated color
    const int N = 8; // quality
    for (int i=-N; i<=N; i++)
    for (int j=-N; j<=N; j++) {
        vec2 off = vec2(i,j) / float(N); // blur offset
        if (dot(off, off)<1.) { // inside disk
            // blurred texture
            col += textureLod(tex, p+.05*off, 4.);
        }
    }
    // output
    return col/col.a;
}
void main()
{
    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        seed = TIME + dot(sin(gl_FragCoord.xy), vec2(453.712, 983.234));
        vec2 off = random2()-.5; // AA offset
        // // pixel coordinates centered at the origin
        vec2 p = 0.5 * aspect(center((gl_FragCoord.xy + off) / RENDERSIZE), RENDERSIZE);
        vec3 ro = vec3(-1,.9,1.6); // ray origin
        mat3 ca = lookAt(ro, vec3(0), vec3(0, 1, 0));
        vec3 rd = ca * normalize(vec3(p,1.5)); // ray direction
        // depth of field
        float t = intersect(ro, ca[2]); // focus distance
        vec3 fp = ro + rd*t; // focus plane
        ro += uniformVector()*.05;
        rd = normalize(fp-ro);
        // render
        vec3 col = render(ro, rd);
        // blend the color with the previous frame color
        gl_FragColor = vec4(col, 1) + IMG_THIS_PIXEL(bufferA);
    }
    else // Shadertoy Image
    {
        // base color
        vec4 data = IMG_THIS_PIXEL(bufferA);
        vec3 col = data.rgb / data.a;
        // post processing
        col = gamma2linear(col); // gamma correction
        col = col * 1.1 - 0.1; // darkening
        // bloom
        vec3 bloom = bloom(bufferA).rgb;
        col += 0.4 * bloom;
        col = saturate(col);
        col = col*col * (3. - 2. * col); // constrast
        col = tonemapACES(col); // tonemapping
        gl_FragColor = vec4(col, 1);
    }
}
