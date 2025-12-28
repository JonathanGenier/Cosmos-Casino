using CosmosCasino.Core.Console;
using CosmosCasino.Core.Console.Command;
using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Services;
using System;

/// <summary>
/// UI log sink responsible for bridging the core logging system
/// to the in-game console.
/// This sink subscribes to the core log buffer, formats incoming
/// log entries for presentation, and forwards them to the
/// <see cref="ConsoleUi"/>.
/// This class contains no logging logic of its own and does not
/// influence log filtering, safety, or persistence. Its sole
/// responsibility is presentation adaptation.
/// </summary>
public sealed class ConsoleAdapter : IDisposable
{
    #region FIELDS

    private readonly ConsoleUi _consoleUi;
    private readonly ConsoleManager _consoleManager;
    private bool _isDisposed;

    #endregion

    #region CONSTRUCTORS

    /// <summary>
    /// Initializes the log console sink and attaches it to the core
    /// logging buffer.
    /// Existing log entries are replayed immediately to populate
    /// the UI with historical output, after which the sink subscribes
    /// to receive new log entries as they are added.
    /// </summary>
    /// <param name="consoleUi">
    /// Log console UI controller that will receive formatted log output.
    /// </param>
    /// <param name="consoleManager">
    /// Console Manager (core) instance that acts as the authoritative source of
    /// structured log entries. The sink replays its existing buffered
    /// logs on initialization and subscribes to its <see cref="ConsoleManager.EntryAdded"/>
    /// event to receive new log entries in real time.
    /// </param>
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

    #region METHODS

    /// <summary>
    /// Appends a raw command line entered by the user to the console output.
    /// This represents the echo of user input prior to command execution
    /// and is displayed distinctly from command results or log entries.
    /// </summary>
    /// <param name="command">
    /// Raw command string entered by the user.
    /// </param>
    public void AppendCommand(string command)
    {
        _consoleUi.AppendLog($" > {command}");
    }

    /// <summary>
    /// Appends the result of a command execution to the console output.
    /// Output visibility is controlled by the <see cref="ConsoleCommandResult.ShowInConsole"/>
    /// flag, allowing commands to opt out of visual feedback when appropriate
    /// (e.g. commands with implicit visual side effects).
    /// </summary>
    /// <param name="result">
    /// Result returned by the executed command.
    /// </param>
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

    /// <summary>
    /// Formats a log entry into a BBCode string suitable for display
    /// in a RichTextLabel.
    /// This includes timestamp formatting, category labeling,
    /// and severity-based colorization.
    /// </summary>
    /// <param name="entry">
    /// Log entry to format.
    /// </param>
    /// <returns>
    /// BBCode-formatted string representing the log entry.
    /// </returns>
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

    /// <summary>
    /// Callback invoked when a new log entry is added to the core
    /// log buffer.
    /// Formats the entry for UI presentation and forwards it to
    /// the log console controller.
    /// </summary>
    /// <param name="entry">
    /// Newly added log entry from the core logging buffer.
    /// </param>
    private void OnEntryAdded(ConsoleLogEntry entry)
    {
        _consoleUi.AppendLog(Format(entry));
    }

    /// <summary>
    /// Callback invoked when the core console buffer is cleared.
    /// Clears all visible log output from the console UI to ensure
    /// presentation state remains consistent with core state.
    /// </summary>
    private void OnCleared()
    {
        _consoleUi.Clear();
    }

    /// <summary>
    /// Handles submission of a console command entered by the user.
    /// Echoes the raw command to the console UI, delegates execution
    /// to the core console manager, and appends the resulting feedback
    /// to the console output when appropriate.
    /// </summary>
    /// <param name="command">
    /// Raw command string entered by the user.
    /// </param>
    private void OnCommandSubmitted(string command)
    {
        AppendCommand(command);
        var result = _consoleManager.ExecuteCommand(command);
        AppendCommandResult(result);
    }

    #endregion
}