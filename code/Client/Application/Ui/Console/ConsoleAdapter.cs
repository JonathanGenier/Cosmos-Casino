using CosmosCasino.Core.Application.Console;
using CosmosCasino.Core.Application.Console.Command;
using System;

/// <summary>
/// Provides an adapter that bridges the core console manager and the user interface, enabling real-time display and
/// interaction with structured log entries and command results in a console UI.
/// </summary>
/// <remarks>The ConsoleAdapter replays existing log entries to the UI upon initialization and subscribes to
/// receive new log entries and command submissions in real time. It ensures that the UI remains synchronized with the
/// core logging buffer and command execution state. Dispose the adapter when it is no longer needed to release event
/// subscriptions and prevent memory leaks.</remarks>
public sealed class ConsoleAdapter : IDisposable
{
    #region Fields

    private readonly ConsoleUi _consoleUi;
    private readonly ConsoleManager _consoleManager;
    private bool _isDisposed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the ConsoleAdapter class and sets up event handlers for console UI and log
    /// management.
    /// </summary>
    /// <remarks>This constructor subscribes to relevant events from the provided ConsoleUi and ConsoleManager
    /// instances. Existing log entries from the ConsoleManager are processed during initialization.</remarks>
    /// <param name="consoleUi">The ConsoleUi instance used to interact with the console user interface. Cannot be null.</param>
    /// <param name="consoleManager">The ConsoleManager instance that manages console logs and events. Cannot be null.</param>
    public ConsoleAdapter(ConsoleUi consoleUi, ConsoleManager consoleManager)
    {
        _consoleUi = consoleUi;
        _consoleManager = consoleManager;

        foreach (var entry in _consoleManager.GetLogs())
        {
            OnEntryAdded(entry);
        }

        _consoleManager.EntryAdded += OnEntryAdded;
        _consoleManager.Cleared += OnCleared;
        _consoleUi.CommandSubmitted += OnCommandSubmitted;
    }

    #endregion

    #region Commands

    /// <summary>
    /// Appends the specified command to the console log output.
    /// </summary>
    /// <param name="command">The command text to append to the console log. Cannot be null.</param>
    public void AppendCommand(string command)
    {
        _consoleUi.AppendLog($" > {command}");
    }

    /// <summary>
    /// Appends the specified command result message to the console log if it should be displayed.
    /// </summary>
    /// <remarks>The message is displayed in green if the command succeeded, or red if it failed. Messages
    /// that are empty or not intended for console display are ignored.</remarks>
    /// <param name="result">The result of the console command to append. Only results with <see cref="ConsoleCommandResult.ShowInConsole"/>
    /// set to <see langword="true"/> and a non-empty <see cref="ConsoleCommandResult.Message"/> are appended.</param>
    public void AppendCommandResult(ConsoleCommandResult result)
    {
        if (!result.ShowInConsole)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(result.Message))
        {
            return;
        }

        string color = result.Success ? "#9cff9c" : "#ff6b6b";
        _consoleUi.AppendLog($"[color={color}] {result.Message}[/color]");
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Detaches the adapter from all subscribed core and UI events.
    /// This method releases cross-layer event subscriptions bridging
    /// the core console manager and the UI console, ensuring that no
    /// dangling references or callbacks remain after the adapter
    /// is no longer in use.
    /// </summary>
    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _consoleManager.EntryAdded -= OnEntryAdded;
        _consoleManager.Cleared -= OnCleared;
        _consoleUi.CommandSubmitted -= OnCommandSubmitted;
    }

    #endregion

    #region Formatting

    /// <summary>
    /// Formats a log entry as a color-coded string suitable for console output.
    /// </summary>
    /// <remarks>The returned string includes color tags for use with consoles that support color formatting.
    /// The timestamp is formatted as hours, minutes, and seconds.</remarks>
    /// <param name="entry">The log entry to format. Must not be null.</param>
    /// <returns>A string representing the formatted log entry, including timestamp, category, and message with color codes.</returns>
    private string Format(ConsoleLogEntry entry)
    {
        string color = GetConsoleLogColor(entry.DisplayKind);
        string timeColor = "#888888";

        var timeSpan = TimeSpan.FromMilliseconds(entry.TimestampMs);
        string time = timeSpan.ToString(@"hh\:mm\:ss");

        return $"[color={timeColor}][{time}][/color] [color={color}] [{entry.Category}] {entry.Message}[/color]";
    }

    /// <summary>
    /// Resolves the display color associated with a log entry’s semantic kind.
    /// Colors are chosen to provide immediate visual distinction between
    /// general output, system diagnostics, events, warnings, and errors
    /// within the console UI.
    /// </summary>
    /// <param name="displayKind">
    /// Semantic classification of the log entry used for color selection.
    /// </param>
    /// <returns>
    /// A BBCode-compatible color string representing the visual style
    /// for the specified log display kind.
    /// </returns>
    private string GetConsoleLogColor(ConsoleLogDisplayKind displayKind)
    {
        return displayKind switch
        {
            ConsoleLogDisplayKind.General => "#ffffff",
            ConsoleLogDisplayKind.Event => "#38e1ff",
            ConsoleLogDisplayKind.System => "#f58d42",
            ConsoleLogDisplayKind.Muted => "#8c00ff",
            ConsoleLogDisplayKind.Warning => "#f5f542",
            ConsoleLogDisplayKind.Error => "#eb2323",
            _ => "#ffffff",
        };
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the event when a new console log entry is added.
    /// </summary>
    /// <param name="entry">The log entry that was added to the console.</param>
    private void OnEntryAdded(ConsoleLogEntry entry)
    {
        _consoleUi.AppendLog(Format(entry));
    }

    /// <summary>
    /// Handles logic to be performed when the console UI is cleared.
    /// </summary>
    private void OnCleared()
    {
        _consoleUi.Clear();
    }

    /// <summary>
    /// Handles the submission of a command by executing it and displaying the result.
    /// </summary>
    /// <param name="command">The command string to execute. Cannot be null or empty.</param>
    private void OnCommandSubmitted(string command)
    {
        AppendCommand(command);
        var result = _consoleManager.ExecuteCommand(command);
        AppendCommandResult(result);
    }

    #endregion
}