using Godot;
using System;

/// <summary>
/// Controller for the debug log console user interface.
/// Owns the behavior of the log console presentation, including visibility
/// toggling, log entry display, and scroll management.
/// This controller is UI-only and does not handle input detection or logging
/// logic directly; it reacts to coordination decisions made by higher-level
/// systems such as <see cref="UiManager"/>.
/// </summary>
public sealed partial class ConsoleUi : Control
{
    #region NODES

    [Export]
    private RichTextLabel _textLog;

    [Export]
    private LineEdit _lineEdit;

    #endregion

    #region EVENTS

    /// <summary>
    /// Raised when the user submits a command from the console.
    /// </summary>
    public event Action<string> CommandSubmitted;

    #endregion

    #region METHODS

    /// <summary>
    /// Called when the log console UI node enters the scene tree and is ready.
    /// Used to perform final UI-side initialization and emit diagnostic
    /// readiness information for debugging purposes.
    /// </summary>
    /// <inheritdoc/>
    public override void _Ready()
    {
        _lineEdit.TextSubmitted += OnCommandSubmitted;
    }

    /// <summary>
    /// Cleans up UI event subscriptions owned by the console UI.
    /// <para>
    /// Detaches input callbacks associated with the command input field
    /// to ensure no dangling references remain after the UI node
    /// leaves the scene tree.
    /// </para>
    /// </summary>
    /// <inheritdoc/>
    public override void _ExitTree()
    {
        _lineEdit.TextSubmitted -= OnCommandSubmitted;
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

    /// <summary>
    /// Clears all visible log output from the console display.
    /// This affects presentation only and does not modify the
    /// underlying log buffer or logging pipeline.
    /// </summary>
    public void Clear()
    {
        _textLog.Clear();
    }

    /// <summary>
    /// Handles submission of a command entered by the user.
    /// Raises the <see cref="CommandSubmitted"/> event and resets
    /// the input field for subsequent commands.
    /// </summary>
    /// <param name="command">
    /// Raw command string entered by the user.
    /// </param>
    private void OnCommandSubmitted(string command)
    {
        CommandSubmitted?.Invoke(command);
        _lineEdit.Clear();
    }

    #endregion
}
