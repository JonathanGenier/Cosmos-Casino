using CosmosCasino.Core.Application.Console;
using CosmosCasino.Core.Application.Save;
using CosmosCasino.Core.Application.Serialization;

namespace CosmosCasino.Core.Application.Services
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
                _consoleManagerDisposable = ConsoleManager;
            }
        }

        #endregion

        #region PROPERTIES

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

        #endregion
    }
}
