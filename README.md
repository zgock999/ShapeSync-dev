# ShapeSync

ShapeSync is a Unity toolset for building and attaching deformable character
Outfits that follow Figure body morphs.  The Core works without UniVRM; the
VRM Integration companion is an optional layer for VRM 1.0 initialization,
expression baking, and SpringBone transport.

## Requirements

- Unity 6.0 LTS or later (validation baseline: Unity 6000.3.18f1)
- Universal Render Pipeline (URP) 17.0.0 or later. The validation baseline is
  `com.unity.render-pipelines.universal` 17.3.0; ShapeSync Phase0 supports URP
  only, and the URP package supplies the required Lit/Unlit shader identities.
- A graphics API with async compute queue and fence support, such as D3D12 or
  Vulkan. D3D11 is not a supported or guaranteed configuration for Texture
  StackMachine processing.
- Git 2.14 or later, with HTTPS access to the package repository
- NuGetForUnity 4.5.0 for the R3 .NET core dependency
- UniVRM 0.131.1 only when using the optional VRM Integration companion

ShapeSync Core does not require UniVRM or the Unity Input System.
The Core package includes the default Texture StackMachine Factory Settings
asset. Consumer projects must use Linear color space for the Core Slim Tests
and the automatic Texture StackMachine Factory path.

## Install

The package repository is consumed through Unity Package Manager. The
installation order is significant because the VRM companion depends on the
ShapeSync Core package, while R3's .NET core assemblies are supplied by
NuGetForUnity rather than by a ShapeSync package.

### Choose the project template

The recommended starting point is Unity Hub's **Universal 3D** template. On
Unity `6000.3.18f1`, the installed template package
`com.unity.template.3d-cross-platform-17.0.14` was measured to provide URP
`17.0.1` in its manifest, `m_ActiveColorSpace: 1` (Linear), and a
`GraphicsSettings.m_CustomRenderPipeline` assignment to its URP asset. With
that template, step 4 and the color-space part of step 6 below are
**confirmations**, not additional setup actions. The template does not know
about ShapeSync; the Core package supplies its default Factory Settings asset.

If the project was created from Built-in RP or another non-URP template,
follow the explicit URP installation in step 4 and set Linear color space in
step 6. Built-in RP, HDRP, and custom SRP are outside ShapeSync Phase0
support.

### 1. Add the OpenUPM scoped registry

In **Edit > Project Settings > Package Manager > Scoped Registries**, add:

```text
Name: OpenUPM
URL: https://package.openupm.com
Scopes: com.cysharp, com.vrmc, com.github-glitchenzo
```

`com.cysharp` and `com.vrmc` are required for ShapeSync dependencies.
`com.github-glitchenzo` is required only to install NuGetForUnity from the
same registry.

### 2. Install NuGetForUnity and the NuGet R3 package

In Package Manager, add the package by name:

```text
com.github-glitchenzo.nugetforunity 4.5.0
```

Open the Unity menu **NuGet > Manage NuGet Packages**, search for the NuGet
package `R3`, and install version `1.3.1` there. This is a different package
from the `R3 1.3.1` entry shown by Unity Package Manager in step 3: the latter
is the `com.cysharp.r3` Unity adapter. Do not treat that Package Manager entry
as proof that the NuGet package was installed.

Complete this step through the NuGetForUnity UI. It creates or updates the
consumer-side `Assets/packages.config` and `NuGet.config` and restores the
required .NET closure. Do not copy R3 DLLs into this repository or into either
ShapeSync package manually. Verify the actual payload, rather than a fixed
dependency count: `Assets/packages.config` contains `R3` version `1.3.1` with
`manuallyInstalled="true"`, and
`Assets/Packages/R3.1.3.1/lib/.../R3.dll` exists. The transitive package count
is environment-dependent.

### 3. Install the R3 Unity adapter

In Package Manager, add by name:

```text
com.cysharp.r3 1.3.1
```

### 4. Confirm or install URP

For a Universal 3D project, confirm that the manifest contains URP `17.0.1`
or a later `17.x` version and that a URP Render Pipeline Asset is assigned in
**Project Settings > Graphics** / **Quality**. No installation action is
needed when those template defaults are present.

For a Built-in RP or other non-URP project, install and resolve:

```text
com.unity.render-pipelines.universal 17.0.0 or later
```

The validation baseline is `17.3.0` on Unity `6000.3.18f1`. The lower bound is
the Unity 6 URP 17.x line required by the Phase0 shader identities; `17.3.0`
is the tested version, not a request to change the fixed ShapeSync package tag.
For an application scene, assign a URP Render Pipeline Asset in the usual
**Project Settings > Graphics** / **Quality** locations. Built-in RP, HDRP,
and custom SRP are outside ShapeSync Phase0 support.

For Windows, open **Edit > Project Settings > Player > Other Settings >
Rendering > Graphics APIs for Windows** and confirm that D3D11 is not the
first or only API. Select an async-compute-capable API such as D3D12 or Vulkan.
The Texture StackMachine uses an async compute queue and `GraphicsFence`; D3D11
is not supported or guaranteed.

