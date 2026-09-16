/*{
    "CATEGORIES": [
        "Filter"
    ],
    "CREDIT": "Florian Berger <https://www.shadertoy.com/user/flockaroo>",
    "DESCRIPTION": "Hand drawing effect, converted from <https://www.shadertoy.com/view/XtVGD1>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        }
    ],
    "ISFVSN": "2"
}*/

#define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#include "lygia/generative/random.glsl"
#include "lygia/math/const.glsl"
#include "lygia/space/polar2cart.glsl"


vec4 getCol(vec2 pos)
{
    // take aspect ratio into account
    vec2 uv = ((pos - RENDERSIZE.xy * 0.5) / RENDERSIZE.y * RENDERSIZE.y) / RENDERSIZE.xy + 0.5;
    vec4 c1 = texture(inputImage, uv);
    vec4 e = smoothstep(vec4(-0.05), vec4(0), vec4(uv, vec2(1) - uv));
    // c1 = mix(vec4(1, 1, 1, 0), c1, e.x * e.y * e.z * e.w);
    float d = clamp(dot(c1.xyz, vec3(-0.5, 1., -0.5)), 0., 1.);
    vec4 c2 = vec4(0.7);
    return c1; // min(mix(c1, c2, 1.8 * d), 0.7);
}

vec4 getColHT(vec2 pos)
{
 	return smoothstep(0.95, 1.05, getCol(pos) * 0.8 + 0.2 + random4(pos * 0.7));
}

float getVal(vec2 pos)
{
    vec4 c = getCol(pos);
 	return pow(dot(c.xyz, vec3(0.333)), 1.) * 1.; // TODO
}

vec2 getGrad(vec2 pos, float eps)
{
   	vec2 d = vec2(eps, 0);
    return vec2(
        getVal(pos + d.xy) - getVal(pos - d.xy),
        getVal(pos + d.yx) - getVal(pos - d.yx)
    ) / eps / 2.;
}

#define AngleNum 3
#define SampNum 16

void main()
{
    vec2 pos = gl_FragCoord.xy + 4. * sin(TIME * vec2(1, 1.7)) * RENDERSIZE.y / 400.;
    vec3 col = vec3(0);
    vec3 col2 = vec3(0);
    float sum = 0.;

    for (int i = 0; i < AngleNum; i++) {
        float ang = TWO_PI / float(AngleNum) * (float(i) + 0.8);
        vec2 v = polar2cart(vec2(ang, 1));

        for (int j = 0; j < SampNum; j++) {
            vec2 dpos = v.yx * vec2(1, -1) * float(j) * RENDERSIZE.y / 400.;
            vec2 dpos2 = v.xy * float(j * j) / float(SampNum) * 0.5 * RENDERSIZE.y / 400.;

            for (float s = -1.; s <= 1.; s += 2.) {
                vec2 pos2 = pos + s * dpos + dpos2;
                vec2 pos3 = pos + (s * dpos + dpos2).yx * vec2(1, -1) * 2.;
               	vec2 g = getGrad(pos2, 0.4);
               	float fact = dot(g, v) - 0.5 * abs(dot(g, v.yx * vec2(1, -1)));
               	float fact2 = dot(normalize(g + vec2(EPSILON)), v.yx * vec2(1, -1));

                fact = clamp(fact, 0., 0.05);
                fact2 = abs(fact2);

                fact *= 1. - float(j) / float(SampNum);
               	col += fact;
               	col2 += fact2 * getColHT(pos3).xyz;
               	sum += fact2;
            }
        }
    }
    col /= float(SampNum * AngleNum) * 0.75 / sqrt(RENDERSIZE.y);
    col2 /= sum;
    col.x *= 0.6 + 0.8 * random4(pos * 0.7).x;
    col.x = 1. - col.x;
    col.x *= col.x * col.x;

    vec2 s = sin(pos.xy * 0.1 / sqrt(RENDERSIZE.y / 400.));
    vec3 karo = vec3(1);
    karo -= 0.5 * vec3(0.25, 0.1, 0.1) * dot(exp(-s*s * 80.), vec2(1));
    float r = length(pos - RENDERSIZE.xy * 0.5) / RENDERSIZE.x;
    float vign = 1. - r*r*r;
    gl_FragColor = vec4(col.x * col2, 1);
}
