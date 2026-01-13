namespace CosmosCasino.Core.Game.Build
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
        /// Structure-related build operations (walls, doors, etc.).
        /// </summary>
        Structure,

        /// <summary>
        /// Furniture-related build operations (placeable objects).
        /// </summary>
        Furniture,
    }
}
