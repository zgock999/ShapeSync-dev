# ShapeSync Package Release Process

This document describes the deterministic release-tree process for the
semi-public package repository. The distribution unit is the Git repository's
UPM package subfolder; do not create a second distribution channel.

## Source and output model

The source repository remains a Unity development project. The release output
is a pure package tree containing the Core and optional VRM companion packages:

```text
Packages/
  net.zgock-lab.shapesync/
  net.zgock-lab.shapesync.vrm/
```

`TestProject/` is a validation project and is never part of either package
subfolder. `PlayTest/`, Rich Tests, Human Test evidence, Sandbox assets, and
development-only documentation are excluded by the extraction rules.

## 1. Generate the release tree

Run the extraction script from the repository root. Use a new ignored staging
directory for each release candidate:

```powershell
$stage = 'Temp/Spec22/Packaging'

& .\Tools\Spec22\Export-Release.ps1 `
  -OutputPath "$stage/release-tree" `
  -AuditPath "$stage/export.audit.json"
```

The script extracts Core, the VRM companion, their tests, package metadata,
and Unity `.meta` files from the approved source roots. It also validates
source/output file sets, GUID preservation, metadata pairing, asmdef
inventory, exclusion rules, active preprocessor branches, and deterministic
content.

The export must finish with zero audit issues. Run it a second time into an
independent staging directory and compare the SHA-256 manifest of both output
trees. A release candidate is not reproducible if any path or file hash differs.

## 2. Finalize package metadata

Finalize the generated tree after extraction:

```powershell
& .\Tools\Spec22\Finalize-PackageMetadata.ps1 `
  -ReleaseRoot "$stage/release-tree" `
  -AuditPath "$stage/license.audit.json"
```

Finalization writes the package `README`, `LICENSE.md`, `CHANGELOG.md`, and
third-party notices, and then audits package identity, version, author,
license, headers, provenance, external dependencies, and binary presence.

Both package manifests must remain lockstep. Substitute the release values in
the following record before running the release checks:

```text
Core:      net.zgock-lab.shapesync      <package-version>
Companion: net.zgock-lab.shapesync.vrm   <package-version>
Unity:     6000.0 or later
License:   MIT
Author:    zgock999
```

The Core package declares only `com.cysharp.r3`. The companion declares Core,
`com.vrmc.gltf`, and `com.vrmc.vrm`. R3 .NET assemblies are restored by the
consumer through NuGetForUnity and are not copied into the release tree.

## 3. Review the generated tree

Before a tag is prepared, review both audit JSON files and verify:

- package IDs, versions, and package subfolders are correct;
- every shipped source file has its paired `.meta` file and preserved GUID;
- no development-only path, third-party binary, or unapproved dependency is
  present;
- the Core package remains usable without UniVRM or
  `SHAPESYNC_USE_UNIVRM`;
- the VRM companion is tested only after Core and UniVRM are available;
- generated restore output under the validation project is untracked.

Record the actual file counts and audit values in
`Docs/codex/ImplementSpec22.md`; do not replace measured values with an
expected count.

## 4. Tag and installation readiness

The package version and Git revision are lockstep release parameters. Define
the deployment repository and revision once, then form both consumer URLs
from those values:

```powershell
$packageRepository = '<owner>/<repository>'
$repositoryDirectory = '<repository-directory>'
$packageVersion = '<package-version>'
$gitRevision = '<tag-or-commit>'

$coreUrl = "https://github.com/$packageRepository.git?path=Packages/net.zgock-lab.shapesync#$gitRevision"
$companionUrl = "https://github.com/$packageRepository.git?path=Packages/net.zgock-lab.shapesync.vrm#$gitRevision"
```

Spec23 applied values: `packageRepository = zgock999/ShapeSync-dev`, `repositoryDirectory = ShapeSync-dev`, `packageVersion = 0.2.0-preview8`, `gitRevision = 0.2.0-preview8`. These are the current application values,
not fixed requirements of this reusable process; Spec24 replaces them at the
parameter line above.

