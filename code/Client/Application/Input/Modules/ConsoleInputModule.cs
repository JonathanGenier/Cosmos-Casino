using Godot;

/// <summary>
/// Provides an input module that processes console-related debug input and emits intent signals for toggling the
/// in-application console UI.
/// </summary>
/// <remarks>This module is intended for use in development or debug builds to facilitate quick access to console
/// functionality via input actions. It processes input early in the frame to ensure responsive handling of
/// developer-facing controls. The module is always enabled and operates within the application input scope.</remarks>
public sealed class ConsoleInputModule : IProcessInputModule
{
    #region Fields

    private readonly InputManager _input;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the ConsoleInputModule class using the specified input manager.
    /// </summary>
    /// <param name="input">The input manager that provides input handling for the console module. Cannot be null.</param>
    public ConsoleInputModule(InputManager input)
    {
        _input = input;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current input phase.
    /// </summary>
    public InputPhase Phase => InputPhase.Debug;

    /// <summary>
    /// Gets the input scope used for interpreting user input.
    /// </summary>
    public InputScope Scope => InputScope.App;

    /// <summary>
    /// Gets a value indicating whether the module is enabled.
    /// </summary>
    public bool IsEnabled => true;

    #endregion

    #region Godot Process

    /// <summary>
    /// Handles per-frame input processing and emits a signal to toggle the console UI when the corresponding input
    /// action is triggered.
    /// </summary>
    /// <remarks>This method should be called once per frame, typically from the engine's process loop. It
    /// listens for the "toggle_console" input action and emits the ToggleConsoleUi signal when the action is
    /// detected.</remarks>
    /// <param name="delta">The elapsed time, in seconds, since the previous frame. This value can be used for frame-dependent calculations.</param>
    public void Process(double delta)
    {
        if (Input.IsActionJustPressed("toggle_console"))
        {
            _input.EmitSignal(InputManager.SignalName.ToggleConsoleUi);
        }
    }

    #endregion
}