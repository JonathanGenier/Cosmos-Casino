using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using System;
using System.Collections.Generic;

/// <summary>
/// Resolves deterministic dirty Client structure sections from authoritative Core structure cells.
/// </summary>
internal static class StructureSectionInvalidation
{
    #region Full Projection

    /// <summary>
    /// Gets every section currently occupied by generated opaque Blocks.
    /// </summary>
    /// <param name="mapManager">The authoritative map manager to query.</param>
    /// <returns>Occupied sections in deterministic coordinate order.</returns>
    internal static IReadOnlyList<StructureRenderSectionCoord> GetOccupiedBlockSections(MapManager mapManager)
    {
        ArgumentNullException.ThrowIfNull(mapManager);

        var occupiedSections = new HashSet<StructureRenderSectionCoord>();

        foreach (StructureSnapshot snapshot in mapManager.GetStructureSnapshots())
        {
            if (!StructureSectionSnapshotFactory.IsGeneratedOpaqueBlock(snapshot.Definition.Id))
            {
                continue;
            }

            foreach (MapCellCoord cell in snapshot.Definition.Footprint.Resolve(snapshot.Anchor, snapshot.Rotation))
            {
                occupiedSections.Add(StructureRenderSectionMath.ToSectionCoord(cell));
            }
        }

        return SortSections(occupiedSections);
    }

    #endregion

    #region Dirty Sections

    /// <summary>
    /// Gets sections whose exposed faces may have changed around the specified authoritative cells.
    /// </summary>
    /// <param name="affectedCells">The authoritative cells changed by a successful build result.</param>
    /// <returns>Dirty sections in deterministic coordinate order.</returns>
    internal static IReadOnlyList<StructureRenderSectionCoord> GetDirtySections(IReadOnlyList<MapCellCoord> affectedCells)
    {
        ArgumentNullException.ThrowIfNull(affectedCells);

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

        return SortSections(dirtySections);
    }

    #endregion

    #region Helpers

    private static IReadOnlyList<StructureRenderSectionCoord> SortSections(HashSet<StructureRenderSectionCoord> sections)
    {
        var sortedSections = new List<StructureRenderSectionCoord>(sections);
        sortedSections.Sort(CompareSections);
        return sortedSections;
    }

    private static int CompareSections(StructureRenderSectionCoord left, StructureRenderSectionCoord right)
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

    #endregion
}
