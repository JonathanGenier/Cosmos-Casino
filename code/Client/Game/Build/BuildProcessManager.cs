using CosmosCasino.Core.Game.Build;
using System;

/// <summary>
/// Manages build operations on the client side, coordinating build intents and handling build completion notifications.
/// </summary>
/// <remarks>ClientBuildManager provides an interface for executing build intents through an associated
/// BuildManager and exposes events to notify subscribers when a build operation completes. This class is intended to be
/// initialized with a BuildManager instance before use. It is not thread-safe; callers should ensure appropriate
/// synchronization if accessed from multiple threads.</remarks>
public sealed partial class BuildProcessManager : InitializableNodeManager
{
    #region Fields

    private BuildManager? _buildManager;
    private BuildPreviewManager? buildPreviewManager;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a build operation has completed, providing the result of the build process.
    /// </summary>
    /// <remarks>Subscribe to this event to receive notification when a build finishes, regardless of success
    /// or failure. The event provides a BuildResult object containing details about the outcome of the build.</remarks>
    public event Action<BuildResult>? BuildCompleted;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current instance of the build preview manager used to manage and display build previews within the
    /// application.
    /// </summary>
    public BuildPreviewManager BuildPreviewManager
    {
        get => buildPreviewManager ?? throw new InvalidOperationException($"{nameof(BuildPreviewManager)} has not been initialized.");
        private set => buildPreviewManager = value;
    }

    /// <summary>
    /// Gets or sets the build manager instance used to coordinate build operations.
    /// </summary>
    private BuildManager BuildManager
    {
        get => _buildManager ?? throw new InvalidOperationException($"{nameof(BuildManager)} has not been initialized.");
        set => _buildManager = value;
    }

    #endregion

    #region Build Processes

    /// <summary>
    /// Evaluates the specified build intent and returns the result of the build process.
    /// </summary>
    /// <param name="buildIntent">The build intent to evaluate. Cannot be null.</param>
    /// <returns>A BuildResult object that contains the outcome of evaluating the specified build intent.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the BuildProcessManager is not initialized.</exception>
    public BuildResult EvaluateBuildIntent(BuildIntent buildIntent)
    {
        ArgumentNullException.ThrowIfNull(buildIntent);

        if (!IsInitialized)
        {
            throw new InvalidOperationException($"{nameof(BuildProcessManager)} is not initialized.");
        }

        return BuildManager.Evaluate(buildIntent);
    }

    /// <summary>
    /// Executes the specified build intent by applying its operations and triggers the build completion event.
    /// </summary>
    /// <remarks>This method applies the operations described in the provided build intent and raises the
    /// BuildCompleted event with the result. Ensure that event handlers for BuildCompleted are registered before
    /// calling this method if you need to respond to build completion.</remarks>
    /// <param name="buildIntent">The build intent that defines the set of operations to apply. Cannot be null.</param>
    public void ExecuteBuildIntent(BuildIntent buildIntent)
    {
        ArgumentNullException.ThrowIfNull(buildIntent);

        if (!IsInitialized)
        {
            throw new InvalidOperationException($"{nameof(BuildProcessManager)} is not initialized.");
        }

        BuildResult buildResult = BuildManager.Execute(buildIntent);
        BuildCompleted?.Invoke(buildResult);
    }

    #endregion

    #region Godot Methods

    /// <summary>
    /// Initializes the build system and its associated managers using the specified services.
    /// </summary>
    /// <param name="services">The set of services required to configure the build system, including build management, preview resources, build
    /// context, and cursor management. Cannot be null.</param>
    public void Initialize(BuildProcessServices services)
    {
        ArgumentNullException.ThrowIfNull(services);

        BuildManager = services.BuildManager;
        BuildPreviewManager = new BuildPreviewManager();
        BuildPreviewManager.Initialize(services.PreviewResources);

        MarkInitialized();
    }

    /// <summary>
    /// Initializes the control when it is ready for interaction by adding the preview manager as a child element.
    /// </summary>
    /// <remarks>Override this method to perform setup tasks that require the control to be fully constructed.
    /// This method is called by the framework when the control is ready, and is typically used to add child elements or
    /// initialize state.</remarks>
    protected override void OnReady()
    {
        AddChild(BuildPreviewManager);
    }

    #endregion
}