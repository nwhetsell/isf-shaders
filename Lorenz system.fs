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
contributors: Inigo Quiles
description: Segment SDF
use: lineSDF(<vec2> st, <vec2> A, <vec2> B)
*/
#define FNC_LINESDF 
float lineSDF( in vec2 st, in vec2 a, in vec2 b ) {
    vec2 b_to_a = b - a;
    vec2 to_a = st - a;
    float h = saturate(dot(to_a, b_to_a)/dot(b_to_a, b_to_a));
    return length(to_a - h * b_to_a );
}
float lineSDF(vec3 p, vec3 a, vec3 b) {
    //https://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
    return length(cross(p - a, p - b))/length(b - a);
}
/*
contributors: Patricio Gonzalez Vivo
description: 'Fix the aspect ratio of a space keeping things squared for you.'
use: <vec2> aspect(<vec2> st, <vec2> st_size)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/draw_shapes.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_ASPECT 
vec2 aspect(vec2 st, vec2 s) {
    st.x = st.x * (s.x / s.y);
    return st;
}
/*
contributors: Patricio Gonzalez Vivo
description: "It center the coordinates from 0 to 1 to -1 to 1\nSo the center goes\
    \ from 0.5 to 0.0. \n"
use: <float|vec2|vec3> center(<float|vec2|vec3> st)
examples:
    - https://raw.githubusercontent.com/patriciogonzalezvivo/lygia_examples/main/draw_shapes.frag
license:
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Prosperity License - https://prosperitylicense.com/versions/3.0.0
    - Copyright (c) 2021 Patricio Gonzalez Vivo under Patron License - https://lygia.xyz/license
*/
#define FNC_CENTER 
float center(float x) { return x * 2.0 - 1.0; }
vec2 center(vec2 v) { return v * 2.0 - 1.0; }
vec3 center(vec3 v) { return v * 2.0 - 1.0; }
// https://en.wikipedia.org/wiki/Single-precision_floating-point_format#Notable_single-precision_cases
#define FLT_MAX 3.402823466e+38
#define SQRT1_2 0.7071067811865475244008443621048
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
