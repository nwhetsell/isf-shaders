/*{
    "CATEGORIES": [
        "Generator"
    ],
    "CREDIT": "Flyguy <https://www.shadertoy.com/user/Flyguy>",
    "DESCRIPTION": "Lorenz system plotter, converted from <https://www.shadertoy.com/view/XddGWj>",
    "INPUTS": [
        {
            "NAME": "O",
            "LABEL": "Sigma",
            "TYPE": "float",
            "DEFAULT": 10,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "P",
            "LABEL": "Rho",
            "TYPE": "float",
            "DEFAULT": 28,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "B",
            "LABEL": "Beta",
            "TYPE": "float",
            "DEFAULT": 2.6666666667,
            "MAX": 100,
            "MIN": 0
        },
        {
            "NAME": "SPEED",
            "LABEL": "Speed",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "FOCUS",
            "LABEL": "Thickness",
            "TYPE": "float",
            "DEFAULT": 1,
            "MAX": 10,
            "MIN": 0
        },
        {
            "NAME": "INTENSITY",
            "LABEL": "Intensity",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "FADE",
            "LABEL": "Fade",
            "TYPE": "float",
            "DEFAULT": 0.99,
            "MAX": 1,
            "MIN": 0
        },
        {
            "NAME": "VIEW_SCALE",
            "LABEL": "View scale",
            "TYPE": "float",
            "DEFAULT": 0.015,
            "MAX": 1,
            "MIN": 0
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "lastData",
            "PERSISTENT": true,
            "FLOAT": true
        }
    ]
}*/

#include "lygia/sdf/lineSDF.glsl"
#include "lygia/space/aspect.glsl"
#include "lygia/space/center.glsl"
#include "lygia-additions/const.glsl"


// Calculate the next position
vec3 Integrate(vec3 cur, float dt)
{
    vec3 next = vec3(
        O * (cur.y - cur.x),
        cur.x * (P - cur.z) - cur.y,
        cur.x * cur.y - B * cur.z
    );
    return cur + next * dt;
}


void main()
{
    vec2 uv = 0.5 * aspect(center(gl_FragCoord.xy / RENDERSIZE), RENDERSIZE);
    uv.y += 0.375;

    vec3 last = IMG_PIXEL(lastData, vec2(0)).xyz;
    vec3 next = vec3(0);

    #define STEPS 96
    #define MODE xz
    float dist = FLT_MAX;
    for (int i = 0; i < STEPS; i++) {
        next = Integrate(last, 0.016 * SPEED);
        dist = min(dist, lineSDF(uv, last.MODE * VIEW_SCALE, next.MODE * VIEW_SCALE));
        last = next;
    }

    float c = smoothstep(FOCUS / RENDERSIZE.y, 0., dist);

    c += (INTENSITY / 8.5) * exp(-1000. * dist*dist);

    // Pixel (0,0) saves the current position.
    if (floor(gl_FragCoord.xy) == vec2(0)) {
        if (FRAMEINDEX == 0) {
            // Set up initial conditions.
            vec3 start = vec3(0.1, 0.001, 0);
            gl_FragColor = vec4(start, 0);
        } else {
            // Save current position.
            gl_FragColor = vec4(next, 0);
        }
    } else {
        gl_FragColor = vec4(vec3(c) + IMG_THIS_PIXEL(lastData).rgb * FADE, 1);
    }
}
