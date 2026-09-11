/*{
    "CATEGORIES": [
        "Filter",
        "Generator"
    ],
    "CREDIT": "cornusammonis <https://www.shadertoy.com/user/cornusammonis>",
    "DESCRIPTION": "Fake fluid dynamical system that creates viscous-fingering–like flow pattern, converted from <https://www.shadertoy.com/view/XddSRX>",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "inputImageAmount",
            "LABEL": "Input image amount",
            "TYPE": "float",
            "DEFAULT": 0,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "restart",
            "LABEL": "Restart",
            "TYPE": "event"
        },
        {
            "NAME": "enableMouse",
            "LABEL": "Enable mouse",
            "TYPE": "bool",
            "DEFAULT": false
        },
        {
            "NAME": "mouse",
            "TYPE": "point2D",
            "DEFAULT": [0.5, 0.5],
            "MIN": [0, 0],
            "MAX": [1, 1]
        },
        {
            "NAME": "curlScale",
            "LABEL": "Curl scale",
            "TYPE": "float",
            "DEFAULT": -0.6,
            "MIN": -1,
            "MAX": 0
        },
        {
            "NAME": "laplacianScale",
            "LABEL": "Laplacian scale",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": -1,
            "MAX": 1
        },
        {
            "NAME": "laplacianDivergenceScale",
            "LABEL": "Laplacian divergence scale",
            "TYPE": "float",
            "DEFAULT": -0.8,
            "MIN": -1,
            "MAX": 1
        },
        {
            "NAME": "divergenceScale",
            "LABEL": "Divergence scale",
            "TYPE": "float",
            "DEFAULT": -0.05,
            "MIN": -1,
            "MAX": 1
        },
        {
            "NAME": "divergenceUpdateScale",
            "LABEL": "Divergence update scale",
            "TYPE": "float",
            "DEFAULT": -0.04,
            "MIN": -1,
            "MAX": 1
        },
        {
            "NAME": "divergenceSmoothing",
            "LABEL": "Divergence smoothing",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "advectionDistanceScale",
            "LABEL": "Advection distance scale",
            "TYPE": "float",
            "DEFAULT": 6,
            "MIN": 1,
            "MAX": 10
        },
        {
            "NAME": "curlRotationAnglePower",
            "LABEL": "Curl rotation angle power",
            "TYPE": "float",
            "DEFAULT": 1,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "selfAmplification",
            "LABEL": "Self-amplification",
            "TYPE": "float",
            "DEFAULT": 1,
            "MIN": 0,
            "MAX": 10
        },
        {
            "NAME": "updateSmoothing",
            "LABEL": "Update smoothing",
            "TYPE": "float",
            "DEFAULT": 0.8,
            "MIN": 0,
            "MAX": 1
        },
        {
            "NAME": "diagonalWeight",
            "LABEL": "Diagonal weight",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MIN": 0,
            "MAX": 1
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "fluid",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {

        }
    ]
}*/

#include "lygia/generative/snoise.glsl"
#include "lygia/math/rotate2d.glsl"


//
// Shadertoy Buffer A
//

struct FluidComponents {
    vec3 center;

    vec3 north;
    vec3 east;
    vec3 south;
    vec3 west;

    vec3 northwest;
    vec3 southwest;
    vec3 northeast;
    vec3 southeast;
};

FluidComponents FluidComponents_create(vec2 center, vec2 stepSizes)
{
    float step_x = stepSizes.x;
    float step_y = stepSizes.y;

    FluidComponents components;

    components.center = IMG_NORM_PIXEL(fluid, fract(center)).xyz;

    components.north = IMG_NORM_PIXEL(fluid, fract(center + vec2(      0,  step_y))).xyz;
    components.east  = IMG_NORM_PIXEL(fluid, fract(center + vec2( step_x,       0))).xyz;
    components.south = IMG_NORM_PIXEL(fluid, fract(center + vec2(      0, -step_y))).xyz;
    components.west  = IMG_NORM_PIXEL(fluid, fract(center + vec2(-step_x,       0))).xyz;

    components.northwest = IMG_NORM_PIXEL(fluid, fract(center + vec2(-step_x,  step_y))).xyz;
    components.southwest = IMG_NORM_PIXEL(fluid, fract(center + vec2(-step_x, -step_y))).xyz;
    components.northeast = IMG_NORM_PIXEL(fluid, fract(center + vec2( step_x,  step_y))).xyz;
    components.southeast = IMG_NORM_PIXEL(fluid, fract(center + vec2( step_x, -step_y))).xyz;

    return components;
}

