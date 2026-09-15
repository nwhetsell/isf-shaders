/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "Dave Hoskins <https://www.shadertoy.com/user/Dave_Hoskins>",
    "DESCRIPTION": "Mountains, converted from <https://www.shadertoy.com/view/4slGD4>",
    "INPUTS": [
        {
            "NAME": "cameraSpeed",
            "LABEL": "Camera speed",
            "TYPE": "float",
            "DEFAULT": 1.5,
            "MAX": 50,
            "MIN": 0
        },
        {
            "NAME": "cameraRollAmplitude",
            "LABEL": "Camera roll amplitude",
            "TYPE": "float",
            "DEFAULT": 0.15,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "mountainHeight",
            "LABEL": "Mountain height",
            "TYPE": "float",
            "DEFAULT": 0.75,
            "MAX": 2,
            "MIN": 0
        },
        {
            "NAME": "mountainSize",
            "LABEL": "Mountain size",
            "TYPE": "float",
            "DEFAULT": 0.15,
            "MAX": 2,
            "MIN": 0
        },
        {
            "NAME": "terrain",
            "LABEL": "Terrain",
            "TYPE": "float",
            "DEFAULT": 0.25,
            "MAX": 2,
            "MIN": 0
        },
        {
            "NAME": "perturbationAngle",
            "LABEL": "Perturbation angle",
            "TYPE": "float",
            "DEFAULT": 52.14991,
            "MAX": 360,
            "MIN": -360
        },
        {
            "NAME": "perturbationScale",
            "LABEL": "Perturbation scale",
            "TYPE": "float",
            "DEFAULT": 2.2201844917,
            "MAX": 2.5,
            "MIN": -2.5
        },
        {
            "NAME": "perturbationShift",
            "LABEL": "Perturbation shift",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": -10
        },
        {
            "NAME": "flatness",
            "LABEL": "Flatness",
            "TYPE": "float",
            "DEFAULT": 5,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "detail",
            "LABEL": "Detail",
            "TYPE": "float",
            "DEFAULT": 66,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "cragginess",
            "LABEL": "Cragginess",
            "TYPE": "float",
            "DEFAULT": -0.4,
            "MAX": 1,
            "MIN": -1
        },
        {
            "NAME": "fogDistance",
            "LABEL": "Fog distance",
            "TYPE": "float",
            "DEFAULT": 5,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "cloudHeight",
            "LABEL": "Cloud height",
            "TYPE": "float",
            "DEFAULT": 200,
            "MAX": 1000,
            "MIN": 0
        },
        {
            "NAME": "cloudSpeed",
            "LABEL": "Cloud speed",
            "TYPE": "float",
            "DEFAULT": 0,
            "MAX": 100,
            "MIN": -100
        }
    ],
    "ISFVSN": "2"
}*/

#define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#define FBM_NOISE_FNC(UV) gnoise(UV)
#define FBM_NOISE3_FNC(UV) snoise(UV)
#define FBM_SCALE_SCALAR 2.7
#define FBM_AMPLITUDE_INITIAL 0.7
float random_slow(vec2);
#define GNOISE_NOISE2_FNC(UV) random_slow(UV)
#include "lygia/generative/fbm.glsl"
#include "lygia/generative/gnoise.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/rotate2d.glsl"
#include "lygia/space/polar2cart.glsl"
#include "lygia-additions/gnoise.glsl"
#include "lygia/generative/random.glsl"
float random_slow(in vec2 p) {
    return random2(vec3(p.x, RANDOM_SCALE.x * p.yx / RANDOM_SCALE.yz)).x;
}


// Stereo version code thanks to Croqueteer :)
//#define STEREO

float treeLine = 0.0;
float treeCol = 0.0;

vec3 sunLight  = normalize(vec3(0.4, 0.4, 0.48));
vec3 sunColour = vec3(1, 0.9, 0.83);
float specular = 0.;
vec3 cameraPos;
float ambient;

float Trees(vec2 p)
{
    return gnoise(p * 13.) * treeLine;
}

// Low def version for ray-marching through the height field...
float Terrain(in vec2 p, out vec2 pos, out float w)
{
    // There's some real magic numbers in here!
    // The gnoise calls add large mountain ranges for more variation over distances...
    pos = p * 0.05;
    w = gnoise(pos * terrain) * mountainHeight + mountainSize;
    w = detail * w*w;

    // This peturbs the fractal positions for each iteration down...
    // Helps make nice twisted landscapes...
    mat2 rotate2D = perturbationScale * rotate2d(perturbationAngle * DEG2RAD) + perturbationShift * mat2(0., 0., 0.04, 0.1);

    float f = 0.;
    for (int i = 0; i < 5; i++) {
        f += gnoise(pos) * w;

        pos = rotate2D * pos; // Non-commutative matrix multiplication
        w *= cragginess;
    }

    f += pow(abs(gnoise(pos * 0.002)), flatness) * 275. - flatness;

    return f;
}

