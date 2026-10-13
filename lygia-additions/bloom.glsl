#ifndef BLOOM_QUALITY
#define BLOOM_QUALITY 9
#endif

#ifndef FNC_BLOOM
#define FNC_BLOOM

vec3 bloom(sampler2D tex, float bloomSpread, float lod)
{
#ifndef ISF_EDITOR_WEBSITE
    vec2 pos = gl_FragCoord.xy / RENDERSIZE;
    vec4 bloom = vec4(0);
    for (int y = -BLOOM_QUALITY; y <= BLOOM_QUALITY; y++)
    for (int x = -BLOOM_QUALITY; x <= BLOOM_QUALITY; x++)  {
        bloom += textureLod(tex, pos + (vec2(x, y) / RENDERSIZE) * bloomSpread, lod);
    }
    return (bloom / bloom.a).rgb;
#else
    return vec3(0);
#endif
}

#endif
