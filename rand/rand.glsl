// #define ISF_EDITOR_WEBSITE
#ifdef ISF_EDITOR_WEBSITE
#define RANDOM_HIGHER_RANGE
#define RANDOM_SINLESS
#include "../lygia/generative/random.glsl"
#endif

#ifndef RAND
#define RAND

// Linear congruential generator for random numbers
// (https://en.wikipedia.org/wiki/Linear_congruential_generator).

int seed = 1;

// This is the multiplier (a) and increment (c) from used in Microsoft’s Visual
// C implementation; see
// https://en.wikipedia.org/wiki/Linear_congruential_generator#Parameters_in_common_use
// in the “Microsoft Visual/Quick C/C++” row. The multiplier may not be a good
// choice. Table 4 (page 258) of “Tables of Linear Congruential Generators of
// Different Sizes and Good Lattice Structure” (Pierre L’Ecuyer 1999,
// https://www.ams.org/journals/mcom/1999-68-225/S0025-5718-99-00996-5/S0025-5718-99-00996-5.pdf)
// may have better constants (the 2^31 modulus row gives 37769685, 26757677, or
// 20501397), but that article has errata that does not seem to be easily
// available, so just use the Microsoft value.
const int a = 0x343fd;
const int c = 0x269ec3;
const int randBitMask = 0x7fff;
int rand(void)
{
    seed = seed * a + c;
    // Return bits 16 to 30.
#ifdef ISF_EDITOR_WEBSITE
    // Approximation of bitwise operations, based on
    // https://stackoverflow.com/questions/1700871/how-do-i-perform-bit-operations-in-glsl#answer-1700928
    return int(fract((float(seed) / float(65536)) / float(0x8000)) * float(0x8000));
#else
    return (seed >> 16) & randBitMask;
#endif
}

float frand(void)
{
    return float(rand()) / float(randBitMask);
}

void srand(int s)
{
    seed = s;
}

// This is a pseudo-random integer generator attributed to Hugo Elias. The
// original source for this seems to be
// http://freespace.virgin.net/hugo.elias/models/m_perlin.htm, but that website
// is defunct. This appears to be a Python translation:
// https://gist.github.com/dragon0/f70e2637e6d4e64a6ab210faf8a85a50
int hash(int n)
{
#ifdef ISF_EDITOR_WEBSITE
    return int(pow(2., 16.) * random(float(n)));
#else
    n = (n << 13) ^ n;
    return n * (n * n * 15731 + 789221) + 1376312589;
#endif
}

#endif
