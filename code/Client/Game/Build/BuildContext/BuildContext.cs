using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Map.Cell;
using System;
using System.Collections.Generic;

/// <summary>
/// Provides methods and properties for managing the current build context within an application.
/// </summary>
/// <remarks>The BuildContext class allows for setting, retrieving, and clearing the active build context. It is
/// typically used to maintain contextual information relevant to build operations. This class is sealed and cannot be
/// inherited.</remarks>
public sealed class BuildContext
{
    #region Fields

    private BuildContextBase? _activeContext;
    private MapCellCoord? _startCell;
    private MapCellCoord? _currentCell;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a context is activated.
    /// </summary>
    /// <remarks>This event is raised when a BuildContext is set.</remarks>
    public event Action? ContextActivated;

    /// <summary>
    /// Occurs when the current context is deactivated.
    /// </summary>
    /// <remarks>This event is raised when the active BuildContext is cleared.</remarks>
    public event Action? ContextDeactivated;

    /// <summary>
    /// Occurs when a build process is started.
    /// </summary>
    /// <remarks>This event is raised before any build steps are executed.</remarks>
    public event Action? BuildStarted;

    /// <summary>
    /// Occurs when the build state changes.
    /// </summary>
    /// <remarks>Use this event to respond to changes in the build process. The event is raised whenever a relevant 
    /// change in the build occurs.</remarks>
    public event Action? BuildChanged;

    /// <summary>
    /// Occurs when the build process has completed.
    /// </summary>
    /// <remarks>Use this event to perform actions after the build has finished. The event is raised regardless of 
    /// whether the build succeeded or failed.</remarks>
    public event Action? BuildEnded;

    /// <summary>
    /// Occurs when the current build is cleared.
    /// </summary>
    /// <remarks>Use this event to perform cleanup when a build is no longer available. The event is raised after 
    /// the build has been cleared and before a new build is started, if applicable.</remarks>
    public event Action? BuildCleared;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the currently active build context, if one is available.
    /// </summary>
    public BuildContextBase? ActiveContext => _activeContext;

    /// <summary>
    /// Gets a value indicating whether a preview operation is currently active in the context.
    /// </summary>
    public bool IsBuildActive => _activeContext != null && _startCell.HasValue && _currentCell.HasValue;

    #endregion

    #region Context Management

    /// <summary>
    /// Sets the active build context to the specified context instance.
    /// </summary>
    /// <remarks>If an active build context is already set, it will be replaced by the specified context. A
    /// warning is logged when replacing an existing context.</remarks>
    /// <param name="context">The build context to set as the active context. Cannot be null.</param>
    public void SetContext(BuildContextBase context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Clear();
        _activeContext = context;
        ContextActivated?.Invoke();
    }

    #endregion

    #region Build Management

    /// <summary>
    /// Begins a preview operation at the specified cursor context, allowing users to visualize changes before
    /// committing them.
    /// </summary>
    /// <remarks>This method has no effect if there is no active context or if a preview is already in
    /// progress. Typically used to initiate a visual preview in response to user interaction.</remarks>
    /// <param name="start">The cursor context that specifies the starting world position for the preview operation. Cannot be null.</param>
    public void BeginBuild(CursorContext start)
    {
        if (_activeContext == null || _startCell != null)
        {
            return;
        }

        _startCell = MapToWorld.WorldToCell(start.WorldPosition);
        _currentCell = _startCell;
        BuildStarted?.Invoke();
    }

    /// <summary>
    /// Updates the preview state based on the specified cursor context.
    /// </summary>
    /// <remarks>If the preview state changes, the method triggers the PreviewChanged event. No action is
    /// taken if the active context or preview start cell is not set, or if the cursor position has not
    /// changed.</remarks>
    /// <param name="current">The current cursor context containing the world position used to update the preview.</param>
    public void UpdateBuild(CursorContext current)
    {
        if (_activeContext == null || _startCell == null)
        {
            return;
        }

        var currentCell = MapToWorld.WorldToCell(current.WorldPosition);

        if (currentCell == _currentCell)
        {
            return;
        }

        _currentCell = currentCell;
        BuildChanged?.Invoke();
    }

