using CosmosCasino.Core.Game.Map;
using Godot;

/// <summary>
/// Provides coordinate conversion utilities between map-space cell coordinates
/// and world-space positions used by the rendering and interaction systems.
/// </summary>
public static class MapMath
{
    #region Methods

    /// <summary>
    /// Converts a map cell coordinate to the world-space position at the
    /// center of the corresponding cell.
    /// </summary>
    /// <param name="cell">The map cell coordinate to convert.</param>
    /// <returns>The world-space position representing the center of the cell.</returns>
    public static Vector3 MapToWorld(MapCoord cell)
    {
        float cellSize = 1.0f;

        return new Vector3(
            (cell.X * cellSize) + (cellSize * 0.5f),
            0f,
            (cell.Y * cellSize) + (cellSize * 0.5f));
    }

    /// <summary>
    /// Converts a world-space position to the corresponding map cell coordinate
    /// by flooring the position to the containing cell.
    /// </summary>
    /// <param name="position">The world-space position to convert.</param>
    /// <returns>The map cell coordinate containing the given position.</returns>
    public static MapCoord WorldToMap(Vector3 position)
    {
        float cellSize = 1.0f;

        int cellX = Mathf.FloorToInt(position.X / cellSize);
        int cellY = Mathf.FloorToInt(position.Z / cellSize);

        return new MapCoord(cellX, cellY);
    }

    #endregion
}