float Terrain(in vec2 p)
{
    vec2 pos;
    float w;
    return Terrain(p, pos, w);
}

// Map to lower resolution for height field mapping for Scene function...
float Map(in vec3 p)
{
    float h = Terrain(p.xz);

    float ff = gnoise(p.xz * 0.3) + gnoise(p.xz * 3.3) * 0.5;
    treeLine = smoothstep(ff, 0. + ff * 2., h) * smoothstep(1. + ff * 3., 0.4 + ff, h);
    treeCol = Trees(p.xz);
    h += treeCol;

    return p.y - h;
}

// High def version only used for grabbing normal information.
float Terrain2(in vec2 p)
{
    vec2 pos;
    float w;
    float f = Terrain(p, pos, w);

    treeCol = Trees(p);
    f += treeCol;
    if (treeCol > 0.)
        return f;

    mat2 rotate2D = perturbationScale * rotate2d(perturbationAngle * DEG2RAD) + perturbationShift * mat2(0., 0., 0.04, 0.1);

    // That's the last of the low resolution, now go down further for the Normal data...
    for (int i = 0; i < 6; i++) {
        f += gnoise(pos) * w;

        pos = rotate2D * pos; // Non-commutative matrix multiplication
        w *= cragginess;
    }

    return f;
}

// Simply Perlin clouds that fade to the horizon...
vec3 GetClouds(in vec3 sky, in vec3 rd)
{
    if (rd.y < 0.01)
        return sky;

    float v = (cloudHeight - cameraPos.y) / rd.y;
    rd.xz *= v;
    rd.xz += cameraPos.xz;
    rd.xz *= 0.01;
    float f = (fbm(rd.xz + cloudSpeed * TIME) - 0.55) * 5.;
    // Uses the ray's y component for horizon fade of fixed colour clouds...
    sky = mix(sky, vec3(0.55, 0.55, 0.52), clamp(f * rd.y - 0.1, 0., 1.));

    return sky;
}

// Grab all sky information for a given ray from camera
vec3 GetSky(in vec3 rd)
{
    float sunAmount = max(dot(rd, sunLight), 0.);
    float v = pow(1. - max(rd.y, 0.), 5.) * 0.5;
    vec3 sky = v * sunColour * 0.4 + vec3(0.11, 0.22, 0.5);
    // Wide glare effect...
    sky += sunColour * pow(sunAmount, 6.5) * 0.32;
    // Actual sun...
    sky += sunColour * min(pow(sunAmount, 1150.), 0.3) * 0.65;
    return sky;
}

// Merge mountains into the sky background for correct disappearance...
vec3 ApplyFog(in vec3 rgb, in float distance, in vec3 dir)
{
    float fogAmount = exp(-distance * fogDistance * 1e-5);
    return mix(GetSky(dir), rgb, fogAmount);
}

// Calculate sun light...
void DoLighting(inout vec3 mat, in vec3 pos, in vec3 normal, in vec3 eyeDir, in float dis)
{
    float h = dot(sunLight, normal);
    float c = max(h, 0.) + ambient;
    mat = mat * sunColour * c;
    // Specular...
    if (h > 0.) {
        vec3 R = reflect(sunLight, normal);
        float specAmount = pow(max(dot(R, normalize(eyeDir)), 0.), 3.) * specular;
        mat = mix(mat, sunColour, specAmount);
    }
}

