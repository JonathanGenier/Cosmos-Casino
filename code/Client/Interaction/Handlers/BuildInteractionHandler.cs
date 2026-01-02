using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Map;
using System.Numerics;

/// <summary>
/// Interaction handler responsible for interpreting primary interaction
/// gestures as build intent.
/// This handler manages build previews, placement ranges, and final
/// build application, but does not handle input routing, tool switching,
/// or global command logic.
/// </summary>
public sealed class BuildInteractionHandler : IInteractionHandler
{
    #region FIELDS

    private BuildContext _buildContext;

    #endregion

    #region CONSTRUCTOR

    /// <summary>
    /// Initializes a new build interaction handler bound to the
    /// specified build context.
    /// </summary>
    /// <param name="buildContext">
    /// Shared build context containing the current build intent
    /// selected by the user.
    /// </param>
    public BuildInteractionHandler(BuildContext buildContext)
    {
        _buildContext = buildContext;
    }

    #endregion

    #region METHODS
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
        // No-op for now.
        // Later: initialize preview state.
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
        // No-op for now.
        // Later: update preview visuals.
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
        // 1. No build intent selected → do nothing
        if (!_buildContext.HasIntent)
        {
            return;
        }

        // 2. Invalid cursor resolution → do nothing
        if (!end.IsValid)
        {
            return;
        }

        // 3. Resolve world position → cell
        // (Replace this with your actual grid conversion logic)
        CellCoord cell = MapGridMath.WorldToCell(
            end.WorldPosition.X,
            end.WorldPosition.Y,
            end.WorldPosition.Z);

        // 4. Create build intent (floor only, for now)
        FloorType floorType = _buildContext.SelectedFloor!.Value;

        var intent = BuildIntent.CreateFloor(floorType, cell);

        // 5. Dispatch (for now: log it)
        ConsoleLog.System(
            nameof(BuildInteractionHandler),
            $"BuildIntent created: Floor={floorType}, Cell={cell}"
        );

        // Later:
        // _buildManager.Execute(intent);
    }

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
        // No-op for now.
        // Later: clear preview state.
    }

    #endregion
}
