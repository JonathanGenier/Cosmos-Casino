using Godot;

/// <summary>
/// Defines a contract for handling input events that were not consumed during the standard input processing phase.
/// </summary>
/// <remarks>Implement this interface to respond to input events that remain unhandled after the primary input
/// modules have processed them. This is typically used for global input handling, such as shortcuts or fallback
/// behaviors, that should occur only if no other input handler has consumed the event.</remarks>
public interface IUnhandledInputModule : IInputModule
{
    /// <summary>
    /// Handles input events that are not processed by godot _process method.
    /// </summary>
    /// <remarks>Override this method to implement custom handling for input events that are not consumed by
    /// default input logic. This method is typically called after all other input handlers have had a chance to process
    /// the event.</remarks>
    /// <param name="event">The input event that was not handled by previous input processing methods. Cannot be null.</param>
    void UnhandledInput(InputEvent @event);
}
