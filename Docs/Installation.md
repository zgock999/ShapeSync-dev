# Installation and Dependencies

## Requirements

ShapeSync requires Unity 6.0 LTS or later. Development and validation use
Unity `6000.3.18f1` with Universal Render Pipeline `17.3.0`.

Install and resolve `com.unity.render-pipelines.universal` `17.0.0` or later.
The lower bound follows the Unity 6 URP 17.x line required by the Phase0
shader identities; `17.3.0` is the tested baseline. ShapeSync Phase0 supports
URP only. Built-in RP, HDRP, and custom SRP are outside the supported scope.

The consumer project must use **Linear** color space. The Core Slim Test and
the automatic Texture StackMachine Factory additionally require the
project-owned asset at
`Assets/Resources/zgock/ShapeSync/TextureStaticMachineFactorySettings.asset`.
The Core-only define set must not contain `SHAPESYNC_USE_UNIVRM`.

## Choose the project template

The recommended starting point is Unity Hub's **Universal 3D** template. On
Unity `6000.3.18f1`, the installed template package
`com.unity.template.3d-cross-platform-17.0.14` was measured to provide URP
`17.0.1` in its manifest, `m_ActiveColorSpace: 1` (Linear), and a
`GraphicsSettings.m_CustomRenderPipeline` assignment to its URP asset. In
that route, installation step 5 and the color-space part of step 8 below are
**confirmations**, not additional setup actions. The template does not create
ShapeSync's Factory Settings asset, so step 9 remains required.

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
   `4.5.0` by name.
3. Open NuGetForUnity's package manager and install NuGet package `R3`
   version `1.3.1`. Keep its restored .NET dependency closure in the consumer
   project's generated package area; do not manually copy it into ShapeSync.
   This UI step creates or updates the consumer-side `Assets/packages.config`
   and `NuGet.config`; keep those files with the consumer project. For a
   command-line verification, `nugetforunity restore <project-path>` is
   equivalent to the UI restore step.
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
6. In Package Manager, choose **Add package from git URL** and add:

   ```text
   https://github.com/zgock999/ShapeSync-dev.git?path=Packages/net.zgock-lab.shapesync#0.2.0-preview
   ```

   The `?path=` subfolder must precede the `#0.2.0-preview` revision.
7. Let Unity compile. Core is ready when the
   `zgock.ShapeSync.Runtime` and `zgock.ShapeSync.Editor` assemblies compile
   without UniVRM installed.

8. Confirm or set **Project Settings > Player > Other Settings > Rendering >
   Color Space**. For a Universal 3D project, confirm it is **Linear**; the
   measured template default is `m_ActiveColorSpace: 1`, so no change is
   needed when it is present. For a Built-in RP or other project, set it to
   **Linear**. ShapeSync's material and texture contracts use Linear RGBA.
9. Create the project-owned Texture StackMachine Factory settings after Core
   has resolved. The package intentionally does not write into `Assets/Resources`.
   Add this temporary file as `Assets/Editor/ShapeSyncConsumerSetup.cs`, then
   run **ShapeSync > Setup > Create Texture Factory Settings** once:

   ```csharp
   #if UNITY_EDITOR
   using System;
   using UnityEditor;
   using UnityEngine;
   using zgock.ShapeSync.StackMachine;

   internal static class ShapeSyncConsumerSetup
   {
       private const string Folder = "Assets/Resources/zgock/ShapeSync";
       private const string AssetPath = Folder + "/TextureStaticMachineFactorySettings.asset";
       private const string PrefabPath = "Packages/net.zgock-lab.shapesync/Runtime/StackMachine/Texture/TextureStackMachineHost.prefab";

       [MenuItem("ShapeSync/Setup/Create Texture Factory Settings")]
       private static void CreateTextureFactorySettings()
       {
           EnsureFolder("Assets/Resources");
           EnsureFolder("Assets/Resources/zgock");
           EnsureFolder(Folder);

           if (AssetDatabase.LoadAssetAtPath<TextureStaticMachineFactorySettings>(AssetPath) != null)
               return;

           TextureStackMachineHost prefab = AssetDatabase.LoadAssetAtPath<TextureStackMachineHost>(PrefabPath);
           if (prefab == null)
               throw new InvalidOperationException("ShapeSync TextureStackMachineHost prefab was not found: " + PrefabPath);

           TextureStaticMachineFactorySettings settings = ScriptableObject.CreateInstance<TextureStaticMachineFactorySettings>();
           SerializedObject serialized = new SerializedObject(settings);
           serialized.FindProperty("textureStackMachineHostPrefab").objectReferenceValue = prefab;
           serialized.ApplyModifiedPropertiesWithoutUndo();
           AssetDatabase.CreateAsset(settings, AssetPath);
           AssetDatabase.SaveAssets();
           Selection.activeObject = settings;
       }

       private static void EnsureFolder(string path)
       {
           if (AssetDatabase.IsValidFolder(path)) return;
           string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
           string leaf = path.Substring(parent.Length).TrimStart('/');
           EnsureFolder(parent);
           AssetDatabase.CreateFolder(parent, leaf);
       }
   }
   #endif
   ```

   After the asset is created, the temporary setup script may be removed.
   Keep the generated `.asset` and its `.meta` in the consumer project.

Core-only projects stop after step 9. Steps 1, 2, 3, 4, 6, and the Factory
Settings action in step 9 are required in both routes. For Universal 3D,
step 5 and the color-space check in step 8 are confirmations; for Built-in RP,
step 5 installs URP and step 8 sets Linear. The Core package has no UniVRM or
Unity Input System dependency.

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

Before verification, confirm that URP is installed, the project Color Space is
**Linear**, the Core package is resolved with `SHAPESYNC_USE_UNIVRM` absent, and
the project-owned Factory Settings asset exists at the exact
`Assets/Resources/zgock/ShapeSync` path. For package test discovery, add the
Core package ID to the consumer project's `testables` list in
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

## Licensing and redistribution

ShapeSync source is MIT licensed; see the repository root `LICENSE`. R3 and
UniVRM are external dependencies and are not redistributed by ShapeSync.
Follow their own license and distribution terms when adding them to a project.

## Package URL notes

The two ShapeSync URLs use the same lockstep `0.2.0-preview` tag. The Core URL
must be installed before the companion because the companion declares Core as a
package dependency. The `?path=` component selects a package subfolder in the
repository and must appear before the `#revision` component.
