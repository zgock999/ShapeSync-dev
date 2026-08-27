// SPDX-License-Identifier: MIT
// Copyright (c) 2026 zgock999

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using zgock.ShapeSync.Editor;
using zgock.ShapeSync.Materials;
using zgock.ShapeSync.StackMachine;
using zgock.ShapeSync.StackMachine.Humanoid;

namespace zgock.ShapeSync.Tests.EditMode
{
    public sealed class HumanoidIndividualAssetStagerTests
    {
        private const string Root = ShapeSyncTestAssetPaths.Spec17StagingRoot;
        private const string Prefix = "__Spec17_6_Staging";
        private static readonly BindingFlags Flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        [Test]
        public void Stage_CreatesNamedIndividualAssetsAndReplacesPersistentMaterialTextures()
        {
            HumanoidBuildResult result = null; Material source = null; Material target = null; Texture2D sampler = null; RenderTexture baseTexture = null; RenderTexture normalTexture = null; UrpLitMaterialShaderAdapter adapter = null;
            try
            {
                CreateFolder();
                sampler = new Texture2D(2, 2, TextureFormat.RGBA32, true, true) { wrapMode = TextureWrapMode.Mirror, filterMode = FilterMode.Point, anisoLevel = 3 };
                sampler.SetPixel(0, 0, new Color(.25f, .5f, .75f, .6f)); sampler.Apply(true, false);
                source = new Material(Shader.Find("Universal Render Pipeline/Lit")); source.SetTexture("_BaseMap", sampler); source.SetTexture("_BumpMap", sampler);
                target = new Material(source); baseTexture = CreateTexture(sampler); normalTexture = CreateTexture(sampler); target.SetTexture("_BaseMap", baseTexture); target.SetTexture("_BumpMap", normalTexture);
                adapter = ScriptableObject.CreateInstance<UrpLitMaterialShaderAdapter>(); result = new HumanoidBuildResult(CreateMesh(source, target, adapter, new MaterialId("outfitA", "cloth")));

                Assert.That(InvokeStage(Root, "Look", result, out object stage, out StackMachineDiagnostic diagnostic), Is.True, diagnostic?.message);
                Assert.That(AssetDatabase.LoadAssetAtPath<Mesh>(Root + "/" + Prefix + ".asset"), Is.Not.Null);
                Assert.That(AssetDatabase.LoadAssetAtPath<Material>(Root + "/" + Prefix + "_outfitA_cloth.mat"), Is.Not.Null);
                Assert.That(AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/" + Prefix + "_outfitA_cloth_0.png"), Is.Not.Null);
                Assert.That(AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/" + Prefix + "_outfitA_cloth_1.png"), Is.Not.Null);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(Root + "/" + Prefix + "_outfitA_cloth.mat");
                Assert.That(material.GetTexture("_BaseMap"), Is.EqualTo(AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/" + Prefix + "_outfitA_cloth_0.png")));
                Assert.That(material.GetTexture("_BumpMap"), Is.EqualTo(AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/" + Prefix + "_outfitA_cloth_1.png")));
                Assert.That(source.GetTexture("_BaseMap"), Is.SameAs(sampler));
                Assert.That(source.GetTexture("_BumpMap"), Is.SameAs(sampler));
                Assert.That(target.GetTexture("_BaseMap"), Is.SameAs(baseTexture));
                Assert.That(target.GetTexture("_BumpMap"), Is.SameAs(normalTexture));
                int assetPathCount = 0; foreach (object ignored in (IEnumerable)stage.GetType().GetProperty("AssetPaths", Flags).GetValue(stage)) assetPathCount++;
                Assert.That(assetPathCount, Is.EqualTo(4));
            }
            finally { result?.Dispose(); Destroy(source); Destroy(target); Destroy(sampler); Release(baseTexture); Release(normalTexture); Destroy(adapter); DeleteFolder(); }
        }

        [Test]
        public void Stage_RejectsNonEmptyFolderWithoutCreatingArtifacts()
        {
            try
            {
                CreateFolder(); File.WriteAllText(Path.Combine(Path.GetFullPath(Root), "occupied.txt"), "x");
                Assert.That(InvokeValidate(Root, out StackMachineDiagnostic diagnostic), Is.False);
                Assert.That(diagnostic.domainCode, Is.EqualTo("PublishOutputFolderNotEmpty"));
                Assert.That(AssetDatabase.FindAssets(string.Empty, new[] { Root }), Is.Empty);
            }
            finally { DeleteFolder(); }
        }

        [Test]
        public void Stage_RejectsNonEmptyFolderWithoutDeletingExistingArtifact()
        {
            HumanoidBuildResult result = null; Material source = null; Material target = null; Texture2D sampler = null; RenderTexture texture = null; UrpUnlitMaterialShaderAdapter adapter = null;
            try
            {
                CreateFolder();
                AssetDatabase.CreateAsset(new Mesh(), Root + "/" + Prefix + ".asset");
                sampler = new Texture2D(2, 2); source = new Material(Shader.Find("Universal Render Pipeline/Unlit")); source.SetTexture("_BaseMap", sampler); target = new Material(source); texture = CreateTexture(sampler); target.SetTexture("_BaseMap", texture); adapter = ScriptableObject.CreateInstance<UrpUnlitMaterialShaderAdapter>(); result = new HumanoidBuildResult(CreateMesh(source, target, adapter, new MaterialId(string.Empty, "body")));
                Assert.That(InvokeStage(Root, "Look", result, out _, out StackMachineDiagnostic diagnostic), Is.False);
                Assert.That(diagnostic.domainCode, Is.EqualTo("PublishOutputFolderNotEmpty"));
                Assert.That(AssetDatabase.LoadAssetAtPath<Mesh>(Root + "/" + Prefix + ".asset"), Is.Not.Null);
            }
            finally { result?.Dispose(); Destroy(source); Destroy(target); Destroy(sampler); Release(texture); Destroy(adapter); DeleteFolder(); }
        }

        [Test]
        public void Stage_RejectsMissingDocumentNameAndResultBeforeCreatingArtifacts()
        {
            try
            {
                CreateFolder();
                Assert.That(InvokeStage(Root, string.Empty, null, out _, out StackMachineDiagnostic nameDiagnostic), Is.False);
                Assert.That(nameDiagnostic.domainCode, Is.EqualTo("PublishDocumentNameRequired"));
                Assert.That(InvokeStage(Root, "Look", null, out _, out StackMachineDiagnostic resultDiagnostic), Is.False);
                Assert.That(resultDiagnostic.domainCode, Is.EqualTo("PublishMeshRequired"));
                Assert.That(AssetDatabase.FindAssets(string.Empty, new[] { Root }), Is.Empty);
            }
            finally { DeleteFolder(); }
        }

        [Test]
        public void Stage_FailureAfterFirstTexture_ReturnsResidualArtifactPathsForWarning()
        {
            HumanoidBuildResult result = null; Material source = null; Material target = null; Texture2D sampler = null; RenderTexture baseTexture = null; RenderTexture normalTexture = null; UrpLitMaterialShaderAdapter adapter = null;
            try
            {
                CreateFolder();
                sampler = new Texture2D(2, 2); source = new Material(Shader.Find("Universal Render Pipeline/Lit")); source.SetTexture("_BaseMap", sampler); source.SetTexture("_BumpMap", sampler);
                target = new Material(source); baseTexture = CreateTexture(sampler); normalTexture = CreateTexture(sampler); target.SetTexture("_BaseMap", baseTexture); target.SetTexture("_BumpMap", normalTexture);
                adapter = ScriptableObject.CreateInstance<UrpLitMaterialShaderAdapter>(); result = new HumanoidBuildResult(CreateMesh(source, target, adapter, new MaterialId(string.Empty, "body")));
                int writes = 0; SetWriter((path, bytes) => { if (writes++ == 1) throw new IOException("injected"); File.WriteAllBytes(path, bytes); });
                Assert.That(InvokeStage(Root, "Look", result, out _, out string[] residuals, out StackMachineDiagnostic diagnostic), Is.False);
                Assert.That(diagnostic.domainCode, Is.EqualTo("PublishAssetStagingFailed"));
                Assert.That(residuals, Is.EqualTo(new[] { Root + "/" + Prefix + "_body_0.png" }));
                Assert.That(diagnostic.detail, Does.Contain(Root + "/" + Prefix + "_body_0.png"));
                Assert.That(AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/" + Prefix + "_body_0.png"), Is.Not.Null);
            }
            finally { SetWriter(File.WriteAllBytes); result?.Dispose(); Destroy(source); Destroy(target); Destroy(sampler); Release(baseTexture); Release(normalTexture); Destroy(adapter); DeleteFolder(); }
        }

        [Test]
        public void Stage_ImportFailure_ReturnsWrittenPngAsResidualArtifact()
        {
            HumanoidBuildResult result = null; Material source = null; Material target = null; Texture2D sampler = null; RenderTexture texture = null; UrpUnlitMaterialShaderAdapter adapter = null;
            try
            {
                CreateFolder(); sampler = new Texture2D(2, 2); source = new Material(Shader.Find("Universal Render Pipeline/Unlit")); source.SetTexture("_BaseMap", sampler);
                target = new Material(source); texture = CreateTexture(sampler); target.SetTexture("_BaseMap", texture); adapter = ScriptableObject.CreateInstance<UrpUnlitMaterialShaderAdapter>(); result = new HumanoidBuildResult(CreateMesh(source, target, adapter, new MaterialId(string.Empty, "body")));
                SetImporter(_ => throw new IOException("injected"));
                Assert.That(InvokeStage(Root, "Look", result, out _, out string[] residuals, out StackMachineDiagnostic diagnostic), Is.False);
                Assert.That(diagnostic.domainCode, Is.EqualTo("PublishAssetStagingFailed"));
                Assert.That(residuals, Is.EqualTo(new[] { Root + "/" + Prefix + "_body.png" }));
                Assert.That(File.Exists(Path.Combine(Path.GetFullPath(Root), Prefix + "_body.png")), Is.True);
            }
            finally { SetImporter(path => AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate)); SetWriter(File.WriteAllBytes); result?.Dispose(); Destroy(source); Destroy(target); Destroy(sampler); Release(texture); Destroy(adapter); DeleteFolder(); }
        }

        [Test]
        public void Stage_CreatesAvatarAssetWhenMeshPayloadOwnsAvatar()
        {
            HumanoidBuildResult result = null; Material source = null; Material target = null; UrpUnlitMaterialShaderAdapter adapter = null; GameObject avatarRoot = null;
            try
            {
                CreateFolder(); avatarRoot = new GameObject("avatarRoot"); Avatar avatar = AvatarBuilder.BuildGenericAvatar(avatarRoot, string.Empty);
                source = new Material(Shader.Find("Universal Render Pipeline/Unlit")); target = new Material(source); adapter = ScriptableObject.CreateInstance<UrpUnlitMaterialShaderAdapter>();
                result = new HumanoidBuildResult(CreateMesh(source, target, adapter, new MaterialId(string.Empty, "body"), avatar));
                Assert.That(InvokeStage(Root, "Look", result, out _, out StackMachineDiagnostic diagnostic), Is.True, diagnostic?.message);
                Assert.That(AssetDatabase.LoadAssetAtPath<Avatar>(Root + "/" + Prefix + "_avatar.asset"), Is.Not.Null);
            }
            finally { result?.Dispose(); Destroy(source); Destroy(target); Destroy(adapter); Destroy(avatarRoot); DeleteFolder(); }
        }

#if SHAPESYNC_USE_UNIVRM
        [Test]
        public void Stage_PersistsEveryMToonBaseColorPropertyForIndependentMaterialIds()
        {
            HumanoidBuildResult result = null; Material shirtSource = null; Material skirtSource = null; Material shirtTarget = null; Material skirtTarget = null; Texture2D sampler = null; RenderTexture shirtTexture = null; RenderTexture skirtTexture = null; MToon10MaterialShaderAdapter adapter = null;
            try
            {
                CreateFolder();
                sampler = new Texture2D(2, 2); sampler.SetPixel(0, 0, Color.white); sampler.Apply(false, false);
                Shader shader = Shader.Find("VRM10/Universal Render Pipeline/MToon10"); Assert.That(shader, Is.Not.Null);
                shirtSource = new Material(shader); skirtSource = new Material(shader);
                shirtTarget = new Material(shirtSource); skirtTarget = new Material(skirtSource);
                shirtTexture = CreateTexture(sampler); skirtTexture = CreateTexture(sampler);
                shirtTarget.SetTexture("_MainTex", shirtTexture); shirtTarget.SetTexture("_ShadeTex", shirtTexture);
                skirtTarget.SetTexture("_MainTex", skirtTexture); skirtTarget.SetTexture("_ShadeTex", skirtTexture);
                adapter = ScriptableObject.CreateInstance<MToon10MaterialShaderAdapter>();
                result = new HumanoidBuildResult(CreateMesh(
                    new[] { shirtSource, skirtSource }, new[] { shirtTarget, skirtTarget }, adapter,
                    new[] { new MaterialId("shirt-1", "Body"), new MaterialId("skirt-1", "Body") }));

                Assert.That(InvokeStage(Root, "Look", result, out _, out StackMachineDiagnostic diagnostic), Is.True, diagnostic?.message);
                Texture2D shirtPng = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/" + Prefix + "_shirt-1_Body.png");
                Texture2D skirtPng = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/" + Prefix + "_skirt-1_Body.png");
                Material shirt = AssetDatabase.LoadAssetAtPath<Material>(Root + "/" + Prefix + "_shirt-1_Body.mat");
                Material skirt = AssetDatabase.LoadAssetAtPath<Material>(Root + "/" + Prefix + "_skirt-1_Body.mat");
                Assert.That(shirt.GetTexture("_MainTex"), Is.EqualTo(shirtPng)); Assert.That(shirt.GetTexture("_ShadeTex"), Is.EqualTo(shirtPng));
                Assert.That(skirt.GetTexture("_MainTex"), Is.EqualTo(skirtPng)); Assert.That(skirt.GetTexture("_ShadeTex"), Is.EqualTo(skirtPng));
            }
            finally { result?.Dispose(); Destroy(shirtSource); Destroy(skirtSource); Destroy(shirtTarget); Destroy(skirtTarget); Destroy(sampler); Release(shirtTexture); Release(skirtTexture); Destroy(adapter); DeleteFolder(); }
        }
#endif

        private static bool InvokeStage(string folder, string name, HumanoidBuildResult result, out object stage, out string[] residuals, out StackMachineDiagnostic diagnostic)
        {
            object[] args = { folder, name, result, null, null, null };
            bool ok = (bool)Stager.GetMethod("TryStage", Flags).Invoke(null, args); stage = args[3]; residuals = (string[])args[4]; diagnostic = (StackMachineDiagnostic)args[5]; return ok;
        }
        private static bool InvokeStage(string folder, string name, HumanoidBuildResult result, out object stage, out StackMachineDiagnostic diagnostic) => InvokeStage(folder, name, result, out stage, out _, out diagnostic);
        private static bool InvokeValidate(string folder, out StackMachineDiagnostic diagnostic) { object[] args = { folder, null }; bool ok = (bool)Stager.GetMethod("TryValidateEmptyOutputFolder", Flags).Invoke(null, args); diagnostic = (StackMachineDiagnostic)args[1]; return ok; }
        private static Type Stager => typeof(HumanoidCompilerWindow).Assembly.GetType("zgock.ShapeSync.Editor.HumanoidIndividualAssetStager", true);
        private static void SetWriter(Action<string, byte[]> writer) => Stager.GetField("WriteAllBytes", Flags).SetValue(null, writer);
        private static void SetImporter(Action<string> importer) => Stager.GetField("ImportAsset", Flags).SetValue(null, importer);
        private static InMemoryHumanoidMesh CreateMesh(Material source, Material target, MaterialShaderAdapter adapter, MaterialId id, Avatar avatar = null)
        {
            var mesh = new Mesh { subMeshCount = 1 }; var result = new InMemoryHumanoidMesh(mesh, avatar);
            Invoke(result, "TrySetMaterials", new object[] { new[] { target }, null }); Invoke(result, "TrySetMaterialSlots", new object[] { new[] { new HumanoidBuildMaterialSlot(id, 0, source, adapter) }, null }); return result;
        }
        private static InMemoryHumanoidMesh CreateMesh(Material[] sources, Material[] targets, MaterialShaderAdapter adapter, MaterialId[] ids)
        {
            var mesh = new Mesh { subMeshCount = targets.Length }; var result = new InMemoryHumanoidMesh(mesh);
            var slots = new HumanoidBuildMaterialSlot[targets.Length];
            for (int i = 0; i < slots.Length; i++) slots[i] = new HumanoidBuildMaterialSlot(ids[i], i, sources[i], adapter);
            Invoke(result, "TrySetMaterials", new object[] { targets, null }); Invoke(result, "TrySetMaterialSlots", new object[] { slots, null }); return result;
        }
        private static RenderTexture CreateTexture(Texture source) { var texture = new RenderTexture(new RenderTextureDescriptor(2, 2, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat, 0) { sRGB = false }); texture.Create(); Graphics.Blit(source, texture); return texture; }
        private static void CreateFolder() { DeleteFolder(); string parent = ShapeSyncTestAssetPaths.ConsumerFolderPath("zgock/ShapeSync/Tests/EditMode/Spec17"); AssetDatabase.CreateFolder(parent, "__Spec17_6_Staging"); }
        private static void DeleteFolder() { AssetDatabase.DeleteAsset(Root); }
        private static void Release(RenderTexture texture) { if (texture == null) return; if (RenderTexture.active == texture) RenderTexture.active = null; texture.Release(); Destroy(texture); }
        private static void Destroy(UnityEngine.Object value) { if (value != null) UnityEngine.Object.DestroyImmediate(value); }
        private static object Invoke(object instance, string method, object[] arguments) => instance.GetType().GetMethod(method, Flags).Invoke(instance, arguments);
    }
}
