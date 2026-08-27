#!/bin/zsh

shaders=()
rules=()

for file in *.glsl; do
  name="$(basename "$file" .glsl)"
  escaped_name=${name// /\\ }
  shaders+="\t$escaped_name.fs \\\\\n"
  rules+="\n$escaped_name.fs: $escaped_name.glsl\n\tclang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output=\"\$@\" \"\$<\""
done

IFS=""
echo "shaders = \\\\
$shaders
all: \$(shaders)
$rules" > Makefile
