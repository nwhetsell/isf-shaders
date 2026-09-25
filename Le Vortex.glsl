/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "Leon Denise <https://www.shadertoy.com/user/leon>",
    "DESCRIPTION": "Tribute to Marc-Antoine Mathieu, converted from <https://www.shadertoy.com/view/XlfBR7>",
    "INPUTS": [
        {
            "NAME": "donut",
            "LABEL": "Outer radius",
            "TYPE": "float",
            "DEFAULT": 30,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "cell",
            "LABEL": "Room length",
            "TYPE": "float",
            "DEFAULT": 4,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "height",
            "LABEL": "Room depth",
            "TYPE": "float",
            "DEFAULT": 2,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "thin",
            "LABEL": "Wall thickness",
            "TYPE": "float",
            "DEFAULT": 0.04,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "radius",
            "LABEL": "Inner radius",
            "TYPE": "float",
            "DEFAULT": 15,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "speed",
            "LABEL": "Speed",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "cameraX",
            "LABEL": "Camera x",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "cameraY",
            "LABEL": "Camera y",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "cameraZ",
            "LABEL": "Camera z",
            "TYPE": "float",
            "DEFAULT": -20,
            "MAX": 100,
            "MIN": -100
        },
        {
            "NAME": "yAxisRotation",
            "LABEL": "y-axis rotation",
            "TYPE": "float",
            "DEFAULT": 22.5,
            "MAX": 180,
            "MIN": -180
        },
        {
            "NAME": "xAxisRotation",
            "LABEL": "x-axis rotation",
            "TYPE": "float",
            "DEFAULT": 30,
            "MAX": 180,
            "MIN": -180
        },
        {
            "NAME": "boxHeight",
            "LABEL": "Box height",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "boxToroidalSeparation",
            "LABEL": "Box toroidal separation",
            "TYPE": "float",
            "DEFAULT": 0.43,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "boxPoloidalSeparation",
            "LABEL": "Box poloidal separation",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "boxProportion",
            "LABEL": "Box proportion",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "windowGrilleThickness",
            "LABEL": "Window grille thickness",
            "TYPE": "float",
            "DEFAULT": 0.008,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "windowGrilleDepth",
            "LABEL": "Window grille depth",
            "TYPE": "float",
            "DEFAULT": 0.04,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "windowFrameArea",
            "LABEL": "Window frame area",
            "TYPE": "float",
            "DEFAULT": 0.08,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "windowFrameDepth",
            "LABEL": "Window frame depth",
            "TYPE": "float",
            "DEFAULT": 0.006,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "backgroundColor",
            "LABEL": "Background color",
            "TYPE": "color",
            "DEFAULT": [0, 0, 0, 0]
        }
    ],
    "ISFVSN": "2"
}*/

// #define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#include "lygia/generative/random.glsl" // LYGIA’s random2 isn’t exactly the same as the RNG in the Shadertoy shader.
#include "lygia/math/const.glsl"
#include "lygia/math/rotate2d.glsl"
#include "lygia/sdf/boxSDF.glsl"
#include "lygia/sdf/sphereSDF.glsl"
#include "lygia/space/cart2polar.glsl"
#include "lygia/space/polar2cart.glsl"
#include "lygia-additions/opRepeat.glsl"


// Raymarching sketch inspired by the work of Marc-Antoine Mathieu
// Leon 2017-11-21
// using code from IQ, Mercury, LJ, Duke, Koltes

#define STEPS 250.
#define VOLUME 0.001

float map(vec3);
float getShadow(vec3 pos, vec3 at, float k)
{
    vec3 dir = normalize(at - pos);
    float maxt = length(at - pos);
    float f = 1.;
    float t = VOLUME * 50.;
    for (float i = 0.; i <= 1.; i += 1. / 15.) {
        float dist = map(pos + dir * t);
        if (dist < VOLUME) {
            return 0.;
        }
        f = min(f, k * dist / t);
        t += dist;
        if (t >= maxt) {
            break;
        }
    }
    return f;
}

vec3 getNormal(vec3 p)
{
    return normalize(vec3(
        map(p + vec3(EPSILON,0,0)) - map(p - vec3(EPSILON,0,0)),
        map(p + vec3(0,EPSILON,0)) - map(p - vec3(0,EPSILON,0)),
        map(p + vec3(0,0,EPSILON)) - map(p - vec3(0,0,EPSILON))
    ));
}

void camera(inout vec3 p)
{
    p.xz *= rotate2d(-yAxisRotation * DEG2RAD);
    p.yz *= rotate2d(-xAxisRotation * DEG2RAD);
}

float windowGrille(vec3 pos, float height, float width, vec2 indexes)
{
    float randomness = random(indexes);

    vec3 p = pos;
    p.xy = opRepeat(p.xy, vec2(height * (0.6 + randomness * 0.4), width * (0.3 + randomness * 0.7)));
    float scene = boxSDF(p, vec3(windowGrilleThickness, width, windowGrilleDepth) * 2.);
    scene = min(scene, boxSDF(p, vec3(height, windowGrilleThickness, windowGrilleDepth) * 2.));
    scene = max(scene, boxSDF(pos, vec3(height, width, windowGrilleDepth)));
    return scene;
}

