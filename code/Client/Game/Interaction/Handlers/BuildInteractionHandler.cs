using CosmosCasino.Core.Game.Build;
using System;

/// <summary>
/// Handles player build interactions by translating input gestures into build actions using the provided build context
/// and client build manager.
/// </summary>
/// <remarks>This handler coordinates the process of initiating, updating, finalizing, and cancelling build
/// operations in response to user gestures. It raises the BuildRequested event when a build action is ready to be
/// processed. This class is typically used in scenarios where user input drives in-game construction or placement
/// mechanics.</remarks>
public sealed class BuildInteractionHandler : IInteractionHandler
{
    #region Fields

    private readonly BuildContext _buildContext;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildInteractionHandler class using the specified build context.
    /// </summary>
    /// <param name="buildContext">The build context to associate with this handler. Cannot be null.</param>
    public BuildInteractionHandler(BuildContext buildContext)
    {
        ArgumentNullException.ThrowIfNull(buildContext);
        _buildContext = buildContext;
    }

    #endregion

    #region Input Gesture Handlers

    /// <summary>
    /// Handles the initiation of a primary gesture using the specified cursor context.
    /// </summary>
    /// <param name="start">The context information associated with the start of the primary gesture.</param>
    public void OnPrimaryGestureStarted(CursorContext start)
    {
        _buildContext.BeginBuild(start);
    }

    /// <summary>
    /// Handles updates to the primary gesture by processing the change from the initial to the current cursor context.
    /// </summary>
    /// <param name="start">The initial cursor context at the start of the gesture.</param>
    /// <param name="current">The current cursor context representing the latest state of the gesture.</param>
    public void OnPrimaryGestureUpdated(CursorContext start, CursorContext current)
    {
        _buildContext.UpdateBuild(current);
    }

    /// <summary>
    /// Handles the completion of the primary gesture by determining the affected cells and initiating a build request
    /// if appropriate.
    /// </summary>
    /// <remarks>This method is typically called when a user completes a primary input gesture, such as a drag
    /// or selection, to trigger a build action based on the selected area. No action is taken if the build context does
    /// not have an intent or if the end context is invalid.</remarks>
    /// <param name="start">The context representing the starting point of the gesture, including its world position.</param>
    /// <param name="end">The context representing the ending point of the gesture, including its world position. Must be valid for the
    /// operation to proceed.</param>
    public void OnPrimaryGestureEnded(CursorContext start, CursorContext end)
    {
        _buildContext.EndBuild(end);
    }

    /// <summary>
    /// Handles cancellation of the primary gesture, allowing the application to respond appropriately when a gesture is
    /// interrupted.
    /// </summary>
    /// <param name="start">The context associated with the cursor at the start of the gesture.</param>
    public void OnPrimaryGestureCancelled(CursorContext start)
    {
        _buildContext.CancelBuild();
    }

    #endregion
}
