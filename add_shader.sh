#!/bin/sh

echo '/*{
    "CATEGORIES": [
        "Filter",
        "Generator"
    ],
    "CREDIT": "",
    "DESCRIPTION": "",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        }
    ],
    "ISFVSN": "2"
}*/

void main()
{

}' > "$1.glsl"

./make_makefile.sh

make