float window(vec3 pos, float height, float width, vec2 indexes)
{
    float frame = boxSDF(pos, vec3(height, width, windowFrameDepth));
    frame = max(frame, -boxSDF(pos, vec3(height - windowFrameArea, width - windowFrameArea, windowFrameDepth * 2.)));
    float scene = windowGrille(pos, height, width, indexes);
    scene = min(scene, frame);
    return scene;
}

float boxes(vec3 pos, vec2 indexes)
{
    float randomness1 = random(indexes);
    vec2 separation = vec2(
        cell * boxToroidalSeparation * (0.3 + randomness1),
        cell * boxPoloidalSeparation * (0.5 + randomness1)
    );
    float randomness2 = random(vec2(floor(pos.y / separation.x), floor(pos.z / separation.y)));

    vec3 p = pos;
    p.y = opRepeat(p.y - separation.x * 0.5, separation.x);
    p.z = opRepeat(p.z - separation.y * 0.5, separation.y);
    float randomizedHeight = boxHeight + 0.8 * randomness1 + randomness2;
    float scene = boxSDF(p, vec3(randomizedHeight, 0.1 + 0.2 * vec2(randomness1, randomness2)));
    scene = max(scene, boxSDF(pos, vec3(randomizedHeight, 0, 0) + cell * boxProportion));
    return scene;
}

float getCellIndexX(inout vec3 p)
{
    p.xz = cart2polar(p.xz);

    float angleIncrement = 1. / radius;
    float angle = p.x + angleIncrement;
    float x = floor(angle / (2. * angleIncrement));
    if (abs(x) < HALF_PI * radius)
        x = abs(x);
    angle = mod(angle, 2. * angleIncrement) - angleIncrement;

    p.xz = polar2cart(vec2(angle, p.z));

    p.x -= radius;

    return x;
}

float cellSize = cell + thin;

float getCellIndexY(inout vec3 p)
{
    p.y += TIME * speed;
    float y = floor(p.y / cellSize);
    p.y = opRepeat(p.y - cellSize * 0.5, cellSize);
    return y;
}

vec2 getCellIndexes(inout vec3 p)
{
    return vec2(getCellIndexX(p), getCellIndexY(p));
}

float map(vec3 pos)
{
    vec3 cameraOffset = vec3(-4, 0, 0);

    // donut distortion
    vec3 pDonut = pos + cameraOffset;
    pDonut.xy += vec2(donut, radius);
    pDonut.xz = cart2polar(pDonut.xz);
    pDonut.x *= donut;
    pDonut.z -= donut;
    pDonut.zy *= rotate2d(-TIME * 0.05 * speed);
    pDonut.xyz = pDonut.zxy;

    // ground
    vec3 p = pDonut;
    float scene = sphereSDF(vec3(p.x, 0, p.z), radius - height);

    // walls
    p = pDonut;
    getCellIndexY(p);
    scene = min(scene, max(abs(p.y) - thin, sphereSDF(vec3(p.x, 0, p.z), radius)));
    getCellIndexX(p);
    scene = min(scene, max(abs(p.z) - thin, p.x));

    // horizontal window
    p = pDonut;
    p.xz *= rotate2d(-1. / radius);
    vec2 indexes = getCellIndexes(p);
    float windowHeight = 0.75;
    p.x += windowHeight * 1.5;
    float windowWidth = 0.5;
    scene = max(scene, -boxSDF(p, vec3(windowHeight, thin + 0.01, windowWidth)));
    scene = min(scene, window(p.xzy, windowHeight, windowWidth, indexes));

    // vertical window
    p = pDonut;
    p.y += cell * 0.5;
    indexes = getCellIndexes(p);
    windowHeight = 0.75;
    p.x += windowHeight * 1.25;
    windowWidth = 1.5;
    scene = max(scene, -boxSDF(p, vec3(windowHeight, windowWidth, thin + 0.01)));
    scene = min(scene, window(p, windowHeight, windowWidth, indexes));

    // boxes
    p = pDonut;
    p.xz *= rotate2d(-1. / radius);
    p.y += cell * 0.5;
    indexes = getCellIndexes(p);
    p.x += height;
    scene = min(scene, boxes(p, indexes));

    return scene;
}

void main()
{
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE) / RENDERSIZE.y;
    vec3 eye = vec3(cameraX, cameraY, cameraZ);
    vec3 ray = normalize(vec3(uv, 1.3));
    camera(eye);
    camera(ray);
    float dither = random(uv + fract(TIME));
    vec3 pos = eye;
    float shade = 0.;
    bool isTorus = false;
    for (float i = 0.; i <= 1.; i += 1. / STEPS) {
        float dist = map(pos);
        if (dist < VOLUME) {
            shade = 1. - i;
            isTorus = true;
            break;
        }
        dist *= 0.5 + 0.1 * dither;
        pos += ray * dist;
    }

    if (isTorus) {
        vec3 light = vec3(40, 100, -10);
        float shadow = getShadow(pos, light, 4.);
        gl_FragColor.rgb = vec3(sqrt(smoothstep(0., 0.5, shade * shadow)));
        gl_FragColor.a = 1.;
    } else {
        gl_FragColor = backgroundColor;
    }
}
