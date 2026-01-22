using CosmosCasino.Core.Application.Console;
using Godot;
using System;

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

    private ConsoleManager? _consoleManager;
    private ConsoleUi? _consoleUi;
    private ConsoleAdapter? _consoleAdapter;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the console user interface is currently visible.
    /// </summary>
    public bool IsConsoleUiVisible => ConsoleUi.Visible;

    private ConsoleManager ConsoleManager
    {
        get => _consoleManager ?? throw new InvalidOperationException($"{nameof(ConsoleManager)} has not been initialized.");
        set => _consoleManager = value;
    }

    private ConsoleUi ConsoleUi
    {
        get => _consoleUi ?? throw new InvalidOperationException($"{nameof(ConsoleUi)} has not been initialized.");
        set => _consoleUi = value;
    }

    private ConsoleAdapter ConsoleAdapter
    {
        get => _consoleAdapter ?? throw new InvalidOperationException($"{nameof(ConsoleAdapter)} has not been initialized.");
        set => _consoleAdapter = value;
    }

    #endregion

    #region Public API

    /// <summary>
    /// Initializes the component with the specified input and console managers.
    /// </summary>
    /// <param name="consoleManager">The console manager to be used for console operations. Cannot be null.</param>
    public void Initialize(ConsoleManager consoleManager)
    {
        ConsoleManager = consoleManager;
        MarkInitialized();
    }

    /// <summary>
    /// Displays the console user interface for the application.
    /// </summary>
    public void ShowConsoleUi()
    {
        ConsoleUi.ShowConsoleUi();
    }

    /// <summary>
    /// Hides the console user interface, preventing it from being displayed to the user.
    /// </summary>
    public void HideConsoleUi()
    {
        ConsoleUi.HideConsoleUi();
    }

    /// <summary>
    /// Forces the current UI element to release input focus immediately.
    /// </summary>
    /// <remarks>Call this method to programmatically remove focus from the active UI element, which may be
    /// necessary in scenarios where focus management is not handled automatically. This method has no effect if no
    /// element currently holds focus.</remarks>
    public void ForceToReleaseFocus()
    {
        ConsoleUi.ForceReleaseFocus();
    }

    #endregion

    #region Godot Processes

    /// <summary>
    /// Initializes the console UI components and subscribes to input events when the node is ready.
    /// </summary>
    /// <remarks>This method is called by the engine when the node enters the scene tree. It sets up the
    /// console UI, adapter, and event handlers required for console functionality. Override this method to perform
    /// additional initialization when extending this class.</remarks>
    protected override void OnReady()
    {
        ConsoleUi = AddNode(GD.Load<PackedScene>(AppUiPaths.Console).Instantiate<ConsoleUi>());
        ConsoleUi.HideConsoleUi();
        ConsoleAdapter = new ConsoleAdapter(ConsoleUi, ConsoleManager);
    }

    /// <summary>
    /// Performs cleanup operations when the application is exiting.
    /// </summary>
    /// <remarks>This method is called during the application's shutdown sequence to release resources and
    /// detach event handlers. Override this method to implement additional cleanup logic if necessary.</remarks>
    protected override void OnExit()
    {
        ConsoleAdapter?.Dispose();
    }

    #endregion
}