// Hack the height, position, and normal data to create the coloured landscape
vec3 TerrainColour(vec3 pos, vec3 normal, float dis)
{
    vec3 mat;
    specular = 0.0;
    ambient = 0.1;
    vec3 dir = normalize(pos - cameraPos);

    vec3 matPos = pos * 2.; // I had change scale halfway though, this lazy multiply allow me to keep the graphic scales I had

    float f = clamp(gnoise(matPos.xz * 0.05), 0., 1.);//*10.8;
    f += gnoise(matPos.xz * 0.1 + normal.yz * 1.08) * 0.85;
    f *= 0.55;
    vec3 m = mix(
        vec3(0.63 * f + 0.2, 0.7 * f + 0.1, 0.7 * f + 0.1),
        vec3(f * 0.43 + 0.1, f * 0.3 + 0.2, f * 0.35 + 0.1),
        f * 0.65
    );
    mat = m * (f * m + vec3(0.36, 0.3, 0.28));
    // Should have used smoothstep to add colours, but left it using 'if' for sanity...
    if (normal.y < 0.5) {
        float v = normal.y;
        float c = (0.5 - normal.y) * 4.;
        c = clamp(c*c, 0.1, 1.);
        f = gnoise(vec2(matPos.x * 0.09, matPos.z * 0.095 + matPos.yy * 0.15));
        f += gnoise(vec2(matPos.x * 2.233, matPos.z * 2.23)) * 0.5;
        mat = mix(mat, vec3(0.4 * f), c);
        specular += 0.1;
    }

    // Grass. Use the normal to decide when to plonk grass down...
    if (matPos.y < 45.35 && normal.y > 0.65) {
        m = vec3(gnoise(matPos.xz * 0.023) * 0.5 + 0.15, gnoise(matPos.xz * 0.03) * 0.6 + 0.25, 0);
        m *= (normal.y - 0.65) * 0.6;
        mat = mix(mat, m, clamp((normal.y - 0.65) * 1.3 * (45.35 - matPos.y) * 0.1, 0., 1.));
    }

    if (treeCol > 0.) {
        mat = vec3(0.02 + gnoise(matPos.xz * 5.) * 0.03, 0.05, 0);
        normal = normalize(normal + vec3(gnoise(matPos.xz * 33.) * 1. - 0.5, 0, gnoise(matPos.xz * 33.) * 1. - 0.5));
        specular = 0.;
    }

    // Snow topped mountains...
    if (matPos.y > 80. && normal.y > 0.42) {
        float snow = clamp((matPos.y - 80. - gnoise(matPos.xz * 0.1) * 28.) * 0.035, 0., 1.);
        mat = mix(mat, vec3(0.7, 0.7, 0.8), snow);
        specular += snow;
        ambient += snow * 0.3;
    }

    // Beach effect...
    if (matPos.y < 1.45) {
        if (normal.y > 0.4) {
            f = gnoise(matPos.xz * 0.084) * 1.5;
            f = clamp((1.45 - f - matPos.y) * 1.34, 0., 0.67);
            float t = normal.y - 0.4;
            t = t*t;
            mat = mix(mat, t + vec3(0.09, 0.07, 0.03), f);
        }
        // Cheap under water darkening...it's wet after all...
        if (matPos.y < 0.) {
            mat *= 0.2;
        }
    }

    float disSqrd = dis * dis; // Squaring it gives better distance scales.
    DoLighting(mat, pos, normal,dir, disSqrd);

    // Do the water...
    if (matPos.y < 0.) {
        // Pull back along the ray direction to get water surface point at y = 0.0 ...
        float time = TIME * 0.03;
        vec3 watPos = matPos;
        watPos += -dir * (watPos.y / dir.y);
        // Make some dodgy waves...
        float tx = cos(watPos.x * 0.052) * 4.5;
        float tz = sin(watPos.z * 0.072) * 4.5;
        vec2 co = gnoise2(vec2(watPos.x * 4.7 + 1.3 + tz, watPos.z * 4.69 + time * 35. - tx));
        co += gnoise2(vec2(watPos.z * 8.6 + time * 13. - tx, watPos.x * 8.712 + tz)) * 0.4;
        vec3 nor = normalize(vec3(co.x, 20., co.y));
        nor = normalize(reflect(dir, nor));//normalize((-2.0*(dot(dir, nor))*nor)+dir);
        // Mix it in at depth transparancy to give beach cues..
        tx = watPos.y - matPos.y;
        mat = mix(mat, GetClouds(GetSky(nor) * vec3(0.3, 0.3, 0.5), nor) * 0.1 + vec3(0.0, 0.02, 0.03), clamp((tx) * 0.4, 0.6, 1.));
        // Add some extra water glint...
        // mat += vec3(.1)*clamp(1.-pow(tx+.5, 3.)*texture(iChannel1, watPos.xz*.1, -2.).x, 0.,1.0);
        float sunAmount = max(dot(nor, sunLight), 0.);
        mat = mat + sunColour * pow(sunAmount, 228.5) * 0.6;
        vec3 temp = (watPos - cameraPos * 2.) * 0.5;
        disSqrd = dot(temp, temp);
    }
    mat = ApplyFog(mat, disSqrd, dir);
    return mat;
}

