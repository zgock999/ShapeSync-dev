# Installation and Dependencies

## Requirements

ShapeSync requires Unity 6.0 LTS or later. Development and validation use
Unity `6000.3.18f1` with Universal Render Pipeline `17.3.0`.

Install and resolve `com.unity.render-pipelines.universal` `17.0.0` or later.
The lower bound follows the Unity 6 URP 17.x line required by the Phase0
shader identities; `17.3.0` is the tested baseline. ShapeSync Phase0 supports
URP only. Built-in RP, HDRP, and custom SRP are outside the supported scope.

Use a graphics API with async compute queue and fence support, such as D3D12
or Vulkan. D3D11 is not a supported or guaranteed configuration because the
Texture StackMachine uses an async compute queue and `GraphicsFence`.

The consumer project must use **Linear** color space. The Core package ships
the default Texture StackMachine Factory Settings asset used by the automatic
Factory path and Core Slim Tests.
The Core-only define set must not contain `SHAPESYNC_USE_UNIVRM`.

## Choose the project template

The recommended starting point is Unity Hub's **Universal 3D** template. On
Unity `6000.3.18f1`, the installed template package
`com.unity.template.3d-cross-platform-17.0.14` was measured to provide URP
`17.0.1` in its manifest, `m_ActiveColorSpace: 1` (Linear), and a
`GraphicsSettings.m_CustomRenderPipeline` assignment to its URP asset. In
that route, installation step 5 and the color-space part of step 8 below are
**confirmations**, not additional setup actions. The template does not know
about ShapeSync; the Core package supplies its default Factory Settings asset.

If the project was created from Built-in RP or another non-URP template,
follow the explicit URP installation in step 5 and set Linear color space in
step 8. Built-in RP, HDRP, and custom SRP are outside ShapeSync Phase0
support.

## Install the Core package

ShapeSync is installed from the package repository through Unity Package
Manager. Perform these steps in order:

1. In **Edit > Project Settings > Package Manager > Scoped Registries**, add:

   ```text
   Name: OpenUPM
   URL: https://package.openupm.com
   Scopes: com.cysharp, com.vrmc, com.github-glitchenzo
   ```

   The `com.github-glitchenzo` scope is only needed for NuGetForUnity.
2. In Package Manager, add `com.github-glitchenzo.nugetforunity` version
   `4.5.0` by name. Then open **NuGet > Manage NuGet Packages** in the Unity
   main menu. The Package Manager entry `R3 1.3.1` belongs to step 3 and is
   the `com.cysharp.r3` Unity adapter; it is not the NuGet package required by
   this step.
3. In the NuGetForUnity window, search for NuGet package `R3` and install
   version `1.3.1`. Complete this through the UI; do not substitute a
   hand-written `packages.config` or the CLI. Verify the actual payload
   instead of a fixed dependency count: consumer `Assets/packages.config`
   contains the `R3` `1.3.1` entry with `manuallyInstalled="true"`, and
   `Assets/Packages/R3.1.3.1/lib/.../R3.dll` exists. The transitive closure
   count is environment-dependent. Keep the restored .NET closure in the
   consumer project; do not copy it into ShapeSync.
4. In Package Manager, add `com.cysharp.r3` version `1.3.1` by name.
5. Confirm or install URP. For a Universal 3D project, confirm that the
   manifest contains URP `17.0.1` or a later `17.x` version and that a URP
   Render Pipeline Asset is assigned in **Project Settings > Graphics** /
   **Quality**. No installation action is needed when those template defaults
   are present. For a Built-in RP or other non-URP project, install and
   resolve:

   ```text
   com.unity.render-pipelines.universal 17.0.0 or later
   ```

   The validation baseline is `17.3.0` on Unity `6000.3.18f1`. For an
   application scene, assign a URP Render Pipeline Asset under **Project
   Settings > Graphics** / **Quality**.
   On Windows, open **Edit > Project Settings > Player > Other Settings >
   Rendering > Graphics APIs for Windows** and confirm that D3D11 is not the
   first or only API. Use D3D12 or Vulkan for async compute queue/fence
   support; D3D11 is not supported or guaranteed.
6. In Package Manager, choose **Add package from git URL** and add:

   ```text
   https://github.com/zgock999/ShapeSync-dev.git?path=Packages/net.zgock-lab.shapesync#0.2.0-preview3
   ```

   The `?path=` subfolder must precede the `#0.2.0-preview3` revision.
