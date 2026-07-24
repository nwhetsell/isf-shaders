/*{
    "CATEGORIES": [
        "Filter",
        "Generator"
    ],
    "CREDIT": "Mykhailo Moroz <https://www.shadertoy.com/user/michael0884>",
    "DESCRIPTION": "Water flowing infinitely inside a looped space without gravity, converted from <https://www.shadertoy.com/view/ttBcWm>",
    "INPUTS": [
        {
            "NAME" : "inputImage",
            "TYPE" : "image"
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "bufferA_positionAndMass",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferA_velocity",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferB",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {
            "TARGET": "bufferC",
            "PERSISTENT": true,
            "FLOAT": true
        },
        {

        }
    ]
}*/

// Constants and functions from LYGIA <https://github.com/patriciogonzalezvivo/lygia>
#define PI 3.1415926535897932384626433832795


//
// ShaderToy Common
//

#define loop(i,x) for(int i = 0; i < x; i++)
#define range(i,a,b) for(int i = a; i <= b; i++)

#define dt 1.5

#define border_h 5.
vec2 R;
vec4 Mouse;
float time;

#define mass 1.

#define fluid_rho 0.5

float Pf(vec2 rho)
{
    //return 0.2*rho.x; //gas
    float GF = 1.;//smoothstep(0.49, 0.5, 1. - rho.y);
    return mix(0.5*rho.x,0.04*rho.x*(rho.x/fluid_rho - 1.), GF); //water pressure
}

mat2 Rot(float ang)
{
    return mat2(cos(ang), -sin(ang), sin(ang), cos(ang));
}

vec2 Dir(float ang)
{
    return vec2(cos(ang), sin(ang));
}


float sdBox( in vec2 p, in vec2 b )
{
    vec2 d = abs(p)-b;
    return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
}

float border(vec2 p)
{
    float bound = -sdBox(p - R*0.5, R*vec2(0.5, 0.5));
    float box = sdBox(Rot(0.*time)*(p - R*vec2(0.5, 0.6)) , R*vec2(0.05, 0.01));
    float drain = -sdBox(p - R*vec2(0.5, 0.7), R*vec2(1.5, 2.5));
    return max(drain,min(bound, box));
}

#define h 1.
vec3 bN(vec2 p)
{
    vec3 dx = vec3(-h,0,h);
    vec4 idx = vec4(-1./h, 0., 1./h, 0.25);
    vec3 r = idx.zyw*border(p + dx.zy)
           + idx.xyw*border(p + dx.xy)
           + idx.yzw*border(p + dx.yz)
           + idx.yxw*border(p + dx.yx);
    return vec3(normalize(r.xy), r.z + 1e-4);
}


// The ShaderToy shader uses the functions `floatBitsToUint` and
// `uintBitsToFloat` to pack more than 4 floats (5 in this case) into a
// 4-component pixel. These functions are available in GLSL v3.30 (OpenGL v3.3)
// and later, but some ISF hosts (notably Videosync) use GLSL v1.50
// (OpenGL v3.2). We can work around this by effectively running ShaderToy
// buffers twice, but the packing operations in the ShaderToy shader also
// perform a `clamp` on the packed data. Without the `clamp` calls, this shader
// seems to blow up numerically.
#define POST_UNPACK(X) (clamp(X, 0., 1.) * 2. - 1.)
#define PRE_PACK(X) clamp(0.5 * X + 0.5, 0., 1.)


struct particle
{
    vec2 X;
    vec2 V;
    vec2 M;
};

particle getParticle(vec4 positionAndMass, vec4 velocity, vec2 pos)
{
    particle P;
    P.X = POST_UNPACK(positionAndMass.xy) + pos;
    P.V = POST_UNPACK(velocity.xy);
    P.M = positionAndMass.zw;
    return P;
}

vec3 hash32(vec2 p)
{
	vec3 p3 = fract(vec3(p.xyx) * vec3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yxz+33.33);
    return fract((p3.xxy+p3.yzz)*p3.zyx);
}

float G(vec2 x)
{
    return exp(-dot(x,x));
}

float G0(vec2 x)
{
    return exp(-length(x));
}

