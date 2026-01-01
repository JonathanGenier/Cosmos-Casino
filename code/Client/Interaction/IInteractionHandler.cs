/// <summary>
/// Defines a handler responsible for interpreting primary interaction
/// gestures and translating them into tool-specific world actions.
/// Implementations receive gesture lifecycle callbacks but do not
/// manage input state, tool switching, or global input routing.
/// </summary>
public interface IInteractionHandler
{
    /// <summary>
    /// Called when a primary interaction gesture begins.
    /// Implementations should capture initial state and prepare any
    /// visual feedback or preview associated with the gesture.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the gesture.
    /// </param>
    void OnPrimaryGestureStarted(CursorContext start);

    /// <summary>
    /// Called while a primary interaction gesture is in progress.
    /// Implementations may update visual feedback or previews based
    /// on the current cursor context.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the gesture.
    /// </param>
    /// <param name="current">
    /// Current cursor context during the gesture.
    /// </param>
    void OnPrimaryGestureUpdated(CursorContext start, CursorContext current);

    /// <summary>
    /// Called when a primary interaction gesture completes.
    /// Implementations should finalize the gesture and commit any
    /// resulting world changes.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the gesture.
    /// </param>
    /// <param name="end">
    /// Cursor context captured at the end of the gesture.
    /// </param>
    void OnPrimaryGestureEnded(CursorContext start, CursorContext end);

    /// <summary>
    /// Called when an in-progress primary interaction gesture is
    /// cancelled before completion.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the gesture.
    /// </param>
    void OnPrimaryGestureCancelled(CursorContext start);
}
