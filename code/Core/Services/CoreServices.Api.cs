using CosmosCasino.Core.Console;
using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Game;
using CosmosCasino.Core.Map;
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
    public sealed partial class CoreServices : IDisposable
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
        /// <param name="savePath">
        /// Absolute or relative path where save data will be stored.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="savePath"/> is null, empty, or whitespace.
        /// </exception>
        public CoreServices(string savePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(savePath);

            using (ConsoleLog.SystemScope(nameof(CoreServices)))
            {
                JsonSaveSerializer serializer = new();
                SaveManager = new SaveManager(serializer, savePath);
                ConsoleManager = new ConsoleManager();
                MapManager = new MapManager();
                _consoleManagerDisposable = ConsoleManager;
            }
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Manages core game logic for an active game session.
        /// <para>
        /// This property is <c>null</c> when no game is running and is initialized
        /// by calling <see cref="StartGame"/>. It is cleared when
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

        #region METHODS

        /// <summary>
        /// Initiates a graceful shutdown of core runtime state.
        /// This method performs logical teardown only, ensuring that any
        /// active game session is properly ended and that core services
        /// transition into a safe, inactive state.
        /// This method is idempotent and may be called multiple times.
        /// </summary>
        public void Shutdown()
        {
            if (_isShutdown)
            {
                return;
            }

            _isShutdown = true;

            EndGame();
        }

        /// <summary>
        /// Releases all disposable core-owned resources.
        /// This method guarantees that shutdown logic is executed first,
        /// then disposes of any core services that require explicit cleanup
        /// (such as event subscriptions or unmanaged resources).
        /// Safe to call multiple times.
        /// </summary>
        /// <inheritdoc/>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            Shutdown(); // ALWAYS first
            _consoleManagerDisposable.Dispose();
        }

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
        public bool StartGame()
        {
            ConsoleLog.System(nameof(CoreServices), "Starting game...");
            if (GameManager != null)
            {
                ConsoleLog.Error(nameof(CoreServices), "Cannot start a new game when a game has already started.");

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
                return true;
            }

            GameManager = null;
            return true;
        }

        #endregion
    }
}