The `?path=` component must precede `#<tag-or-commit>`. The revision must be an
existing tag or a complete commit ID. The tag is prepared only after the
release-tree and metadata audits pass. Package version and Git revision are
kept equal for the applied release candidate, while the package version shown
in package metadata remains a release artifact value rather than a repository
name.

## 5. Validation matrix

The release matrix has both Unity version and graphics API axes. Do not fix
these values in this reusable process; derive them for each release from the
declared package minimum, the validation baseline, and a fresh project created
with each version:

| Axis | Source of the value | Required coverage |
|---|---|---|
| Unity version | package manifest `unity` field and the validation baseline | `<declared-minimum-unity>` and `<validation-baseline-unity>` |
| Graphics API | each version's default API plus an async-compute-capable API | `<minimum-version-default-api>` and `<supported-async-compute-api>` |

The minimum-version default API is a required cross-check even when it is not
supported by ShapeSync. If it is non-async, record the structured reject and
the absence of recurring exceptions; run the Slim Test lanes on the supported
API. For each `(Unity version, Graphics API)` pair, run the three lanes below.
The matrix is a release procedure, not an acceptance condition for an
individual implementation Spec.

The two known findings are intentionally directional. A TreeView generic-type
compatibility problem is detectable only on the declared minimum Unity
version, while a D3D11 exception is detectable only on the version whose
default is D3D11; a newer validation version may default to D3D12 and avoid it.
If development were based only on the minimum version, the TreeView deprecation
would first appear on the newer version. Neither is a question of choosing the
right development baseline: declaring a version range creates the obligation
to cross-check both ends and their default APIs.

Use `Tools/Spec22/Run-SlimTestMatrix.ps1` with the generated tree to validate
the consumer setup. The required order is:

1. Core-only EditMode and PlayMode with an empty define set;
2. Core plus VRM companion EditMode and PlayMode with
   `SHAPESYNC_USE_UNIVRM`;
3. Core-only EditMode and PlayMode again with the define removed.

Record each lane's total, passed, failed, skipped, and inconclusive counts,
the resolved package lock, and whether the final Core-only result matches the
initial result. Known batchmode-only exceptions must be named explicitly and
must not hide compilation, package-resolution, or inconclusive failures.

## 6. Assemble the complete package repository tree

The finalized release tree is only the distributable package payload. It is
not, by itself, the repository that will be deployed.
Before deployment, assemble a separate repository root with the following
layout:

```text
<repository-directory>/
  Packages/             finalized output from the previous steps
  TestProject/          copied validation project
  Docs/                 package-facing installation, packaging, API, and guides
  README.md             repository root README
  LICENSE               repository root license
  .gitignore            package-repository-specific ignore rules
```

Run the assembly script against the finalized release root, not against a
composite evidence/test stage:

```powershell
& .\Tools\Spec22\Assemble-PackageRepository.ps1 `
  -ReleaseRoot "$stage/release-tree" `
  -AssemblyRoot "$stage/$repositoryDirectory"
```

`Assemble-PackageRepository.ps1` is the reproducible deployment-input step.
It copies the two finalized package folders, root license files, the root
README, the package-facing documents, and the full `Docs/Guides/` tree. It
copies `TestProject/` while excluding generated `Library/`, `Temp/`, `Obj/`,
`Build/`, `Builds/`, `Logs/`, `UserSettings/`, and `Assets/Packages/` output.
The script verifies the required package, documentation, README-link, and
TestProject paths and rejects generated evidence or an incomplete release
root. The generated root `.gitignore` is
`Tools/Spec22/PackageRepository.gitignore`; it intentionally does not ignore
the tracked root `Packages/` directory.

The assembled repository root is the input for the remote deployment and
anonymous Git URL installation checks in Spec22.5. The local finalized release
tree remains the input for package metadata and license audits.

## Scope boundary

This process prepares and audits a local release candidate. Remote repository
deployment, anonymous Git URL installation, tag publication, and final
external acceptance are separate release actions and belong to Spec22.5.
