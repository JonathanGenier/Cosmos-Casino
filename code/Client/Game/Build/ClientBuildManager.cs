using CosmosCasino.Core.Build;
using System;

/// <summary>
/// Manages build operations on the client side, coordinating build intents and handling build completion notifications.
/// </summary>
/// <remarks>ClientBuildManager provides an interface for executing build intents through an associated
/// BuildManager and exposes events to notify subscribers when a build operation completes. This class is intended to be
/// initialized with a BuildManager instance before use. It is not thread-safe; callers should ensure appropriate
/// synchronization if accessed from multiple threads.</remarks>
public sealed partial class ClientBuildManager : InitializableNodeManager
{
    #region Fields

    private BuildManager _buildManager;

    #endregion


    #region Events

    /// <summary>
    /// Occurs when a build operation has completed, providing the result of the build process.
    /// </summary>
    /// <remarks>Subscribe to this event to receive notification when a build finishes, regardless of success
    /// or failure. The event provides a BuildResult object containing details about the outcome of the build.</remarks>
    public event Action<BuildResult> BuildCompleted;

    #endregion

    #region Godot Methods

    /// <summary>
    /// Initializes the instance with the specified build manager.
    /// </summary>
    /// <param name="buildManager">The build manager to associate with this instance. Cannot be null.</param>
    public void Initialize(BuildManager buildManager)
    {
        _buildManager = buildManager;
        MarkInitialized();
    }

    #endregion

    #region Build Intent

    /// <summary>
    /// Executes the specified build intent by applying its operations and triggers the build completion event.
    /// </summary>
    /// <remarks>This method applies the operations described in the provided build intent and raises the
    /// BuildCompleted event with the result. Ensure that event handlers for BuildCompleted are registered before
    /// calling this method if you need to respond to build completion.</remarks>
    /// <param name="buildIntent">The build intent that defines the set of operations to apply. Cannot be null.</param>
    public void ExecuteBuildIntent(BuildIntent buildIntent)
    {
        BuildResult buildResult = _buildManager.ApplyBuildOperations(buildIntent);
        BuildCompleted?.Invoke(buildResult);
    }

    #endregion
}