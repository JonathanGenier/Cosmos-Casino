using Godot;

/// <summary>
/// Development-only debug node that visualizes the current
/// world-space cursor position.
/// <para>
/// The visualizer queries <see cref="CursorService"/> each frame and
/// renders a small marker at the resolved cursor position, allowing
/// verification of raycasting, plane fallback, and camera interaction
/// behavior during development.
/// </para>
/// <para>
/// This node has no gameplay responsibility and should not be included
/// in production builds.
/// </para>
/// </summary>
public sealed partial class CursorDebugVisualizer : Node3D
{
    #region INSPECTOR

    /// <summary>
    /// Radius of the visual marker used to represent the cursor
    /// position in world space.
    /// </summary>
    [Export]
    private float _markerSize = 0.05f;

    #endregion

    #region FIELDS

    private MeshInstance3D _marker;

    #endregion

    #region METHODS

    /// <summary>
    /// Initializes the debug visualizer by creating and attaching
    /// a simple spherical marker used to represent the cursor
    /// position in the world.
    /// </summary>
    public override void _Ready()
    {
        _marker = new MeshInstance3D
        {
            Mesh = new SphereMesh
            {
                Radius = _markerSize,
                Height = _markerSize,
            }
        };

        AddChild(_marker);
    }

    /// <summary>
    /// Updates the visualizer each frame by querying the cursor
    /// world position and synchronizing the marker's transform.
    /// <para>
    /// The marker is hidden when the cursor position cannot be
    /// resolved (e.g., when input is blocked by UI).
    /// </para>
    /// </summary>
    /// <param name="delta">
    /// Frame time in seconds.
    /// </param>
    public override void _Process(double delta)
    {
        if (CursorService.TryGetCursorPosition(out var position))
        {
            GlobalPosition = position;
            Visible = true;
        }
        else
        {
            Visible = false;
        }
    }

    #endregion
}