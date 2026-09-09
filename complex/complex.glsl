#ifndef COMPLEX
#define COMPLEX

vec2 complex_multiply(in vec2 v1, in vec2 v2)
{
    return vec2(
        v1.x * v2.x - v1.y * v2.y,
        v1.x * v2.y + v1.y * v2.x
    );
}

float complex_magnitudeSquared(in vec2 v)
{
    return v.x * v.x + v.y * v.y;
}

float complex_magnitude(in vec2 v)
{
    return length(v);
}

vec2 complex_conjugate(in vec2 v)
{
    return vec2(v.x, -v.y);
}

// https://en.wikipedia.org/wiki/Complex_number#Complex_conjugate,_absolute_value,_argument_and_division
vec2 complex_divide(in vec2 v1, in vec2 v2)
{
    return complex_multiply(v1, complex_conjugate(v2)) / complex_magnitudeSquared(v2);
}

float complex_argument(in vec2 v)
{
    return atan(v.y, v.x);
}

#endif
