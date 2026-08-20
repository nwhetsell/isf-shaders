/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "chronos <https://www.shadertoy.com/user/chronos>",
    "DESCRIPTION": "Volume tracing turbulently distorted SDFs, converted from <https://www.shadertoy.com/view/W3SSRm>",
    "INPUTS": [
        {
            "NAME": "focal",
            "LABEL": "Focal length",
            "TYPE": "float",
            "DEFAULT": 2.25,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "twist",
            "LABEL": "Twist",
            "TYPE": "float",
            "DEFAULT": 5,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "resolution",
            "LABEL": "Resolution",
            "TYPE": "float",
            "DEFAULT": 10,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "saturation",
            "LABEL": "Saturation",
            "TYPE": "float",
            "DEFAULT": 50,
            "MAX": 100,
            "MIN": 0
        }
    ],
    "ISFVSN": "2"
}*/

// Constants and functions from LYGIA <https://github.com/patriciogonzalezvivo/lygia>
#define PI 3.1415926535897932384626433832795

mat2 rotate2d(const in float r) {
    float c = cos(r);
    float s = sin(r);
    return mat2(c, s, -s, c);
}


vec3 cmap(float x)
{
    return pow(0.5 + 0.5 * cos(PI * x + vec3(1, 2, 3)), vec3(2.5));
}

void main()
{
    vec2 uv = (2. * gl_FragCoord.xy - RENDERSIZE) / RENDERSIZE.y;
    vec3 ro = vec3(0, 0, TIME);
    vec3 rd = normalize(vec3(uv, -focal));
    vec3 color = vec3(0);
    float t = 0.;
    for (int i = 0; i < 99; i++) {
        vec3 p = t * rd + ro;

        float T = (t + TIME) / twist;
        p.xy = rotate2d(T) * p.xy;

        for (float f = 0.; f < 9.; f++)  {
            float a = exp(f) / exp2(f);
            p += cos(p.yzx * a + TIME) / a;
        }
        float d = 1. / saturation + abs((ro - p - vec3(0, 1, 0)).y - 1.) / resolution;
        color += cmap(t) * 2e-3 / d;
        t += d;
    }

    color *= color * color;
    color = 1. - exp(-color);
    color = pow(color, vec3(1. / 2.2));
    gl_FragColor = vec4(color, 1);
}
