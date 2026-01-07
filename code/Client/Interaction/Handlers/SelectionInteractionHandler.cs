using CosmosCasino.Core.Console.Logging;

/// <summary>
/// Interaction handler responsible for interpreting primary interaction
/// gestures as selection intent.
/// This handler manages selection previews and final selection logic,
/// but does not handle input routing, tool switching, or command execution.
/// </summary>
public sealed class SelectionInteractionHandler : IInteractionHandler
{
    // ===========================================================================
    // PRIMARY

    /// <summary>
    /// Called when a primary selection gesture begins.
    /// Initializes selection state and prepares any visual feedback
    /// required to represent the selection intent.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the selection gesture.
    /// </param>
    public void OnPrimaryGestureStarted(CursorContext start)
    {

    }

    /// <summary>
    /// Called while a primary selection gesture is in progress.
    /// Updates selection previews or highlighted elements based on
    /// the current cursor context.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the selection gesture.
    /// </param>
    /// <param name="current">
    /// Current cursor context during the gesture.
    /// </param>
    public void OnPrimaryGestureUpdated(CursorContext start, CursorContext current)
    {

    }

    /// <summary>
    /// Called when a primary selection gesture completes.
    /// Finalizes the selection and applies it to the relevant
    /// world elements.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the selection gesture.
    /// </param>
    /// <param name="end">
    /// Cursor context captured at the end of the selection gesture.
    /// </param>
    public void OnPrimaryGestureEnded(CursorContext start, CursorContext end)
    {

    }

    // ===========================================================================
    // CANCEL


    /// <summary>
    /// Called when an in-progress primary selection gesture is cancelled.
    /// Clears any active selection previews and restores the previous
    /// selection state if necessary.
    /// </summary>
    /// <param name="start">
    /// Cursor context captured at the start of the cancelled gesture.
    /// </param>
    public void OnPrimaryGestureCancelled(CursorContext start)
    {

    }
}
