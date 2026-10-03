shaders = \
	CA\ Molecular\ dynamics.fs \
	Cell\ system\ 2.fs \
	circle\ dithering.fs \
	Colormap.fs \
	Domain-warped\ FBM\ noise.fs \
	Endless\ living\ creature.fs \
	Ether.fs \
	Everflow.fs \
	expansive\ reaction-diffusion.fs \
	Gaussian\ SmoothLife.fs \
	Iterated\ function\ system.fs \
	Le\ Vortex.fs \
	Lorenz\ system.fs \
	MattiasCRT.fs \
	Mountains.fs \
	notebook\ drawings.fs \
	oscilloscope\ analysis\ luminance.fs \
	Palettes.fs \
	Physarum\ Polycephalum\ Simulation.fs \
	Rainier\ mood.fs \
	Random\ slime\ mold\ generator.fs \
	Shattered\ Crystal.fs \
	spilled.fs \
	Strange\ Cloud.fs \
	Suture\ Fluid.fs \
	The\ Weave.fs \

all: $(shaders)

clang_options = --preprocess --comments --no-line-commands -fdirectives-only --language=c

CA\ Molecular\ dynamics.fs: CA\ Molecular\ dynamics.glsl
	clang $(clang_options) --output="$@" "$<"
Cell\ system\ 2.fs: Cell\ system\ 2.glsl
	clang $(clang_options) --output="$@" "$<"
circle\ dithering.fs: circle\ dithering.glsl
	clang $(clang_options) --output="$@" "$<"
Colormap.fs: Colormap.glsl
	clang $(clang_options) --output="$@" "$<"
Domain-warped\ FBM\ noise.fs: Domain-warped\ FBM\ noise.glsl
	clang $(clang_options) --output="$@" "$<"
Endless\ living\ creature.fs: Endless\ living\ creature.glsl
	clang $(clang_options) -D__VERSION__=300 --output="$@" "$<"
Ether.fs: Ether.glsl
	clang $(clang_options) --output="$@" "$<"
Everflow.fs: Everflow.glsl
	clang $(clang_options) --output="$@" "$<"
expansive\ reaction-diffusion.fs: expansive\ reaction-diffusion.glsl
	cd lygia; git apply ../'expansive reaction-diffusion.patch';
	clang $(clang_options) -fno-directives-only -D__VERSION__=300 --output="$@" "$<"
	cd lygia; git reset --hard
Gaussian\ SmoothLife.fs: Gaussian\ SmoothLife.glsl
	clang $(clang_options) --output="$@" "$<"
Iterated\ function\ system.fs: Iterated\ function\ system.glsl rand/rand.glsl
	clang $(clang_options) --output="$@" "$<"
Le\ Vortex.fs: Le\ Vortex.glsl
	cd lygia; git apply ../'Le Vortex.patch';
	clang $(clang_options) -D__VERSION__=300 --output="$@" "$<"
	cd lygia; git reset --hard
Lorenz\ system.fs: Lorenz\ system.glsl
	clang $(clang_options) --output="$@" "$<"
MattiasCRT.fs: MattiasCRT.glsl
	clang $(clang_options) --output="$@" "$<"
Mountains.fs: Mountains.glsl
	cd lygia; git apply ../Mountains.patch;
	clang $(clang_options) -D__VERSION__=300 --output="$@" "$<"
	cd lygia; git reset --hard
notebook\ drawings.fs: notebook\ drawings.glsl
	clang $(clang_options) --output="$@" "$<"
oscilloscope\ analysis\ luminance.fs: oscilloscope\ analysis\ luminance.glsl
	clang $(clang_options) --output="$@" "$<"
Palettes.fs: Palettes.glsl
	clang $(clang_options) --output="$@" "$<"
Physarum\ Polycephalum\ Simulation.fs: Physarum\ Polycephalum\ Simulation.glsl
	clang $(clang_options) --output="$@" "$<"
Rainier\ mood.fs: Rainier\ mood.glsl
	clang $(clang_options) --output="$@" "$<"
Random\ slime\ mold\ generator.fs: Random\ slime\ mold\ generator.glsl
	clang $(clang_options) --output="$@" "$<"
Shattered\ Crystal.fs: Shattered\ Crystal.glsl
	clang $(clang_options) --output="$@" "$<"
spilled.fs: spilled.glsl
	clang $(clang_options) --output="$@" "$<"
Strange\ Cloud.fs: Strange\ Cloud.glsl
	clang $(clang_options) --output="$@" "$<"
Suture\ Fluid.fs: Suture\ Fluid.glsl
	clang $(clang_options) --output="$@" "$<"
The\ Weave.fs: The\ Weave.glsl
	clang $(clang_options) --output="$@" "$<"
