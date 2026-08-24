using CosmosCasino.Core.Game.Map;
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Resolves exposed generated Block faces for render and collision builders.
/// </summary>
internal static class StructureSectionExposedFaceResolver
{
    #region Constants

    private const int MaxFacesPerBlock = 6;

    #endregion

    #region Resolution

    /// <summary>
    /// Resolves all exposed faces for compatible Blocks inside a section snapshot.
    /// </summary>
    /// <param name="snapshot">The disposable section snapshot.</param>
    /// <returns>Exposed faces in deterministic block and face order.</returns>
    internal static IReadOnlyList<StructureSectionExposedFace> Resolve(StructureRenderSectionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var exposedFaces = new List<StructureSectionExposedFace>(snapshot.BlockCount * MaxFacesPerBlock);

        foreach (MapCellCoord cell in snapshot.RenderableBlocks)
        {
            Vector3 center = ToSectionLocalCenter(snapshot.OriginCell, cell);

            foreach (StructureBlockFace face in StructureBlockFace.All)
            {
                if (StructureRenderSectionMath.TryAddOffset(cell, face.NeighborOffset, out MapCellCoord neighbor)
                    && snapshot.IsCompatibleOpaqueBlockAt(neighbor))
                {
                    continue;
                }

                exposedFaces.Add(new StructureSectionExposedFace(cell, center, face));
            }
        }

        return exposedFaces;
    }

    private static Vector3 ToSectionLocalCenter(MapCellCoord originCell, MapCellCoord cell)
    {
        return new Vector3(
            (cell.X - originCell.X) * WorldGridMetrics.GridUnitSize,
            (cell.Y - originCell.Y) * WorldGridMetrics.VerticalGridUnitSize,
            (cell.Z - originCell.Z) * WorldGridMetrics.GridUnitSize);
    }

    #endregion
}
