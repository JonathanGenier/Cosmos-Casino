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
    private readonly BuildProcessManager _buildProcessManager;

    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildPreviewFlow class with the specified build context, preview manager, and
    /// cursor manager.
    /// </summary>
    /// <param name="buildContext">The build context to use for managing build-related state and operations. Cannot be null.</param>
    /// <param name="buildPreviewManager">The manager responsible for handling build preview functionality. Cannot be null.</param>
    /// <param name="buildProcessManager">The manager responsible for evaluating build intents and processing build operations. Cannot be null.</param>
    public BuildPreviewFlow(BuildContext buildContext, BuildPreviewManager buildPreviewManager, BuildProcessManager buildProcessManager)
    {
        ArgumentNullException.ThrowIfNull(buildContext);
        ArgumentNullException.ThrowIfNull(buildPreviewManager);
        ArgumentNullException.ThrowIfNull(buildProcessManager);

        _buildContext = buildContext;
        _buildPreviewManager = buildPreviewManager;
        _buildProcessManager = buildProcessManager;

        _buildContext.BuildStarted += OnBuildStarted;
        _buildContext.BuildChanged += OnBuildChanged;
        _buildContext.BuildCleared += OnBuildCleared;
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

        _buildContext.BuildStarted -= OnBuildStarted;
        _buildContext.BuildChanged -= OnBuildChanged;
        _buildContext.BuildCleared -= OnBuildCleared;
        _isDisposed = true;
    }

    #endregion

    #region Event Handlers

    private void OnBuildStarted()
    {
        _buildPreviewManager.EnterDragMode();
        OnBuildChanged();
    }

    /// <summary>
    /// Handles changes to the build preview state and updates the build preview display accordingly.
    /// </summary>
    private void OnBuildChanged()
    {
        if (_buildPreviewManager.CurrentMode != BuildPreviewMode.Drag)
        {
            return;
        }

        var buildIntent = _buildContext.TryCreateBuildIntent();

        if (buildIntent == null)
        {
            _buildPreviewManager.ClearDragPreview();
            return;
        }

        var buildResult = _buildProcessManager.EvaluateBuildIntent(buildIntent);
        _buildPreviewManager.ShowPreview(buildResult);
    }

    private void OnBuildCleared()
    {
        _buildPreviewManager.ExitDragMode();
    }

    #endregion
}