namespace CosmosCasino.Core.Debug.Environment
{
    /// <summary>
    /// Defines the application environment at compile-time.
    /// This is the authoritative source for Dev vs Prod behavior.
    /// </summary>
    public static class AppEnvironment
    {
        public static readonly EnvironmentType Current;

        static AppEnvironment()
        {
#if DEBUG
            Current = EnvironmentType.Dev;
#else
            Current = EnvironmentType.Prod;
#endif
        }

        public static bool IsDev => Current == EnvironmentType.Dev;
        public static bool IsProd => Current == EnvironmentType.Prod;
    }
}
