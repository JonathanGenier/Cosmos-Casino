/// <summary>
/// Defines methods for handling the lifecycle of a primary interaction gesture, including initiation, updates,
/// completion, and cancellation.
/// </summary>
/// <remarks>Implement this interface to respond to user interaction gestures, such as drag or selection
/// operations, by managing state and providing visual feedback throughout the gesture's lifecycle. Methods receive
/// cursor context information to enable context-aware handling of gestures.</remarks>
public interface IInteractionHandler
{
    /// <summary>
    /// Handles the initiation of a primary gesture using the specified cursor context.
    /// </summary>
    /// <param name="start">The context representing the state of the cursor at the start of the gesture. Cannot be null.</param>
    void OnPrimaryGestureStarted(CursorContext start);

    /// <summary>
    /// Handles updates to the primary gesture by processing the change from the initial to the current cursor context.
    /// </summary>
    /// <param name="start">The cursor context representing the state at the start of the gesture. Cannot be null.</param>
    /// <param name="current">The cursor context representing the current state of the gesture. Cannot be null.</param>
    void OnPrimaryGestureUpdated(CursorContext start, CursorContext current);

    /// <summary>
    /// Handles the completion of the primary gesture, providing context for both the gesture's start and end states.
    /// </summary>
    /// <param name="start">The context representing the state of the cursor at the beginning of the gesture.</param>
    /// <param name="end">The context representing the state of the cursor at the end of the gesture.</param>
    void OnPrimaryGestureEnded(CursorContext start, CursorContext end);

    /// <summary>
    /// Handles the cancellation of the primary gesture, allowing for cleanup or state reset as needed.
    /// </summary>
    /// <param name="start">The context associated with the gesture at the time of cancellation. Cannot be null.</param>
    void OnPrimaryGestureCancelled(CursorContext start);
}
