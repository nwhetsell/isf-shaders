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

// Constants and functions from LYGIA <https://github.com/patriciogonzalezvivo/lygia>
#define PI 3.1415926535897932384626433832795
#define TWO_PI 6.2831853071795864769252867665590
#define DEG2RAD (PI / 180.0)

// https://github.com/patriciogonzalezvivo/lygia/blob/main/math/rotate2d.glsl
mat2 rotate2d(const in float r) {
    float c = cos(r);
    float s = sin(r);
    return mat2(c, s, -s, c);
}

// https://github.com/patriciogonzalezvivo/lygia/blob/main/sdf/boxSDF.glsl
float boxSDF( vec3 p, vec3 b ) {
    vec3 d = abs(p) - b;
    return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));
}

// https://github.com/patriciogonzalezvivo/lygia/blob/main/sdf/sphereSDF.glsl
float sphereSDF(vec3 p) { return length(p); }
float sphereSDF(vec3 p, float s) { return sphereSDF(p) - s; }

// Raymarching sketch inspired by the work of Marc-Antoine Mathieu
// Leon 2017-11-21
// using code from IQ, Mercury, LJ, Duke, Koltes

#define STEPS 250.
#define VOLUME 0.001

// raymarching toolbox
float rng(vec2 seed) {
    return fract(sin(dot(seed * 0.1684, vec2(54.649, 321.547))) * 450315.);
}

mat2 rotate2dCounterclockwise(const in float r) {
    return rotate2d(-r);
}

float amod(inout vec2 p, float count) {
    float an = TWO_PI / count;
    float a = atan(p.y, p.x) + an / 2.;
    float c = floor(a / an);
    c = mix(c, abs(c), step(count * 0.5, abs(c)));
    a = mod(a, an) - an / 2.;
    p.xy = vec2(cos(a), sin(a)) * length(p);
    return c;
}

float amodIndex(vec2 p, float count) {
    float an = TWO_PI / count;
    float a = atan(p.y, p.x) + an / 2.;
    float c = floor(a / an);
    c = mix(c, abs(c), step(count * 0.5, abs(c)));
    return c;
}

float repeat(float v, float c) { return mod(v, c) - c / 2.; }
vec2  repeat(vec2 v,  vec2 c)  { return mod(v, c) - c / 2.; }
vec3  repeat(vec3 v,  float c) { return mod(v, c) - c / 2.; }

float smoo(float a, float b, float r) { return clamp(0.5 + 0.5 * (b - a) / r, 0., 1.); }
float smin(float a, float b, float r) {
    float h = smoo(a, b, r);
    return mix(b, a, h) - r * h * (1. - h);
}
float smax(float a, float b, float r) {
    float h = smoo(a, b, r);
    return mix(a, b, h) + r * h * (1. - h);
}

vec2 displaceLoop(vec2 p, float r) {
    return vec2(length(p) - r, atan(p.y, p.x));
}

