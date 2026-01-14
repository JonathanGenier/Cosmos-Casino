using CosmosCasino.Core.Game.Build;
using System;

/// <summary>
/// Coordinates the process of handling build requests within a game flow, managing interactions and delegating build
/// operations to the appropriate client manager.
/// </summary>
/// <remarks>This class subscribes to build request events from an interaction manager and forwards build intents
/// to a client build manager for execution. It implements IDisposable to ensure event handlers are properly
/// unsubscribed and resources are released when the flow is disposed. Instances of this class are not
/// thread-safe.</remarks>
public class BuildRequestFlow : IGameFlow, IDisposable
{
    #region Fields

    private readonly InteractionManager _interactionManager;
    private readonly BuildProcessManager _clientBuildManager;
    private bool _isDisposed;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the BuildRequestFlow class with the specified interaction and build managers.
    /// </summary>
    /// <param name="interactionManager">The InteractionManager instance used to handle user interactions and build requests. Cannot be null.</param>
    /// <param name="clientBuildManager">The ClientBuildManager instance responsible for managing build operations. Cannot be null.</param>
    public BuildRequestFlow(InteractionManager interactionManager, BuildProcessManager clientBuildManager)
    {
        ArgumentNullException.ThrowIfNull(interactionManager);
        ArgumentNullException.ThrowIfNull(clientBuildManager);

        _interactionManager = interactionManager;
        _clientBuildManager = clientBuildManager;

        _interactionManager.BuildRequested += OnBuildRequested;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>Call this method when you are finished using the object to release any resources it holds.
    /// After calling Dispose, the object should not be used.</remarks>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _interactionManager.BuildRequested -= OnBuildRequested;
    }

    #endregion

    #region BuildRequest Methods

    /// <summary>
    /// Handles a build request by executing the specified build intent.
    /// </summary>
    /// <param name="buildIntent">The build intent that defines the parameters and actions for the build operation. Cannot be null.</param>
    private void OnBuildRequested(BuildIntent buildIntent)
    {
        _clientBuildManager.ExecuteBuildIntent(buildIntent);
    }

    #endregion
}