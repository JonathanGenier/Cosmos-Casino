using CosmosCasino.Core.Build;
using CosmosCasino.Core.Map.Cell;
using CosmosCasino.Core.Map.Grid;
using Godot;
using System;
using System.Collections.Generic;

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
    private ClientBuildManager _clientBuildManager;

    #endregion

    #region CONSTRUCTOR

    /// <summary>
    /// Initializes a new interaction handler responsible for translating
    /// player input into build-related actions using the provided client
    /// build manager and active build context.
    /// </summary>
    /// <param name="clientBuildManager">
    /// Client-side build manager used to issue build requests.
    /// </param>
    /// <param name="buildContext">
    /// Context describing the current build mode, selection, and targeting state.
    /// </param>
    public BuildInteractionHandler(ClientBuildManager clientBuildManager, BuildContext buildContext)
    {
        _clientBuildManager = clientBuildManager;
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
        _clientBuildManager.ExecuteBuildIntent(intent);
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

    private static IReadOnlyList<MapCellCoord> GetCellsBetween(
    Vector3 startWorld,
    Vector3 endWorld)
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
