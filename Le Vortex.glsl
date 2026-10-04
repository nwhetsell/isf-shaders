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
            "NAME": "fieldOfView",
            "LABEL": "Camera field of view",
            "TYPE": "float",
            "DEFAULT": 75.1371840577,
            "MAX": 180,
            "MIN": 0
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
            "NAME": "lightRadius",
            "LABEL": "Light distance",
            "TYPE": "float",
            "DEFAULT": 108.1665382639,
            "MIN": 0,
            "MAX": 200
        },
        {
            "NAME": "lightPhi",
            "LABEL": "Light phi (degrees)",
            "TYPE": "float",
            "DEFAULT": 95.3045714391,
            "MIN": 0,
            "MAX": 180
        },
        {
            "NAME": "lightTheta",
            "LABEL": "Light theta (degrees)",
            "TYPE": "float",
            "DEFAULT": 68.1985905136,
            "MIN": 0,
            "MAX": 360
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

// The default camera field of view is:
//    2 × atan(1 / 1.3) = 75.1371840577°
// The default light parameters are:
//   lightRadius = length(vec3(40, 100, -10))
//               = sqrt(40 * 40 + 100 * 100 + -10 * -10)
//               = sqrt(11700) ≈ 108.1665382639
//   lightPhi = acos(-10 / lightRadius) ≈ 95.3045714391°
//   lightTheta = atan(100 / 40) = atan(2.5) ≈ 68.1985905136°

#include "lygia-additions/const.glsl"

// #define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#include "lygia/generative/random.glsl" // LYGIA’s random2 isn’t exactly the same as the RNG in the Shadertoy shader.
#define RAYMARCH_CAMERA_FOV fieldOfView
#define RAYMARCH_SAMPLES 251
#define RAYMARCH_MIN_DIST 0.
#define RAYMARCH_MAX_DIST FLT_MAX
#define RAYMARCH_MIN_HIT_DIST 0.001
#include "lygia/lighting/raymarch/cast.glsl"
#define RAYMARCH_SOFTSHADOW_ITERATIONS 16
#define RAYMARCH_SHADOW_MIN_DIST RAYMARCH_MIN_HIT_DIST * 50.
#define RAYMARCH_SHADOW_SOLID_ANGLE 0.25
#include "lygia/lighting/raymarch/softShadow.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/rotate2d.glsl"
#include "lygia/math/rotate3dX.glsl"
#include "lygia/math/rotate3dY.glsl"
#include "lygia/sdf/boxSDF.glsl"
#include "lygia/sdf/sphereSDF.glsl"
#include "lygia/space/aspect.glsl"
#include "lygia/space/cart2polar.glsl"
#include "lygia/space/center.glsl"
#include "lygia/space/polar2cart.glsl"
#include "lygia-additions/opRepeat.glsl"


// Raymarching sketch inspired by the work of Marc-Antoine Mathieu
// Leon 2017-11-21
// using code from IQ, Mercury, LJ, Duke, Koltes

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

Material raymarchMap(vec3 pos)
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
    float scene = min(1000., sphereSDF(vec3(p.x, 0, p.z), radius - height));

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

    Material mat = materialNew();
    mat.position = pos;
    mat.sdf = scene;
    return mat;
}

void main()
{
    vec2 uv = 0.5 * aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);

    mat3 rotationMatrix = rotate3dY(yAxisRotation * DEG2RAD) * rotate3dX(-xAxisRotation * DEG2RAD);

    // In theory, it should be possible to separate the camera position and
    // ray direction into a typical view matrix, but it’s not clear how to do
    // this rigorously.

    vec3 rayOrigin = vec3(cameraX, cameraY, cameraZ) * rotationMatrix;

    float fov = 1.0 / tan(RAYMARCH_CAMERA_FOV * DEG2RAD * 0.5);
    vec3 rayDirection = normalize(vec3(uv, fov)) * rotationMatrix;

    Material res = raymarchCast(rayOrigin, rayDirection);

    if (res.valid) {
        vec3 light = polar2cart(lightRadius, lightPhi * DEG2RAD, lightTheta * DEG2RAD);
        float shadow = raymarchSoftShadow(res.position, normalize(light - res.position));
        gl_FragColor.rgb = vec3(sqrt(smoothstep(0., 0.5, res.albedo.r * shadow)));
        gl_FragColor.a = 1.;
    } else {
        gl_FragColor = backgroundColor;
    }
}
