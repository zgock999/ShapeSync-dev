// SPDX-License-Identifier: MIT
// Copyright (c) 2026 zgock999

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using zgock.ShapeSync.StackMachine;

namespace zgock.ShapeSync.Editor
{
    /// <summary>Validates Editor publish paths without creating assets or folders.</summary>
    public static class HumanoidPublishPathValidator
    {
        /// <summary>Converts an absolute folder selected by the Editor dialog into an existing Assets-relative folder.</summary>
        public static bool TryResolveOutputFolder(string absoluteFolder, out string assetFolder, out StackMachineDiagnostic diagnostic)
        {
            assetFolder = null;
            diagnostic = null;
            if (string.IsNullOrWhiteSpace(absoluteFolder)) return Reject("PublishOutputFolderRequired", "Pure Humanoid publish requires a selected output folder under Assets.", out diagnostic);
            try
            {
                string selected = Path.GetFullPath(absoluteFolder).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string assets = Path.GetFullPath(UnityEngine.Application.dataPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (!string.Equals(selected, assets, StringComparison.OrdinalIgnoreCase)
                    && !selected.StartsWith(assets + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                    && !selected.StartsWith(assets + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                    return Reject("PublishOutputFolderOutsideAssets", "Pure Humanoid output folder must be under this project's Assets folder.", out diagnostic);

                string tail = selected.Length == assets.Length ? string.Empty : selected.Substring(assets.Length).Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace('\\', '/');
                string relative = string.IsNullOrEmpty(tail) ? "Assets" : "Assets/" + tail;
                if (!Directory.Exists(selected))
                    return Reject("PublishOutputFolderRequired", "Pure Humanoid publish requires an existing output folder under Assets.", out diagnostic);
                // SaveFolderPanel can create a physical child directory under Assets without
                // synchronously importing it into AssetDatabase. Import the selected folder
                // before the asset-path validation used by the staging / Prefab transactions.
                if (relative != "Assets" && !AssetDatabase.IsValidFolder(relative))
                {
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    if (!AssetDatabase.IsValidFolder(relative))
                        return Reject("PublishOutputFolderRequired", "Pure Humanoid publish requires an existing output folder under Assets.", out diagnostic);
                }
                assetFolder = relative;
                return true;
            }
            catch (Exception exception)
            {
                diagnostic = StackMachineDiagnostic.CreateDomain("humanoid", "PublishOutputFolderInvalid", "Pure Humanoid output folder could not be resolved.", detail: exception.Message);
                return false;
            }
        }

        /// <summary>Validates a VRM asset folder relative to the selected Pure Humanoid destination.</summary>
        public static bool TryValidateVrmRelativeFolder(string relativeFolder, out StackMachineDiagnostic diagnostic, bool requireNonEmpty = false)
        {
            diagnostic = null;
            string relative = (relativeFolder ?? string.Empty).Trim().Replace('\\', '/').Trim('/');
            if (requireNonEmpty && string.IsNullOrEmpty(relative))
                return Reject("VrmPublishRelativeFolderRequired", "VRM Asset Relative Folder is required when Transport VRM Physics is enabled.", out diagnostic);
            if (relative.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) || string.Equals(relative, "Assets", StringComparison.OrdinalIgnoreCase)
                || relative == ".." || relative.StartsWith("../", StringComparison.Ordinal) || relative.Contains("/../") || relative.EndsWith("/..", StringComparison.Ordinal))
                return Reject("VrmPublishRelativeFolderInvalid", "VRM Asset Relative Folder must be relative to the selected destination and cannot contain Assets/ or ../.", out diagnostic);
            return true;
        }

        /// <summary>Validates the persistent Material/Texture/Mesh/Avatar dependency graph of a published Prefab.</summary>
        /// <remarks>Shared Shader and script infrastructure is intentionally outside the Pure Humanoid asset contract.</remarks>
        public static bool TryValidateOutputReferences(string prefabAssetPath, string outputFolder, out StackMachineDiagnostic diagnostic)
        {
            diagnostic = null;
            string prefabPath = NormalizeAssetPath(prefabAssetPath);
            string folderPath = NormalizeAssetPath(outputFolder).TrimEnd('/');
            if (string.IsNullOrWhiteSpace(prefabPath) || string.IsNullOrWhiteSpace(folderPath))
                return Reject("PublishOutputReferenceValidationInvalid", "Pure Humanoid output reference validation requires a Prefab path and output folder.", out diagnostic);
            if (AssetDatabase.LoadMainAssetAtPath(prefabPath) == null)
                return Reject("PublishOutputReferenceValidationInvalid", "Pure Humanoid output reference validation could not load the published Prefab.", out diagnostic, prefabPath);
            try
            {
                string[] dependencies = AssetDatabase.GetDependencies(prefabPath, true);
                for (int i = 0; i < dependencies.Length; i++)
                {
                    string dependencyPath = NormalizeAssetPath(dependencies[i]);
                    // Third-party package assets are shared infrastructure declared by the consumer
                    // environment. ShapeSync's own package assets remain subject to output containment;
                    // package-owned resources such as UniVRM's default icon are not publish artifacts
                    // that can be copied into the output folder.
                    if (IsSharedPackageReference(dependencyPath)) continue;
                    UnityEngine.Object dependency = AssetDatabase.LoadMainAssetAtPath(dependencyPath);
                    if (!IsPureHumanoidReference(dependency)) continue;
                    if (!IsUnderFolder(dependencyPath, folderPath))
                        return Reject("PublishOutputReferenceOutsideFolder", "Pure Humanoid output references an asset outside its output folder.", out diagnostic, dependencyPath);
                }
                return true;
            }
            catch (Exception exception)
            {
                diagnostic = StackMachineDiagnostic.CreateDomain("humanoid", "PublishOutputReferenceValidationFailed", "Pure Humanoid output reference validation failed.", detail: exception.Message);
                return false;
            }
        }

        private static bool Reject(string code, string message, out StackMachineDiagnostic diagnostic)
        {
            diagnostic = StackMachineDiagnostic.CreateDomain("humanoid", code, message);
            return false;
        }

        private static bool Reject(string code, string message, out StackMachineDiagnostic diagnostic, string detail)
        {
            diagnostic = StackMachineDiagnostic.CreateDomain("humanoid", code, message, detail: detail);
            return false;
        }

        private static bool IsPureHumanoidReference(UnityEngine.Object asset)
        {
            return asset is GameObject || asset is Material || asset is Texture
                || asset is Mesh || asset is Avatar;
        }

        private static string NormalizeAssetPath(string path) => (path ?? string.Empty).Trim().Replace('\\', '/').TrimEnd('/');

        private static bool IsUnderFolder(string assetPath, string folderPath)
        {
            return string.Equals(assetPath, folderPath, StringComparison.Ordinal)
                || assetPath.StartsWith(folderPath + "/", StringComparison.Ordinal);
        }

        private static bool IsSharedPackageReference(string assetPath)
        {
            if (!assetPath.StartsWith("Packages/", StringComparison.OrdinalIgnoreCase)) return false;
            return !IsUnderFolder(assetPath, "Packages/net.zgock-lab.shapesync")
                && !IsUnderFolder(assetPath, "Packages/net.zgock-lab.shapesync.vrm");
        }
    }
}
