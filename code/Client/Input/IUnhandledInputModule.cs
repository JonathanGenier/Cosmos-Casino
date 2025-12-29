using Godot;

/// <summary>
/// Defines a module capable of handling unhandled Godot input events
/// during the unhandled input phase of the frame.
/// </summary>
public interface IUnhandledInputModule
{
    /// <summary>
    /// Execution phase that determines when this module is processed
    /// relative to other input modules.
    /// Lower <see cref="InputPhase"/> values are processed earlier in the frame.
    /// </summary>
    InputPhase Phase { get; }

    /// <summary>
    /// Processes a Godot input event that was not consumed
    /// during the standard input phase.
    /// </summary>
    /// <param name="event">
    /// The unhandled input event received from the engine.
    /// </param>
    void UnhandledInput(InputEvent @event);
}
