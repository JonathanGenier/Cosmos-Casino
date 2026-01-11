/// <summary>
/// Specifies the context in which input is being processed.
/// </summary>
/// <remarks>Use this enumeration to indicate whether input should be handled in an application or game context.
/// The selected scope may affect how input events are interpreted or prioritized by the system.</remarks>
public enum InputScope
{
    /// <summary>
    /// Represent General Application State where UI input is processed.
    /// </summary>
    App,

    /// <summary>
    /// Represent Game App State where gameplay input is processed.
    /// </summary>
    Game
}