using CosmosCasino.Core.Application.Save;
using CosmosCasino.Core.Game.Map;

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
    public sealed partial class CoreServices
    {
        #region FIELDS

        private IDisposable _consoleManagerDisposable;
        private bool _isDisposed;
        private bool _isShutdown;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Provides persistence and save/load functionality for the application.
        /// <para>
        /// This service is created at application startup and is guaranteed
        /// to remain valid for the lifetime of <see cref="CoreServices"/>.
        /// </para>
        /// </summary>
        internal SaveManager SaveManager { get; init; }

        /// <summary>
        /// Provides access to the global <see cref="MapManager"/> responsible for
        /// managing map cells, floors, structures, and furniture during gameplay.
        /// This reference is initialized during core service setup and remains
        /// immutable for the lifetime of the application.
        /// </summary>
        internal MapManager MapManager { get; init; }

        #endregion
    }
}
