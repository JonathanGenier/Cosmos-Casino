using Godot;

/// <summary>
/// Handles camera movement, rotation, and zoom input for the game by processing input events and dispatching
/// camera-related intents through the input manager.
/// </summary>
/// <remarks>This module integrates with the game's input system to interpret user actions related to camera
/// control. It processes both continuous input (such as movement and rotation) and discrete actions (such as zoom)
/// during the appropriate input phases. The module respects UI blocking states to prevent camera input when user
/// interface elements require exclusive input focus.</remarks>
public sealed class CameraInputModule : IInputModule, IGameInputModule
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
        if (!_isEnabled)
        {
            return;
        }

        var inputState = _inputManager.State;

        MoveCamera(inputState);
        RotateCamera(inputState);
        ZoomCamera(inputState);
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

    #region Camera Input Methods

    /// <summary>
    /// Processes camera movement input and emits a signal indicating the desired movement direction.
    /// </summary>
    /// <param name="inputState">The current input state containing button presses that determine camera movement. Must not be null.</param>
    private void MoveCamera(InputState inputState)
    {
        Vector2 moveDirection = Vector2.Zero;

        if (inputState[InputButton.MoveLeft])
        {
            moveDirection.X -= 1;
        }

        if (inputState[InputButton.MoveRight])
        {
            moveDirection.X += 1;
        }

        if (inputState[InputButton.MoveForward])
        {
            moveDirection.Y += 1;
        }

        if (inputState[InputButton.MoveBackward])
        {
            moveDirection.Y -= 1;
        }

        _inputManager.EmitSignal(InputManager.SignalName.MoveCamera, moveDirection);
    }

    /// <summary>
    /// Processes input to determine the camera rotation direction and emits a signal to rotate the camera accordingly.
    /// </summary>
    /// <param name="inputState">The current input state containing button presses. Used to detect whether the rotate left or rotate right
    /// buttons are pressed.</param>
    private void RotateCamera(InputState inputState)
    {
        float rotationDirection = 0f;

        if (inputState[InputButton.RotateLeft])
        {
            rotationDirection -= 1;
        }

        if (inputState[InputButton.RotateRight])
        {
            rotationDirection += 1;
        }

        _inputManager.EmitSignal(InputManager.SignalName.RotateCamera, rotationDirection);
    }

    /// <summary>
    /// Processes camera zoom input based on the current input state. Emits a zoom signal if the UI is not blocking
    /// input and a scroll action is detected.
    /// </summary>
    /// <remarks>No zoom signal is emitted if input is blocked by the UI. This method should be called in
    /// response to input events where camera zooming is appropriate.</remarks>
    /// <param name="inputState">The current input state containing user interaction data. The scroll delta value determines whether a zoom
    /// signal is emitted.</param>
    private void ZoomCamera(InputState inputState)
    {
        if (_inputManager.IsInputBlockedByUi)
        {
            return;
        }

        if (inputState.ScrollDelta != 0f)
        {
            _inputManager.EmitSignal(InputManager.SignalName.ZoomCamera, inputState.ScrollDelta);
        }
    }

    #endregion
}