using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map.Cell;
using System;

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
    private BuildOperation _currentBuildOperation;
    private BuildInteractionMode _currentBuildInteractionMode;

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
    /// Begins a new build operation at the specified starting context using the provided build operation.
    /// </summary>
    /// <remarks>If a build is already in progress or the starting cell is already set, this method does
    /// nothing. The build operation is initiated at the cell corresponding to the provided world position, and any
    /// registered build start event handlers are invoked.</remarks>
    /// <param name="start">The context representing the starting position for the build operation. Must specify a valid world position.</param>
    /// <param name="buildOperation">The build operation to perform. Determines the type of build action that will be initiated.</param>
    public void BeginBuild(CursorContext start, BuildOperation buildOperation)
    {
        if (_activeContext == null || _startCell != null)
        {
            return;
        }

        _startCell = MapToWorld.WorldToCell(start.WorldPosition);
        _currentCell = _startCell;
        _currentBuildOperation = buildOperation;
        BuildStarted?.Invoke();
    }

    /// <summary>
    /// Updates the active build preview based on the current cursor position and interaction mode.
    /// </summary>
    /// <remarks>
    /// This method refreshes the build state when either the cursor enters a different map cell
    /// or the build interaction mode changes (for example, due to modifier input).
    /// If neither the cell position nor the interaction mode has changed, no update is performed.
    /// </remarks>
    /// <param name="current">
    /// The current cursor context containing the world position used to determine the active map cell.
    /// </param>
    /// <param name="buildInteractionMode">
    /// The current interaction mode that influences how the build preview is interpreted and generated.
    /// </param>
    public void UpdateBuild(CursorContext current, BuildInteractionMode buildInteractionMode)
    {
        if (_activeContext == null || _startCell == null)
        {
            return;
        }

        var newCell = MapToWorld.WorldToCell(current.WorldPosition);
        bool cellChanged = newCell != _currentCell;
        bool interactionChanged = SetBuildInteractionMode(buildInteractionMode);

        if (!cellChanged && !interactionChanged)
        {
            return;
        }

        _currentCell = newCell;
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

    /// <summary>
    /// Cancels the current build operation and resets the build state.
    /// </summary>
    /// <remarks>Call this method to abort an in-progress build and clear any associated resources or state.
    /// After calling this method, the build cannot be resumed and must be started again if needed.</remarks>
    public void CancelBuild()
    {
        ClearBuild();
    }

    #endregion

    #region Active Context 

    /// <summary>
    /// Attempts to create a new build intent based on the current context and cell selection.
    /// </summary>
    /// <remarks>Returns <see langword="null"/> if there is no active context or if the required cell
    /// selections are not set.</remarks>
    /// <returns>A <see cref="BuildIntent"/> instance if a build intent can be created; otherwise, <see langword="null"/>.</returns>
    public BuildIntent? TryCreateBuildIntent()
    {
        if (_activeContext == null
            || _startCell == null
            || _currentCell == null
            || _currentBuildOperation == BuildOperation.None)
        {
            return null;
        }

        _activeContext.TryCreateBuildIntent(
            _startCell.Value,
            _currentCell.Value,
            _currentBuildOperation,
            _currentBuildInteractionMode,
            out var intent);

        return intent;
    }

    /// <summary>
    /// Attempts to create a build intent based on the specified cursor context.
    /// </summary>
    /// <param name="cursorContext">The cursor context that provides a single cell position and validation state. Must be valid to create 
    /// a build intent.</param>
    /// <returns>A <see cref="BuildIntent"/> representing the build action to perform if the context is valid; otherwise, <see
    /// langword="null"/>.</returns>
    public BuildIntent? TryCreateCursorPreviewIntent(CursorContext cursorContext)
    {
        if (!cursorContext.IsValid || _activeContext == null)
        {
            return null;
        }

        var cell = cursorContext.CellPosition;

        _activeContext.TryCreateBuildIntent(
            cell,
            cell,
            BuildOperation.Place,
            BuildInteractionMode.Default,
            out var intent);

        return intent;
    }

    #endregion

    #region State Management

    private bool SetBuildInteractionMode(BuildInteractionMode buildInteractionMode)
    {
        if (_currentBuildInteractionMode == buildInteractionMode)
        {
            return false;
        }

        _currentBuildInteractionMode = buildInteractionMode;
        return true;
    }

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
        if (_startCell == null && _currentCell == null && _currentBuildOperation == BuildOperation.None)
        {
            return;
        }

        _startCell = null;
        _currentCell = null;
        _currentBuildOperation = BuildOperation.None;
        _currentBuildInteractionMode = BuildInteractionMode.Default;
        BuildCleared?.Invoke();
    }

    #endregion
}