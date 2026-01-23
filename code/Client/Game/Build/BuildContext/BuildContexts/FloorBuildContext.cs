using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using System;
using System.Collections.Generic;

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
    /// <param name="buildInteractionMode">The interaction mode that influences how cells are selected for the build operation.</param>
    /// <param name="intent">When this method returns, contains the build intent for the floor if the operation succeeds; otherwise, null.
    /// This parameter is passed uninitialized.</param>
    /// <returns>true if a build intent was successfully created; otherwise, false.</returns>
    public override bool TryCreateBuildIntent(
        MapCellCoord startCell,
        MapCellCoord endCell,
        BuildOperation buildOperation,
        BuildInteractionMode buildInteractionMode,
        out BuildIntent intent)
    {
        var cells = GetCells(startCell, endCell, buildOperation, buildInteractionMode);

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
    /// <param name="buildInteractionMode">The interaction mode that influences how cells are selected for the build operation.</param>
    /// <returns>A read-only list of map cell coordinates within the specified area if the build operation is Place or Remove;
    /// otherwise, an empty list.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the specified build operation is not supported.</exception>
    public override IReadOnlyList<MapCellCoord> GetCells(
        MapCellCoord startCell,
        MapCellCoord endCell,
        BuildOperation buildOperation,
        BuildInteractionMode buildInteractionMode)
    {
        switch (buildOperation)
        {
            case BuildOperation.Place:
                return GetPlaceCells(startCell, endCell, buildInteractionMode);
            case BuildOperation.Remove:
                return GetCellsRectangleArea(startCell, endCell);
            case BuildOperation.None:
                return Array.Empty<MapCellCoord>();
            default:
                throw new InvalidOperationException($"Unsupported build operation: {buildOperation}");
        }
    }

    /// <summary>
    /// Returns a collection of map cell coordinates representing the area or path between two specified cells, based on
    /// the selected build interaction mode.
    /// </summary>
    /// <remarks>The returned collection is read-only and its shape varies according to the build interaction
    /// mode. For example, rectangle and square modes return all cells within the defined area, while line modes return
    /// cells along a path between the endpoints.</remarks>
    /// <param name="startCell">The starting cell coordinate that defines one endpoint of the area or path.</param>
    /// <param name="endCell">The ending cell coordinate that defines the other endpoint of the area or path.</param>
    /// <param name="buildInteractionMode">The build interaction mode that determines how the area or path between the cells is calculated. Supported modes
    /// include rectangle, square, straight line, dynamic line, and circle.</param>
    /// <returns>An immutable list of map cell coordinates that make up the selected area or path. The contents and shape of the
    /// list depend on the specified build interaction mode.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an unsupported build interaction mode is specified.</exception>
    private IReadOnlyList<MapCellCoord> GetPlaceCells(
        MapCellCoord startCell,
        MapCellCoord endCell,
        BuildInteractionMode buildInteractionMode)
    {
        switch (buildInteractionMode)
        {
            case BuildInteractionMode.Default:
                return GetCellsRectangleArea(startCell, endCell);
            case BuildInteractionMode.ShiftAlternative:
                return GetCellsSquareArea(startCell, endCell);
            case BuildInteractionMode.CtrlAlternative:
                return GetCellsStraightLine(startCell, endCell);
            case BuildInteractionMode.AltAlternative:
                return GetCellsDynamicLine(startCell, endCell);
            case BuildInteractionMode.ShiftCtrlAlternative:
                return GetCellsCircleArea(startCell, endCell);
            default:
                throw new InvalidOperationException($"Unsupported build interaction mode: {buildInteractionMode}");
        }
    }

    #endregion
}
