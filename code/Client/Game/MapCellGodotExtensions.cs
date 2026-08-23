using CosmosCasino.Core.Game.Map;
using Godot;

/// <summary>
/// Translates authoritative map-cell coordinates into Godot world positions.
/// </summary>
public static class MapCellGodotExtensions
{
    #region Methods

    /// <summary>
    /// Converts a global logical map-cell coordinate into the Godot world-space center of that cell.
    /// </summary>
    /// <param name="cell">The authoritative map-cell coordinate.</param>
    /// <returns>The Godot world-space center for <paramref name="cell"/>.</returns>
    public static Vector3 ToGodotCenter(this MapCellCoord cell)
    {
        return MapMath.CellToWorldCenter(cell.ToMapCoord()).ToGodotVector3(cell.ToElevation());
    }

    #endregion
}