    /// <summary>
    /// Ends the current build operation and finalizes any changes based on the specified cursor context.
    /// </summary>
    /// <remarks>If the cursor has moved to a different cell since the last update, the build change event is
    /// triggered before the build end event. This method has no effect if there is no active build context.</remarks>
    /// <param name="current">The current cursor context containing the world position at the time the build operation ends.</param>
    public void EndBuild(CursorContext current)
    {
        if (_activeContext == null || _startCell == null)
        {
            return;
        }

        var currentCell = MapToWorld.WorldToCell(current.WorldPosition);

        if (currentCell != _currentCell)
        {
            _currentCell = currentCell;
            BuildChanged?.Invoke();
        }

        BuildEnded?.Invoke();
        ClearBuild();
    }

    /// <summary>
    /// Clears the current preview selection and raises the preview changed event if a preview was active.
    /// </summary>
    /// <remarks>Use this method to reset any ongoing preview operation. If no preview is currently active,
    /// the method has no effect. Calling this method triggers the <c>PreviewChanged</c> event only if a preview was
    /// previously set.</remarks>
    public void CancelContext()
    {
        Clear();
    }

    #endregion

    #region Active Context 

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing the current selection preview.
    /// </summary>
    /// <remarks>The returned list reflects the current preview state and may change as the selection is
    /// updated. If the selection preview is not active, the list will be empty.</remarks>
    /// <returns>An <see cref="IReadOnlyList{MapCellCoord}"/> containing the coordinates of the selected cells. Returns an empty
    /// list if no selection preview is active.</returns>
    public IReadOnlyList<MapCellCoord> GetCells()
    {
        if (_activeContext == null || _startCell == null || _currentCell == null)
        {
            return Array.Empty<MapCellCoord>();
        }

        return _activeContext.GetCells(_startCell.Value, _currentCell.Value);
    }

    /// <summary>
    /// Attempts to create a new build intent based on the current context and cell selection.
    /// </summary>
    /// <remarks>Returns <see langword="null"/> if there is no active context or if the required cell
    /// selections are not set.</remarks>
    /// <returns>A <see cref="BuildIntent"/> instance if a build intent can be created; otherwise, <see langword="null"/>.</returns>
    public BuildIntent? TryCreateBuildIntent()
    {
        if (_activeContext == null || _startCell == null || _currentCell == null)
        {
            return null;
        }

        _activeContext.TryCreateBuildIntent(_startCell.Value, _currentCell.Value, out var intent);
        return intent;
    }

    /// <summary>
    /// Attempts to create a build intent based on the specified cursor context.
    /// </summary>
    /// <param name="cursorContext">The cursor context that provides a single cell position and validation state. Must be valid to create 
    /// a build intent.</param>
    /// <returns>A <see cref="BuildIntent"/> representing the build action to perform if the context is valid; otherwise, <see
    /// langword="null"/>.</returns>
    public BuildIntent? TryCreateBuildIntent(CursorContext cursorContext)
    {
        if (!cursorContext.IsValid || _activeContext == null)
        {
            return null;
        }

        var cell = cursorContext.CellPosition;

        _activeContext.TryCreateBuildIntent(cell, cell, out var intent);
        return intent;
    }

    #endregion

    #region State Management

    /// <summary>
    /// Resets the current state by clearing the active context and any associated build data.
    /// </summary>
    private void Clear()
    {
        ClearBuild();

        if (_activeContext == null)
        {
            return;
        }

        _activeContext = null;
        ContextDeactivated?.Invoke();
    }

    /// <summary>
    /// Resets the build state and raises the BuildCleared event.
    /// </summary>
    /// <remarks>Call this method to clear any in-progress build data and notify subscribers that the build
    /// has been cleared. This method is typically used to reset the build process to its initial state.</remarks>
    private void ClearBuild()
    {
        if (_startCell == null && _currentCell == null)
        {
            return;
        }

        _startCell = null;
        _currentCell = null;
        BuildCleared?.Invoke();
    }

    #endregion
}