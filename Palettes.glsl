/*{
    "CATEGORIES": [
        "Filter"
    ],
    "CREDIT": "Inigo Quilez <https://iquilezles.org>",
    "DESCRIPTION": "Cosine based palettes, based on <https://www.shadertoy.com/view/ll2GD3>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "usePredefinedPalette",
            "LABEL": "Use predefined palette",
            "TYPE": "bool",
            "DEFAULT": true
        },
        {
            "NAME": "id",
            "LABEL": "Predefined palette",
            "TYPE": "long",
            "DEFAULT" : 0,
            "VALUES": [0, 1, 2, 3, 4, 5, 6],
            "LABELS" : [
                "Rainbow",
                "Dusk",
                "Rhododendron",
                "Green Earth",
                "Brown Earth",
                "Neon",
                "Watermelon"
            ]
        },
        {
            "NAME": "a",
            "LABEL": "Color a",
            "TYPE": "color",
            "DEFAULT": [0.5, 0.5, 0.5, 1]
        },
        {
            "NAME": "b",
            "LABEL": "Color b",
            "TYPE": "color",
            "DEFAULT": [0.5, 0.5, 0.5, 1]
        },
        {
            "NAME": "c",
            "LABEL": "Color c",
            "TYPE": "color",
            "DEFAULT": [1, 1, 1, 1]
        },
        {
            "NAME": "d",
            "LABEL": "Color d",
            "TYPE": "color",
            "DEFAULT": [0, 0.33, 0.67, 1]
        }
    ],
    "ISFVSN": "2"
}*/

#include "lygia/color/luminance.glsl"
#include "lygia/color/palette.glsl"


void main()
{
    vec4 inputColor = IMG_THIS_PIXEL(inputImage);
    float t = luminance(inputColor);

    vec3 color;
    if (usePredefinedPalette) {
             if (id == 0) color = palette( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,0.33,0.67) );
        else if (id == 1) color = palette( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,0.10,0.20) );
        else if (id == 2) color = palette( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.3,0.20,0.20) );
        else if (id == 3) color = palette( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,0.5),vec3(0.8,0.90,0.30) );
        else if (id == 4) color = palette( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,0.7,0.4),vec3(0.0,0.15,0.20) );
        else if (id == 5) color = palette( t, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(2.0,1.0,0.0),vec3(0.5,0.20,0.25) );
        else if (id == 6) color = palette( t, vec3(0.8,0.5,0.4),vec3(0.2,0.4,0.2),vec3(2.0,1.0,1.0),vec3(0.0,0.25,0.25) );
    } else {
        color = palette(t, a.rgb, b.rgb, c.rgb, d.rgb);
    }

    gl_FragColor = vec4(color, inputColor.a);
}
