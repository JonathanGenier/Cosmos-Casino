using System;

/// <summary>
/// Coordinates input handling for build operations by subscribing to relevant input and cursor events within a build
/// context.
/// </summary>
public sealed class BuildInputFlow : IDisposable
{
    #region Fields

    private readonly InputManager _inputManager;
    private readonly CursorManager _cursorManager;
    private readonly BuildContext _buildContext;

    private bool _isPrimaryHeld;
    private bool _isSubscribed;
    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the BuildInputFlow class with the specified input, cursor, and build context
    /// managers.
    /// </summary>
    /// <param name="inputManager">The input manager responsible for handling user input during the build flow. Cannot be null.</param>
    /// <param name="cursorManager">The cursor manager used to control cursor behavior within the build context. Cannot be null.</param>
    /// <param name="buildContext">The build context that manages the state and events for the build flow. Cannot be null.</param>
    public BuildInputFlow(InputManager inputManager, CursorManager cursorManager, BuildContext buildContext)
    {
        ArgumentNullException.ThrowIfNull(inputManager);
        ArgumentNullException.ThrowIfNull(cursorManager);
        ArgumentNullException.ThrowIfNull(buildContext);

        _inputManager = inputManager;
        _cursorManager = cursorManager;
        _buildContext = buildContext;

        _buildContext.ContextActivated += OnBuildContextActivated;
        _buildContext.ContextDeactivated += OnBuildContextDeactivated;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases all resources used by the instance and unsubscribes from build context events.
    /// </summary>
    /// <remarks>Call this method when the instance is no longer needed to ensure proper cleanup of event
    /// subscriptions and resources. After calling <see cref="Dispose"/>, the instance should not be used.</remarks>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        OnBuildContextDeactivated();
        _buildContext.ContextActivated -= OnBuildContextActivated;
        _buildContext.ContextDeactivated -= OnBuildContextDeactivated;
        _isDisposed = true;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Subscribes to input and cursor events required for build context activation if not already subscribed.
    /// </summary>
    /// <remarks>This method ensures that event handlers for build-related input and cursor changes are
    /// registered only once per activation. Subsequent calls have no effect if subscriptions are already
    /// active.</remarks>
    private void OnBuildContextActivated()
    {
        if (_isSubscribed)
        {
            return;
        }

        _inputManager.BuildPrimaryPressed += OnBuildPrimaryPressed;
        _inputManager.BuildPrimaryReleased += OnBuildPrimaryReleased;
        _cursorManager.CursorCellChanged += OnCursorCellChanged;
        _isSubscribed = true;
    }

    /// <summary>
    /// Handles cleanup when the build context is deactivated by unsubscribing from input and cursor events.
    /// </summary>
    /// <remarks>Call this method when the build context is no longer active to ensure that event handlers are
    /// properly removed and internal state is reset. This helps prevent unintended input handling and potential memory
    /// leaks.</remarks>
    private void OnBuildContextDeactivated()
    {
        if (!_isSubscribed)
        {
            return;
        }

        _inputManager.BuildPrimaryPressed -= OnBuildPrimaryPressed;
        _inputManager.BuildPrimaryReleased -= OnBuildPrimaryReleased;
        _cursorManager.CursorCellChanged -= OnCursorCellChanged;
        _isPrimaryHeld = false;
        _isSubscribed = false;
    }

    /// <summary>
    /// Initiates the build process when the primary build input is pressed, using the current cursor context if
    /// available.
    /// </summary>
    private void OnBuildPrimaryPressed()
    {
        if (!_cursorManager.TryGetCursorContext(out var startContext))
        {
            return;
        }

        _isPrimaryHeld = true;
        _buildContext.BeginBuild(startContext);
    }

    /// <summary>
    /// Handles updates when the cursor cell changes during a primary action.
    /// </summary>
    /// <param name="current">The current cursor context representing the new cell position.</param>
    private void OnCursorCellChanged(CursorContext current)
    {
        if (!_isPrimaryHeld)
        {
            return;
        }

        _buildContext.UpdateBuild(current);
    }

    /// <summary>
    /// Handles the release of the primary build action, finalizing the build process if it is currently active.
    /// </summary>
    private void OnBuildPrimaryReleased()
    {
        if (!_isPrimaryHeld)
        {
            return;
        }

        if (!_cursorManager.TryGetCursorContext(out var endContext))
        {
            return;
        }

        _isPrimaryHeld = false;
        _buildContext.EndBuild(endContext);
    }

    /// <summary>
    /// Cancels the current build operation.
    /// </summary>
    private void OnCancel()
    {
        _buildContext.CancelContext();
    }

    #endregion
}