### 5. Install ShapeSync Core from Git

In Package Manager, choose **Add package from git URL** and enter:

```text
https://github.com/zgock999/ShapeSync-dev.git?path=Packages/net.zgock-lab.shapesync#0.2.0-preview5
```

The `?path=` subfolder must appear before `#0.2.0-preview5`. The revision is
the lockstep package tag and must not be replaced with an unverified short
SHA.

### 6. Confirm or set Linear color space

For a Universal 3D project, confirm **Project Settings > Player > Other
Settings > Rendering > Color Space** is **Linear**. The measured template
default is `m_ActiveColorSpace: 1`; no change is needed when it is present.
For a Built-in RP or other project, set the same property to **Linear**.
ShapeSync's material and texture contracts use Linear RGBA.

### 7. Install UniVRM only for VRM workflows

For VRM use, add these packages in order:

```text
com.vrmc.gltf 0.131.1
com.vrmc.vrm 0.131.1
```

Core-only projects skip this step.

### 8. Enable the optional VRM companion

After Core is installed, add the companion from git:

```text
https://github.com/zgock999/ShapeSync-dev.git?path=Packages/net.zgock-lab.shapesync.vrm#0.2.0-preview5
```

Then add `SHAPESYNC_USE_UNIVRM` under **Project Settings > Player > Scripting
Define Symbols**. Keep the symbol absent for Core-only projects.

For Core-only projects, steps 1, 2, 3, 5, and 6 are required in both routes.
In the Universal 3D route, step 4 and step 6 are confirmations of
template-provided settings; in the Built-in RP route, step 4 installs URP and
step 6 sets Linear. Step 7 and step 8 are only for
VRM workflows.

## Troubleshooting

### R3 types are missing

If Unity reports errors such as:

```text
The type or namespace name 'Collections' does not exist in the namespace 'R3'
The type or namespace name 'FrameProvider' could not be found
```

or `Observable<>` / `Unit` errors inside `com.cysharp.r3`, the NuGet R3 package
was not installed. If `Assets/packages.config` is empty or does not contain
`R3` with `manuallyInstalled="true"`, step 2 was not completed; the similarly
named `R3 1.3.1` in Unity Package Manager is only the step 3 adapter. Open
**NuGet > Manage NuGet Packages**, install NuGet `R3 1.3.1`, verify
`Assets/Packages/R3.1.3.1/lib/.../R3.dll`, and let Unity recompile.

### Texture processing fails on Windows

If the log contains an exception such as:

```text
NotSupportedException: Cannot determine if this AsyncQueueSynchronisation Graphics...
```

check **Player Settings > Other Settings > Rendering > Graphics APIs for
Windows**. D3D11 does not provide the async compute queue/fence capability used
by Texture StackMachine. Use D3D12 or Vulkan; D3D11 is not supported or
guaranteed.

### The VRM companion cannot find Core

The companion was added before the Core git package. Remove the companion,
add the Core URL first, wait for package resolution, then add the companion
and enable `SHAPESYNC_USE_UNIVRM`.

### Git reports a pathspec error

An incorrect `?path=` value produces an error similar to:

```text
Cannot checkout repository ... pathspec ... did not match any file(s) known to git
```

Use the exact URL above, including `.git`, the package subfolder, and the
`#0.2.0-preview5` revision.

## Documentation

- [Installation and dependencies](Docs/Installation.md)
- [Release packaging process](Docs/Packaging.md)
- [Getting Started (Japanese)](Docs/Guides/tutorial.html)
- [Getting Started (English)](Docs/Guides/tutorial_en.html)
- [Advanced Features (Japanese)](Docs/Guides/advanced.html)
- [Advanced Features (English)](Docs/Guides/advanced_en.html)
- [VRM Integration Guide (Japanese)](Docs/Guides/vrm.html)
- [Building the API reference](Docs/ApiReferenceBuild.md)

## Testing

The package repository contains Slim Tests only. Before testing, confirm that
URP is installed, the project Color Space is **Linear**, and the Core package
is resolved with `SHAPESYNC_USE_UNIVRM` absent. The default Factory Settings
asset is shipped inside the Core package. For package test discovery, add the
Core package ID to the consumer project's
`testables` list in `Packages/manifest.json`:

```json
"testables": ["net.zgock-lab.shapesync"]
```

This enables the package's test assemblies for the Test Runner and is not a
runtime dependency. Then open **Window > General > Test Runner** and run the
package EditMode and PlayMode assemblies. A clean Core-only run is expected to
be approximately 1,175 EditMode tests and 136 PlayMode tests. In batchmode,
the two documented environment-specific failures may appear; any other
failure or any inconclusive result is a test failure and must be reported.

The internal Sandbox, Rich Tests, Human Test evidence, and PlayTest assets are
not part of the package distribution.

## License

ShapeSync is released under the [MIT License](LICENSE). R3 and UniVRM are
external dependencies; install them according to the steps above and follow
their own license terms. ShapeSync does not redistribute their source or
binaries.
