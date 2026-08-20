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

// Calculate the next position
vec3 Integrate(vec3 cur, float dt)
{
	vec3 next = vec3(0);

    next.x = O * (cur.y - cur.x);
    next.y = cur.x * (P - cur.z) - cur.y;
    next.z = cur.x * cur.y - B * cur.z;

    return cur + next * dt;
}

// Distance to a line segment
float dfLine(vec2 start, vec2 end, vec2 uv)
{
	vec2 line = end - start;
	float frac = dot(uv - start, line) / dot(line, line);
	return distance(start + line * clamp(frac, 0., 1.), uv);
}


void main()
{
    vec2 res = RENDERSIZE.xy / RENDERSIZE.y;
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.y;
    uv -= 0.5 * res;
    uv.y += 0.375;

    vec3 last = IMG_PIXEL(lastData, vec2(0)).xyz;
    vec3 next = vec3(0);

#define FLT_MAX 3.402823466e+38
#define STEPS 96
#define MODE xz
    float d = FLT_MAX;
    for (int i = 0; i < STEPS; i++) {
       	next = Integrate(last, 0.016 * SPEED);
        d = min(d, dfLine(last.MODE * VIEW_SCALE, next.MODE * VIEW_SCALE, uv));
        last = next;
    }

    float c = smoothstep(FOCUS / RENDERSIZE.y, 0., d);

    c += (INTENSITY / 8.5) * exp(-1000. * d*d);

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
