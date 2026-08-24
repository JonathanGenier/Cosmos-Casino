using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Owns batched collision regions for repeated authoritative Core structures.
/// </summary>
public sealed partial class StructureInstanceCollisionManager : InitializableNodeManager
{
    #region Fields

    private readonly Dictionary<StructureInstanceBatchKey, StructureInstanceCollisionRegionView> _regions = new();
    private readonly Dictionary<StructureId, StructureInstanceHandle> _handles = new();

    private MapManager? _mapManager;
    private StructurePresentationCatalog? _presentationCatalog;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of live repeated-structure collision regions owned by this Client projection.
    /// </summary>
    internal int RegionViewCount => _regions.Count;

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
    /// Initializes the repeated-structure collision projection and reconstructs from authoritative Core state.
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
    /// Rebuilds the complete disposable repeated-structure collision projection from current authoritative Core state.
    /// </summary>
    public void RebuildAllFromAuthoritativeState()
    {
        ClearRegions();

        foreach (StructureSnapshot snapshot in MapManager.GetStructureSnapshots())
        {
            if (TryGetInstancedPresentation(snapshot.Definition.Id, out StructurePresentationDefinition presentation))
            {
                AddInstance(snapshot.Id, snapshot.Anchor, snapshot.Rotation, presentation);
            }
        }
    }

    /// <summary>
    /// Applies one successful authoritative build result to the repeated-structure collision projection.
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

        foreach (BuildStructureResult structure in buildResult.Structures)
        {
            if (structure.Outcome != BuildOperationOutcome.Valid)
            {
                continue;
            }

            switch (structure.Kind)
            {
                case BuildStructureResultKind.Created:
                    if (TryGetInstancedPresentation(structure.DefinitionId, out StructurePresentationDefinition presentation))
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
    /// Releases repeated-structure collision regions owned by this manager.
    /// </summary>
    protected override void OnExit()
    {
        ClearRegions();
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
        StructureInstanceCollisionRegionView region = GetOrCreateRegion(key, presentation);
        int slot = region.AddInstance(structureId, anchor, rotation);

        _handles[structureId] = new StructureInstanceHandle(key, slot);
    }

    private void RemoveInstance(StructureId structureId)
    {
        if (!_handles.TryGetValue(structureId, out StructureInstanceHandle handle))
        {
            return;
        }

        if (!_regions.TryGetValue(handle.BatchKey, out StructureInstanceCollisionRegionView? region))
        {
            _handles.Remove(structureId);
            return;
        }

        if (region.RemoveInstance(structureId, out StructureId? movedStructureId, out int movedSlot)
            && movedStructureId.HasValue)
        {
            _handles[movedStructureId.Value] = new StructureInstanceHandle(handle.BatchKey, movedSlot);
        }

        _handles.Remove(structureId);

        if (region.ActiveCount == 0)
        {
            RemoveRegion(handle.BatchKey);
        }
    }

    #endregion

    #region Region Operations

    private StructureInstanceCollisionRegionView GetOrCreateRegion(
        StructureInstanceBatchKey key,
        StructurePresentationDefinition presentation)
    {
        if (_regions.TryGetValue(key, out StructureInstanceCollisionRegionView? region))
        {
            return region;
        }

        region = new StructureInstanceCollisionRegionView();
        region.Initialize(key, presentation);
        AddChild(region);
        _regions.Add(key, region);
        return region;
    }

    private void RemoveRegion(StructureInstanceBatchKey key)
    {
        if (!_regions.Remove(key, out StructureInstanceCollisionRegionView? region))
        {
            return;
        }

        region.ClearCollision();
        region.QueueFree();
    }

    private void ClearRegions()
    {
        foreach (StructureInstanceCollisionRegionView region in _regions.Values.ToArray())
        {
            region.ClearCollision();
            region.QueueFree();
        }

        _regions.Clear();
        _handles.Clear();
    }

    #endregion

    #region Helpers

    private bool TryGetInstancedPresentation(
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
