using Godot;

/// <summary>
/// Handles camera movement, rotation, and zoom input for the game by processing input events and dispatching
/// camera-related intents through the input manager.
/// </summary>
/// <remarks>This module integrates with the game's input system to interpret user actions related to camera
/// control. It processes both continuous input (such as movement and rotation) and discrete actions (such as zoom)
/// during the appropriate input phases. The module respects UI blocking states to prevent camera input when user
/// interface elements require exclusive input focus.</remarks>
public sealed class CameraInputModule : IProcessInputModule, IUnhandledInputModule, IGameInputModule
{
    #region Fields

    private readonly InputManager _inputManager;
    private bool _isEnabled;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the CameraInputModule class using the specified input manager.
    /// </summary>
    /// <param name="inputManager">The input manager that provides input events and state information for the module. Cannot be null.</param>
    public CameraInputModule(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the input phase associated with the camera inputs.
    /// </summary>
    public InputPhase Phase => InputPhase.Camera;

    /// <summary>
    /// Gets the input scope associated with this instance.
    /// </summary>
    public InputScope Scope => InputScope.Game;

    /// <summary>
    /// Gets a value indicating whether the module is enabled.
    /// </summary>
    public bool IsEnabled => _isEnabled;

    #endregion

    #region Godot Process

    /// <summary>
    /// Processes camera movement and rotation input for the current frame.
    /// </summary>
    /// <remarks>Call this method once per frame to update camera controls based on user input. The method
    /// emits signals for camera movement and rotation, which can be handled by other components to perform the actual
    /// camera updates.</remarks>
    /// <param name="delta">The elapsed time, in seconds, since the last frame. This value can be used to make movement frame rate
    /// independent.</param>
    public void Process(double delta)
    {
        Vector2 moveDirection = Input.GetVector("camera_move_left", "camera_move_right", "camera_move_backward", "camera_move_forward");
        float rotateDirection = Input.GetAxis("camera_rotate_right", "camera_rotate_left");
        _inputManager.EmitSignal(InputManager.SignalName.MoveCamera, moveDirection);
        _inputManager.EmitSignal(InputManager.SignalName.RotateCamera, rotateDirection);
    }

    /// <summary>
    /// Handles unhandled input events related to camera zoom actions.
    /// </summary>
    /// <remarks>This method should be called to process input events that have not been handled elsewhere. If
    /// input is currently blocked by the UI, the method ignores the event.</remarks>
    /// <param name="event">The input event to process. Represents a user action that may trigger camera zoom in or out.</param>
    public void UnhandledInput(InputEvent @event)
    {
        if (_inputManager.IsInputBlockedByUi())
        {
            return;
        }

        if (@event.IsActionPressed("camera_zoom_in"))
        {
            _inputManager.EmitSignal(InputManager.SignalName.ZoomCamera, -1.0f);
        }
        else if (@event.IsActionPressed("camera_zoom_out"))
        {
            _inputManager.EmitSignal(InputManager.SignalName.ZoomCamera, 1.0f);
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles changes to the game state by updating the enabled status of the component.
    /// </summary>
    /// <remarks>The component is disabled when the game enters the Loading state and enabled for all other
    /// states.</remarks>
    /// <param name="state">The new state of the game. Determines whether the component should be enabled or disabled.</param>
    public void OnGameStateChanged(GameState state)
    {
        _isEnabled = state != GameState.Loading;
    }

    #endregion
}