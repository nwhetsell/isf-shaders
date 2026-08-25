# ISF Shaders

This is a collection of [ISF shaders](https://isf.video/).
You should be able to use these shaders in any app that supports ISF.

Each shader consists of an .fs and a .glsl file.
The .fs file is the .glsl file with `#include` directives expanded using the [Clang](https://clang.llvm.org) preprocessor.
(This is needed because the GLSL preprocessor [does not support `#include`](https://wikis.khronos.org/opengl/Core_Language_(GLSL)#Preprocessor_directives).)
Due to limitations of the Clang preprocessor, `#if`/`#else` directives are also evaluated when expanding `#include` directives.

Many shaders use code from [LYGIA](https://github.com/patriciogonzalezvivo/lygia), which for non-commercial use is distributed under the [Prosperity Public License 3.0.0](https://prosperitylicense.com/versions/3.0.0).

On macOS, after cloning this repository you can run the [make_links.sh](make_links.sh) script to add symbolic links to .fs files in the repository’s parent folder. This can be used to expose these shaders to apps like [Videosync](https://www.showsync.com/videosync).

Many of these shaders are intended to be used with floating-point buffers.
Not all ISF hosts support floating-point buffers:
Videosync supports floating-point buffers (in [v2.0.12](https://support.showsync.com/release-notes/videosync/2.0#2012) and later),
but https://editor.isf.video does not.
If floating-point buffers are not available, most of these shaders will look very different (if they run at all).

<!--
For screenshots, image sizes and corresponding ImageMagick -crop arguments are:
* 1824x1424 : '1576x1176+124+92'
* 1736x1336 : '1576x1176+80+64'
* 1692x1292 : '1576x1176+48+40'
-->
<table>
  <tr>
    <th>Shader</th>
    <th>Original Author</th>
    <th>Licenses</th>
    <th>Demos</th>
    <th>Videosync Screenshot</th>
  </tr>
  <tr>
    <td><a href="CA%20Molecular%20dynamics.fs">CA Molecular dynamics</a></td>
    <td><a href="https://github.com/MichaelMoroz">Mykhailo Moroz</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/3s3cWr">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/6a5b6b524d2be7001ab74daa">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “CA Molecular dynamics” shader" src="https://github.com/user-attachments/assets/0dfd7e90-2f53-4429-aff7-7ce9e9b224ce" />
    </td>
  </tr>
  <tr>
    <td><a href="Cell%20system%202.fs">Cell system 2</a></td>
    <td><a href="https://github.com/MichaelMoroz">Mykhailo Moroz</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/3tSfRW">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/68cada72abb222001a42145e">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “Cell system 2” shader" src="https://github.com/user-attachments/assets/61a7d910-fe93-4435-a089-46296b671bdb" />
    </td>
  </tr>
  <tr>
    <td><a href="circle%20dithering.fs">circle dithering</a></td>
    <td><a href="https://www.shadertoy.com/user/FabriceNeyret2">Fabrice Neyret</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/MdSfWK">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/6a8995e2734c1a00193077c5">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “circle dithering” shader" src="https://github.com/user-attachments/assets/6096eb6e-0a52-4989-8d86-5b9e27aef640" />
    </td>
  </tr>
  <tr>
    <td><a href="Endless%20living%20creature.fs">Endless living creature</a></td>
    <td><a href="https://www.shadertoy.com/user/leon">Leon Denise</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/tljXWy">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/69c3befc13a628001ac60cb1">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “Endless living creature” shader" src="https://github.com/user-attachments/assets/f71f48c1-371b-414e-b63f-63f04d7106a9" />
    </td>
  </tr>
  <tr>
    <td><a href="Ethereal Spectrum Cascade.fs">Ethereal Spectrum Cascade</a></td>
    <td><a href="https://www.shadertoy.com/user/GPT4POWERUSER">GPT4POWERUSER</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/DsVSRy">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/6a8acb8bec1b1d001980c44f">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “Ethereal Spectrum Cascade” shader" src="https://github.com/user-attachments/assets/2c77d57d-31fc-418f-a903-9cb75352fd04" />
    </td>
  </tr>
  <tr>
    <td><a href="Everflow.fs">Everflow</a></td>
    <td><a href="https://github.com/MichaelMoroz">Mykhailo Moroz</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/ttBcWm">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/6a63abd74fe9d6001a8723be">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “Everflow” shader" src="https://github.com/user-attachments/assets/58878b46-f931-41e1-9bea-a0772debc691" />
    </td>
  </tr>
  <tr>
    <td><a href="Gaussian%20SmoothLife.fs">Gaussian SmoothLife</a></td>
    <td><a href="https://www.shadertoy.com/user/cornusammonis">cornusammonis</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/XtVXzV">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/6885276e2f6812001a55f70b">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “Gaussian SmoothLife” shader" src="https://github.com/user-attachments/assets/4c51ad5b-868f-450b-8068-69bd912841d4" />
    </td>
  </tr>
  <tr>
    <td><a href="Le%20Vortex.fs">Le Vortex</a></td>
    <td><a href="https://www.shadertoy.com/user/leon">Leon Denise</a></td>
    <!-- Based on page 34 or 36 of Le Processus (1993) <https://fr.wikipedia.org/wiki/Le_Processus> by Marc-Antoine Mathieu <https://fr.wikipedia.org/wiki/Marc-Antoine_Mathieu> -->
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/XlfBR7">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/6918c41f66081f001a4b4adc">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “Le Vortex” shader" src="https://github.com/user-attachments/assets/a18fb39d-b84a-4917-9361-72a5829b1044" />
    </td>
  </tr>
  <tr>
    <td><a href="Lorenz%20system.fs">Lorenz system</a></td>
    <td><a href="https://www.shadertoy.com/user/Flyguy">Flyguy</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/XddGWj">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/696fc12680748c001a899e44">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “Lorenz system” shader" src="https://github.com/user-attachments/assets/cc974df7-b9a2-4b33-be46-8ce07cdff7dc" />
    </td>
  </tr>
  <tr>
    <td><a href="oscilloscope%20analysis%20luminance.fs">oscilloscope analysis luminance</a></td>
    <td><a href="https://www.shadertoy.com/user/FabriceNeyret2">Fabrice Neyret</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <p><a href="https://www.shadertoy.com/view/Nttyz4">Shadertoy</a> only <!-- This shader uses <a href="https://registry.khronos.org/OpenGL-Refpages/gl4/html/fwidth.xhtml"><code>fwidth</code></a>, which isn’t available on https://editor.isf.video --></p>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “oscilloscope analysis luminance” shader" src="https://github.com/user-attachments/assets/8c9afa33-4983-4490-a60c-866368a44f2c" />
    </td>
  </tr>
  <tr>
    <td><a href="Physarum%20Polycephalum%20Simulation.fs">Physarum Polycephalum Simulation</a></td>
    <td><a href="https://github.com/MichaelMoroz">Mykhailo Moroz</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/tlKGDh">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/68618b61932476001a3ae982">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “Physarum Polycephalum Simulation” shader" src="https://github.com/user-attachments/assets/ec175ac4-7659-4611-9dcb-e0a9490f7d88" />
    </td>
  </tr>
  <tr>
    <td><a href="Random%20slime%20mold%20generator.fs">Random slime mold generator</a></td>
    <td><a href="https://github.com/MichaelMoroz">Mykhailo Moroz</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/ttsfWn">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/689247373bc53b001a4d81b3">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “Random slime mold generator” shader" src="https://github.com/user-attachments/assets/9f804502-69c1-485b-bcfe-dc99238c77ed" />
    </td>
  </tr>
  <tr>
    <td><a href="Shattered%20Crystal.fs">Shattered Crystal</a></td>
    <td><a href="https://www.shadertoy.com/user/Hyeve">Hyeve</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/ssXcR2">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/69ac8d9ad1b29b0019164aea">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt=" of “Shattered Crystal” shader" src="https://github.com/user-attachments/assets/f7195e50-5297-431e-ac79-12533ad5321e" />
    </td>
  </tr>
  <tr>
    <td><a href="spilled.fs">spilled</a></td>
    <td><a href="https://www.flockaroo.at">Florian Berger</a></td>
    <td><a href="https://spdx.org/licenses/CC-BY-NC-SA-3.0.html">CC-BY-NC-SA-3.0</a></td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/MsGSRd">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/688bd4413bc53b001a4d37fc">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “spilled” shader" src="https://github.com/user-attachments/assets/627a035e-1122-4bfa-8f7d-4f81cbded1d7" />
    </td>
  </tr>
  <tr>
    <td><a href="Suture%20Fluid.fs">Suture Fluid</a></td>
    <td><a href="https://www.shadertoy.com/user/cornusammonis">cornusammonis</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/XddSRX">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/688e4c703bc53b001a4d55ce">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “Suture Fluid” shader" src="https://github.com/user-attachments/assets/a6b2b1b0-b25a-4bf4-8ae5-306ef065fa31" />
    </td>
  </tr>
  <tr>
    <td><a href="The Weave.fs">The Weave</a></td>
    <td><a href="https://www.shadertoy.com/user/chronos">chronos</a></td>
    <td>n/a</td>
    <td>
      <ul>
        <li><a href="https://www.shadertoy.com/view/W3SSRm">Shadertoy</a></li>
        <li><a href="https://editor.isf.video/shaders/688f907d3bc53b001a4d6198">ISF</a></li>
      </ul>
    </td>
    <td>
      <img width="197" alt="Screenshot of “The Weave” shader" src="https://github.com/user-attachments/assets/4d138968-ae6b-46a4-8163-ad4607f6bad2" />
    </td>
  </tr>
</table>
