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

    private readonly BuildProcessManager _clientBuildManager;
    private readonly BuildContext _buildContext;

    private bool _isDisposed;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the BuildRequestFlow class with the specified build process manager and build
    /// context.
    /// </summary>
    /// <param name="clientBuildManager">The build process manager used to coordinate and manage build operations. Cannot be null.</param>
    /// <param name="buildContext">The build context that provides information and state for the build process. Cannot be null.</param>
    public BuildRequestFlow(BuildProcessManager clientBuildManager, BuildContext buildContext)
    {
        ArgumentNullException.ThrowIfNull(clientBuildManager);
        ArgumentNullException.ThrowIfNull(buildContext);

        _clientBuildManager = clientBuildManager;
        _buildContext = buildContext;

        _buildContext.BuildEnded += OnBuildEnded;
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

        _buildContext.BuildEnded -= OnBuildEnded;
        _isDisposed = true;
    }

    #endregion

    #region BuildRequest Methods

    private void OnBuildEnded()
    {
        var buildIntent = _buildContext.TryCreateBuildIntent();

        if (buildIntent == null)
        {
            throw new InvalidOperationException("Build intent should be available when build ends.");
        }

        _clientBuildManager.ExecuteBuildIntent(buildIntent);
    }

    #endregion
}