float map(vec3);
float getShadow(vec3 pos, vec3 at, float k) {
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

vec3 getNormal(vec3 p) {
    vec2 e = vec2(0.01, 0);
    return normalize(vec3(
        map(p + e.xyy) - map(p - e.xyy),
        map(p + e.yxy) - map(p - e.yxy),
        map(p + e.yyx) - map(p - e.yyx)
    ));
}

void camera(inout vec3 p) {
    p.xz *= rotate2dCounterclockwise(yAxisRotation * DEG2RAD);
    p.yz *= rotate2dCounterclockwise(xAxisRotation * DEG2RAD);
}

float windowCross(vec3 pos, vec4 size, float salt) {
    vec3 p = pos;
    float sx = size.x * (0.6 + salt * 0.4);
    float sy = size.y * (0.3 + salt * 0.7);
    vec2 sxy = vec2(sx, sy);
    p.xy = repeat(p.xy + sxy / 2., sxy);
    float scene = boxSDF(p, size.zyw * 2.);
    scene = min(scene, boxSDF(p, size.xzw * 2.));
    scene = max(scene, boxSDF(pos, size.xyw));
    return scene;
}

float window(vec3 pos, vec2 dimension, float salt) {
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

float boxes(vec3 pos, float salt) {
    vec3 p = pos;
    float ry = cell * boxToroidalSeparation * (0.3 + salt);
    float rz = cell * boxPoloidalSeparation * (0.5 + salt);
    float salty = rng(vec2(floor(pos.y / ry), floor(pos.z / rz)));
    pos.y = repeat(pos.y, ry);
    pos.z = repeat(pos.z, rz);
    float height = boxHeight + 0.8 * salt + salty;
    float scene = boxSDF(pos, vec3(height, 0.1 + 0.2 * salt, 0.1 + 0.2 * salty));
    scene = max(scene, boxSDF(p, vec3(height + cell * boxProportion, cell * boxProportion, cell * boxProportion)));
    return scene;
}

float map(vec3 pos) {
    vec3 camOffset = vec3(-4, 0, 0);

    float scene = 1000.;
    vec3 p = pos + camOffset;
    float segments = PI * radius;
    float indexX, indexY, salt;
    vec2 seed;

    // donut distortion
    vec3 pDonut = p;
    pDonut.x += donut;
    pDonut.y += radius;
    pDonut.xz = displaceLoop(pDonut.xz, donut);
    pDonut.z *= donut;
    pDonut.xzy = pDonut.xyz;
    pDonut.xz *= rotate2dCounterclockwise(TIME * 0.05 * speed);

    // ground
    p = pDonut;
    scene = min(scene, sphereSDF(vec3(p.x, 0, p.z), radius-height));

    // walls
    p = pDonut;
    float py = p.y + TIME * speed;
    indexY = floor(py / (cell + thin));
    p.y = repeat(py, cell + thin);
    scene = min(scene, max(abs(p.y) - thin, sphereSDF(vec3(p.x, 0, p.z), radius)));
    amod(p.xz, segments);
    p.x -= radius;
    scene = min(scene, max(abs(p.z) - thin, p.x));

    // horizontal window
    p = pDonut;
    p.xz *= rotate2dCounterclockwise(PI / segments);
    py = p.y + TIME * speed;
    indexY = floor(py / (cell + thin));
    p.y = repeat(py, cell + thin);
    indexX = amodIndex(p.xz, segments);
    amod(p.xz, segments);
    seed = vec2(indexX, indexY);
    salt = rng(seed);
    p.x -= radius;
    vec2 dimension = vec2(0.75, 0.5);
    p.x += dimension.x * 1.5;
    scene = max(scene, -boxSDF(p, vec3(dimension.x, 0.1, dimension.y)));
    scene = min(scene, window(p.xzy, dimension, salt));

    // vertical window
    p = pDonut;
    py = p.y + cell / 2. + TIME * speed;
    indexY = floor(py / (cell + thin));
    p.y = repeat(py, cell + thin);
    indexX = amodIndex(p.xz, segments);
    amod(p.xz, segments);
    seed = vec2(indexX, indexY);
    salt = rng(seed);
    p.x -= radius;
    dimension.y = 1.5;
    p.x += dimension.x * 1.25;
    scene = max(scene, -boxSDF(p, vec3(dimension, 0.1)));
    scene = min(scene, window(p, dimension, salt));

    // elements
    p = pDonut;
    p.xz *= rotate2dCounterclockwise(PI / segments);
    py = p.y + cell / 2. + TIME * speed;
    indexY = floor(py / (cell + thin));
    p.y = repeat(py, cell + thin);
    indexX = amodIndex(p.xz, segments);
    amod(p.xz, segments);
    seed = vec2(indexX, indexY);
    salt = rng(seed);
    p.x -= radius - height;
    scene = min(scene, boxes(p, salt));

    return scene;
}

void main()
{
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 eye = vec3(cameraX, cameraY, cameraZ);
    vec3 ray = normalize(vec3(uv, 1.3));
    camera(eye);
    camera(ray);
    float dither = rng(uv + fract(TIME));
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
