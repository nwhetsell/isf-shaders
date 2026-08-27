/*{
    "CATEGORIES": [
        "Filter"
    ],
    "CREDIT": "Fabrice Neyret <https://www.shadertoy.com/user/FabriceNeyret2>",
    "DESCRIPTION": "Oscilloscope luminance analysis, converted from <https://www.shadertoy.com/view/Nttyz4>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "gridDensity",
            "LABEL": "Grid density",
            "TYPE": "float",
            "DEFAULT": 3,
            "MAX": 20,
            "MIN": 0
        }
    ],
    "ISFVSN": "2"
}*/
float gridStep(float p)
{
    return smoothstep(1., 0., abs(p) / fwidth(p));
}
void main()
{
    vec2 normalizedCoordinate = gl_FragCoord.xy / RENDERSIZE;
    vec2 gridSize = RENDERSIZE / exp2(gridDensity);
    vec2 gridCoordinate = round(normalizedCoordinate * gridSize) / gridSize;
    gridSize = gridSize * (normalizedCoordinate - gridCoordinate) + 0.5;
    gl_FragColor = vec4(0);
    vec4 pixelColor;
    pixelColor = IMG_NORM_PIXEL(inputImage, vec2(normalizedCoordinate.x, gridCoordinate.y)); // horizontal RGB profiles
    gl_FragColor = mix(gl_FragColor, vec4(1), gridStep(gridSize.y - length(pixelColor.rgb) / sqrt(3.)));
    pixelColor = IMG_NORM_PIXEL(inputImage, vec2(gridCoordinate.x, normalizedCoordinate.y)); // vertical RGB profiles
    gl_FragColor = mix(gl_FragColor, vec4(1), gridStep(gridSize.x - length(pixelColor.rgb) / sqrt(3.)));
    gl_FragColor = sqrt(gl_FragColor);
}
