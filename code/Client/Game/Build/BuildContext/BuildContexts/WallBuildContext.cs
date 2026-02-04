using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
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
    /// <param name="buildInteractionMode">The interaction mode that influences cell selection for the build operation.</param>
    /// <param name="intent">When this method returns, contains the created build intent if successful; otherwise, null.</param>
    /// <returns>true if a build intent was successfully created; otherwise, false.</returns>
    public override bool TryCreateBuildIntent(
        MapCoord startCell,
        MapCoord endCell,
        BuildOperation buildOperation,
        BuildInteractionMode buildInteractionMode,
        out BuildIntent intent)
    {
        var cells = GetCells(
            startCell,
            endCell,
            buildOperation,
            buildInteractionMode);

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
    /// <param name="startCell">The starting cell coordinate that defines one corner of the area to process.</param>
    /// <param name="endCell">The ending cell coordinate that defines the opposite corner of the area to process.</param>
    /// <param name="buildOperation">The type of build operation to perform, such as placing or removing cells.</param>
    /// <param name="buildInteractionMode">Specifies how the build operation interacts with existing cells, influencing which cells are included in the
    /// result.</param>
    /// <returns>A read-only list of map cell coordinates affected by the build operation. The list will be empty if no cells are
    /// affected.</returns>
    /// <exception cref="InvalidOperationException">Thrown if an unsupported build operation is specified.</exception>
    public override IReadOnlyList<MapCoord> GetCells(
        MapCoord startCell,
        MapCoord endCell,
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
                return Array.Empty<MapCoord>();
            default:
                throw new InvalidOperationException($"Unsupported build operation: {buildOperation}");
        }
    }

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing the cells affected by the place build operation between
    /// the specified start and end cells, using the given interaction mode.
    /// </summary>
    /// <param name="startCell">The starting cell coordinate for the build operation. Determines one endpoint of the affected cell range.</param>
    /// <param name="endCell">The ending cell coordinate for the build operation. Determines the other endpoint of the affected cell range.</param>
    /// <param name="buildInteractionMode">The interaction mode that specifies how the cells between the start and end coordinates are selected. Different
    /// modes may result in different patterns or shapes of affected cells.</param>
    /// <returns>A read-only list of <see cref="MapCoord"/> values representing the cells selected by the build operation.
    /// The list may be empty if no cells are selected.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="buildInteractionMode"/> is not a supported value.</exception>
    private IReadOnlyList<MapCoord> GetPlaceCells(
        MapCoord startCell,
        MapCoord endCell,
        BuildInteractionMode buildInteractionMode)
    {
        switch (buildInteractionMode)
        {
            case BuildInteractionMode.Default:
                return GetCellsRectangleLine(startCell, endCell);
            case BuildInteractionMode.ShiftAlternative:
                return GetCellsSquareLine(startCell, endCell);
            case BuildInteractionMode.CtrlAlternative:
                return GetCellsStraightLine(startCell, endCell);
            case BuildInteractionMode.AltAlternative:
                return GetCellsDynamicLine(startCell, endCell);
            case BuildInteractionMode.ShiftCtrlAlternative:
                return GetCellsCircleLine(startCell, endCell);
            default:
                throw new InvalidOperationException($"Unsupported build interaction mode: {buildInteractionMode}");
        }
    }

    #endregion
}