//diffusion amount
#define dif 1.12
vec3 distribution(vec2 x, vec2 p, float K)
{
    vec4 aabb0 = vec4(p - 0.5, p + 0.5);
    vec4 aabb1 = vec4(x - K*0.5, x + K*0.5);
    vec4 aabbX = vec4(max(aabb0.xy, aabb1.xy), min(aabb0.zw, aabb1.zw));
    vec2 center = 0.5*(aabbX.xy + aabbX.zw); //center of mass
    vec2 size = max(aabbX.zw - aabbX.xy, 0.); //only positive
    float m = size.x*size.y/(K*K); //relative amount
    //if any of the dimensions are 0 then the mass is 0
    return vec3(center, m);
}


vec4 V(vec2 p)
{
    return texture(bufferC, p/R);
}


#define pos gl_FragCoord.xy
#define iFrame FRAMEINDEX
#define iResolution RENDERSIZE
#define iTime TIME
#define U gl_FragColor

void main()
{
    if (PASSINDEX == 0 || PASSINDEX == 1) // ShaderToy Buffer A
    {
        R = iResolution.xy; time = iTime;
        // Mouse = iMouse;
        ivec2 p = ivec2(pos);

        particle P;
        P.X = vec2(0);
        P.V = vec2(0);
        P.M = vec2(0);

        // Diffusion and advection: basically integrate over all updated
        // neighbor distributions that fall inside of this pixel. This makes the
        // tracking conservative.
        range(i, -2, 2) range(j, -2, 2)
        {
            vec2 tpos = pos + vec2(i,j);

            particle P0 = getParticle(
                texelFetch(bufferA_positionAndMass, ivec2(mod(tpos, R)), 0),
                texelFetch(bufferB, ivec2(mod(tpos, R)), 0),
                tpos
            );

            P0.X += P0.V*dt; //integrate position

            float difR = 0.9 + 0.21*smoothstep(fluid_rho*0., fluid_rho*0.333, P0.M.x);
            vec3 D = distribution(P0.X, pos, difR);
            //the deposited mass into this cell
            float m = P0.M.x*D.z;

            //add weighted by mass
            P.X += D.xy*m;
            P.V += P0.V*m;
            P.M.y += P0.M.y*m;

            //add mass
            P.M.x += m;
        }

        //normalization
        if(P.M.x != 0.)
        {
            P.X /= P.M.x;
            P.V /= P.M.x;
            P.M.y /= P.M.x;
        }

        //initial condition
        if(iFrame < 1)
        {
            //random
            vec3 rand = hash32(pos);
            if(rand.z < 0.2)
            {
                P.X = pos;
                P.V = 0.5*(rand.xy-0.5) + vec2(sin(2.*pos.x/R.x), cos(2.*pos.x/R.x));
                P.M = vec2(mass, 0.5 - 0.5*sin(10.*pos.x/R.x));
            }
            else
            {
                P.X = pos;
                P.V = vec2(0.);
                P.M = vec2(1e-6);
            }
        }

        if (PASSINDEX == 0) {
            P.X = clamp(P.X - pos, vec2(-0.5), vec2(0.5));
            gl_FragColor = vec4(PRE_PACK(P.X), P.M);
        } else {
            gl_FragColor = vec4(PRE_PACK(P.V), 0, 1);
        }
    }
    else if (PASSINDEX == 2) // ShaderToy Buffer B
    {
        R = iResolution.xy; time = iTime;
        //Mouse = iMouse;
        ivec2 p = ivec2(pos);

        particle P = getParticle(
            texelFetch(bufferA_positionAndMass, ivec2(mod(pos, R)), 0),
            texelFetch(bufferA_velocity, ivec2(mod(pos, R)), 0),
            pos
        );


        if(P.M.x != 0.) //not vacuum
        {
            //Compute the SPH force
            vec2 F = vec2(0.);
            vec3 avgV = vec3(0.);
            range(i, -2, 2) range(j, -2, 2)
            {
                vec2 tpos = pos + vec2(i,j);
                particle P0 = getParticle(
                    texelFetch(bufferA_positionAndMass, ivec2(mod(tpos, R)), 0),
                    texelFetch(bufferA_velocity, ivec2(mod(tpos, R)), 0),
                    tpos
                );
                vec2 dx = P0.X - P.X;
                float avgP = 0.5*P0.M.x*(Pf(P.M) + Pf(P0.M));
                F -= 0.5*G(1.*dx)*avgP*dx;
                avgV += P0.M.x*G(1.*dx)*vec3(P0.V,1.);
            }
            avgV.xy /= avgV.z;

            //viscosity
            F += 0.*P.M.x*(avgV.xy - P.V);

            //gravity
           // F += P.M.x*vec2(0., -0.0004);

            if(Mouse.z > 0.)
            {
                vec2 dm =(Mouse.xy - Mouse.zw)/10.;
                float d = distance(Mouse.xy, P.X)/20.;
                F += 0.001*dm*exp(-d*d);
               // P.M.y += 0.1*exp(-40.*d*d);
            }

            //integrate
            P.V += F*dt/P.M.x;

            //border
            vec3 N = bN(P.X);
            float vdotN = step(N.z, border_h)*dot(-N.xy, P.V);
            P.V += 0.5*(N.xy*vdotN + N.xy*abs(vdotN));
            P.V += 0.*P.M.x*N.xy*step(abs(N.z), border_h)*exp(-N.z);

            if(N.z < 0.) P.V = vec2(0.);


            //velocity limit
            float v = length(P.V);
            P.V /= (v > 1.)?v:1.;
        }


        gl_FragColor = vec4(PRE_PACK(P.V), 0, 1);
    }
    else if (PASSINDEX == 3) // ShaderToy Buffer C
    {
        R = iResolution.xy; time = iTime;
        ivec2 p = ivec2(pos);

        particle P = getParticle(
            texelFetch(bufferA_positionAndMass, ivec2(mod(pos, R)), 0),
            texelFetch(bufferA_velocity, ivec2(mod(pos, R)), 0),
            pos
        );

        //particle render
        vec4 rho = vec4(0.);
        range(i, -1, 1) range(j, -1, 1)
        {
            vec2 tpos = pos + vec2(i,j);
            particle P0 = getParticle(
                texelFetch(bufferA_positionAndMass, ivec2(mod(tpos, R)), 0),
                texelFetch(bufferA_velocity, ivec2(mod(tpos, R)), 0),
                tpos
            );

            vec2 x0 = P0.X; //update position
            //how much mass falls into this pixel
            rho += 1.*vec4(P.V, P.M)*G((pos - x0)/1.);
        }

        gl_FragColor = rho;
    }
    else // ShaderToy Image
    {
        R = iResolution.xy; time = iTime;
        //pos = R*0.5 + pos*0.1;
        ivec2 p = ivec2(pos);

        particle P = getParticle(
            texelFetch(bufferA_positionAndMass, ivec2(mod(pos, R)), 0),
            texelFetch(bufferB, ivec2(mod(pos, R)), 0),
            pos
        );

        //border render
        vec3 Nb = bN(P.X);
        float bord = smoothstep(2.*border_h,border_h*0.5,border(pos));

        vec4 rho = V(pos);
        vec3 dx = vec3(-1., 0., 1.);
        vec4 grad = -0.5*vec4(V(pos + dx.zy).zw - V(pos + dx.xy).zw,
                                V(pos + dx.yz).zw - V(pos + dx.yx).zw);
        vec2 N = pow(length(grad.xz),0.2)*normalize(grad.xz+1e-5);
        vec3 n = normalize(vec3(N, 1));
        vec3 r = reflect(vec3(0,0,1),n);
        // vec3 specular = 0.7*pow(texture(iChannel0, r).xyz,vec3(2.));//pow(max(dot(N, Dir(1.4)), 0.), 3.5);
        float specularb = 0.*G(0.4*(Nb.zz - border_h))*pow(max(dot(Nb.xy, Dir(1.4)), 0.), 3.);

        float a = pow(smoothstep(fluid_rho*0., fluid_rho*2., rho.z),0.1);
        float b = exp(-1.7*smoothstep(fluid_rho*1., fluid_rho*7.5, rho.z));
        vec3 col0 = vec3(1., 0.5, 0.);
        vec3 col1 = vec3(0.1, 0.4, 1.);
        // Output to screen
        float c = tanh(3.*(rho.w - 1.))*0.5 + 0.5;
        gl_FragColor.xyz = mix(col0, col1, c)*(1.5*b + specularb*3.)*a;
        gl_FragColor.xyz = tanh(gl_FragColor.xyz*gl_FragColor.xyz);
        gl_FragColor.a = 1.;
    }
}
