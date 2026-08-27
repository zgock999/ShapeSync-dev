# Installation and Dependencies

## Supported Unity versions

ShapeSync requires Unity 6.0 LTS or later. Development and validation use
Unity 6.3 LTS. Use a Unity 6 project with its normal project-system packages
enabled.

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
   `4.5.0` by name.
3. Open NuGetForUnity's package manager and install NuGet package `R3`
   version `1.3.1`. Keep its restored .NET dependency closure in the consumer
   project's generated package area; do not manually copy it into ShapeSync.
4. In Package Manager, add `com.cysharp.r3` version `1.3.1` by name.
5. In Package Manager, choose **Add package from git URL** and add:

   ```text
   https://github.com/zgock999/ShapeSync-dev.git?path=Packages/net.zgock-lab.shapesync#0.2.0-preview
   ```

   The `?path=` subfolder must precede the `#0.2.0-preview` revision.
6. Let Unity compile. Core is ready when the
   `zgock.ShapeSync.Runtime` and `zgock.ShapeSync.Editor` assemblies compile
   without UniVRM installed.

Core-only projects stop after step 5. The Core package has no UniVRM or Unity
Input System dependency.

## Optional VRM Integration companion

For VRM workflows, install these packages in order before adding the
companion:

```text
com.vrmc.gltf 0.131.1
com.vrmc.vrm 0.131.1
```

Then choose **Add package from git URL** and add:

```text
https://github.com/zgock999/ShapeSync-dev.git?path=Packages/net.zgock-lab.shapesync.vrm#0.2.0-preview
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

Open **Window > General > Test Runner** and run the package EditMode and
PlayMode assemblies. The package repository intentionally provides Slim Tests
only. Rich Tests, PlayTest content, Human Test fixtures, and Sandbox assets
belong to the private development environment.

## Licensing and redistribution

ShapeSync source is MIT licensed; see the repository root `LICENSE`. R3 and
UniVRM are external dependencies and are not redistributed by ShapeSync.
Follow their own license and distribution terms when adding them to a project.

## Package URL notes

The two ShapeSync URLs use the same lockstep `0.2.0-preview` tag. The Core URL
must be installed before the companion because the companion declares Core as a
package dependency. The `?path=` component selects a package subfolder in the
repository and must appear before the `#revision` component.
