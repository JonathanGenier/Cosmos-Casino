/// <summary>
/// Represents the possible states of the game lifecycle.
/// </summary>
/// <remarks>Use this enumeration to track or control the current state of the game, such as when loading
/// resources, actively playing, or when the game is paused. The value can be used to determine which actions are
/// available or how the user interface should respond.</remarks>
public enum GameState
{
    /// <summary>
    /// The game is initializing or loading required resources.
    /// </summary>
    Loading,

    /// <summary>
    /// The game is actively running and accepting gameplay input.
    /// </summary>
    Playing,

    /// <summary>
    /// The game is suspended with gameplay input disabled.
    /// </summary>
    Paused
}