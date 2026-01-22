using System;

/// <summary>
/// Coordinates input-driven lifecycle behavior for the debug console UI by translating high-level input intent
/// into concrete visibility and focus actions.
/// </summary>
/// <remarks>
/// This flow listens for console-related input signals and manages when the console UI should be shown, hidden,
/// or have its keyboard focus released. Input subscriptions are dynamically enabled only while the console is
/// visible to avoid unintended interactions and reduce unnecessary signal handling.
/// </remarks>
public sealed class ConsoleInputFlow : IAppFlow, IDisposable
{
    #region Fields

    private ConsoleUiManager _consoleUiManager;
    private InputManager _inputManager;

    private bool _isSubscribed = false;
    private bool _isDisposed = false;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleInputFlow"/> class and wires the initial console toggle input.
    /// </summary>
    /// <param name="consoleUiManager">
    /// The manager responsible for displaying and controlling the console user interface.
    /// </param>
    /// <param name="inputManager">
    /// The input manager that emits console-related intent signals.
    /// </param>
    public ConsoleInputFlow(ConsoleUiManager consoleUiManager, InputManager inputManager)
    {
        ArgumentNullException.ThrowIfNull(consoleUiManager);
        ArgumentNullException.ThrowIfNull(inputManager);

        _consoleUiManager = consoleUiManager;
        _inputManager = inputManager;
        _inputManager.ToggleConsoleUi += OnToggleConsoleUi;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases all input subscriptions managed by this flow and prevents further event handling.
    /// </summary>
    /// <remarks>
    /// This method is safe to call multiple times. Once disposed, the flow detaches from all input signals
    /// and will no longer react to console-related input events.
    /// </remarks>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _inputManager.ToggleConsoleUi -= OnToggleConsoleUi;
        UnsubscribeToConsoleInput();
        _isDisposed = true;
    }

    #endregion

    #region Event Handler

    /// <summary>
    /// Toggles the visibility of the console UI and manages related input subscriptions accordingly.
    /// </summary>
    /// <remarks>
    /// When the console is shown, additional input signals are subscribed to handle focus release and hiding.
    /// When hidden, those subscriptions are removed to prevent unintended interactions.
    /// </remarks>
    private void OnToggleConsoleUi()
    {
        if (!_consoleUiManager.IsConsoleUiVisible)
        {
            _consoleUiManager.ShowConsoleUi();
            SubscribeToConsoleInput();
        }
        else
        {
            _consoleUiManager.HideConsoleUi();
            UnsubscribeToConsoleInput();
        }
    }

    /// <summary>
    /// Forces the console UI to release keyboard focus when gameplay input is detected.
    /// </summary>
    /// <remarks>
    /// This handler is only active while the console is visible and is used to ensure that gameplay input
    /// does not conflict with text entry within the console.
    /// </remarks>
    private void OnForceConsoleUiToReleaseFocus()
    {
        if (_consoleUiManager.IsConsoleUiVisible)
        {
            _consoleUiManager.ForceToReleaseFocus();
        }
    }

    /// <summary>
    /// Hides the console UI in response to an explicit hide request, such as an escape key action.
    /// </summary>
    private void OnForceConsoleUiToHide()
    {
        if (_consoleUiManager.IsConsoleUiVisible)
        {
            _consoleUiManager.HideConsoleUi();
        }
    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// Subscribes to console-specific input signals that are only relevant while the console is visible.
    /// </summary>
    /// <remarks>
    /// This method guards against duplicate subscriptions and ensures that console-related input handling
    /// is active only when necessary.
    /// </remarks>
    private void SubscribeToConsoleInput()
    {
        if (_isSubscribed)
        {
            return;
        }

        _inputManager.ForceConsoleUiToReleaseFocus += OnForceConsoleUiToReleaseFocus;
        _inputManager.ForceConsoleUiToHide += OnForceConsoleUiToHide;
        _isSubscribed = true;
    }

    /// <summary>
    /// Unsubscribes from console-specific input signals that are no longer needed.
    /// </summary>
    /// <remarks>
    /// This method is called when the console is hidden or the flow is disposed, ensuring that no stale
    /// event handlers remain attached.
    /// </remarks>
    private void UnsubscribeToConsoleInput()
    {
        if (!_isSubscribed)
        {
            return;
        }

        _inputManager.ForceConsoleUiToReleaseFocus -= OnForceConsoleUiToReleaseFocus;
        _inputManager.ForceConsoleUiToHide -= OnForceConsoleUiToHide;
        _isSubscribed = false;
    }

    #endregion
}