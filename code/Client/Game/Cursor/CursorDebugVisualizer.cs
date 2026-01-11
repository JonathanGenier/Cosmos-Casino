using Godot;

/// <summary>
/// Provides a debug visualizer that displays the current cursor position in 3D world space using a visual marker.
/// </summary>
/// <remarks>This class is intended for development and debugging purposes to help visualize the cursor's location
/// within the scene. The marker is automatically shown or hidden based on whether the cursor position can be resolved,
/// such as when input is not blocked by UI elements. Attach this node to your scene and call Initialize with a valid
/// CursorManager to enable visualization.</remarks>
public sealed partial class CursorDebugVisualizer : Node3D
{
    #region Godot Inspector

    /// <summary>
    /// Gets or sets the size of the marker displayed in the Godot editor inspector.
    /// </summary>
    [Export]
    private float _markerSize = 0.05f;

    #endregion

    #region Fields

    private CursorManager _cursorManager;
    private MeshInstance3D _marker;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the instance with the specified cursor manager.
    /// </summary>
    /// <param name="cursorManager">The cursor manager to associate with this instance. Cannot be null.</param>
    public void Initialize(CursorManager cursorManager)
    {
        _cursorManager = cursorManager;
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Called when the node enters the scene tree. Initializes and adds a marker mesh as a child node.
    /// </summary>
    /// <remarks>This method is part of the Godot node lifecycle and is automatically invoked by the engine.
    /// Override this method to perform setup tasks that require the node to be part of the scene tree.</remarks>
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
    /// Updates the global position and visibility of the cursor based on the current cursor state.
    /// </summary>
    /// <remarks>If the cursor manager is unavailable, the method does not update the position or visibility.
    /// When the cursor position is available, the global position is updated and the cursor is made visible; otherwise,
    /// the cursor is hidden.</remarks>
    /// <param name="delta">The elapsed time, in seconds, since the previous frame. This value is provided by the engine and can be used for
    /// time-based calculations.</param>
    public override void _Process(double delta)
    {
        if (_cursorManager == null)
        {
            return;
        }

        if (_cursorManager.TryGetCursorPosition(out var position))
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