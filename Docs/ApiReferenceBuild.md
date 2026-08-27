# ShapeSync API Reference Build Manual

This manual explains how to generate the local ShapeSync API reference with
DocFX. It is an authoring and review tool: generated metadata and HTML output
are ignored by Git and must not be committed.

## Scope

- The Core reference contains `zgock.ShapeSync.Runtime` and
  `zgock.ShapeSync.Editor`.
- The VRM companion reference contains only
  `zgock.ShapeSync.VrmIntegration.Runtime` and
  `zgock.ShapeSync.VrmIntegration.Editor`.
- Test assemblies, PlayTest data, fixtures, and generated Human Test output are
  not part of either reference.

## Prerequisites

1. Open the ShapeSync project once in Unity so that the four `*.csproj` files
   are current.
2. Install a supported .NET SDK. The repository pins DocFX in
   `Docs/Docfx/.config/dotnet-tools.json`; do not install an arbitrary global
   DocFX version for this workflow.
3. Run all commands below from `Docs/Docfx`.

```powershell
Set-Location Docs/Docfx
dotnet tool restore
```

## Build the Core reference

The Core reference must build with the release-default Core-only configuration.
It does not require UniVRM or `SHAPESYNC_USE_UNIVRM`.

```powershell
dotnet tool run docfx metadata docfx.json --logLevel error
dotnet tool run docfx build docfx.json --warningsAsErrors
```

Open `_site/index.html` after a successful build.

## Build the optional VRM companion reference

Before generating this reference, open the project with UniVRM installed and
`SHAPESYNC_USE_UNIVRM` enabled, then let Unity regenerate the project files.
The VRM reference is intentionally separate from Core.

```powershell
dotnet tool run docfx metadata docfx.vrm.json --logLevel error
dotnet tool run docfx build docfx.vrm.json --warningsAsErrors
```

Open `_site-vrm/vrm/index.html` after a successful build.

## Clean rebuild

Use this only from `Docs/Docfx`. These are generated, Git-ignored directories.
Cleaning is useful after changing DocFX source-project inputs or when a removed
API still appears in a generated TOC.

```powershell
Remove-Item -LiteralPath obj/api, _site, obj/vrm-api, _site-vrm -Recurse -Force -ErrorAction SilentlyContinue
```

Then rerun the relevant metadata and build commands above.

## Acceptance checks

- Both commands must finish with `0 warning(s)` and `0 error(s)`.
- Core TOC must contain only `zgock.ShapeSync` and `zgock.ShapeSync.Editor`.
- VRM TOC must contain only `zgock.ShapeSync.VrmIntegration` and its `.Editor`
  namespace.
- Confirm that every new or modified public API has an accurate English XML
  documentation comment. This is a project rule in `AGENT.md`.
- Inherited UnityEngine and UnityEditor member lists are intentionally hidden.
  The reference documents ShapeSync-declared APIs and avoids misleading links
  caused by an unresolved Unity API xref map.

## Troubleshooting

- If a `*.csproj` file or optional UniVRM reference is missing, return to Unity,
  correct the project configuration, and allow Unity to regenerate the files.
- If `dotnet tool run docfx` cannot find the tool, rerun `dotnet tool restore`.
- Do not edit `obj/`, `_site/`, or `_site-vrm/`; edit source comments or the
  configuration files in `Docs/Docfx` instead.
