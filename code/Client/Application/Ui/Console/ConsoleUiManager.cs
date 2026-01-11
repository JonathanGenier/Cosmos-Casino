using CosmosCasino.Core.Console;
using CosmosCasino.Core.Console.Logging;
using Godot;

/// <summary>
/// Manages the initialization and lifecycle of the client-side debug console UI, coordinating input handling and
/// integration with the core console manager.
/// </summary>
/// <remarks>This class is responsible for attaching the debug console UI to the application, wiring it to receive
/// input events for toggling visibility, and ensuring proper cleanup of resources when the console is no longer needed.
/// It should be initialized with the required input and console managers before use. This type is not intended to be
/// inherited.</remarks>
public sealed partial class ConsoleUiManager : InitializableNodeManager
{
    #region FIELDS

    private InputManager _inputManager;
    private ConsoleManager _consoleManager;
    private ConsoleUi _consoleUi;
    private ConsoleAdapter _consoleAdapter;

    #endregion

    #region METHODS

    /// <summary>
    /// Initializes the component with the specified input and console managers.
    /// </summary>
    /// <param name="inputManager">The input manager to be used for handling user input. Cannot be null.</param>
    /// <param name="consoleManager">The console manager to be used for console operations. Cannot be null.</param>
    public void Initialize(InputManager inputManager, ConsoleManager consoleManager)
    {
        _inputManager = inputManager;
        _consoleManager = consoleManager;
        MarkInitialized();
    }

    /// <summary>
    /// Initializes the console UI components and subscribes to input events when the node is ready.
    /// </summary>
    /// <remarks>This method is called by the engine when the node enters the scene tree. It sets up the
    /// console UI, adapter, and event handlers required for console functionality. Override this method to perform
    /// additional initialization when extending this class.</remarks>
    protected override void OnReady()
    {
        using (ConsoleLog.SystemScope(nameof(ConsoleUiManager)))
        {
            _consoleUi = AddNode(GD.Load<PackedScene>(AppUiPaths.Console).Instantiate<ConsoleUi>());
            _consoleUi.Toggle();
            _consoleAdapter = new ConsoleAdapter(_consoleUi, _consoleManager);

            _inputManager.ToggleConsoleUi += OnToggleConsoleUi;
        }
    }

    /// <summary>
    /// Performs cleanup operations when the application is exiting.
    /// </summary>
    /// <remarks>This method is called during the application's shutdown sequence to release resources and
    /// detach event handlers. Override this method to implement additional cleanup logic if necessary.</remarks>
    protected override void OnExit()
    {
        _inputManager.ToggleConsoleUi -= OnToggleConsoleUi;
        _consoleAdapter.Dispose();
    }

    /// <summary>
    /// Toggles the visibility of the console user interface.
    /// </summary>
    private void OnToggleConsoleUi()
    {
        _consoleUi.Toggle();
    }

    #endregion
}
