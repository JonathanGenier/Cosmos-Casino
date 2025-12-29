/// <summary>
/// Contract for input intent detection modules.
/// Implementations are responsible for detecting specific categories of input
/// and reporting semantic input intent, but must not perform gameplay,
/// UI, or application logic directly.
/// </summary>
public interface IProcessInputModule
{
    /// <summary>
    /// Execution phase that determines when this module is processed
    /// relative to other input modules.
    /// Lower <see cref="InputPhase"/> values are processed earlier in the frame.
    /// </summary>
    InputPhase Phase { get; }

    /// <summary>
    /// Processes input for the current frame.
    /// Implementations should perform lightweight input checks and emit
    /// intent through the owning input system without blocking or
    /// performing expensive operations.
    /// </summary>
    /// <param name="delta">Frame delta time in seconds.</param>
    void Process(double delta);
}