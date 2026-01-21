using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

/// <summary>
/// Provides build context and logic for constructing floor elements within a map grid.
/// </summary>
/// <remarks>Use this class to interpret user input or cursor positions as floor-building operations. The context
/// determines the set of map cells to be affected and produces build intents specific to floor construction. This type
/// is sealed and cannot be inherited.</remarks>
public sealed class FloorBuildContext : BuildContextBase
{
    #region Properties

    /// <summary>
    /// Gets the type of build represented by this instance.
    /// </summary>
    public override BuildKind Kind => BuildKind.Floor;

    #endregion

    #region Build Intent

    /// <summary>
    /// Attempts to create a build intent for constructing a floor between the specified start and end map cells.
    /// </summary>
    /// <param name="startCell">The starting cell coordinate of the intended floor area.</param>
    /// <param name="endCell">The ending cell coordinate of the intended floor area.</param>
    /// <param name="buildOperation">The type of build operation to perform (e.g., place or remove).</param>
    /// <param name="intent">When this method returns, contains the build intent for the floor if the operation succeeds; otherwise, null.
    /// This parameter is passed uninitialized.</param>
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
                intent = BuildIntent.PlaceFloor(cells);
                break;
            case BuildOperation.Remove:
                intent = BuildIntent.RemoveFloor(cells);
                break;
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
    /// Returns a read-only list of map cell coordinates representing the area between the specified start and end
    /// cells, based on the given build operation.
    /// </summary>
    /// <param name="startCell">The coordinate of the starting cell that defines one corner of the area to retrieve.</param>
    /// <param name="endCell">The coordinate of the ending cell that defines the opposite corner of the area to retrieve.</param>
    /// <param name="buildOperation">The build operation to perform. Determines whether cells are returned for placement, removal, or if no operation
    /// is performed.</param>
    /// <returns>A read-only list of map cell coordinates within the specified area if the build operation is Place or Remove;
    /// otherwise, an empty list.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the specified build operation is not supported.</exception>
    public override IReadOnlyList<MapCellCoord> GetCells(MapCellCoord startCell, MapCellCoord endCell, BuildOperation buildOperation)
    {
        switch (buildOperation)
        {
            case BuildOperation.Place:
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
