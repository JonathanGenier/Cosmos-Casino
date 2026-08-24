using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using System;
using System.Collections.Generic;

/// <summary>
/// Captures disposable Client section snapshots for generated static Structure projections.
/// </summary>
internal static class StructureSectionSnapshotFactory
{
    #region Snapshot

    /// <summary>
    /// Captures compatible opaque Block cells inside the section and its one-cell halo.
    /// </summary>
    /// <param name="mapManager">The authoritative map manager to query.</param>
    /// <param name="sectionCoord">The Client structure section to snapshot.</param>
    /// <returns>A disposable section snapshot for generated render/collision builders.</returns>
    internal static StructureRenderSectionSnapshot CaptureBlockSection(
        MapManager mapManager,
        StructureRenderSectionCoord sectionCoord)
    {
        ArgumentNullException.ThrowIfNull(mapManager);

        StructureRenderSectionBounds bounds = StructureRenderSectionMath.GetBounds(sectionCoord);
        StructureRenderSectionBounds haloBounds = StructureRenderSectionMath.Expand(bounds, amount: 1);
        var renderableBlocks = new List<MapCellCoord>();
        var compatibleBlocks = new HashSet<MapCellCoord>();

        ForEachCell(haloBounds, cell =>
        {
            if (!mapManager.TryGetStructureSnapshotAt(cell, out StructureSnapshot snapshot)
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

    /// <summary>
    /// Determines whether a Structure definition participates in generated Block-style section surfaces.
    /// </summary>
    /// <param name="definitionId">The Core structure definition identity.</param>
    /// <returns><c>true</c> when the definition is the canonical Block definition.</returns>
    internal static bool IsGeneratedOpaqueBlock(StructureDefinitionId definitionId)
    {
        return definitionId == StructureDefinitions.BlockDefinitionId;
    }

    #endregion

    #region Helpers

    private static int CompareCells(MapCellCoord left, MapCellCoord right)
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

    private static void ForEachCell(StructureRenderSectionBounds bounds, Action<MapCellCoord> visit)
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
