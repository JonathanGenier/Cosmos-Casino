using CosmosCasino.Core.Game.Map;
using System;
using System.Collections.Generic;

/// <summary>
/// Disposable Client snapshot of renderable section cells and compatible one-cell halo occupancy.
/// </summary>
internal sealed class StructureRenderSectionSnapshot
{
    #region Fields

    private readonly HashSet<MapCellCoord> _compatibleOpaqueBlocks;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a disposable section snapshot.
    /// </summary>
    /// <param name="coord">The render section coordinate.</param>
    /// <param name="originCell">The section's minimum global cell used as the mesh-local origin.</param>
    /// <param name="renderableBlocks">Compatible Blocks inside the section interior.</param>
    /// <param name="compatibleOpaqueBlocks">Compatible Blocks inside the section interior plus one-cell halo.</param>
    internal StructureRenderSectionSnapshot(
        StructureRenderSectionCoord coord,
        MapCellCoord originCell,
        IReadOnlyList<MapCellCoord> renderableBlocks,
        IReadOnlySet<MapCellCoord> compatibleOpaqueBlocks)
    {
        ArgumentNullException.ThrowIfNull(renderableBlocks);
        ArgumentNullException.ThrowIfNull(compatibleOpaqueBlocks);

        Coord = coord;
        OriginCell = originCell;
        RenderableBlocks = new List<MapCellCoord>(renderableBlocks).AsReadOnly();
        _compatibleOpaqueBlocks = new HashSet<MapCellCoord>(compatibleOpaqueBlocks);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the render section coordinate.
    /// </summary>
    internal StructureRenderSectionCoord Coord { get; }

    /// <summary>
    /// Gets the section's minimum global cell used as the mesh-local origin.
    /// </summary>
    internal MapCellCoord OriginCell { get; }

    /// <summary>
    /// Gets compatible Blocks inside the section interior.
    /// </summary>
    internal IReadOnlyList<MapCellCoord> RenderableBlocks { get; }

    /// <summary>
    /// Gets the number of compatible Blocks inside the section interior.
    /// </summary>
    internal int BlockCount => RenderableBlocks.Count;

    #endregion

    #region Queries

    /// <summary>
    /// Determines whether a global cell contains a compatible opaque Block in the interior or halo.
    /// </summary>
    /// <param name="cell">The global logical cell to query.</param>
    /// <returns><c>true</c> when the cell contains a compatible opaque Block; otherwise, <c>false</c>.</returns>
    internal bool IsCompatibleOpaqueBlockAt(MapCellCoord cell)
    {
        return _compatibleOpaqueBlocks.Contains(cell);
    }

    #endregion
}