float BinarySubdivision(in vec3 rO, in vec3 rD, vec2 t)
{
    // Home in on the surface by dividing by two and split...
    float halfwayT;

    for (int i = 0; i < 5; i++) {
        halfwayT = dot(t, vec2(0.5));
        float d = Map(rO + halfwayT * rD);
        t = mix(vec2(t.x, halfwayT), vec2(halfwayT, t.y), step(0.5, d));
    }
    return halfwayT;
}

bool Scene(in vec3 rO, in vec3 rD, out float resT, in vec2 fragCoord)
{
    float t = 1. + random_slow(fragCoord.xy) * 1.;
    float oldT = 0.;
    float delta = 0.;
    bool fin = false;
    bool res = false;
    vec2 distances;
    for(int j = 0; j < 150; j++) {
        if (fin || t > 240.)
            break;

        vec3 p = rO + t * rD;
        //if (t > 240.0 || p.y > 195.0) break;
        float h = Map(p); // ...Get this positions height mapping.
        // Are we inside, and close enough to fudge a hit?...
        if (h < 0.5) {
            fin = true;
            distances = vec2(oldT, t);
            break;
        }
        // Delta ray advance - a fudge between the height returned
        // and the distance already travelled.
        // It's a really fiddly compromise between speed and accuracy
        // Too large a step and the tops of ridges get missed.
        delta = max(0.01, 0.3 * h) + t * 0.0065;
        oldT = t;
        t += delta;
    }

    if (fin)
        resT = BinarySubdivision(rO, rD, distances);

    return fin;
}

vec3 CameraPath(float t)
{
    t += (TIME * cameraSpeed + 658.) * 0.006;
    vec2 p = 476. * vec2(sin(3.5 * t), cos(1.5 * t));
    return vec3(35. - p.x, 0.6, 4108. + p.y);
}

// Some would say, most of the magic is done in post! :D
vec3 PostEffects(vec3 rgb, vec2 uv)
{
    return (1. - exp(-rgb * 6.)) * 1.0024;
}

void main()
{
    vec2 xy = -1. + 2. * gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 uv = xy * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.);

    #ifdef STEREO
    float isCyan = mod(gl_FragCoord.x + mod(gl_FragCoord.y, 2.), 2.);
    #endif

    // Use several forward heights, of decreasing influence with distance from the camera.
    float h = 0.;
    float f = 1.;
    for (int i = 0; i < 7; i++) {
        h += Terrain(CameraPath((0.6 - f) * 0.008).xz) * f;
        f -= 0.1;
    }

    cameraPos.xz = CameraPath(0.).xz;
    cameraPos.y = max(h * 0.25 + 3.5, 1.5 + sin(TIME * 5.) * 0.5);

    vec3 cameraTarget;
    cameraTarget.xz = CameraPath(0.1).xz;
    cameraTarget.y = cameraPos.y - smoothstep(60., 300., cameraPos.y) * 150.;

    float roll = cameraRollAmplitude * sin(TIME * 0.2);
    vec3 cw = normalize(cameraTarget - cameraPos);
    vec3 cp = vec3(polar2cart(-vec2(roll + 0.5 * PI, 1.)), 0);
    vec3 cu = normalize(cross(cw, cp));
    vec3 cv = normalize(cross(cu, cw));
    vec3 rd = normalize(uv.x * cu + uv.y * cv + 1.5 * cw);

    #ifdef STEREO
    cameraPos += 0.45 * cu * isCyan; // move camera to the right - the rd vector is still good
    #endif

    vec3 col;
    float distance;
    if (!Scene(cameraPos, rd, distance, gl_FragCoord.xy)) {
        // Missed scene, now just get the sky value...
        col = GetSky(rd);
        col = GetClouds(col, rd);
    } else {
        // Get world coordinate of landscape...
        vec3 pos = cameraPos + distance * rd;
        // Get normal from sampling the high definition height map
        // Use the distance to sample larger gaps to help stop aliasing...
        float p = 0.02 + 0.00005 * distance*distance;
        vec3 nor = vec3(0, Terrain2(pos.xz), 0);
        vec3 v2 = nor - vec3(p, Terrain2(pos.xz + vec2(p, 0)), 0);
        vec3 v3 = nor - vec3(0, Terrain2(pos.xz + vec2(0, -p)), -p);
        nor = cross(v2, v3);
        nor = normalize(nor);

        // Get the colour using all available data...
        col = TerrainColour(pos, nor, distance);
    }

    col = PostEffects(col, uv);

    #ifdef STEREO
    col *= vec3(isCyan, 1. - isCyan, 1. - isCyan);
    #endif

    gl_FragColor = vec4(col, 1.);
}
