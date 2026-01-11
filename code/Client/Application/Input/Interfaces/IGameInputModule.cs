/// <summary>
/// Defines a contract for modules that respond to changes in the game state.
/// </summary>
/// <remarks>Implement this interface to receive notifications when the game state changes. This is typically used
/// to update input handling or UI elements in response to different game states.</remarks>
public interface IGameInputModule
{
    /// <summary>
    /// Handles changes to the current game state.
    /// </summary>
    /// <param name="state">The new state of the game. Specifies the updated status that the game has transitioned to.</param>
    void OnGameStateChanged(GameState state);
}