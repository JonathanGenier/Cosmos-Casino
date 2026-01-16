using System;

/// <summary>
/// Manages the build preview flow, handling user interactions and updating build previews during the building process.`
/// </summary>
/// <remarks>This class coordinates between the build context, preview manager, and cursor manager to provide
/// real-time visual feedback for build placement. It is intended to be used as part of a larger game flow system and
/// should be disposed of when no longer needed to release event subscriptions.</remarks>
public class BuildPreviewFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly BuildContext _buildContext;
    private readonly BuildPreviewManager _buildPreviewManager;
    private readonly CursorManager _cursorManager;

    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildPreviewFlow class with the specified build context, preview manager, and
    /// cursor manager.
    /// </summary>
    /// <param name="buildContext">The build context to use for managing build-related state and operations. Cannot be null.</param>
    /// <param name="buildPreviewManager">The manager responsible for handling build preview functionality. Cannot be null.</param>
    /// <param name="cursorManager">The cursor manager used to control cursor behavior during the build preview flow. Cannot be null.</param>
    public BuildPreviewFlow(
      BuildContext buildContext,
      BuildPreviewManager buildPreviewManager,
      CursorManager cursorManager)
    {
        ArgumentNullException.ThrowIfNull(buildContext);
        ArgumentNullException.ThrowIfNull(buildPreviewManager);
        ArgumentNullException.ThrowIfNull(cursorManager);

        _buildContext = buildContext;
        _buildPreviewManager = buildPreviewManager;
        _cursorManager = cursorManager;

        Subscribe();
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Updates the build preview based on the current cursor position and active build context.
    /// </summary>
    /// <remarks>Call this method once per frame to ensure that the build preview accurately reflects the
    /// user's current cursor position and the active build context. If there is no active build context, the method
    /// does nothing. If the cursor position cannot be determined, all build previews are hidden.</remarks>
    public void Process()
    {
        if (_buildContext.ActiveContext == null)
        {
            return;
        }

        if (_cursorManager.TryGetCursorPosition(out var worldPos))
        {
            _buildPreviewManager.ShowPreview(_buildContext.ActiveContext.Kind, worldPos);
        }
        else
        {
            _buildPreviewManager.HideAllPreviews();
        }
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases all resources used by the current instance.
    /// </summary>
    /// <remarks>Call this method when the instance is no longer needed to unsubscribe from events and release
    /// resources. After calling this method, further operations on the instance may result in an exception or undefined
    /// behavior.</remarks>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        Unsubscribe();
        _isDisposed = true;
    }

    #endregion

    #region Subscription

    /// <summary>
    /// Subscribes to context and preview change events on the build context.
    /// </summary>
    /// <remarks>This method enables the current instance to respond to changes in the build context by
    /// attaching event handlers. It should be called when event notifications are required and unsubscribed
    /// appropriately to avoid memory leaks.</remarks>
    private void Subscribe()
    {
        _buildContext.ContextChanged += OnBuildContextChanged;
        _buildContext.PreviewChanged += OnPreviewChanged;
    }

    /// <summary>
    /// Unsubscribes from build context and preview change notifications to stop receiving related events.
    /// </summary>
    /// <remarks>Call this method to detach event handlers when they are no longer needed, such as during
    /// cleanup or disposal, to prevent memory leaks and unintended event processing.</remarks>
    private void Unsubscribe()
    {
        _buildContext.ContextChanged -= OnBuildContextChanged;
        _buildContext.PreviewChanged -= OnPreviewChanged;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles changes to the build context by updating the build preview state.
    /// </summary>
    /// <param name="buildContext">The new build context. If <see langword="null"/>, all build previews are hidden and cleared.</param>
    private void OnBuildContextChanged(BuildContextBase? buildContext)
    {
        if (buildContext == null)
        {
            _buildPreviewManager.HideAllPreviews();
            _buildPreviewManager.ClearBuildPreview();
        }
    }

    /// <summary>
    /// Handles changes to the build preview state and updates the build preview display accordingly.
    /// </summary>
    private void OnPreviewChanged()
    {
        if (!_buildContext.HasPreview || _buildContext.ActiveContext == null)
        {
            _buildPreviewManager.ClearBuildPreview();
            return;
        }

        var cells = _buildContext.GetCells();

        _buildPreviewManager.ShowBuildPreview(_buildContext.ActiveContext.Kind, cells);
    }

    #endregion
}