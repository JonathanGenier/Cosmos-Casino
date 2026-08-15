using CosmosCasino.Core.Game;
using Godot;

/// <summary>
/// Translates between Core horizontal world coordinates and Godot vectors.
/// </summary>
public static class WorldCoordGodotExtensions
{
    #region Methods

    /// <summary>
    /// Converts a Godot world position into an engine-agnostic Core horizontal world coordinate.
    /// </summary>
    /// <param name="worldPosition">
    /// The Godot world position to translate.
    /// </param>
    /// <returns>
    /// A Core horizontal world coordinate using Godot X as X and Godot Z as Y.
    /// </returns>
    public static WorldCoord ToWorldCoord(this Vector3 worldPosition)
    {
        return new WorldCoord(worldPosition.X, worldPosition.Z);
    }

    /// <summary>
    /// Converts an engine-agnostic Core horizontal world coordinate into a Godot world position.
    /// </summary>
    /// <param name="worldCoord">
    /// The Core horizontal world coordinate to translate.
    /// </param>
    /// <param name="y">
    /// The Godot vertical Y coordinate to apply.
    /// </param>
    /// <returns>
    /// A Godot world position using Core X as Godot X and Core Y as Godot Z.
    /// </returns>
    public static Vector3 ToGodotVector3(this WorldCoord worldCoord, float y = 0f)
    {
        return new Vector3(worldCoord.X, y, worldCoord.Y);
    }

    #endregion
}
