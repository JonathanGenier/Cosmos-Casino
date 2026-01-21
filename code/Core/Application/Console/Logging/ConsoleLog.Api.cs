using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CosmosCasino.Core.Application.Console.Logging
{
    /// <summary>
    /// PUBLIC API
    /// Internal implementation of the ConsoleLog logging pipeline.
    /// </summary>
    public static partial class ConsoleLog
    {
        #region METHODS

        /// <summary>
        /// Writes an informational message to the console log, including the source file name for context.
        /// </summary>
        /// <remarks>Use this method to record general informational events or status updates. The log
        /// entry will include the name of the source file from which the method was called, aiding in traceability
        /// during debugging or monitoring.</remarks>
        /// <param name="message">The message text to log at the informational level.</param>
        /// <param name="file">The full path of the source file invoking the log method. This parameter is automatically supplied by the
        /// compiler and is used to display the file name in the log entry.</param>
        public static void Info(
            string message,
            [CallerFilePath] string file = "")
        {
            Write(ConsoleLogLevel.Info, ConsoleLogSafety.Safe, ConsoleLogKind.General, $"{Path.GetFileName(file)}", message);
        }

        /// <summary>
        /// Writes a verbose-level log message to the console output.
        /// </summary>
        /// <remarks>Use this method to record detailed diagnostic information that is typically only
        /// needed during development or troubleshooting. Verbose messages are intended for low-level events and may
        /// produce extensive output.</remarks>
        /// <param name="message">The message to log at the verbose level.</param>
        /// <param name="file">The full path of the source file that invoked the log method. This parameter is automatically supplied by
        /// the compiler and should not be set manually.</param>
        public static void Verbose(
            string message,
            [CallerFilePath] string file = "")
        {
            Write(ConsoleLogLevel.Verbose, ConsoleLogSafety.Unsafe, ConsoleLogKind.General, $"{Path.GetFileName(file)}", message);
        }

        /// <summary>
        /// Writes a warning-level message to the console log, including the source file name for context.
        /// </summary>
        /// <remarks>Use this method to highlight potential issues or important events that do not prevent
        /// normal operation but may require attention. The source file name is included in the log output to aid in
        /// identifying the origin of the warning.</remarks>
        /// <param name="message">The message to log at the warning level. This should describe the warning condition or issue.</param>
        /// <param name="file">The full path of the source file from which the log method is called. This parameter is automatically
        /// supplied by the compiler and is used to display the file name in the log entry.</param>
        public static void Warning(
            string message,
            [CallerFilePath] string file = "")
        {
            Write(ConsoleLogLevel.Warning, ConsoleLogSafety.Safe, ConsoleLogKind.General, $"{Path.GetFileName(file)}", message);
        }

        /// <summary>
        /// Writes an error-level log message to the console, including the source file name for context.
        /// </summary>
        /// <remarks>Use this method to record errors that require attention or indicate failures in
        /// application logic. The log entry will include the name of the source file to help identify the origin of the
        /// error. This method is intended for general error reporting and does not throw exceptions.</remarks>
        /// <param name="message">The error message to log. This should describe the issue or exception encountered.</param>
        /// <param name="file">The full path of the source file from which the log is generated. This parameter is automatically supplied
        /// by the compiler and is used to display the file name in the log output.</param>
        public static void Error(
            string message,
            [CallerFilePath] string file = "")
        {
            Write(ConsoleLogLevel.Error, ConsoleLogSafety.Safe, ConsoleLogKind.General, $"{Path.GetFileName(file)}", message);
        }

        /// <summary>
        /// Writes an gameplay event message to the console log.
        /// </summary>
        /// <remarks>Use this method to record significant events or actions in the game for
        /// informational purposes. The log entry includes the name of the source file to help trace the origin of the
        /// event.</remarks>
        /// <param name="message">The event message to log. This value is displayed in the console output.</param>
        /// <param name="file">The full path of the source file that invoked the method. This parameter is automatically supplied by the
        /// compiler and is used to identify the origin of the log entry.</param>
        public static void Event(
            string message,
            [CallerFilePath] string file = "")
        {
            Write(ConsoleLogLevel.Info, ConsoleLogSafety.Safe, ConsoleLogKind.Event, $"{Path.GetFileName(file)}", message);
        }

        /// <summary>
        /// Writes a system-level informational message to the console log.
        /// </summary>
        /// <remarks>Use this method to log messages that represent system events or status updates. The
        /// log entry will include the name of the source file for context.</remarks>
        /// <param name="message">The message text to be logged as a system event.</param>
        /// <param name="file">The full file path of the source code file that invoked this method. This parameter is automatically
        /// supplied by the compiler and is used for log context.</param>
        public static void System(
            string message,
            [CallerFilePath] string file = "")
        {
            Write(ConsoleLogLevel.Info, ConsoleLogSafety.Unsafe, ConsoleLogKind.System, $"{Path.GetFileName(file)}", message);
        }

        /// <summary>
        /// Writes a debug-level message to the console output when the application is compiled in debug mode.
        /// </summary>
        /// <remarks>This method is only included in builds where the DEBUG conditional compilation symbol
        /// is defined. It is intended for diagnostic purposes and will not log messages in release builds. Caller
        /// information parameters are automatically populated and typically do not need to be specified
        /// manually.</remarks>
        /// <param name="message">The message to log at the debug level. This should describe the event or state to be recorded.</param>
        /// <param name="file">The full path of the source file that invoked the method. This parameter is automatically supplied by the
        /// compiler and is used to identify the origin of the log entry.</param>
        /// <param name="line">The line number in the source file where the method was called. This parameter is automatically supplied by
        /// the compiler and helps pinpoint the location of the log entry.</param>
        /// <param name="member">The name of the member (method, property, etc.) that invoked the method. This parameter is automatically
        /// supplied by the compiler and provides context for the log entry.</param>
        [Conditional("DEBUG")]
        public static void Debug(
            string message,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            Write(ConsoleLogLevel.Verbose, ConsoleLogSafety.Unsafe, ConsoleLogKind.Debug, $"{Path.GetFileName(file)}:{line}::{member}", message);
        }

        #endregion
    }
}