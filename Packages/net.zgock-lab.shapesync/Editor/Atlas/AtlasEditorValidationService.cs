// SPDX-License-Identifier: MIT
// Copyright (c) 2026 zgock999

using System.Collections.Generic;
using UnityEngine;
using zgock.ShapeSync.Materials;
using zgock.ShapeSync.StackMachine;

namespace zgock.ShapeSync.Editor.Atlas
{
    /// <summary>Runs non-mutating Atlas Editor Dry Run validation for the listed candidate snapshot.</summary>
    public static class AtlasEditorValidationService
    {
        /// <summary>Validates current editor input and records a successful verification only after every check succeeds.</summary>
        public static bool TryDryRun(AtlasEditorState state, out AtlasLayoutResult layout, out StackMachineDiagnostic diagnostic)
        {
            layout = null;
            if (state == null || state.Snapshot == null) { diagnostic = StackMachineDiagnostic.CreateDomain("atlas", "AtlasEditorEntriesRequired", "Atlas Editor requires listed entries before Dry Run."); return false; }
            AtlasSchemaDocument document = CreateDocument(state);
            if (!AtlasLayoutOracle.Solve(document, out layout, out diagnostic)) { state.MarkDryRunFailed(); return false; }
            var targetsByMesh = new Dictionary<Mesh, List<AtlasMeshValidator.Target>>();
            for (int i = 0; i < state.Entries.Count; i++)
            {
                AtlasEditorEntryState entry = state.Entries[i]; if (entry.Excluded) continue;
                MaterialProxyEntry binding = entry.Candidate.ValidationBinding; Mesh mesh = binding?.renderer?.sharedMesh;
                if (mesh == null) { state.MarkDryRunFailed(); diagnostic = StackMachineDiagnostic.CreateDomain("atlas", "AtlasEditorMeshRequired", "Atlas Editor candidate requires a readable SkinnedMeshRenderer mesh.", detail: "owner=" + entry.Candidate.Owner + ";materialId=" + entry.Candidate.MaterialId); return false; }
                if (!layout.TryGetCell(entry.Candidate.MaterialId, out AtlasLayoutCell cell)) { state.MarkDryRunFailed(); diagnostic = StackMachineDiagnostic.CreateDomain("atlas", "AtlasEditorLayoutCellRequired", "Atlas Editor layout did not assign an included candidate.", detail: "materialId=" + entry.Candidate.MaterialId); return false; }
                if (!targetsByMesh.TryGetValue(mesh, out List<AtlasMeshValidator.Target> targets)) { targets = new List<AtlasMeshValidator.Target>(); targetsByMesh.Add(mesh, targets); }
                Material[] materials = binding.renderer.sharedMaterials;
                Material material = binding.materialChannel >= 0 && binding.materialChannel < materials.Length ? materials[binding.materialChannel] : null;
                MaterialProxySemanticValues values = binding.configuredValues;
                targets.Add(new AtlasMeshValidator.Target(entry.Candidate.Owner, entry.Candidate.MaterialId, binding.materialChannel, false,
                    material, binding.adapter, values.baseColorTexture, values.normalTexture, cell.PageIndex,
                    values.applyUvTransform, values.uvScale, values.uvOffset));
            }
            foreach (KeyValuePair<Mesh, List<AtlasMeshValidator.Target>> pair in targetsByMesh)
                if (!AtlasMeshValidator.TryValidateResolved(pair.Key, pair.Value, out diagnostic)) { state.MarkDryRunFailed(); layout = null; return false; }
            state.SetLayoutPreview(layout); state.TryMarkDryRunSucceeded(out _); diagnostic = null; return true;
        }

        internal static AtlasSchemaDocument CreateDocument(AtlasEditorState state)
        {
            var entries = new List<AtlasSchemaEntry>(); var identities = new List<AtlasSourceMaterialIdentity>();
            for (int i = 0; i < state.Entries.Count; i++)
            {
                AtlasEditorEntryState entry = state.Entries[i];
                entries.Add(new AtlasSchemaEntry(entry.Candidate.MaterialId, entry.PageGroupingKey, entry.Excluded ? 0 : entry.CellLevelX, entry.Excluded ? 0 : entry.CellLevelY, entry.Excluded));
                identities.Add(new AtlasSourceMaterialIdentity(entry.Candidate.MaterialId, entry.Candidate.SourceMaterialIdentity));
            }
            return new AtlasSchemaDocument(AtlasSchemaVersion.Current, state.PageExtent, AtlasPackingAlgorithm.FirstFitBuddyV1, true, new AtlasValidationIdentity(state.Snapshot.FigureIdentity, state.Snapshot.DocumentIdentity, identities), entries);
        }
    }
}
