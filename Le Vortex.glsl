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
#include "lygia/generative/random.glsl"
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

vec2 pointWithQuantizedAngle(in vec2 p, in float count, out float angleIndex)
{
    p = cart2polar(p);
    float angleIncrement = TWO_PI / count;
    float angle = p.x + angleIncrement * 0.5;
    angleIndex = floor(angle / angleIncrement);
    if (abs(angleIndex) < count * 0.5)
        angleIndex = abs(angleIndex);
    angle = mod(angle, angleIncrement) - angleIncrement * 0.5;
    return polar2cart(vec2(angle, p.y));
}

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

float windowCross(vec3 pos, vec4 size, float salt)
{
    vec3 p = pos;
    float sx = size.x * (0.6 + salt * 0.4);
    float sy = size.y * (0.3 + salt * 0.7);
    vec2 sxy = vec2(sx, sy);
    p.xy = opRepeat(p.xy, sxy);
    float scene = boxSDF(p, size.zyw * 2.);
    scene = min(scene, boxSDF(p, size.xzw * 2.));
    scene = max(scene, boxSDF(pos, size.xyw));
    return scene;
}

float window(vec3 pos, vec2 dimension, float salt)
{
    float thinn = 0.008;
    float depth = 0.04;
    float depthCadre = 0.006;
    float padding = 0.08;
    float scene = windowCross(pos, vec4(dimension, thinn, depth), salt);
    float cadre = boxSDF(pos, vec3(dimension, depthCadre));
    cadre = max(cadre, -boxSDF(pos, vec3(dimension - padding, depthCadre * 2.)));
    scene = min(scene, cadre);
    return scene;
}

float boxes(vec3 pos, float salt)
{
    vec3 p = pos;
    float ry = cell * boxToroidalSeparation * (0.3 + salt);
    float rz = cell * boxPoloidalSeparation * (0.5 + salt);
    float salty = random(vec2(floor(pos.y / ry), floor(pos.z / rz)));
    pos.y = opRepeat(pos.y - ry * 0.5, ry);
    pos.z = opRepeat(pos.z - rz * 0.5, rz);
    float height = boxHeight + 0.8 * salt + salty;
    float scene = boxSDF(pos, vec3(height, 0.1 + 0.2 * salt, 0.1 + 0.2 * salty));
    scene = max(scene, boxSDF(p, vec3(height + cell * boxProportion, cell * boxProportion, cell * boxProportion)));
    return scene;
}

float segments = PI * radius;

float getCellIndexX(inout vec3 p)
{
    float x;
    p.xz = pointWithQuantizedAngle(p.xz, segments, x);
    p.x -= radius;
    return x;
}

float getCellIndexY(inout vec3 p)
{
    p.y += TIME * speed;
    float cellSize = cell + thin;
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
    p.xz *= rotate2d(-PI / segments);
    vec2 indexes = getCellIndexes(p);
    vec2 dimension = vec2(0.75, 0.5);
    p.x += dimension.x * 1.5;
    scene = max(scene, -boxSDF(p, vec3(dimension.x, 0.1, dimension.y)));
    scene = min(scene, window(p.xzy, dimension, random(indexes)));

    // vertical window
    p = pDonut;
    p.y += cell * 0.5;
    indexes = getCellIndexes(p);
    p.x += dimension.x * 1.25;
    dimension.y = 1.5;
    scene = max(scene, -boxSDF(p, vec3(dimension, 0.1)));
    scene = min(scene, window(p, dimension, random(indexes)));

    // elements
    p = pDonut;
    p.xz *= rotate2d(-PI / segments);
    p.y += cell * 0.5;
    indexes = getCellIndexes(p);
    p.x += height;
    scene = min(scene, boxes(p, random(indexes)));

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
