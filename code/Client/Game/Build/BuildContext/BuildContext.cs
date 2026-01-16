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

    private MapCellCoord? _previewStartCell;
    private MapCellCoord? _previewCurrentCell;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the build context changes.
    /// </summary>
    /// <remarks>Subscribers are notified whenever the associated build context is updated or replaced. The
    /// event handler receives the new build context, or <see langword="null"/> if the context is cleared.</remarks>
    public event Action<BuildContextBase?>? ContextChanged;

    /// <summary>
    /// Occurs when the preview state changes, allowing subscribers to respond to updates before the change is
    /// finalized.
    /// </summary>
    public event Action? PreviewChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the currently active build context, if one is available.
    /// </summary>
    public BuildContextBase? ActiveContext => _activeContext;

    /// <summary>
    /// Gets a value indicating whether a preview operation is currently active in the context.
    /// </summary>
    public bool HasPreview => _activeContext != null && _previewStartCell.HasValue && _previewCurrentCell.HasValue;

    #endregion

    #region Context Management

    /// <summary>
    /// Sets the active build context to the specified context instance.
    /// </summary>
    /// <remarks>If an active build context is already set, it will be replaced by the specified context. A
    /// warning is logged when replacing an existing context.</remarks>
    /// <param name="context">The build context to set as the active context. Cannot be null.</param>
    public void Set(BuildContextBase context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _activeContext = context;
        ClearPreview();
        ContextChanged?.Invoke(_activeContext);
    }

    /// <summary>
    /// Clears the current active context, resetting it to its default state.
    /// </summary>
    public void Clear()
    {
        _activeContext = null;
        ClearPreview();
        ContextChanged?.Invoke(null);
    }

    #endregion

    #region Preview Managemenet

    /// <summary>
    /// Returns a read-only list of map cell coordinates representing the current selection preview.
    /// </summary>
    /// <remarks>The returned list reflects the current preview state and may change as the selection is
    /// updated. If the selection preview is not active, the list will be empty.</remarks>
    /// <returns>An <see cref="IReadOnlyList{MapCellCoord}"/> containing the coordinates of the selected cells. Returns an empty
    /// list if no selection preview is active.</returns>
    public IReadOnlyList<MapCellCoord> GetCells()
    {
        if (_activeContext == null || _previewStartCell == null || _previewCurrentCell == null)
        {
            return Array.Empty<MapCellCoord>();
        }

        return _activeContext.GetCells(_previewStartCell.Value, _previewCurrentCell.Value);
    }

    /// <summary>
    /// Begins a preview operation at the specified cursor context, allowing users to visualize changes before
    /// committing them.
    /// </summary>
    /// <remarks>This method has no effect if there is no active context or if a preview is already in
    /// progress. Typically used to initiate a visual preview in response to user interaction.</remarks>
    /// <param name="start">The cursor context that specifies the starting world position for the preview operation. Cannot be null.</param>
    public void BeginPreview(CursorContext start)
    {
        if (_activeContext == null || _previewStartCell != null)
        {
            return;
        }

        _previewStartCell = MapToWorld.WorldToCell(start.WorldPosition);
        _previewCurrentCell = _previewStartCell;

        PreviewChanged?.Invoke();
    }

    /// <summary>
    /// Updates the preview state based on the specified cursor context.
    /// </summary>
    /// <remarks>If the preview state changes, the method triggers the PreviewChanged event. No action is
    /// taken if the active context or preview start cell is not set, or if the cursor position has not
    /// changed.</remarks>
    /// <param name="current">The current cursor context containing the world position used to update the preview.</param>
    public void UpdatePreview(CursorContext current)
    {
        if (_activeContext == null || _previewStartCell == null)
        {
            return;
        }

        var currentCell = MapToWorld.WorldToCell(current.WorldPosition);

        if (currentCell == _previewCurrentCell)
        {
            return;
        }

        _previewCurrentCell = currentCell;
        PreviewChanged?.Invoke();
    }

    /// <summary>
    /// Clears the current preview selection and raises the preview changed event if a preview was active.
    /// </summary>
    /// <remarks>Use this method to reset any ongoing preview operation. If no preview is currently active,
    /// the method has no effect. Calling this method triggers the <c>PreviewChanged</c> event only if a preview was
    /// previously set.</remarks>
    public void ClearPreview()
    {
        if (_previewStartCell == null && _previewCurrentCell == null)
        {
            return;
        }

        _previewStartCell = null;
        _previewCurrentCell = null;
        PreviewChanged?.Invoke();
    }

    #endregion
}