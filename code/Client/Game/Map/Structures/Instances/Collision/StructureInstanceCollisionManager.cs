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

        RebuildDirtyRegions(_regions.Keys);
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

        ApplyChanges(buildResult.Structures);
    }

    /// <summary>
    /// Applies repeated-structure collision changes from one successful authoritative transaction.
    /// </summary>
    /// <param name="structures">The already-classified structure changes to apply.</param>
    public void ApplyChanges(IReadOnlyList<BuildStructureResult> structures)
    {
        ArgumentNullException.ThrowIfNull(structures);

        var dirtyKeys = new HashSet<StructureInstanceBatchKey>();

        foreach (BuildStructureResult structure in structures)
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
                        MarkExistingInstanceDirty(structure.StructureId, dirtyKeys);
                        StructureInstanceBatchKey key = AddInstance(
                            structure.StructureId,
                            structure.Anchor,
                            structure.Rotation,
                            presentation);
                        dirtyKeys.Add(key);
                    }

                    break;

                case BuildStructureResultKind.Removed:
                    if (RemoveInstance(structure.StructureId, out StructureInstanceBatchKey removedKey))
                    {
                        dirtyKeys.Add(removedKey);
                    }

                    break;

                default:
                    throw new InvalidOperationException($"Unsupported structure result kind: {structure.Kind}");
            }
        }

        RebuildDirtyRegions(dirtyKeys);
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

    private StructureInstanceBatchKey AddInstance(
        StructureId structureId,
        MapCellCoord anchor,
        FootprintRotation rotation,
        StructurePresentationDefinition presentation)
    {
        if (_handles.ContainsKey(structureId))
        {
            RemoveInstance(structureId, out _);
        }

        StructureInstanceBatchKey key = CreateBatchKey(presentation, anchor);
        StructureInstanceCollisionRegionView region = GetOrCreateRegion(key, presentation);
        int slot = region.AddInstance(structureId, anchor, rotation);

        _handles[structureId] = new StructureInstanceHandle(key, slot);
        return key;
    }

    private bool RemoveInstance(
        StructureId structureId,
        out StructureInstanceBatchKey removedKey)
    {
        removedKey = default;

        if (!_handles.TryGetValue(structureId, out StructureInstanceHandle handle))
        {
            return false;
        }

        removedKey = handle.BatchKey;

        if (!_regions.TryGetValue(handle.BatchKey, out StructureInstanceCollisionRegionView? region))
        {
            _handles.Remove(structureId);
            return true;
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

        return true;
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

    private void MarkExistingInstanceDirty(
        StructureId structureId,
        HashSet<StructureInstanceBatchKey> dirtyKeys)
    {
        if (_handles.TryGetValue(structureId, out StructureInstanceHandle existingHandle))
        {
            dirtyKeys.Add(existingHandle.BatchKey);
        }
    }

    private void RebuildDirtyRegions(IEnumerable<StructureInstanceBatchKey> dirtyKeys)
    {
        var sortedKeys = new List<StructureInstanceBatchKey>(dirtyKeys);
        sortedKeys.Sort(CompareBatchKeys);

        foreach (StructureInstanceBatchKey key in sortedKeys)
        {
            if (!_regions.TryGetValue(key, out StructureInstanceCollisionRegionView? region))
            {
                continue;
            }

            if (region.ActiveCount == 0)
            {
                RemoveRegion(key);
                continue;
            }

            region.RebuildCollision();
        }

        static int CompareBatchKeys(StructureInstanceBatchKey left, StructureInstanceBatchKey right)
        {
            int presentationComparison = left.PresentationKey.Value.CompareTo(right.PresentationKey.Value);

            if (presentationComparison != 0)
            {
                return presentationComparison;
            }

            int xComparison = left.SectionCoord.X.CompareTo(right.SectionCoord.X);

            if (xComparison != 0)
            {
                return xComparison;
            }

            int yComparison = left.SectionCoord.Y.CompareTo(right.SectionCoord.Y);

            if (yComparison != 0)
            {
                return yComparison;
            }

            return left.SectionCoord.Z.CompareTo(right.SectionCoord.Z);
        }
    }

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
