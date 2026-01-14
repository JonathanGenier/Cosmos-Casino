using System;

/// <summary>
/// Manages the user interface components and initialization sequence for the game, including the build UI subsystem.
/// </summary>
/// <remarks>GameUiManager coordinates the setup and readiness of UI elements within the game scene. It ensures
/// that dependent UI managers, such as BuildUiManager, are properly initialized and added to the scene tree. The
/// UiReady event is raised when the UI is fully initialized and ready for interaction. This class should be initialized
/// before use by calling Initialize().</remarks>
public partial class GameUiManager : InitializableNodeManager
{
    #region Fields

    private BuildUiManager? _buildUiManager;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the manager responsible for build-related user interface operations.
    /// </summary>
    public BuildUiManager BuildUiManager
    {
        get => _buildUiManager ?? throw new InvalidOperationException($"{nameof(BuildUiManager)} is not initialized.");
        private set => _buildUiManager = value;
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Initializes the component and prepares it for use.
    /// </summary>
    /// <remarks>Call this method before performing any operations that depend on the component being
    /// initialized. This method should only be called once during the component's lifecycle.</remarks>
    public void Initialize()
    {
        BuildUiManager = CreateInitializableNode<BuildUiManager>(bm => bm.Initialize());
        MarkInitialized();
    }

    /// <summary>
    /// Performs initialization logic when the node is ready and adds the UI manager to the scene tree.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the UI manager has not been initialized before the node enters the scene tree.</exception>
    protected override void OnReady()
    {
        AddChild(BuildUiManager);
    }

    #endregion
}