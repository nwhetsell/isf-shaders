/*{
    "CATEGORIES": [
        "Filter"
    ],
    "CREDIT": "Fabrice Neyret <https://www.shadertoy.com/user/FabriceNeyret2>",
    "DESCRIPTION": "Circle dithering, converted from <https://www.shadertoy.com/view/MdSfWK>",
    "INPUTS": [
        {
            "NAME" : "inputImage",
            "TYPE" : "image"
        }
    ],
    "ISFVSN": "2"
}*/
#define O gl_FragColor
#define U gl_FragCoord.xy
#define iResolution RENDERSIZE
#define iTime TIME
// ref image: http://www.boredpanda.com/single-line-plotter-scribbles-sergej-stoppel/
// ( doing it simpler: circles instead of scribbles ;-) )
float L = 8., // L*T = neightborhood size
      T = 4., // grid step for circle centers
      d = 1.; // density
#define T(U) texture(inputImage, (U)/R).r
//#define T(U) sqrt( texture(inputImage, (U)/R).r * 1.4 )
//#define T(U) length(texture(inputImage, (U)/R).rgb)
#define rnd(P) fract( sin( dot(P,vec2(12.1,31.7)) + 0.*iTime )*43758.5453123)
#define rnd2(P) fract( sin( (P) * mat2(12.1,-37.4,-17.3,31.7) )*43758.5453123)
#define C(U,P,r) smoothstep(1.5,0.,abs(length(P-U)-r))
//#define C(U,P,r) exp(-.5*dot(P-U,P-U)/(r*r)) * sin(1.5*6.28*length(P-U)/r) // Gabor
void main()
{
    vec2 R = iResolution.xy;
//  O += T(U)-O; return;
    O = vec4(1);
    for (float j = -L; j <=L; j++) // test potential circle centers in a window around U
        for (float i = -L; i <=L; i++) {
         // vec2 P = U+vec2(i,j);
            vec2 P = floor( U/T + vec2(i,j) ) *T; // potential circle center
            P += T*(rnd2(P)-.5);
            float v = T(P), // target grey value
                  r = mix(2., L*T ,v); // target radius
            if ( rnd(P) < (1.-v)/ r*4.*d /L*T*T ) // draw circle with probability
                O -= C(U,P,r)*.2 ; // * (1.-texture(iChannel0, (U)/R)); // colored variant
        }
 // O = sqrt(O);
}
