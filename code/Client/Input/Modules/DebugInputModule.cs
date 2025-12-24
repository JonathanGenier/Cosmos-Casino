using Godot;

/// <summary>
/// Input module responsible for detecting debug-related input intent.
/// This module monitors debug-specific input actions and emits semantic
/// debug intent signals through the <see cref="InputManager"/> without
/// performing any UI or gameplay logic directly.
/// </summary>
public sealed class DebugInputModule : IInputModule
{
    #region FIELDS

    /// <summary>
    /// Reference to the owning input manager used to emit debug intent signals.
    /// </summary>
    private readonly InputManager _input;

    #endregion

    #region CONSTRUCTORS

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugInputModule"/>.
    /// </summary>
    /// <param name="input">
    /// Input manager through which debug intent signals are dispatched.
    /// </param>
    public DebugInputModule(InputManager input)
    {
        _input = input;
    }

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Execution phase for this input module.
    /// Debug input is processed early in the frame to ensure responsive
    /// handling of developer-facing controls.
    /// </summary>
    public InputPhase Phase => InputPhase.Debug;

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Processes debug-related input for the current frame.
    /// When a debug toggle action is detected, a corresponding intent
    /// signal is emitted via the input manager.
    /// </summary>
    /// <param name="delta">
    /// Frame delta time in seconds.
    /// </param>
    public void Process(double delta)
    {
        if (Input.IsActionJustPressed("toggle_console"))
        {
            _input.EmitSignal(InputManager.SignalName.ToggleConsoleUi);
        }
    }

    #endregion
}