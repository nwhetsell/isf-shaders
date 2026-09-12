shaders = \
	CA\ Molecular\ dynamics.fs \
	Cell\ system\ 2.fs \
	circle\ dithering.fs \
	Colormap.fs \
	Domain-warped\ FBM\ noise.fs \
	Endless\ living\ creature.fs \
	Ether.fs \
	Everflow.fs \
	Gaussian\ SmoothLife.fs \
	Iterated\ function\ system.fs \
	Le\ Vortex.fs \
	Lorenz\ system.fs \
	Mountains.fs \
	oscilloscope\ analysis\ luminance.fs \
	Physarum\ Polycephalum\ Simulation.fs \
	Rainier\ mood.fs \
	Random\ slime\ mold\ generator.fs \
	Shattered\ Crystal.fs \
	spilled.fs \
	Suture\ Fluid.fs \
	The\ Weave.fs \

all: $(shaders)

CA\ Molecular\ dynamics.fs: CA\ Molecular\ dynamics.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Cell\ system\ 2.fs: Cell\ system\ 2.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
circle\ dithering.fs: circle\ dithering.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Colormap.fs: Colormap.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Domain-warped\ FBM\ noise.fs: Domain-warped\ FBM\ noise.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Endless\ living\ creature.fs: Endless\ living\ creature.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Ether.fs: Ether.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Everflow.fs: Everflow.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Gaussian\ SmoothLife.fs: Gaussian\ SmoothLife.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Iterated\ function\ system.fs: Iterated\ function\ system.glsl rand/rand.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Le\ Vortex.fs: Le\ Vortex.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Lorenz\ system.fs: Lorenz\ system.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Mountains.fs: Mountains.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
oscilloscope\ analysis\ luminance.fs: oscilloscope\ analysis\ luminance.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Physarum\ Polycephalum\ Simulation.fs: Physarum\ Polycephalum\ Simulation.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Rainier\ mood.fs: Rainier\ mood.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Random\ slime\ mold\ generator.fs: Random\ slime\ mold\ generator.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Shattered\ Crystal.fs: Shattered\ Crystal.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
spilled.fs: spilled.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Suture\ Fluid.fs: Suture\ Fluid.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
The\ Weave.fs: The\ Weave.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