7. Let Unity compile. Core is ready when the
   `zgock.ShapeSync.Runtime` and `zgock.ShapeSync.Editor` assemblies compile
   without UniVRM installed.

8. Confirm or set **Project Settings > Player > Other Settings > Rendering >
   Color Space**. For a Universal 3D project, confirm it is **Linear**; the
   measured template default is `m_ActiveColorSpace: 1`, so no change is
   needed when it is present. For a Built-in RP or other project, set it to
   **Linear**. ShapeSync's material and texture contracts use Linear RGBA.

Core-only projects stop after step 8. Steps 1, 2, 3, 4, 6, and 8 are
required in both routes. For Universal 3D, step 5 and the color-space check
in step 8 are confirmations; for Built-in RP, step 5 installs URP and step 8
sets Linear. The Core package has no UniVRM or Unity Input System dependency.

## Optional VRM Integration companion

For VRM workflows, install these packages in order before adding the
companion:

```text
com.vrmc.gltf 0.131.1
com.vrmc.vrm 0.131.1
```

Then choose **Add package from git URL** and add:

```text
https://github.com/zgock999/ShapeSync-dev.git?path=Packages/net.zgock-lab.shapesync.vrm#0.2.0-preview3
```

Finally add the scripting define symbol below in **Project Settings > Player >
Scripting Define Symbols**:

```text
SHAPESYNC_USE_UNIVRM
```

With Core, UniVRM, the companion, and the symbol present, these companion
assemblies compile:

- `zgock.ShapeSync.VrmIntegration.Runtime`
- `zgock.ShapeSync.VrmIntegration.Editor`

The Core does not reference UniVRM types and remains usable when the symbol is
absent. Do not add `SHAPESYNC_USE_UNIVRM` merely to use Core-only workflows.

## Verification

Before verification, confirm that URP is installed, the project Color Space is
**Linear**, and the Core package is resolved with `SHAPESYNC_USE_UNIVRM` absent.
The default Factory Settings asset is shipped inside the Core package. For
package test discovery, add the Core package ID to the consumer project's
`testables` list in
`Packages/manifest.json`:

```json
"testables": ["net.zgock-lab.shapesync"]
```

This enables the package's test assemblies for the Test Runner and is not a
runtime dependency. Open **Window > General > Test Runner** and run the
package EditMode and PlayMode assemblies. A clean Core-only run is expected to
be approximately 1,175 EditMode tests and 136 PlayMode tests. In batchmode,
the two documented environment-specific failures may appear; any other
failure or any inconclusive result is a test failure and must be reported.

The package repository intentionally provides Slim Tests only. Rich Tests,
PlayTest content, Human Test fixtures, and Sandbox assets belong to the private
development environment.

## Troubleshooting

### R3 types are missing

If Unity reports `CS0246` for `Observable<>` or `Unit` inside `com.cysharp.r3`,
the NuGet R3 package was not installed. An empty `Assets/packages.config` means
step 2 was not completed; the similarly named `R3 1.3.1` in Unity Package
Manager is only the step 3 adapter. Open **NuGet > Manage NuGet Packages**,
install NuGet `R3 1.3.1`, verify
`Assets/Packages/R3.1.3.1/lib/.../R3.dll`, and let Unity recompile.

### Texture processing fails on Windows

If the log contains an exception such as
`NotSupportedException: Cannot determine if this AsyncQueueSynchronisation
Graphics...`, check **Player Settings > Other Settings > Rendering > Graphics
APIs for Windows**. D3D11 does not provide the async compute queue/fence
capability used by Texture StackMachine. Use D3D12 or Vulkan; D3D11 is not
supported or guaranteed.

## Licensing and redistribution

ShapeSync source is MIT licensed; see the repository root `LICENSE`. R3 and
UniVRM are external dependencies and are not redistributed by ShapeSync.
Follow their own license and distribution terms when adding them to a project.

## Package URL notes

The two ShapeSync URLs use the same lockstep `0.2.0-preview3` tag. The Core URL
must be installed before the companion because the companion declares Core as a
package dependency. The `?path=` component selects a package subfolder in the
repository and must appear before the `#revision` component.