vec3 FluidComponents_laplacian(FluidComponents components, float centerWeight, float edgeWeight, float vertexWeight)
{
    return centerWeight * components.center +
           edgeWeight   * (components.north + components.east + components.west + components.south) +
           vertexWeight * (components.northwest + components.southwest + components.northeast + components.southeast);
}


void main()
{
    vec2 position = gl_FragCoord.xy;
    vec2 texelSize = 1. / RENDERSIZE;
    vec2 normalizedPosition = position * texelSize;

    if (PASSINDEX == 0) // Shadertoy Buffer A
    {
        // It’s unclear whether dividing by the advection distance scale is what
        // was intended in the original Shadertoy shader.
        float laplacianCenterWeight = -20. / advectionDistanceScale;
        float laplacianEdgeWeight = 4. / advectionDistanceScale;
        float laplacianVertexWeight = 1. / advectionDistanceScale;

        FluidComponents components = FluidComponents_create(normalizedPosition, texelSize);

        // .x and .y are the x and y components, .z is divergence

        // laplacian of all components
        vec3 laplacian = FluidComponents_laplacian(components, laplacianCenterWeight, laplacianEdgeWeight, laplacianVertexWeight);
        float scaledLaplacianDivergence = laplacianDivergenceScale * laplacian.z;
        vec2 normalizedCenterComponent = components.center.xy == vec2(0) ? components.center.xy : normalize(components.center.xy);

        // calculate divergence
        // vectors point inwards towards the center point
        float divergence = components.south.y - components.north.y - components.east.x + components.west.x +
                           diagonalWeight * (components.northwest.x - components.northwest.y -
                                             components.northeast.x - components.northeast.y +
                                             components.southwest.x + components.southwest.y +
                                             components.southeast.y - components.southeast.x);
        float smoothedDivergence = components.center.z + divergenceUpdateScale * divergence + divergenceSmoothing * laplacian.z;

        // reverse advection
        vec3 advectionLaplacian = FluidComponents_laplacian(
            FluidComponents_create(normalizedPosition - components.center.xy * advectionDistanceScale * texelSize, texelSize),
            0.25, 0.125, 0.0625
        );

        // temp values for the update rule
        vec2 ab = laplacianScale * laplacian.xy +
                  scaledLaplacianDivergence * normalizedCenterComponent.xy +
                  divergenceScale * smoothedDivergence * components.center.xy +
                  selfAmplification * advectionLaplacian.xy;

        // calculate curl
        // vectors point clockwise about the center point
        float curl = components.north.x - components.south.x - components.east.y + components.west.y +
                     diagonalWeight * (components.northwest.x + components.northwest.y +
                                       components.northeast.x - components.northeast.y +
                                       components.southwest.y - components.southwest.x -
                                       components.southeast.y - components.southeast.x);

        // rotate
        ab = rotate2d(curlScale * sign(curl) * pow(abs(curl), curlRotationAnglePower)) * ab;

        vec3 abd = updateSmoothing * components.center + (1. - updateSmoothing) * vec3(ab, smoothedDivergence);

        if (enableMouse) {
            vec2 displacement = position - mouse * RENDERSIZE;
            float distance = length(displacement);
            if (distance > 0.) {
                abd.xy += exp(-0.1 * distance) * normalize(displacement);
            }
        }

        // initialize with noise
        if (FRAMEINDEX < 1 || restart) {
            vec2 scaledPosition = 16. * normalizedPosition;
            gl_FragColor.rgb = vec3(snoise(scaledPosition + 1.1), snoise(scaledPosition + 2.2), snoise(scaledPosition + 3.3));
            gl_FragColor.a = 1.;
        } else {
            gl_FragColor.rg = clamp(length(abd.xy) > 1. ? normalize(abd.xy) : abd.xy, -1., 1.);
            gl_FragColor.b = clamp(abd.z, -1., 1.);
            gl_FragColor.a = 1.;
            gl_FragColor = mix(gl_FragColor, IMG_PIXEL(inputImage, gl_FragCoord.xy), inputImageAmount);
        }
    }
    else // Shadertoy Image
    {
        vec3 abd = normalize(IMG_NORM_PIXEL(fluid, normalizedPosition).xyz);

        vec3 color = 0.5 + 0.6 * cross(abd, vec3(0.5, -0.4, 0.5));
        vec3 divergence = vec3(0.1 * abd.z);

        gl_FragColor = vec4(color + divergence, 1);
    }
}
