using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using System;
using System.Collections.Generic;

/// <summary>
/// Provides build context logic for constructing walls within a map, including cell selection and build intent
/// creation.
/// </summary>
/// <remarks>This class specializes the build process for wall structures by determining which map cells are
/// affected and generating the appropriate build intent. It is intended for use in scenarios where wall placement and
/// construction are required. Instances of this class are immutable and thread-safe.</remarks>
public sealed class WallBuildContext : BuildContextBase
{
    #region Properties

    /// <summary>
    /// Gets the type of build operation represented by this instance.
    /// </summary>
    public override BuildKind Kind => BuildKind.Wall;

    #endregion

    #region Build Intent

    /// <summary>
    /// Attempts to create a build intent representing a wall between the specified start and end cells.
    /// </summary>
    /// <param name="startCell">The starting cell coordinate for the wall.</param>
    /// <param name="endCell">The ending cell coordinate for the wall.</param>
    /// <param name="buildOperation">The type of build operation to perform.</param>
    /// <param name="intent">When this method returns, contains the created build intent if successful; otherwise, null.</param>
    /// <returns>true if a build intent was successfully created; otherwise, false.</returns>
    public override bool TryCreateBuildIntent(MapCellCoord startCell, MapCellCoord endCell, BuildOperation buildOperation, out BuildIntent intent)
    {
        var cells = GetCells(startCell, endCell, buildOperation);

        if (cells.Count == 0)
        {
            intent = null!;
            return false;
        }

        switch (buildOperation)
        {
            case BuildOperation.Place:
                intent = BuildIntent.PlaceWall(cells);
                return true;
            case BuildOperation.Remove:
                intent = BuildIntent.RemoveWall(cells);
                return true;
            case BuildOperation.None:
                intent = null!;
                break;
            default:
                throw new InvalidOperationException($"Unsupported build operation: {buildOperation}");
        }

        return true;
    }

    #endregion

    #region Get Cells

    /// <summary>
    /// Returns a read-only list of map cell coordinates affected by the specified build operation between the given
    /// start and end cells.
    /// </summary>
    /// <remarks>If <paramref name="buildOperation"/> is <see cref="BuildOperation.Place"/>, the returned
    /// cells form a line between <paramref name="startCell"/> and <paramref name="endCell"/>. If <paramref
    /// name="buildOperation"/> is <see cref="BuildOperation.Remove"/>, the returned cells cover the area defined by the
    /// two coordinates.</remarks>
    /// <param name="startCell">The starting cell coordinate for the operation. Defines one endpoint of the affected area or line.</param>
    /// <param name="endCell">The ending cell coordinate for the operation. Defines the other endpoint of the affected area or line.</param>
    /// <param name="buildOperation">The type of build operation to perform. Determines how the affected cells are selected.</param>
    /// <returns>A read-only list of <see cref="MapCellCoord"/> values representing the cells affected by the operation. Returns
    /// an empty list if <paramref name="buildOperation"/> is <see cref="BuildOperation.None"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="buildOperation"/> is not a supported value.</exception>
    public override IReadOnlyList<MapCellCoord> GetCells(MapCellCoord startCell, MapCellCoord endCell, BuildOperation buildOperation)
    {
        switch (buildOperation)
        {
            case BuildOperation.Place:
                return GetCellsLine(startCell, endCell);
            case BuildOperation.Remove:
                return GetCellsArea(startCell, endCell);
            case BuildOperation.None:
                return Array.Empty<MapCellCoord>();
            default:
                throw new InvalidOperationException($"Unsupported build operation: {buildOperation}");
        }
    }

    #endregion
}
