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

// #define ISF_EDITOR_WEBSITE

#include "lygia/color/tonemap/aces.glsl"
#include "lygia/generative/random.glsl"
#include "lygia/lighting/common/henyeyGreenstein.glsl"
#include "lygia/math/const.glsl"
#include "lygia/math/rotate3dX.glsl"
#include "lygia/math/rotate3dY.glsl"
#include "lygia/math/rotate3dZ.glsl"
#include "lygia/space/aspect.glsl"
#include "lygia/space/center.glsl"


//increase this number for a better GI
#define IndirectSamples 1

//increase to remove more noise, but might make the result blurrier
#define SamplesLimit 150

//GI bounces
#define Bounces 1


#define PixelAcceptance 1.5
#define PixelCheckDistance .5


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
    return rotate3dZ(-rotEuler.z) * rotate3dX(-rotEuler.x) * rotate3dY(rotEuler.y);
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
        pos -= LightDir * dist * 2.5;	//goes 2.5 times faster since we don't need details
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
  return abs(v.x) > abs(v.z) ? vec3(-v.y, v.x, 0.0)  : vec3(0.0, -v.z, v.y);
}

vec3 getCosineWeightedSample(vec3 dir) {
	vec3 o1 = normalize(ortho(dir));
	vec3 o2 = normalize(cross(dir, o1));
	vec2 r = vec2(randomFloat(), randomFloat());
	r.x = r.x * TWO_PI;
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
    if(  min(RENDERSIZE, prevFragCoord) != prevFragCoord
      && max(vec2(0.)      , prevFragCoord) != prevFragCoord) return MaxDist;
    vec4 prevPos = IMG_NORM_PIXEL(bufferA, prevFragCoord/RENDERSIZE);
    return length(prevPos-hit);
}

vec4 previousSample(vec4 hit){
    vec2 prevUv = pos2uv(getCam(TIME-TIMEDELTA), hit.xyz);
    vec2 prevFragCoord = prevUv * RENDERSIZE.y + RENDERSIZE/2.0;

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

    Camera cam = getCam(TIME);
    if(finaldist < PixelAcceptance*length(hit.xyz-cam.pos)/cam.focalLength/RENDERSIZE.y)
        return IMG_NORM_PIXEL(bufferC, finalpfc/RENDERSIZE);
    return vec4(0.);
}

#ifdef ISF_EDITOR_WEBSITE

mat3 gaussianFilter = mat3(41, 26, 7,
                           26, 16, 4,
                           7,  4,  1) / 273.;

vec3 bloom(float scale, float threshold, vec2 fragCoord){
    float logScale = log2(scale);
    vec3 bloom = vec3(0);
    for(int y = -2; y <= 2; y++)
    for(int x = -2; x <= 2; x++)
        bloom += gaussianFilter[abs(x)][abs(y)] * textureLod(bufferD, (fragCoord+vec2(x, y)*scale)/RENDERSIZE, logScale).rgb;

    return max(bloom - vec3(threshold), vec3(0));
}

#endif


void main()
{
    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        gl_FragColor = IMG_THIS_PIXEL(bufferB);
    }
    else if (PASSINDEX == 1) // Shadertoy Buffer B
    {
        vec2 uv = 0.5 * aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);

        Camera cam = getCam(TIME);
        if (traceB(cam.pos, uv2dir(cam, uv)))
            gl_FragColor = vec4(cam.pos, 1);
        else
            gl_FragColor = vec4(0);
    }
    else if (PASSINDEX == 2) // Shadertoy Buffer C
    {
        seed = .256435865*gl_FragCoord.x+.316548465*gl_FragCoord.y+sin(TIME)*16886.3158915;

        Camera cam = getCam(TIME);

        vec4 hit = IMG_THIS_PIXEL(bufferB);
        if (hit.a == 0.) {
            vec2 uv = 0.5 * aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);
            gl_FragColor = vec4(directBackground(uv2dir(cam, uv)), 1.);
        } else {

            #if 0
            gl_FragColor = previousSample(hit);
            gl_FragColor.rgb = gl_FragColor.a == 0. ? vec3(1., 0., 0.) : vec3(0., 1., 0.);
            gl_FragColor.a = 1.;
            #else
            vec3 normal = normalEstimation(hit.xyz);

            vec3 iLight = vec3(0.);
            for(int i = 0; i < IndirectSamples; i++)
                iLight += sampleIndirectLight(hit.xyz, normal)/float(IndirectSamples);


            gl_FragColor = previousSample(hit);
            gl_FragColor.a += gl_FragColor.a > float(SamplesLimit) ? 0. : float(IndirectSamples);
            iLight = clamp(iLight, vec3(.0), vec3(gl_FragColor.a/10.)); // clamp gi for low sample count
            gl_FragColor.rgb = mix(gl_FragColor.rgb, iLight, 1.0/(gl_FragColor.a/float(IndirectSamples)));
            #endif
        }
    }
    else if (PASSINDEX == 3) // Shadertoy Buffer D
    {
        Camera cam = getCam(TIME);

        vec4 col = IMG_THIS_PIXEL(bufferC);
        vec4 tx = IMG_THIS_PIXEL(bufferB);
        vec3 finalPos = tx.rgb;
        col.a = length(cam.pos-finalPos);

        vec2 uv = (gl_FragCoord.xy-RENDERSIZE/2.0) / RENDERSIZE.y;
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
        pos += stepDist * dir * random(gl_FragCoord.xy/vec2(1024)) /*texture(iChannel2, gl_FragCoord.xy/vec2(1024)).a*/;
        for(int i = 0; i < FogSteps; i++){
            volAbs *= stepAbs;
            volCol += stepCol*volAbs*directLight(pos, -LightDir);
            pos += dir * stepDist;
        }

        col.rgb = col.rgb*volAbs + volCol;

        gl_FragColor = col;
    }
    else // Shadertoy Image
    {
        Camera cam = getCam(TIME);

        vec4 col = vec4(0.);
        float frd = RENDERSIZE.y*cam.aperture*DoFClamping;
        int rd = int(ceil(frd - .5));
        for(int y = -rd; y <= rd; y++){
            int ln = int(ceil(sqrt(frd*frd-float(y*y)) - .5));
            for(int x = -ln; x <= ln; x++){
                vec4 p = IMG_PIXEL(bufferD, ivec2(clamp(gl_FragCoord.xy + vec2(x, y), vec2(0), RENDERSIZE-1.)));
                float dof = min(abs(p.a-cam.focalDistance)/p.a, DoFClamping) * RENDERSIZE.y*cam.aperture;
                p.a = 1.;
                p *= clamp((dof - length(vec2(x, y))) + .5, 0.0, 1.0) / (dof*dof+.5);
                col += p;
           	}
        }

        col /= col.a;

#ifdef ISF_EDITOR_WEBSITE
        if (enableBloom)
            col.rgb += bloom(.07 * RENDERSIZE.y, .0, gl_FragCoord.xy) * .06;
#endif

        gl_FragColor = vec4(tonemapACES(col.rgb), 1);
    }
}
