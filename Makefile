shaders = \
	CA\ Molecular\ dynamics.fs \
	Cell\ system\ 2.fs \
	circle\ dithering.fs \
	Endless\ living\ creature.fs \
	Ethereal\ Spectrum\ Cascade.fs \
	Everflow.fs \
	Gaussian\ SmoothLife.fs \
	Le\ Vortex.fs \
	Lorenz\ system.fs \
	oscilloscope\ analysis\ luminance.fs \
	Physarum\ Polycephalum\ Simulation.fs \
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
Endless\ living\ creature.fs: Endless\ living\ creature.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Ethereal\ Spectrum\ Cascade.fs: Ethereal\ Spectrum\ Cascade.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Everflow.fs: Everflow.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Gaussian\ SmoothLife.fs: Gaussian\ SmoothLife.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Le\ Vortex.fs: Le\ Vortex.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Lorenz\ system.fs: Lorenz\ system.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
oscilloscope\ analysis\ luminance.fs: oscilloscope\ analysis\ luminance.glsl
	clang --preprocess --comments --no-line-commands -fdirectives-only --language=c --output="$@" "$<"
Physarum\ Polycephalum\ Simulation.fs: Physarum\ Polycephalum\ Simulation.glsl
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
