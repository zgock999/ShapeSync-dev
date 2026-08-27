# ShapeSync

ShapeSync is a Unity toolset for building and attaching deformable character
Outfits that follow Figure body morphs.  The Core works without UniVRM; the
VRM Integration companion is an optional layer for VRM 1.0 initialization,
expression baking, and SpringBone transport.

## Requirements

- Unity 6.0 LTS or later (validation baseline: Unity 6000.3.18f1)
- Git 2.14 or later, with HTTPS access to the package repository
- NuGetForUnity 4.5.0 for the R3 .NET core dependency
- UniVRM 0.131.1 only when using the optional VRM Integration companion

ShapeSync Core does not require UniVRM or the Unity Input System.

## Install

The package repository is consumed through Unity Package Manager. The
installation order is significant because the VRM companion depends on the
ShapeSync Core package, while R3's .NET core assemblies are supplied by
NuGetForUnity rather than by a ShapeSync package.

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

### 2. Install NuGetForUnity and R3

In Package Manager, add the package by name:

```text
com.github-glitchenzo.nugetforunity 4.5.0
```

Open NuGetForUnity's package manager, search for `R3`, and install version
`1.3.1`. Allow it to restore its .NET dependency closure into the consumer
project. Do not copy R3 DLLs into this repository or into either ShapeSync
package manually.

### 3. Install the R3 Unity adapter

In Package Manager, add by name:

```text
com.cysharp.r3 1.3.1
```

### 4. Install UniVRM only for VRM workflows

For VRM use, add these packages in order:

```text
com.vrmc.gltf 0.131.1
com.vrmc.vrm 0.131.1
```

Core-only projects skip this step.

### 5. Install ShapeSync Core from Git

In Package Manager, choose **Add package from git URL** and enter:

```text
https://github.com/zgock999/ShapeSync-dev.git?path=Packages/net.zgock-lab.shapesync#0.2.0-preview
```

The `?path=` subfolder must appear before `#0.2.0-preview`. The revision is
the lockstep package tag and must not be replaced with an unverified short
SHA.

### 6. Enable the optional VRM companion

After Core is installed, add the companion from git:

```text
https://github.com/zgock999/ShapeSync-dev.git?path=Packages/net.zgock-lab.shapesync.vrm#0.2.0-preview
```

Then add `SHAPESYNC_USE_UNIVRM` under **Project Settings > Player > Scripting
Define Symbols**. Keep the symbol absent for Core-only projects.

Core-only installation is steps 1, 2, 3, and 5.

## Troubleshooting

### R3 types are missing

If Unity reports errors such as:

```text
The type or namespace name 'Collections' does not exist in the namespace 'R3'
The type or namespace name 'FrameProvider' could not be found
```

NuGetForUnity or the NuGet `R3` package was skipped. Install NuGetForUnity,
restore `R3 1.3.1`, and allow Unity to recompile before adding ShapeSync.

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
`#0.2.0-preview` revision.

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

The package repository contains Slim Tests only. After installing the Core,
open **Window > General > Test Runner** and run the package EditMode and
PlayMode assemblies. The internal Sandbox, Rich Tests, Human Test evidence,
and PlayTest assets are not part of the package distribution.

## License

ShapeSync is released under the [MIT License](LICENSE). R3 and UniVRM are
external dependencies; install them according to the steps above and follow
their own license terms. ShapeSync does not redistribute their source or
binaries.
