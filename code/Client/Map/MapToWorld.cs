using CosmosCasino.Core.Map.Cell;
using CosmosCasino.Core.Map.Grid;
using Godot;

/// <summary>
/// Provides helper methods for converting logical map coordinates
/// into world-space positions for visual placement.
/// </summary>
public static class MapToWorld
{
    #region Methods

    /// <summary>
    /// Converts a logical map cell coordinate into the world-space
    /// position representing the center of that cell.
    /// </summary>
    /// <param name="cell">
    /// Logical map cell coordinate to convert.
    /// </param>
    /// <returns>
    /// World-space position corresponding to the center of the specified cell.
    /// </returns>
    public static Vector3 CellToWorld(MapCellCoord cell)
    {
        float cellSize = MapGrid.CellSize;

        return new Vector3(
            (cell.X * cellSize) + (cellSize * 0.5f),
            0f,
            (cell.Y * cellSize) + (cellSize * 0.5f));
    }

    #endregion
}