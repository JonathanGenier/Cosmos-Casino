using Godot;

/// <summary>
/// Represents a 3D node that provides a visual preview of a floor placement, typically used in grid-based building or
/// placement systems.
/// </summary>
/// <remarks>This class is intended to be used as a visual aid for users when positioning floor elements in a 3D
/// environment. It is designed to snap its position to a grid, ensuring accurate placement alignment. The node should
/// be added to the scene tree before invoking its preview functionality.</remarks>
public sealed partial class FloorPreview : Node3D
{
    /// <summary>
    /// Sets the object's global position in world coordinates, snapping the position to the nearest grid point.
    /// </summary>
    /// <remarks>This method has no effect if the object is not currently part of the scene tree.</remarks>
    /// <param name="worldPosition">The target position in world coordinates to which the object should be moved.</param>
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
