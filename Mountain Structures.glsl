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
#include "lygia/color/space/gamma2linear.glsl"
#include "lygia/color/tonemap/aces.glsl"
#include "lygia/math/saturate.glsl"
#include "lygia/math/rotate2d.glsl"
#include "lygia/space/aspect.glsl"
#include "lygia/space/center.glsl"
#define LOOK_AT_RIGHT_HANDED
#include "lygia/space/lookAt.glsl"


float seed; // randoms seed

// hash functions by me
vec2 hash2() {return fract(sin(seed+=.1)*vec2(8472.5636,9854.4213));}

vec3 hash3() {return fract(sin(seed+=.1)*vec3(7653.1285,6912.8512,5914.7134));}

// normalized random 3d vector
vec3 uniformVector() {
    return normalize(center(hash3()));
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
    for (int j=-N; j<=N; j++)  {
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
        // init randoms seed
        seed = TIME+dot(sin(gl_FragCoord.xy),vec2(453.712,983.234));

        vec2 off = hash2()-.5; // AA offset
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
        vec3 col = data.rgb/data.w;

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
