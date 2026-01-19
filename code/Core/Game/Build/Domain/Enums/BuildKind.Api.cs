namespace CosmosCasino.Core.Game.Build.Domain
{
    /// <summary>
    /// Identifies the high-level category of buildable content
    /// targeted by a build intent.
    /// </summary>
    public enum BuildKind
    {
        /// <summary>
        /// Floor-related build operations (place or remove floors).
        /// </summary>
        Floor,

        /// <summary>
        /// Wall-related build operations (place or remove walls).
        /// </summary>
        Wall,
    }
}
