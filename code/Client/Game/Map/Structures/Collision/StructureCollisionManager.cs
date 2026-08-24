using CosmosCasino.Core.Game.Map;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Owns generated Client collision regions for authoritative Core structures.
/// </summary>
public sealed partial class StructureCollisionManager : InitializableNodeManager
{
    #region Fields

    private readonly Dictionary<StructureRenderSectionCoord, StructureCollisionRegionView> _regionViews = new();

    private MapManager? _mapManager;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of live collision-region views owned by this Client projection.
    /// </summary>
    internal int RegionViewCount => _regionViews.Count;

    private MapManager MapManager
    {
        get => _mapManager ?? throw new InvalidOperationException($"{nameof(MapManager)} has not been initialized.");
        set => _mapManager = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the structure collision projection and reconstructs regions from authoritative Core state.
    /// </summary>
    /// <param name="mapManager">The authoritative map manager to query.</param>
    public void Initialize(MapManager mapManager)
    {
        ArgumentNullException.ThrowIfNull(mapManager);

        MapManager = mapManager;
        RebuildAllFromAuthoritativeState();
        MarkInitialized();
    }

    #endregion

    #region Public API

    /// <summary>
    /// Rebuilds the complete disposable structure-collision projection from current authoritative Core state.
    /// </summary>
    public void RebuildAllFromAuthoritativeState()
    {
        IReadOnlyList<StructureRenderSectionCoord> occupiedSections =
            StructureSectionInvalidation.GetOccupiedBlockSections(MapManager);

        RemoveRegionsExcept(occupiedSections);

        foreach (StructureRenderSectionCoord sectionCoord in occupiedSections)
        {
            RebuildRegion(sectionCoord);
        }
    }

    /// <summary>
    /// Rebuilds only collision regions whose generated faces may have changed around the specified authoritative cells.
    /// </summary>
    /// <param name="affectedCells">The complete batch of authoritative cells changed by a successful build result.</param>
    public void RebuildAffectedCells(IReadOnlyList<MapCellCoord> affectedCells)
    {
        ArgumentNullException.ThrowIfNull(affectedCells);

        if (affectedCells.Count == 0)
        {
            return;
        }

        foreach (StructureRenderSectionCoord sectionCoord in StructureSectionInvalidation.GetDirtySections(affectedCells))
        {
            RebuildRegion(sectionCoord);
        }
    }

    #endregion

    #region Godot Lifecycle

    /// <summary>
    /// Releases generated collision regions owned by this manager.
    /// </summary>
    protected override void OnExit()
    {
        foreach (StructureCollisionRegionView view in _regionViews.Values)
        {
            view.ClearCollision();
            view.QueueFree();
        }

        _regionViews.Clear();
    }

    #endregion

    #region Region Rebuild

    private void RebuildRegion(StructureRenderSectionCoord sectionCoord)
    {
        StructureRenderSectionSnapshot snapshot =
            StructureSectionSnapshotFactory.CaptureBlockSection(MapManager, sectionCoord);

        if (snapshot.BlockCount == 0)
        {
            RemoveRegion(sectionCoord);
            return;
        }

        StructureCollisionMeshBuildResult buildResult = StructureCollisionMeshBuilder.Build(snapshot);

        if (buildResult.TriangleCount == 0)
        {
            RemoveRegion(sectionCoord);
            return;
        }

        StructureCollisionRegionView view = GetOrCreateRegionView(sectionCoord);
        view.ApplyCollision(buildResult);
    }

    private StructureCollisionRegionView GetOrCreateRegionView(StructureRenderSectionCoord sectionCoord)
    {
        if (_regionViews.TryGetValue(sectionCoord, out StructureCollisionRegionView? view))
        {
            return view;
        }

        view = new StructureCollisionRegionView();
        view.Initialize(sectionCoord);
        AddChild(view);
        _regionViews.Add(sectionCoord, view);
        return view;
    }

    private void RemoveRegion(StructureRenderSectionCoord sectionCoord)
    {
        if (!_regionViews.Remove(sectionCoord, out StructureCollisionRegionView? view))
        {
            return;
        }

        view.ClearCollision();
        view.QueueFree();
    }

    private void RemoveRegionsExcept(IReadOnlyList<StructureRenderSectionCoord> retainedSections)
    {
        var retainedSectionSet = new HashSet<StructureRenderSectionCoord>(retainedSections);
        StructureRenderSectionCoord[] existingSections = _regionViews.Keys.ToArray();

        foreach (StructureRenderSectionCoord sectionCoord in existingSections)
        {
            if (!retainedSectionSet.Contains(sectionCoord))
            {
                RemoveRegion(sectionCoord);
            }
        }
    }

    #endregion
}
