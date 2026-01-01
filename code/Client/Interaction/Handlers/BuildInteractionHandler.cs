/// <summary>
/// Interaction handler responsible for interpreting primary interaction
/// gestures as build intent.
/// This handler manages build previews, placement ranges, and final
/// build application, but does not handle input routing, tool switching,
/// or global command logic.
/// </summary>
public sealed class BuildInteractionHandler : IInteractionHandler
{
    // ===========================================================================
    // PRIMARY

    /// <summary>
    /// Called when a primary build gesture begins.
    /// Initializes build preview state and captures any data required
    /// to represent the starting point of the build operation.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the build gesture.
    /// </param>
    public void OnPrimaryGestureStarted(CursorContext start)
    {

    }

    /// <summary>
    /// Called while a primary build gesture is in progress.
    /// Updates build previews or placement indicators based on the
    /// current cursor context.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the build gesture.
    /// </param>
    /// <param name="current">
    /// Current cursor context during the build gesture.
    /// </param>
    public void OnPrimaryGestureUpdated(CursorContext start, CursorContext current)
    {

    }

    /// <summary>
    /// Called when a primary build gesture completes.
    /// Finalizes the build operation and applies the resulting
    /// world modifications.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the build gesture.
    /// </param>
    /// <param name="end">
    /// Cursor context captured at the end of the build gesture.
    /// </param>
    public void OnPrimaryGestureEnded(CursorContext start, CursorContext end)
    {

    }

    // ===========================================================================
    // CANCEL

    /// <summary>
    /// Called when an in-progress primary build gesture is cancelled.
    /// Clears any active build previews and restores the system to a
    /// neutral build state.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the cancelled gesture.
    /// </param>
    public void OnPrimaryGestureCancelled(CursorContext start)
    {

    }

}
