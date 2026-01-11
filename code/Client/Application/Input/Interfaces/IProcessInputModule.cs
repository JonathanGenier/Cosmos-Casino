/// <summary>
/// Defines a contract for modules that process input each frame within an input system.
/// </summary>
/// <remarks>Implementations of this interface are expected to perform per-frame input processing in a
/// non-blocking and efficient manner. Modules should emit input intent to the input system and avoid performing
/// long-running or resource-intensive operations during processing.</remarks>
public interface IProcessInputModule : IInputModule
{
    /// <summary>
    /// Performs processing for the specified time interval.
    /// </summary>
    /// <param name="delta">The elapsed time, in seconds, since the last call to this method. Must be a non-negative value.</param>
    void Process(double delta);
}