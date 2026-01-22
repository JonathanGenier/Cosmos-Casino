using Godot;
using System;

/// <summary>
/// Provides a user interface control for displaying a log console and accepting command input within a Godot
/// application.
/// </summary>
/// <remarks>The ConsoleUi control enables users to view log output and submit commands interactively. It exposes
/// methods to append log entries, clear the display, and toggle visibility, as well as an event for handling submitted
/// commands. This control is intended for integration into debugging or developer tools within a Godot scene.</remarks>
public sealed partial class ConsoleUi : Control
{
    #region Inspector nodes

    [Export]
    private RichTextLabel? _textLog;

    [Export]
    private LineEdit? _lineEdit;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a command is submitted, providing the command text as an argument.
    /// </summary>
    /// <remarks>Subscribers can handle this event to process or respond to user-submitted commands. The event
    /// is raised each time a command is submitted, and the provided string parameter contains the full text of the
    /// command.</remarks>
    public event Action<string>? CommandSubmitted;

    #endregion

    #region Properties

    private RichTextLabel TextLog
    {
        get => _textLog ?? throw new InvalidOperationException($"{nameof(RichTextLabel)} is not initialized.");
        set => _textLog = value;
    }

    private LineEdit LineEdit
    {
        get => _lineEdit ?? throw new InvalidOperationException($"{nameof(LineEdit)} is not initialized.");
        set => _lineEdit = value;
    }

    #endregion

    #region Godot Processes

    /// <summary>
    /// Called when the node enters the scene tree for the first time. Use this method to perform initialization that
    /// depends on the node being added to the scene.
    /// </summary>
    /// <remarks>This method is typically used to connect signals, set up event handlers, or perform other
    /// setup tasks that require the node to be part of the active scene. Override this method to customize
    /// initialization logic for your node.</remarks>
    public override void _Ready()
    {
        if (_textLog == null)
        {
            throw new InvalidOperationException($"{nameof(ConsoleUi)}: TextLog is not assigned.");
        }

        if (_lineEdit == null)
        {
            throw new InvalidOperationException($"{nameof(ConsoleUi)}: LineEdit is not assigned.");
        }

        LineEdit.TextSubmitted += OnCommandSubmitted;
    }

    /// <summary>
    /// Called when the node is about to be removed from the scene tree. Performs cleanup of event handlers and other
    /// resources.
    /// </summary>
    /// <remarks>Override this method to release resources or detach event handlers before the node is freed.
    /// This method is called automatically by the engine; it is not intended to be called directly.</remarks>
    public override void _ExitTree()
    {
        LineEdit.TextSubmitted -= OnCommandSubmitted;
    }

    #endregion

    #region Public API

    /// <summary>
    /// Displays the control and resets the input field to its default state.
    /// </summary>
    public void ShowConsoleUi()
    {
        Visible = true;
        LineEdit.ReleaseFocus();
    }

    /// <summary>
    /// Hides the control and resets the input field to its default state.
    /// </summary>
    /// <remarks>This method sets the control's visibility to hidden, clears the contents of the associated
    /// text input, and removes keyboard focus from the input field. Call this method to reset the input state and hide
    /// the control from the user interface.</remarks>
    public void HideConsoleUi()
    {
        Visible = false;
        LineEdit.Clear();
        LineEdit.ReleaseFocus();
    }

    /// <summary>
    /// Forcibly releases ui focus from the associated line edit control.
    /// </summary>
    /// <remarks>Call this method to remove focus from the line edit, even if it is currently active. This can
    /// be useful when programmatically managing input focus in custom UI workflows.</remarks>
    public void ForceReleaseFocus()
    {
        LineEdit.ReleaseFocus();
    }

    /// <summary>
    /// Appends the specified log message to the end of the log display.
    /// </summary>
    /// <param name="log">The log message to append. Cannot be null.</param>
    public void AppendLog(string log)
    {
        TextLog.AppendText(log + "\n");
        TextLog.ScrollToLine(TextLog.GetLineCount());
    }

    /// <summary>
    /// Clears all visible log output from the console display.
    /// This affects presentation only and does not modify the
    /// underlying log buffer or logging pipeline.
    /// </summary>
    public void ClearLogs()
    {
        TextLog.Clear();
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the submission of a command entered by the user.
    /// </summary>
    /// <remarks>This method raises the CommandSubmitted event with the provided command and clears the input
    /// field. Typically called in response to user actions such as pressing Enter in a command input box.</remarks>
    /// <param name="command">The command text submitted by the user. Cannot be null.</param>
    private void OnCommandSubmitted(string command)
    {
        CommandSubmitted?.Invoke(command);
        LineEdit.Clear();
    }

    #endregion
}
