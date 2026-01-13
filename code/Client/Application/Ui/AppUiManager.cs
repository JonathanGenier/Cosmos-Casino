using CosmosCasino.Core.Application.Console;

/// <summary>
/// Manages the application's user interface components and coordinates the initialization and attachment of UI
/// controllers to the scene tree.
/// </summary>
/// <remarks>AppUiManager is responsible for creating and managing long-lived UI controllers, such as the console
/// UI, and ensuring they are properly initialized and added to the application's scene hierarchy. This class should be
/// initialized before interacting with any managed UI components. It is typically used as a central point for
/// UI-related operations within the application.</remarks>
public sealed partial class AppUiManager : InitializableNodeManager
{
    #region FIELDS

    private ConsoleUiManager _consoleUiManager;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the component with the specified input and console managers.
    /// </summary>
    /// <param name="inputManager">The input manager to be used for handling user input during initialization. Cannot be null.</param>
    /// <param name="consoleManager">The console manager to be used for managing console operations during initialization. Cannot be null.</param>
    public void Initialize(InputManager inputManager, ConsoleManager consoleManager)
    {
        _consoleUiManager = CreateInitializableNode<ConsoleUiManager>(
            cum => cum.Initialize(inputManager, consoleManager));
        MarkInitialized();
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Initializes the node when it enters the scene tree. This method is called by the engine to perform setup tasks
    /// specific to this node.
    /// </summary>
    /// <remarks>Override this method to add initialization logic that should run when the node is ready. This
    /// method is called once during the node's lifecycle, after it and its children have entered the scene
    /// tree.</remarks>
    protected override void OnReady()
    {
        using (ConsoleLog.SystemScope(nameof(AppUiManager)))
        {
            AddChild(_consoleUiManager);
        }
    }

    #endregion
}