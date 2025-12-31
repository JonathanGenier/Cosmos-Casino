using Godot;
using System;

/// <summary>
/// Input module responsible for translating raw player input
/// into high-level camera movement, rotation, and zoom intents.
/// </summary>
public sealed class CameraInputModule : IProcessInputModule, IUnhandledInputModule
{
    #region FIELDS

    private readonly InputManager _input;
    private Func<bool> _isCursorBlockedByUi;

    #endregion

    #region CONSTRUCTORS

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraInputModule"/> and
    /// wires required client-side services used to evaluate UI blocking
    /// and dispatch camera-related input intents.
    /// </summary>
    /// <param name="clientService">
    /// Client service container used to provide UI state queries required
    /// to determine whether camera input should be blocked.
    /// </param>
    /// <param name="input">
    /// Input manager through which camera movement, rotation, and zoom
    /// intents are emitted.
    /// </param>
    public CameraInputModule(ClientServices clientService, InputManager input)
    {
        _input = input;
        _isCursorBlockedByUi = () => clientService.UiManager.IsCursorBlockedByUi;
    }

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Execution phase for this input module.
    /// </summary>
    public InputPhase Phase => InputPhase.Camera;

    #endregion

    #region METHODS

    /// <summary>
    /// Processes continuous camera movement and rotation input
    /// during the per-frame input phase.
    /// </summary>
    /// <param name="delta">
    /// Frame time in seconds.
    /// </param>
    /// <inheritdoc/>
    public void Process(double delta)
    {
        Vector2 moveDirection = Input.GetVector("camera_move_left", "camera_move_right", "camera_move_backward", "camera_move_forward");
        float rotateDirection = Input.GetAxis("camera_rotate_right", "camera_rotate_left");
        _input.EmitSignal(InputManager.SignalName.MoveCamera, moveDirection);
        _input.EmitSignal(InputManager.SignalName.RotateCamera, rotateDirection);
    }


    /// <summary>
    /// Processes discrete camera-related input actions that occur
    /// during the unhandled input phase, such as zooming.
    /// </summary>
    /// <param name="event">
    /// The unhandled input event received from the engine.
    /// </param>
    /// <inheritdoc/>
    public void UnhandledInput(InputEvent @event)
    {
        if (_isCursorBlockedByUi())
        {
            return;
        }

        if (@event.IsActionPressed("camera_zoom_in"))
        {
            _input.EmitSignal(InputManager.SignalName.ZoomCamera, -1.0f);
        }
        else if (@event.IsActionPressed("camera_zoom_out"))
        {
            _input.EmitSignal(InputManager.SignalName.ZoomCamera, 1.0f);
        }
    }

    #endregion
}