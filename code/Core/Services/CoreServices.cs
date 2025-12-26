using CosmosCasino.Core.Console;
using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Game;
using CosmosCasino.Core.Save;
using CosmosCasino.Core.Serialization;

namespace CosmosCasino.Core.Services
{
    /// <summary>
    /// Central composition root for all core (engine-agnostic) services.
    /// <para>
    /// <see cref="CoreServices"/> owns and manages the lifetime of services that
    /// must exist for the application to function, as well as services that are
    /// only valid during an active game session.
    /// </para>
    /// </summary>
    public sealed class CoreServices
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initializes all core services required for the application to function.
        /// <para>
        /// This constructor establishes all mandatory invariants for the core
        /// layer. If this constructor succeeds, all required core services are
        /// guaranteed to be in a valid state.
        /// </para>
        /// </summary>
        /// <param name="serializer">
        /// Serializer implementation used by the save system.
        /// </param>
        /// <param name="savePath">
        /// Absolute or relative path where save data will be stored.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="serializer"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="savePath"/> is null, empty, or whitespace.
        /// </exception>
        public CoreServices(ISerializer serializer, string savePath)
        {
            ArgumentNullException.ThrowIfNull(serializer);
            ArgumentException.ThrowIfNullOrWhiteSpace(savePath);

            ConsoleLog.System("CoreServices", "Setting up...");
            SaveManager = new SaveManager(serializer, savePath);
            ConsoleManager = new ConsoleManager();
            ConsoleLog.System("CoreServices", "Ready");
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Provides persistence and save/load functionality for the application.
        /// <para>
        /// This service is created at application startup and is guaranteed
        /// to remain valid for the lifetime of <see cref="CoreServices"/>.
        /// </para>
        /// </summary>
        public SaveManager SaveManager { get; init; }

        /// <summary>
        /// Manages core game logic for an active game session.
        /// <para>
        /// This property is <c>null</c> when no game is running and is initialized
        /// by calling <see cref="StartNewGame"/>. It is cleared when
        /// <see cref="EndGame"/> is called.
        /// </para>
        /// </summary>
        public GameManager? GameManager { get; private set; }

        /// <summary>
        /// The active debug console instance used to collect log output
        /// and execute debug commands.
        /// </summary>
        public ConsoleManager ConsoleManager { get; private set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Starts a new game session and initializes all game-scoped core services.
        /// <para>
        /// This method may only be called when no game is currently running.
        /// Calling it while a game session is active is considered a logic error.
        /// </para>
        /// </summary>
        /// <returns>
        /// <c>true</c> if the game session was successfully started;
        /// <c>false</c> if a game session is already active and the application
        /// is running in a non-debug build.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a game session is already active.
        /// </exception>
        public bool StartNewGame()
        {
            if (GameManager != null)
            {
                ConsoleLog.Error("Game", "Cannot start a new game when a game has already started.");

#if DEBUG
                throw new InvalidOperationException("Cannot start a new game when a game has already started.");
#else
                
                return false;
#endif
            }

            GameManager = new GameManager(SaveManager);
            return true;
        }

        /// <summary>
        /// Ends the currently active game session and releases all game-scoped
        /// core services.
        /// <para>
        /// This method enforces a strict lifecycle: it may only be called when
        /// a game session is currently active.
        /// </para>
        /// </summary>
        /// <returns>
        /// <c>true</c> if the active game session was successfully ended;
        /// <c>false</c> if no game session is running and the application
        /// is operating in a non-debug build.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no game session is currently running.
        /// </exception>
        public bool EndGame()
        {
            if (GameManager == null)
            {
                ConsoleLog.Error("Game", "Trying to end game but no game is currently running.");

#if DEBUG
                throw new InvalidOperationException("Trying to end game but no game is currently running.");
#else
                return false;
#endif
            }

            GameManager = null;
            return true;
        }

        #endregion
    }
}
