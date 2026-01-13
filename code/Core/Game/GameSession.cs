using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game
{
    /// <summary>
    /// Represents a single game session, providing access to core managers and functionality required to manage
    /// gameplay state and operations.
    /// </summary>
    /// <remarks>A GameSession encapsulates the state and services for a running game instance. Use the static
    /// factory methods to create or load sessions. The session exposes managers for building and map operations, which
    /// coordinate changes to the game world. Instances of GameSession are typically long-lived and should be disposed
    /// of only when the session is complete.</remarks>
    public class GameSession
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the GameSession class.
        /// </summary>
        private GameSession()
        {
            MapManager = new MapManager();
            BuildManager = new BuildManager(MapManager);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Provides access to the core build manager responsible for
        /// validating and executing build intents against the
        /// authoritative game state.
        /// </summary>
        public BuildManager BuildManager { get; private set; }

        /// <summary>
        /// Provides access to the global <see cref="MapManager"/> responsible for
        /// managing map cells, floors, structures, and furniture during gameplay.
        /// This reference is initialized during core service setup and remains
        /// immutable for the lifetime of the application.
        /// </summary>
        internal MapManager MapManager { get; init; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a new instance of the GameSession class.
        /// </summary>
        /// <returns>A new GameSession object representing a fresh game session.</returns>
        public static GameSession CreateNewSession()
        {
            GameSession session = new GameSession();
            return session;
        }

        /// <summary>
        /// Loads a previously saved game session from persistent storage.
        /// </summary>
        /// <returns>A <see cref="GameSession"/> object representing the loaded game session. Returns null if no saved session is
        /// available.</returns>
        /// <exception cref="NotImplementedException">Thrown if the method is not yet implemented.</exception>
        public static GameSession LoadSession()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
