using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Owns generated Client render sections for authoritative Core structures.
/// </summary>
public sealed partial class StructureRenderManager : InitializableNodeManager
{
    #region Fields

    private readonly Dictionary<StructureRenderSectionCoord, StructureRenderSectionView> _sectionViews = new();

    private MapManager? _mapManager;
    private StandardMaterial3D? _blockMaterial;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of live render-section views owned by this Client projection.
    /// </summary>
    internal int SectionViewCount => _sectionViews.Count;

    private MapManager MapManager
    {
        get => _mapManager ?? throw new InvalidOperationException($"{nameof(MapManager)} has not been initialized.");
        set => _mapManager = value;
    }

    private StandardMaterial3D BlockMaterial
    {
        get => _blockMaterial ?? throw new InvalidOperationException("Structure block material has not been initialized.");
        set => _blockMaterial = value;
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the structure renderer and reconstructs its sections from authoritative Core state.
    /// </summary>
    /// <param name="mapManager">The authoritative map manager to query.</param>
    public void Initialize(MapManager mapManager)
    {
        ArgumentNullException.ThrowIfNull(mapManager);

        MapManager = mapManager;
        BlockMaterial = CreateBlockMaterial();
        RebuildAllFromAuthoritativeState();
        MarkInitialized();
    }

    #endregion

    #region Public API

    /// <summary>
    /// Rebuilds the complete disposable structure-renderer projection from current authoritative Core state.
    /// </summary>
    public void RebuildAllFromAuthoritativeState()
    {
        var occupiedSections = new HashSet<StructureRenderSectionCoord>();

        foreach (StructureSnapshot snapshot in MapManager.GetStructureSnapshots())
        {
            if (!IsGeneratedOpaqueBlock(snapshot.Definition.Id))
            {
                continue;
            }

            foreach (MapCellCoord cell in snapshot.Definition.Footprint.Resolve(snapshot.Anchor, snapshot.Rotation))
            {
                occupiedSections.Add(StructureRenderSectionMath.ToSectionCoord(cell));
            }
        }

        RemoveSectionsExcept(occupiedSections);

        foreach (StructureRenderSectionCoord sectionCoord in occupiedSections.OrderBy(static coord => coord.X)
            .ThenBy(static coord => coord.Y)
            .ThenBy(static coord => coord.Z))
        {
            RebuildSection(sectionCoord);
        }
    }

    /// <summary>
    /// Rebuilds only sections whose generated faces may have changed around the specified authoritative cells.
    /// </summary>
    /// <param name="affectedCells">The complete batch of authoritative cells changed by a successful build result.</param>
    public void RebuildAffectedCells(IReadOnlyList<MapCellCoord> affectedCells)
    {
        ArgumentNullException.ThrowIfNull(affectedCells);

        if (affectedCells.Count == 0)
        {
            return;
        }

        var dirtySections = new HashSet<StructureRenderSectionCoord>();

        foreach (MapCellCoord cell in affectedCells)
        {
            StructureRenderSectionCoord owningSection = StructureRenderSectionMath.ToSectionCoord(cell);
            dirtySections.Add(owningSection);

            foreach (MapCellOffset offset in StructureRenderNeighborOffsets.SixAxis)
            {
                if (!StructureRenderSectionMath.TryAddOffset(cell, offset, out MapCellCoord neighbor))
                {
                    continue;
                }

                StructureRenderSectionCoord neighborSection = StructureRenderSectionMath.ToSectionCoord(neighbor);

                if (neighborSection != owningSection)
                {
                    dirtySections.Add(neighborSection);
                }
            }
        }

        foreach (StructureRenderSectionCoord sectionCoord in dirtySections.OrderBy(static coord => coord.X)
            .ThenBy(static coord => coord.Y)
            .ThenBy(static coord => coord.Z))
        {
            RebuildSection(sectionCoord);
        }
    }

    #endregion

    #region Godot Lifecycle

    /// <summary>
    /// Releases generated section views owned by this manager.
    /// </summary>
    protected override void OnExit()
    {
        foreach (StructureRenderSectionView view in _sectionViews.Values)
        {
            view.ClearMesh();
            view.QueueFree();
        }

        _sectionViews.Clear();
    }

    #endregion

    #region Section Rebuild

    private bool IsGeneratedOpaqueBlock(StructureDefinitionId definitionId)
    {
        return definitionId == StructureDefinitions.BlockDefinitionId;
    }

    private StructureRenderSectionSnapshot CaptureSectionSnapshot(StructureRenderSectionCoord sectionCoord)
    {
        StructureRenderSectionBounds bounds = StructureRenderSectionMath.GetBounds(sectionCoord);
        StructureRenderSectionBounds haloBounds = StructureRenderSectionMath.Expand(bounds, amount: 1);
        var renderableBlocks = new List<MapCellCoord>();
        var compatibleBlocks = new HashSet<MapCellCoord>();

        ForEachCell(haloBounds, cell =>
        {
            if (!MapManager.TryGetStructureSnapshotAt(cell, out StructureSnapshot snapshot)
                || !IsGeneratedOpaqueBlock(snapshot.Definition.Id))
            {
                return;
            }

            compatibleBlocks.Add(cell);

            if (bounds.Contains(cell))
            {
                renderableBlocks.Add(cell);
            }
        });

        renderableBlocks.Sort(CompareCells);

        return new StructureRenderSectionSnapshot(
            sectionCoord,
            bounds.Min,
            renderableBlocks,
            compatibleBlocks);
    }

    private void RebuildSection(StructureRenderSectionCoord sectionCoord)
    {
        StructureRenderSectionSnapshot snapshot = CaptureSectionSnapshot(sectionCoord);

        if (snapshot.BlockCount == 0)
        {
            RemoveSection(sectionCoord);
            return;
        }

        StructureRenderSectionMeshBuildResult buildResult = StructureRenderSectionMeshBuilder.Build(snapshot);
        StructureRenderSectionView view = GetOrCreateSectionView(sectionCoord);
        view.ApplyMesh(buildResult.Mesh);
    }

    private StructureRenderSectionView GetOrCreateSectionView(StructureRenderSectionCoord sectionCoord)
    {
        if (_sectionViews.TryGetValue(sectionCoord, out StructureRenderSectionView? view))
        {
            return view;
        }

        view = new StructureRenderSectionView();
        view.Initialize(sectionCoord, BlockMaterial);
        AddChild(view);
        _sectionViews.Add(sectionCoord, view);
        return view;
    }

    private void RemoveSection(StructureRenderSectionCoord sectionCoord)
    {
        if (!_sectionViews.Remove(sectionCoord, out StructureRenderSectionView? view))
        {
            return;
        }

        view.ClearMesh();
        view.QueueFree();
    }

    private void RemoveSectionsExcept(HashSet<StructureRenderSectionCoord> retainedSections)
    {
        StructureRenderSectionCoord[] existingSections = _sectionViews.Keys.ToArray();

        foreach (StructureRenderSectionCoord sectionCoord in existingSections)
        {
            if (!retainedSections.Contains(sectionCoord))
            {
                RemoveSection(sectionCoord);
            }
        }
    }

    #endregion

    #region Helpers

    private StandardMaterial3D CreateBlockMaterial()
    {
        return new StandardMaterial3D
        {
            AlbedoColor = new Color(0.62f, 0.66f, 0.72f, 1f)
        };
    }

    private int CompareCells(MapCellCoord left, MapCellCoord right)
    {
        int xComparison = left.X.CompareTo(right.X);

        if (xComparison != 0)
        {
            return xComparison;
        }

        int yComparison = left.Y.CompareTo(right.Y);

        if (yComparison != 0)
        {
            return yComparison;
        }

        return left.Z.CompareTo(right.Z);
    }

    private void ForEachCell(StructureRenderSectionBounds bounds, Action<MapCellCoord> visit)
    {
        for (int x = bounds.Min.X; ; x++)
        {
            for (int y = bounds.Min.Y; ; y++)
            {
                for (int z = bounds.Min.Z; ; z++)
                {
                    visit(new MapCellCoord(x, y, z));

                    if (z == bounds.Max.Z)
                    {
                        break;
                    }
                }

                if (y == bounds.Max.Y)
                {
                    break;
                }
            }

            if (x == bounds.Max.X)
            {
                break;
            }
        }
    }

    #endregion
}
