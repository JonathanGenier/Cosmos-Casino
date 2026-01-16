using Godot;

/// <summary>
/// Represents a 3D preview node for visualizing wall placement within a scene.
/// </summary>
/// <remarks>Use this class to display a visual representation of a wall before it is placed in the environment.
/// The preview automatically snaps to a grid for precise alignment, aiding users in positioning walls accurately during
/// level editing or construction workflows.</remarks>
public sealed partial class WallPreview : Node3D
{
    /// <summary>
    /// Sets the object's world position to the specified coordinates, snapping the position to the nearest grid point.
    /// </summary>
    /// <remarks>This method has no effect if the object is not currently part of the scene tree.</remarks>
    /// <param name="worldPosition">The target world position to set, in global coordinates. The position will be adjusted to align with the grid.</param>
    public void SetWorldPosition(Vector3 worldPosition)
    {
        if (!IsInsideTree())
        {
            return;
        }

        GlobalPosition = SnapToGrid(worldPosition);
    }

    /// <summary>
    /// Snaps the specified world position to the center of the nearest grid cell on the XZ plane.
    /// </summary>
    /// <remarks>This method is useful for aligning objects to a grid in 3D space, such as in level editors or
    /// grid-based games. The grid cells are assumed to be 1 unit in size.</remarks>
    /// <param name="worldPos">The world position to be snapped to the grid. The Y component is preserved.</param>
    /// <returns>A <see cref="Vector3"/> representing the position aligned to the center of the nearest grid cell on the XZ
    /// plane, with the original Y value.</returns>
    private static Vector3 SnapToGrid(Vector3 worldPos)
    {
        return new Vector3(
            Mathf.Floor(worldPos.X) + 0.5f,
            worldPos.Y,
            Mathf.Floor(worldPos.Z) + 0.5f
        );
    }
}
