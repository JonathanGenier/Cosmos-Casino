using CosmosCasino.Core.Build;
using CosmosCasino.Core.Map.Cell;
using CosmosCasino.Core.Map.Grid;
using Godot;
using System;
using System.Collections.Generic;

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

    private BuildContext _buildContext;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildInteractionHandler class using the specified build context.
    /// </summary>
    /// <param name="buildContext">The build context to associate with this handler. Cannot be null.</param>
    public BuildInteractionHandler(BuildContext buildContext)
    {
        _buildContext = buildContext;
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a build is requested with a specified build intent.
    /// </summary>
    /// <remarks>Subscribers can handle this event to initiate or respond to build operations based on the
    /// provided build intent. The event provides a <see cref="BuildIntent"/> parameter that describes the details of
    /// the requested build.</remarks>
    public event Action<BuildIntent> BuildRequested;

    #endregion

    #region Input Gesture Handlers

    /// <summary>
    /// Handles the initiation of a primary gesture using the specified cursor context.
    /// </summary>
    /// <param name="start">The context information associated with the start of the primary gesture.</param>
    public void OnPrimaryGestureStarted(CursorContext start)
    {
        // No-op for now.
        // Later: initialize preview state.
    }

    /// <summary>
    /// Handles updates to the primary gesture by processing the change from the initial to the current cursor context.
    /// </summary>
    /// <param name="start">The initial cursor context at the start of the gesture.</param>
    /// <param name="current">The current cursor context representing the latest state of the gesture.</param>
    public void OnPrimaryGestureUpdated(CursorContext start, CursorContext current)
    {
        // No-op for now.
        // Later: update preview visuals.
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
        if (!_buildContext.HasIntent || !end.IsValid)
        {
            return;
        }

        var cells = GetCellsBetween(start.WorldPosition, end.WorldPosition);

        if (cells.Count == 0)
        {
            return;
        }

        var floorType = _buildContext.SelectedFloor!.Value;
        var intent = BuildIntent.BuildFloor(cells, floorType);
        BuildRequested?.Invoke(intent);
    }

    /// <summary>
    /// Handles cancellation of the primary gesture, allowing the application to respond appropriately when a gesture is
    /// interrupted.
    /// </summary>
    /// <param name="start">The context associated with the cursor at the start of the gesture.</param>
    public void OnPrimaryGestureCancelled(CursorContext start)
    {
        // No-op for now.
        // Later: clear preview state.
    }

    #endregion

    #region Get Cells

    /// <summary>
    /// Returns a list of map cell coordinates that lie within the rectangular area defined by two world-space
    /// positions.
    /// </summary>
    /// <remarks>The method assumes that both positions are on the same Z layer. The returned cells are
    /// ordered by X and then by Y coordinate.</remarks>
    /// <param name="startWorld">The world-space position representing one corner of the rectangular area.</param>
    /// <param name="endWorld">The world-space position representing the opposite corner of the rectangular area.</param>
    /// <returns>A read-only list of map cell coordinates that are contained within the rectangle defined by the two world
    /// positions. The list includes all cells between the two corners, inclusive.</returns>
    private static IReadOnlyList<MapCellCoord> GetCellsBetween(Vector3 startWorld, Vector3 endWorld)
    {
        MapCellCoord startCell = MapGrid.WorldToCell(
        startWorld.X,
        startWorld.Y,
        startWorld.Z);

        MapCellCoord endCell = MapGrid.WorldToCell(
        endWorld.X,
        endWorld.Y,
        endWorld.Z);

        int minX = Math.Min(startCell.X, endCell.X);
        int maxX = Math.Max(startCell.X, endCell.X);

        int minY = Math.Min(startCell.Y, endCell.Y);
        int maxY = Math.Max(startCell.Y, endCell.Y);

        int z = startCell.Z; // assume same layer for now

        var cells = new List<MapCellCoord>();

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                cells.Add(new MapCellCoord(x, y, z));
            }
        }

        return cells;
    }

    #endregion
}
