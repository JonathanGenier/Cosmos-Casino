namespace CosmosCasino.Core.Debug.Environment
{
    /// <summary>
    /// Defines the application environment at compile-time.
    /// This is the authoritative source for Dev vs Prod behavior.
    /// </summary>
    public static class AppEnvironment
    {
        #region FIELDS

        /// <summary>
        /// Current environment type
        /// </summary>
        public static readonly EnvironmentType Current;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Defines the application environment determined at compile time
        /// and exposes environment-specific flags for runtime checks.
        /// </summary>
        static AppEnvironment()
        {
#if DEBUG
            Current = EnvironmentType.Dev;
#else
            Current = EnvironmentType.Prod;
#endif
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Indicates whether the application is running in a development environment.
        /// </summary>
        public static bool IsDev => Current == EnvironmentType.Dev;

        /// <summary>
        /// Indicates whether the application is running in a production environment.
        /// </summary>
        public static bool IsProd => Current == EnvironmentType.Prod;

        #endregion
    }
}
