using CosmosCasino.Core.Debug;
using CosmosCasino.Core.Debug.Command;
using CosmosCasino.Core.Debug.Logging;
using System;

/// <summary>
/// UI log sink responsible for bridging the core logging system
/// to the in-game debug log console.
/// This sink subscribes to the core log buffer, formats incoming
/// log entries for presentation, and forwards them to the
/// <see cref="ConsoleUi"/>.
/// This class contains no logging logic of its own and does not
/// influence log filtering, safety, or persistence. Its sole
/// responsibility is presentation adaptation.
/// </summary>
public sealed class ConsoleAdapter
{
    #region FIELDS

    /// <summary>
    /// Reference to the log console UI controller that receives
    /// formatted log output for display.
    /// </summary>
    private readonly ConsoleUi _consoleUi;

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
    internal ConsoleAdapter(ConsoleUi consoleUi, ConsoleManager consoleManager)
    {
        _consoleUi = consoleUi;

        foreach (var entry in consoleManager.GetLogs())
        {
            OnEntryAdded(entry);
        }

        consoleManager.EntryAdded += OnEntryAdded;
        consoleManager.Cleared += OnCleared;
    }

    #endregion

    #region INTERNAL METHODS

    /// <summary>
    /// Appends a raw command line entered by the user to the console output.
    /// This represents the echo of user input prior to command execution
    /// and is displayed distinctly from command results or log entries.
    /// </summary>
    /// <param name="command">
    /// Raw command string entered by the user.
    /// </param>
    internal void AppendCommand(string command)
    {
        _consoleUi.AppendLog($" > {command}");
    }

    /// <summary>
    /// Appends the result of a command execution to the console output.
    /// Output visibility is controlled by the <see cref="CommandResult.ShowInConsole"/>
    /// flag, allowing commands to opt out of visual feedback when appropriate
    /// (e.g. commands with implicit visual side effects).
    /// </summary>
    /// <param name="result">
    /// Result returned by the executed command.
    /// </param>
    internal void AppendCommandResult(CommandResult result)
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

    #region PRIVATE METHODS

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
    private string Format(LogEntry entry)
    {
        string color = GetColorByLogLevel(entry);
        string timeColor = "#888888";

        var timeSpan = TimeSpan.FromMilliseconds(entry.TimestampMs);
        string time = timeSpan.ToString(@"hh\:mm\:ss");

        return $"[color={timeColor}][{time}][/color] [color={color}] [{entry.Category}] {entry.Message}[/color]";
    }

    /// <summary>
    /// Resolves the display color associated with a log entry
    /// based on its severity level.
    /// </summary>
    /// <param name="entry">
    /// Log entry whose severity determines the display color.
    /// </param>
    /// <returns>
    /// Hex color string representing the severity color.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when a log level is encountered that has no defined
    /// color mapping.
    /// </exception>
    private string GetColorByLogLevel(LogEntry entry)
    {
        return entry.Level switch
        {
            LogLevel.Error => "#eb2323",
            LogLevel.Warning => "#f5f542",
            LogLevel.Verbose => "#8c00ff",
            LogLevel.Info => GetInfoColorByLogKind(entry.Kind),
            _ => throw new ArgumentOutOfRangeException(
                nameof(entry.Level),
                entry.Level,
                "Log level color not implemented"
            ),
        };
    }

    /// <summary>
    /// Resolves the display color for informational log entries
    /// based on their semantic kind.
    /// </summary>
    /// <param name="kind">
    /// Semantic kind of the log entry.
    /// </param>
    /// <returns>
    /// Hex color string representing the semantic color.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when a log kind is encountered that has no defined
    /// color mapping.
    /// </exception>
    private string GetInfoColorByLogKind(LogKind kind)
    {
        return kind switch
        {
            LogKind.General => "#ffffff",
            LogKind.Event => "#38e1ff",
            LogKind.System => "#f58d42",
            _ => throw new ArgumentOutOfRangeException(
                nameof(kind),
                kind,
                "Log kind color not implemented"
            ),
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
    private void OnEntryAdded(LogEntry entry)
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

    #endregion
}