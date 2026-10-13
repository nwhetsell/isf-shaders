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
        },
        {
            "NAME": "scale",
            "LABEL": "Scale",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MAX": 50,
            "MIN": -50
        },
        {
            "NAME": "feedback",
            "LABEL": "Feedback amount",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "cameraY",
            "LABEL": "Camera y-coordinate",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MAX": 5,
            "MIN": -5
        },
        {
            "NAME": "cameraZ",
            "LABEL": "Camera z-coordinate",
            "TYPE": "float",
            "DEFAULT": 1.6,
            "MAX": 5,
            "MIN": -5
        },
        {
            "NAME": "mainColor",
            "LABEL": "Main color",
            "TYPE": "color",
            "DEFAULT": [0.72, 0.81, 0.9, 1]
        },
        {
            "NAME": "backgroundColor",
            "LABEL": "Background color",
            "TYPE": "color",
            "DEFAULT": [0.6, 0.8, 1, 1]
        },
        {
            "NAME": "lightRadius",
            "LABEL": "Light distance",
            "TYPE": "float",
            "DEFAULT": 2.4494897428,
            "MIN": 0,
            "MAX": 200
        },
        {
            "NAME": "lightPhi",
            "LABEL": "Light phi (degrees)",
            "TYPE": "float",
            "DEFAULT": 65.9051574479,
            "MIN": 0,
            "MAX": 180
        },
        {
            "NAME": "lightTheta",
            "LABEL": "Light theta (degrees)",
            "TYPE": "float",
            "DEFAULT": 26.5650511771,
            "MIN": 0,
            "MAX": 360
        },
        {
            "NAME": "lightDirectionRandomization",
            "LABEL": "Light direction randomization",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "darkenAmount",
            "LABEL": "Darken amount",
            "TYPE": "float",
            "DEFAULT": 1.1,
            "MAX": 10,
            "MIN": 0
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

// The default light parameters are:
//   lightRadius = length(vec3(-1, -3, 1))
//   lightRadius = length(vec3(2, 1, 1))
// 2,1,1
//               = sqrt(2 * 2 + 1 * 1 + 1 * 1)
//               = sqrt(6) ≈ 2.4494897428
//   lightPhi = acos(1 / lightRadius) ≈ 65.9051574479°
//   lightTheta = atan(1 / 2) ≈ 26.5650511771°

#include "lygia/color/blend/darken.glsl"
#define GAMMA 0.4545
#include "lygia/color/space/gamma2linear.glsl"
#include "lygia/color/tonemap/aces.glsl"
#define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#include "lygia/generative/random.glsl"
#define RAYMARCH_BACKGROUND vec3(0)
#define RAYMARCH_SAMPLES 256
#define RAYMARCH_MIN_DIST 0.
#define RAYMARCH_MAX_DIST 12.
#define RAYMARCH_MIN_HIT_DIST 0.001
#include "lygia/lighting/raymarch/cast.glsl"
#include "lygia/lighting/raymarch/normal.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/mmin.glsl"
#include "lygia/math/rotate2d.glsl"
#include "lygia/math/saturate.glsl"
#include "lygia/space/aspect.glsl"
#include "lygia/space/center.glsl"
#define LOOK_AT_RIGHT_HANDED
#include "lygia/space/lookAt.glsl"
#include "lygia/space/polar2cart.glsl"


float seed;

// #define STRICT_RANDOM

vec2 random2(void)
{
#ifdef STRICT_RANDOM
    return fract(sin(seed += 0.1) * vec2(8472.5636, 9854.4213));
#else
    return random2(seed += 0.1);
#endif
}

vec3 random3(void)
{
#ifdef STRICT_RANDOM
    return fract(sin(seed += 0.1) * vec3(7653.1285, 6912.8512, 5914.7134));
#else
    return random3(seed += 0.1);
#endif
}

// normalized random 3d vector
vec3 uniformVector(void) {
    return normalize(center(random3()));
}

// scene sdf (menger sponge)
Material raymarchMap(vec3 p) {
    float d = p.y; // distance

    float s = scale; // scale
    for (int i = 0; i < 6; i++) { // 6 iteration
        // transformation
        p.yz *= rotate2d(-float(i) * 2.);
        p.xz *= rotate2d(-float(i));
        vec3 a = mod(p*s, 2.) - 1.;
        s *= 3.;
        vec3 r = 1. - 3. * abs(a);

        r = abs(r);

        // cross distance
        float da = max(r.x, r.y);
        float db = max(r.y, r.z);
        float dc = max(r.z, r.x);

        float c = mmin(da, db, dc) - 1.;

        d = max(d, c/s);
    }

    Material mat = materialNew();
    mat.position = p;
    mat.sdf = min(d, RAYMARCH_MAX_DIST);
    return mat;
}

// shadow function
float shadow(vec3 ro, vec3 rd) {
    for (float t=0.; t<4.;) {
        vec3 p = ro + rd*t;
        float h = raymarchMap(p).sdf;
        if (h<.001) return 0.;
        t += h;
    }
    return 1.;
}

vec3 render(vec3 ro, vec3 rd)
{
    // light (sun) direction
    vec3 lig = normalize(polar2cart(lightRadius, lightPhi * DEG2RAD, lightTheta * DEG2RAD) + lightDirectionRandomization * uniformVector());

    Material res = raymarchCast(ro, rd);

    float fdis = res.sdf; // final distance
    vec3 frd = rd; // final ray direction

    vec3 col; // accumulated color

    if (res.valid) {
        col = vec3(0);
        vec3 emit = vec3(1); // color mask

        for (int i = 0; i < 5; i++) { // 5 bounces of gi
            float t = res.sdf;

            vec3 p = ro + rd * t; // hit point
// #define STRICT_NORMAL
#ifdef STRICT_NORMAL
            float dist = raymarchMap(p).sdf;
            vec2 e = vec2(dist, 0);
            vec3 n = normalize(dist - vec3(raymarchMap(p - e.xyy).sdf,
                                           raymarchMap(p - e.yxy).sdf,
                                           raymarchMap(p - e.yyx).sdf));
#else
            vec3 n = raymarchNormal(p);
#endif

            ro = p + n * 0.002; // new ray origin
            rd = normalize(n + uniformVector()); // new ray direction, cheap diffuse BRDF
            emit *= mainColor.rgb; // color the object

            float dif = saturate(dot(n, lig)); // diffuse light
            float sha = shadow(ro, lig); // shadow

            // lighting
            col += emit * dif * sha;

            res = raymarchCast(ro, rd);

            if (!res.valid) // we don't hit the surface
                break;
        }
    } else {
        col = backgroundColor.rgb;
    }

    col = mix(col, backgroundColor.rgb, 1. - exp(-fdis*fdis*fdis * 0.001)); // fog
    col += 3. * pow(saturate(dot(frd,lig)), 3.5); // sun glare

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
        seed = TIME + dot(sin(gl_FragCoord.xy), vec2(453.712, 983.234));

        vec2 off = random2()-.5; // AA offset
        // // pixel coordinates centered at the origin
        vec2 p = 0.5 * aspect(center((gl_FragCoord.xy + off) / RENDERSIZE), RENDERSIZE);

        vec3 ro = vec3(-1, cameraY, cameraZ); // ray origin
        mat3 ca = lookAt(ro, vec3(0), vec3(0, 1, 0));

        vec3 rd = ca * normalize(vec3(p,1.5)); // ray direction

        // depth of field
        float t = raymarchCast(ro, ca[2]).sdf; // focus distance
        vec3 fp = ro + rd*t; // focus plane
        ro += uniformVector()*.05;
        rd = normalize(fp-ro);

        // render
        vec3 col = render(ro, rd);

        // blend the color with the previous frame color
        gl_FragColor = vec4(col, 1) + IMG_THIS_PIXEL(bufferA) * feedback;
    }
    else // Shadertoy Image
    {
        // base color
        vec4 data = IMG_THIS_PIXEL(bufferA);
        vec3 col = data.rgb / data.a;

        // post processing

        col = gamma2linear(col);
        col = blendDarken(vec3(1), col, darkenAmount);

        // bloom
        vec3 bloom = bloom(bufferA).rgb;
        col += 0.4 * bloom;
        col = saturate(col);

        col = col*col * (3. - 2. * col); // constrast
        col = tonemapACES(col); // tonemapping

        gl_FragColor = vec4(col, 1);
    }
}
