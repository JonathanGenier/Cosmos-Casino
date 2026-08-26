using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Owns repeated-structure MultiMesh rendering for authoritative Core structures.
/// </summary>
public sealed partial class StructureInstanceRenderManager : InitializableNodeManager
{
    #region Fields

    private readonly Dictionary<StructureInstanceBatchKey, StructureMultiMeshBatchView> _batches = new();
    private readonly Dictionary<StructureId, StructureInstanceHandle> _handles = new();

    private MapManager? _mapManager;
    private StructurePresentationCatalog? _presentationCatalog;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of live MultiMesh batch views owned by this Client projection.
    /// </summary>
    internal int BatchViewCount => _batches.Count;

    private MapManager MapManager
    {
        get => _mapManager ?? throw new InvalidOperationException($"{nameof(MapManager)} has not been initialized.");
        set => _mapManager = value;
    }

    private StructurePresentationCatalog PresentationCatalog
    {
        get => _presentationCatalog ?? throw new InvalidOperationException($"{nameof(StructurePresentationCatalog)} has not been initialized.");
        set => _presentationCatalog = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the repeated-structure renderer and reconstructs from authoritative Core state.
    /// </summary>
    /// <param name="mapManager">The authoritative map manager to query.</param>
    /// <param name="presentationCatalog">The Client presentation catalog.</param>
    public void Initialize(
        MapManager mapManager,
        StructurePresentationCatalog presentationCatalog)
    {
        ArgumentNullException.ThrowIfNull(mapManager);
        ArgumentNullException.ThrowIfNull(presentationCatalog);

        MapManager = mapManager;
        PresentationCatalog = presentationCatalog;
        RebuildAllFromAuthoritativeState();
        MarkInitialized();
    }

    #endregion

    #region Public API

    /// <summary>
    /// Rebuilds the complete disposable repeated-structure projection from current authoritative Core state.
    /// </summary>
    public void RebuildAllFromAuthoritativeState()
    {
        ClearBatches();

        foreach (StructureSnapshot snapshot in MapManager.GetStructureSnapshots())
        {
            if (TryGetMultiMeshPresentation(snapshot.Definition.Id, out StructurePresentationDefinition presentation))
            {
                AddInstance(snapshot.Id, snapshot.Anchor, snapshot.Rotation, presentation);
            }
        }
    }

    /// <summary>
    /// Applies one successful authoritative build result to the repeated-structure projection.
    /// </summary>
    /// <param name="buildResult">The completed authoritative build result.</param>
    public void ApplyBuildResult(BuildResult buildResult)
    {
        ArgumentNullException.ThrowIfNull(buildResult);

        if (buildResult.Outcome == BuildOperationOutcome.Invalid
            || buildResult.Outcome == BuildOperationOutcome.NoOp)
        {
            return;
        }

        ApplyChanges(buildResult.Structures);
    }

    /// <summary>
    /// Applies repeated-structure changes from one successful authoritative transaction.
    /// </summary>
    /// <param name="structures">The already-classified structure changes to apply.</param>
    public void ApplyChanges(IReadOnlyList<BuildStructureResult> structures)
    {
        ArgumentNullException.ThrowIfNull(structures);

        foreach (BuildStructureResult structure in structures)
        {
            if (structure.Outcome != BuildOperationOutcome.Valid)
            {
                continue;
            }

            switch (structure.Kind)
            {
                case BuildStructureResultKind.Created:
                    if (TryGetMultiMeshPresentation(structure.DefinitionId, out StructurePresentationDefinition presentation))
                    {
                        AddInstance(structure.StructureId, structure.Anchor, structure.Rotation, presentation);
                    }

                    break;

                case BuildStructureResultKind.Removed:
                    RemoveInstance(structure.StructureId);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported structure result kind: {structure.Kind}");
            }
        }
    }

    #endregion

    #region Godot Lifecycle

    /// <summary>
    /// Releases MultiMesh batch views owned by this manager.
    /// </summary>
    protected override void OnExit()
    {
        ClearBatches();
    }

    #endregion

    #region Instance Operations

    private void AddInstance(
        StructureId structureId,
        MapCellCoord anchor,
        FootprintRotation rotation,
        StructurePresentationDefinition presentation)
    {
        if (_handles.ContainsKey(structureId))
        {
            RemoveInstance(structureId);
        }

        StructureInstanceBatchKey key = CreateBatchKey(presentation, anchor);
        StructureMultiMeshBatchView batch = GetOrCreateBatch(key, presentation);
        int slot = batch.AddInstance(structureId, anchor, rotation);

        _handles[structureId] = new StructureInstanceHandle(key, slot);
    }

    private void RemoveInstance(StructureId structureId)
    {
        if (!_handles.TryGetValue(structureId, out StructureInstanceHandle handle))
        {
            return;
        }

        if (!_batches.TryGetValue(handle.BatchKey, out StructureMultiMeshBatchView? batch))
        {
            _handles.Remove(structureId);
            return;
        }

        if (batch.RemoveInstance(structureId, out StructureId? movedStructureId, out int movedSlot)
            && movedStructureId.HasValue)
        {
            _handles[movedStructureId.Value] = new StructureInstanceHandle(handle.BatchKey, movedSlot);
        }

        _handles.Remove(structureId);

        if (batch.ActiveCount == 0)
        {
            RemoveBatch(handle.BatchKey);
        }
    }

    #endregion

    #region Batch Operations

    private StructureMultiMeshBatchView GetOrCreateBatch(
        StructureInstanceBatchKey key,
        StructurePresentationDefinition presentation)
    {
        if (_batches.TryGetValue(key, out StructureMultiMeshBatchView? batch))
        {
            return batch;
        }

        batch = new StructureMultiMeshBatchView();
        batch.Initialize(key, presentation);
        AddChild(batch);
        _batches.Add(key, batch);
        return batch;
    }

    private void RemoveBatch(StructureInstanceBatchKey key)
    {
        if (!_batches.Remove(key, out StructureMultiMeshBatchView? batch))
        {
            return;
        }

        batch.QueueFree();
    }

    private void ClearBatches()
    {
        foreach (StructureMultiMeshBatchView batch in _batches.Values.ToArray())
        {
            batch.QueueFree();
        }

        _batches.Clear();
        _handles.Clear();
    }

    #endregion

    #region Helpers

    private bool TryGetMultiMeshPresentation(
        StructureDefinitionId definitionId,
        out StructurePresentationDefinition presentation)
    {
        if (PresentationCatalog.TryGetDefinition(definitionId, out presentation)
            && presentation.RenderStrategy == StructureRenderStrategy.MultiMesh)
        {
            return true;
        }

        presentation = null!;
        return false;
    }

    private StructureInstanceBatchKey CreateBatchKey(
        StructurePresentationDefinition presentation,
        MapCellCoord anchor)
    {
        return new StructureInstanceBatchKey(
            presentation.PresentationKey,
            StructureRenderSectionMath.ToSectionCoord(anchor));
    }

    #endregion
}
