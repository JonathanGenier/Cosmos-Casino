using System;

/// <summary>
/// Manages the build preview flow, handling user interactions and updating build previews during the building process.`
/// </summary>
/// <remarks>This class coordinates between the build context, preview manager, and cursor manager to provide
/// real-time visual feedback for build placement. It is intended to be used as part of a larger game flow system and
/// should be disposed of when no longer needed to release event subscriptions.</remarks>
public class CursorPreviewFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildContext _buildContext;
    private readonly BuildPreviewManager _buildPreviewManager;
    private readonly CursorManager _cursorManager;
    private readonly BuildProcessManager _buildProcessManager;

    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the CursorPreviewFlow class with the specified build context, preview manager,
    /// cursor manager, and build process manager.
    /// </summary>
    /// <param name="buildContext">The build context that provides information about the current build environment. Cannot be null.</param>
    /// <param name="buildPreviewManager">The manager responsible for handling build previews. Cannot be null.</param>
    /// <param name="cursorManager">The manager that tracks and manages cursor state and events. Cannot be null.</param>
    /// <param name="buildProcessManager">The manager that coordinates the build process. Cannot be null.</param>
    public CursorPreviewFlow(BuildContext buildContext, BuildPreviewManager buildPreviewManager, CursorManager cursorManager, BuildProcessManager buildProcessManager)
    {
        ArgumentNullException.ThrowIfNull(buildContext);
        ArgumentNullException.ThrowIfNull(buildPreviewManager);
        ArgumentNullException.ThrowIfNull(cursorManager);
        ArgumentNullException.ThrowIfNull(buildProcessManager);

        _buildContext = buildContext;
        _buildPreviewManager = buildPreviewManager;
        _cursorManager = cursorManager;
        _buildProcessManager = buildProcessManager;

        _cursorManager.CursorCellChanged += OnCursorCellChanged;
        _cursorManager.CursorContextLost += OnCursorContextLost;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases all resources used by the current instance.
    /// </summary>
    /// <remarks>Call this method when the instance is no longer needed to unsubscribe from events and allow
    /// for proper resource cleanup. After calling this method, further use of the instance may result in undefined
    /// behavior.</remarks>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _cursorManager.CursorCellChanged -= OnCursorCellChanged;
        _cursorManager.CursorContextLost -= OnCursorContextLost;
        _isDisposed = true;
    }

    #endregion

    #region Event Handlers

    private void OnCursorCellChanged(CursorContext cursorContext)
    {
        if (_buildContext.ActiveContext == null)
        {
            _buildPreviewManager.ClearGridAndCursorPreviews();
            return;
        }

        _buildPreviewManager.ShowGridPreview(cursorContext);

        if (_buildPreviewManager.CurrentMode != BuildPreviewMode.Cursor)
        {
            return;
        }

        var buildIntent = _buildContext.TryCreateBuildIntent(cursorContext);

        if (buildIntent == null)
        {
            _buildPreviewManager.ClearCursorPreview();
            return;
        }

        var buildResult = _buildProcessManager.EvaluateBuildIntent(buildIntent);
        _buildPreviewManager.ShowPreview(buildResult);
    }

    private void OnCursorContextLost()
    {
        _buildPreviewManager.ClearGridAndCursorPreviews();
    }

    #endregion
}