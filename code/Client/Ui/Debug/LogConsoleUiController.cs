using CosmosCasino.Core.Debug.Logging;
using Godot;

/// <summary>
/// Controller for the debug log console user interface.
/// Owns the behavior of the log console presentation, including visibility
/// toggling, log entry display, and scroll management.
/// This controller is UI-only and does not handle input detection or logging
/// logic directly; it reacts to coordination decisions made by higher-level
/// systems such as <see cref="UiManager"/>.
/// </summary>
public partial class LogConsoleUiController : Control
{
    #region NODES

    [Export]
    private RichTextLabel _textLog;

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Called when the log console UI node enters the scene tree and is ready.
    /// Used to perform final UI-side initialization and emit diagnostic
    /// readiness information for debugging purposes.
    /// </summary>
    public override void _Ready()
    {
        DevLog.System("LogConsoleUiController", "Ready");
    }

    /// <summary>
    /// Toggles the visibility of the log console.
    /// This method updates the presentation state only and does not
    /// perform any input handling or coordination logic.
    /// </summary>
    public void Toggle()
    {
        Visible = !Visible;
    }

    /// <summary>
    /// Appends a formatted log entry to the log console display.
    /// This method updates the visual text buffer only and does not
    /// perform any formatting, filtering, or scrolling logic.
    /// </summary>
    /// <param name="log">
    /// Preformatted log string to append to the console output.
    /// </param>
    public void AppendLog(string log)
    {
        _textLog.AppendText(log + "\n");
    }

    #endregion
}
