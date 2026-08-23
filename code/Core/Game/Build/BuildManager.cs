using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Orchestrates structure-oriented build intents against the authoritative map.
    /// </summary>
    public sealed partial class BuildManager
    {
        #region FIELDS

        private readonly MapManager _mapManager;
        // private readonly EconomyManager _economyManager; // later

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new <see cref="BuildManager"/> bound to the
        /// specified map manager, which is used to apply all build
        /// operations to the authoritative map state.
        /// </summary>
        /// <param name="mapManager">
        /// The map manager responsible for cell creation, mutation,
        /// and cleanup during build operations.
        /// </param>
        internal BuildManager(MapManager mapManager)
        {
            ArgumentNullException.ThrowIfNull(mapManager);

            _mapManager = mapManager;
        }

        #endregion
    }
}
