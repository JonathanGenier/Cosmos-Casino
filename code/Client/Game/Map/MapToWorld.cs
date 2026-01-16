using CosmosCasino.Core.Game.Map.Cell;
using CosmosCasino.Core.Game.Map.Grid;
using Godot;
using System.Data;

/// <summary>
/// Provides methods for converting logical map cell coordinates to world-space positions.
/// </summary>
public static class MapToWorld
{
    #region Methods

    /// <summary>
    /// Converts a map cell coordinate to the corresponding world position at the center of the cell.
    /// </summary>
    /// <remarks>The Y component of the returned position is always 0. The conversion uses the current cell
    /// size defined by <see cref="MapGrid.CellSize"/>.</remarks>
    /// <param name="cell">The map cell coordinate to convert to world space.</param>
    /// <returns>A <see cref="Vector3"/> representing the world position at the center of the specified cell.</returns>
    public static Vector3 CellToWorld(MapCellCoord cell)
    {
        float cellSize = MapGrid.CellSize;

        return new Vector3(
            (cell.X * cellSize) + (cellSize * 0.5f),
            0f,
            (cell.Y * cellSize) + (cellSize * 0.5f));
    }

    /// <summary>
    /// Converts a world-space position to the corresponding map cell coordinates.
    /// </summary>
    /// <remarks>The conversion uses the current cell size defined by <see cref="MapGrid.CellSize"/>. The Y
    /// component of the returned cell coordinate is set to 0.</remarks>
    /// <param name="position">The position in world space to convert to map cell coordinates.</param>
    /// <returns>A <see cref="MapCellCoord"/> representing the cell that contains the specified world position.</returns>
    public static MapCellCoord WorldToCell(Vector3 position)
    {
        float cellSize = MapGrid.CellSize;

        int cellX = Mathf.FloorToInt(position.X / cellSize);
        int cellY = Mathf.FloorToInt(position.Z / cellSize);

        return new MapCellCoord(cellX, cellY, 0);
    }

    #endregion
}