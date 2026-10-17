/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "loicvdb <https://github.com/loicvdb>",
    "DESCRIPTION": "Path tracing with global illumination, converted from <https://www.shadertoy.com/view/Wt3XRX>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "enableBloom",
            "LABEL": "Enable bloom",
            "TYPE": "bool",
            "DEFAULT": false
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
            "TARGET": "bufferB",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferC",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferD",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {

        }
    ]
}*/
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
#define RANDOM_SCALE vec4(443.897, 441.423, .0973, .1099)
#define FNC_RANDOM 
float random(in float x) {
    return fract(sin(x) * 43758.5453);
}
float random(in vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453);
}
float random(in vec3 pos) {
    return fract(sin(dot(pos.xyz, vec3(70.9898, 78.233, 32.4355))) * 43758.5453123);
}
float random(in vec4 pos) {
    float dot_product = dot(pos, vec4(12.9898,78.233,45.164,94.673));
    return fract(sin(dot_product) * 43758.5453);
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
description: some useful math constants
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define EIGHTH_PI 0.39269908169
#define QTR_PI 0.78539816339
#define HALF_PI 1.5707963267948966192313216916398
#define PI 3.1415926535897932384626433832795
#define TWO_PI 6.2831853071795864769252867665590
#define TAU 6.2831853071795864769252867665590
#define INV_PI 0.31830988618379067153776752674503
#define INV_SQRT_TAU 0.39894228040143267793994605993439
#define SQRT_HALF_PI 1.25331413732
#define PHI 1.618033988749894848204586834
#define EPSILON 0.0000001
#define GOLDEN_RATIO 1.6180339887
#define GOLDEN_RATIO_CONJUGATE 0.61803398875
#define GOLDEN_ANGLE 2.39996323
#define DEG2RAD (PI / 180.0)
#define RAD2DEG (180.0 / PI)

// Henyey-Greenstein phase function factor [-1, 1]
// represents the average cosine of the scattered directions
// 0 is isotropic scattering
// > 1 is forward scattering, < 1 is backwards
#define HENYEYGREENSTEIN_SCATTERING 0.76
#define FNC_HENYEYGREENSTEIN 
float henyeyGreenstein(const in float mu) {
    return max(0.0, (1.0 - HENYEYGREENSTEIN_SCATTERING*HENYEYGREENSTEIN_SCATTERING) / ((4. + PI) * pow(1.0 + HENYEYGREENSTEIN_SCATTERING*HENYEYGREENSTEIN_SCATTERING - 2.0 * HENYEYGREENSTEIN_SCATTERING * mu, 1.5)));
}
float henyeyGreenstein(float mu, float g) {
    float gg = g * g;
    return (1.0 / (4.0 * PI)) * ((1.0 - gg) / pow(1.0 + gg - 2.0 * g * mu, 1.5));
}
float henyeyGreenstein(float mu, float g, float dual_lobe_weight) {
    return mix(henyeyGreenstein( mu, -g), henyeyGreenstein(mu, g), dual_lobe_weight);
}

#define fragColor gl_FragColor
#define iResolution RENDERSIZE.xy
#define iTime TIME
#define iTimeDelta TIMEDELTA
//increase this number for a better GI
#define IndirectSamples 1
//increase to remove more noise, but might make the result blurrier
#define SamplesLimit 150
//GI bounces
#define Bounces 1
#define PixelAcceptance 1.5
#define PixelCheckDistance .5
#define Pi 3.14159265359
#define MaxStepsDirect 128
#define MaxStepsIndirect 32
#define MaxShadowSteps 32
#define FogSteps 8
#define MaxDist 4.
#define MinDist .015
#define DoFClamping .3
#define DoFSamples 128
#define Density vec3(.0025, .0045, .006)
#define Anisotropy .4
#define FogRange 13.
vec3 LightDir = normalize(vec3(.0, .0, -1));
vec3 LightColor = vec3(1.) * 6.;
float LightRadius = .02;
struct Camera {
    vec3 pos, rot;
    float focalLength, focalDistance, aperture;
};
mat3 rotationMatrix(vec3 rotEuler){
    float c = cos(rotEuler.x), s = sin(rotEuler.x);
    mat3 rx = mat3(1, 0, 0, 0, c, -s, 0, s, c);
    c = cos(rotEuler.y), s = sin(rotEuler.y);
    mat3 ry = mat3(c, 0, -s, 0, 1, 0, s, 0, c);
    c = cos(rotEuler.z), s = sin(rotEuler.z);
    mat3 rz = mat3(c, -s, 0, s, c, 0, 0, 0, 1);
    return rz * rx * ry;
}
Camera getCam(float time){
    //time = 0.;
    vec3 rot = vec3(cos(time*.4)/6., 1. + time*.2 + sin(time*.2)/4., .5);
    return Camera(vec3(0., 0., -10.) * rotationMatrix(rot), rot, 1., 7.5, .025);
}
vec3 uv2dir(Camera cam, vec2 uv){
    return normalize(vec3(uv, cam.focalLength)) * rotationMatrix(cam.rot);
}
vec2 pos2uv(Camera cam, vec3 pos){
    vec3 dir = normalize(pos - cam.pos) * inverse(rotationMatrix(cam.rot));
    return dir.xy * cam.focalLength / dir.z;
}
vec3 dirFromUv(Camera cam, vec2 uv){
    return normalize(vec3(uv, cam.focalLength)) * rotationMatrix(cam.rot);
}
float sdf(vec3 position, out vec3 diffuseColor, out vec3 emissionColor){
    diffuseColor = vec3(1.);
    emissionColor = vec3(0.);
    float Scale = 2.25;
    float Radius = .25;
    mat3 Rotation;
    float time = 15.;
    Rotation = rotationMatrix(vec3(time, time*.7, time*.4)*.2);
    Scale += sin(time*.5)*.25;
    Radius += cos(time) *.25;
 vec4 scalevec = vec4(Scale, Scale, Scale, abs(Scale)) / Radius;
 float C1 = abs(Scale-1.0), C2 = pow(abs(Scale), float(1- /*iterations*/7));
 vec4 p = vec4(position.xyz*Rotation, 1.0), p0 = p;
 for (int i = 0; i<7; i++) {
     p.xyz = clamp(p.xyz, -1.0, 1.0) * 2.0 - p.xyz;
     p.xyzw *= clamp(max(Radius/dot(p.xyz, p.xyz), Radius), 0.0, 1.0);
        if(i < 3) p.xyz *= Rotation;
     p.xyzw = p*scalevec + p0;
 }
    /*diffuseColor = fract(p0.x)<.1 ? vec3(.2) : vec3(1.);
    emissionColor = fract(p0.x)<.1 ? (normalize(p.xyz)*.5+.5)*10. : vec3(0.);*/
 return (length(p.xyz) - C1) / p.w - C2;
}
float sdf(vec3 position){
    vec3 dc, ec;
    return sdf(position, dc, ec);
}
vec3 normalEstimation(vec3 pos){
  vec2 k = vec2(MinDist, 0);
  return normalize(vec3(sdf(pos + k.xyy) - sdf(pos - k.xyy),
        sdf(pos + k.yxy) - sdf(pos - k.yxy),
        sdf(pos + k.yyx) - sdf(pos - k.yyx)));
}
float henyeyGreenstein(vec3 dirI, vec3 dirO){
 return PI*PI * henyeyGreenstein(dot(dirI, dirO), Anisotropy);
}
vec3 directLight(vec3 pos, vec3 normal){
    //return vec3(0.);
    float dotLight = -dot(normal, LightDir);
    if(dotLight < 0.0) return vec3(0);
    vec3 pos0 = pos;
    float minAngle = LightRadius;
    for(int i = 0; i < MaxShadowSteps; i++){
        float dist = sdf(pos);
        if(dist > MaxDist) break;
        if(dist < MinDist) return vec3(0.0);
        pos -= LightDir * dist * 2.5; //goes 2.5 times faster since we don't need details
        minAngle = min(asin(dist/length(pos-pos0)), minAngle);
    }
    return LightColor * dotLight * clamp(minAngle/LightRadius, .0, 1.0);
}
bool traceB(inout vec3 pos, in vec3 dir){
    pos += dir*sdf(pos);
    pos += dir*sdf(pos);
    for(int i = 0; i < MaxStepsDirect; i++){
        float dist = sdf(pos);
        if(dist > MaxDist) break;
        if(dist < MinDist){
            pos -= (2.*MinDist-dist) * dir;
            pos -= (2.*MinDist-sdf(pos)) * dir;
            pos -= (2.*MinDist-sdf(pos)) * dir;
            return true;
        }
        pos += dir*dist;
    }
    return false;
}
float seed;
float randomFloat(){
    return fract(sin(seed++)*43758.54536156);
}
vec3 ortho(vec3 v) {
  return abs(v.x) > abs(v.z) ? vec3(-v.y, v.x, 0.0) : vec3(0.0, -v.z, v.y);
}
vec3 getCosineWeightedSample(vec3 dir) {
 vec3 o1 = normalize(ortho(dir));
 vec3 o2 = normalize(cross(dir, o1));
 vec2 r = vec2(randomFloat(), randomFloat());
 r.x = r.x * 2.0 * Pi;
 r.y = pow(r.y, .5);
 float oneminus = sqrt(1.0-r.y*r.y);
 return cos(r.x) * oneminus * o1 + sin(r.x) * oneminus * o2 + r.y * dir;
}
vec3 background(vec3 dir){
    return LightColor * Density * 8. * henyeyGreenstein(-LightDir, dir);
}
vec3 directBackground(vec3 dir){
    return LightColor * Density * 1.;
}
bool traceC(inout vec3 pos, in vec3 dir, out vec3 normal){
    for(int i = 0; i < MaxStepsIndirect; i++){
        float dist = sdf(pos);
        if(dist > MaxDist) break;
        if(dist < MinDist){
            normal = normalEstimation(pos);
            pos -= (2.*MinDist-dist) * dir;
            return true;
        }
        pos += dir*dist;
    }
    return false;
}
vec3 sampleIndirectLight(vec3 pos, vec3 normal){
    vec3 dir;
    vec3 abso = vec3(1.), light = vec3(0.), dc, ec;
    for(int i = 0; i < Bounces; i++){
        dir = getCosineWeightedSample(normal);
        if(!traceC(pos, dir, normal)) return light + abso*background(dir);
        sdf(pos, dc, ec);
        light += abso * (ec + dc*directLight(pos, normal));
        abso *= dc;
    }
    return light;
}
float distancePixel(vec2 prevFragCoord, vec4 hit){
    if( min(iResolution.xy, prevFragCoord) != prevFragCoord
      && max(vec2(0.) , prevFragCoord) != prevFragCoord) return MaxDist;
    vec4 prevPos = texture(bufferA, prevFragCoord/iResolution.xy);
    Camera cam = getCam(iTime);
    return length(prevPos-hit);
}
vec4 previousSample(vec4 hit){
    vec2 prevUv = pos2uv(getCam(iTime-iTimeDelta), hit.xyz);
    vec2 prevFragCoord = prevUv * iResolution.y + iResolution.xy/2.0;
    vec2 pfc, finalpfc;
    float dist, finaldist = MaxDist;
    for(int x = -1; x <= 1; x++){
        for(int y = -1; y <= 1; y++){
            pfc = prevFragCoord + PixelCheckDistance*vec2(x, y);
            dist = distancePixel(pfc, hit);
            if(dist < finaldist){
                finalpfc = pfc;
                finaldist = dist;
            }
     }
    }
    Camera cam = getCam(iTime);
    if(finaldist < PixelAcceptance*length(hit.xyz-cam.pos)/cam.focalLength/iResolution.y)
        return texture(bufferC, finalpfc/iResolution.xy);
    return vec4(0.);
}
mat3 gaussianFilter = mat3(41, 26, 7,
                           26, 16, 4,
                           7, 4, 1) / 273.;
vec3 bloom(float scale, float threshold, vec2 fragCoord){
    float logScale = log2(scale);
    vec3 bloom = vec3(0);
    for(int y = -2; y <= 2; y++)
    for(int x = -2; x <= 2; x++)
        bloom += gaussianFilter[abs(x)][abs(y)] * textureLod(bufferD, (fragCoord+vec2(x, y)*scale)/iResolution.xy, logScale).rgb;
    return max(bloom - vec3(threshold), vec3(0));
}
#define fragCoord gl_FragCoord.xy
void main()
{
    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        fragColor = texelFetch(bufferB, ivec2(fragCoord), 0);
    }
    else if (PASSINDEX == 1) // Shadertoy Buffer B
    {
        Camera cam = getCam(iTime);
        vec2 uv = (fragCoord-iResolution.xy/2.0) / iResolution.y;
        vec3 dir = uv2dir(cam, uv);
        fragColor = vec4(0.);
        vec3 pos = cam.pos;
        if(traceB(pos, dir)) fragColor = vec4(pos, 1.);
    }
    else if (PASSINDEX == 2) // Shadertoy Buffer C
    {
        seed = .256435865*fragCoord.x+.316548465*fragCoord.y+sin(iTime)*16886.3158915;
        Camera cam = getCam(iTime);
        vec4 hit = texelFetch(bufferB, ivec2(fragCoord), 0);
        if (hit.a == 0.) {
            vec2 uv = (fragCoord-iResolution.xy/2.0) / iResolution.y;
            fragColor = vec4(directBackground(uv2dir(cam, uv)), 1.);
        } else {
            vec3 normal = normalEstimation(hit.xyz);
            vec3 iLight = vec3(0.);
            for(int i = 0; i < IndirectSamples; i++)
                iLight += sampleIndirectLight(hit.xyz, normal)/float(IndirectSamples);
            fragColor = previousSample(hit);
            fragColor.a += fragColor.a > float(SamplesLimit) ? 0. : float(IndirectSamples);
            iLight = clamp(iLight, vec3(.0), vec3(fragColor.a/10.)); // clamp gi for low sample count
            fragColor.rgb = mix(fragColor.rgb, iLight, 1.0/(fragColor.a/float(IndirectSamples)));
        }
    }
    else if (PASSINDEX == 3) // Shadertoy Buffer D
    {
        Camera cam = getCam(iTime);
        vec4 col = texelFetch(bufferC, ivec2(fragCoord), 0);
        vec4 tx = texelFetch(bufferB, ivec2(fragCoord), 0);
        vec3 finalPos = tx.rgb;
        col.a = length(cam.pos-finalPos);
        vec2 uv = (fragCoord-iResolution.xy/2.0) / iResolution.y;
        vec3 dir = uv2dir(cam, uv);
        if(tx.a != 0.) {
            vec3 dc, ec;
            sdf(finalPos, dc, ec);
            col.rgb = ec + dc * (col.rgb + directLight(finalPos, normalEstimation(finalPos)));
        } else {
            finalPos = cam.pos + dir * 100.;
        }
        vec3 volCol = vec3(0.), volAbs = vec3(1.), pos = cam.pos;
        float stepDist = (tx.a == 0. ? FogRange : min(FogRange, col.a))/float(FogSteps);
        vec3 stepAbs = exp(-Density*stepDist);
        vec3 stepCol = (vec3(1.) - stepAbs) * henyeyGreenstein(-LightDir, dir);
        pos += stepDist * dir * random(fragCoord/vec2(1024)) /*texture(iChannel2, fragCoord/vec2(1024)).a*/;
        for(int i = 0; i < FogSteps; i++){
            volAbs *= stepAbs;
            volCol += stepCol*volAbs*directLight(pos, -LightDir);
            pos += dir * stepDist;
        }
        col.rgb = col.rgb*volAbs + volCol;
        fragColor = col;
    }
    else // Shadertoy Image
    {
        Camera cam = getCam(iTime);
        vec4 col = vec4(0.);
        float frd = iResolution.y*cam.aperture*DoFClamping;
        int rd = int(ceil(frd - .5));
        for(int y = -rd; y <= rd; y++){
            int ln = int(ceil(sqrt(frd*frd-float(y*y)) - .5));
            for(int x = -ln; x <= ln; x++){
                vec4 p = texelFetch(bufferD, ivec2(clamp(fragCoord + vec2(x, y), vec2(0), iResolution.xy-1.)), 0);
                float dof = min(abs(p.a-cam.focalDistance)/p.a, DoFClamping) * iResolution.y*cam.aperture;
                p.a = 1.;
                p *= clamp((dof - length(vec2(x, y))) + .5, 0.0, 1.0) / (dof*dof+.5);
                col += p;
            }
        }
        col /= col.a;
        if (enableBloom)
            col.rgb += bloom(.07 * iResolution.y, .0, fragCoord) * .06;
        fragColor = vec4(tonemapACES(col.rgb), 1);
    }
}
