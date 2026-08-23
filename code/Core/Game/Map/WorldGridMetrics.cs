namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Defines the domain-neutral physical dimensions of the logical world grid.
    /// </summary>
    public static class WorldGridMetrics
    {
        /// <summary>
        /// Width and depth of one logical grid unit in world space.
        /// </summary>
        public const float GridUnitSize = 1f;

        /// <summary>
        /// Height of one logical vertical grid unit in world space.
        /// </summary>
        public const float VerticalGridUnitSize = 0.5f;

        /// <summary>
        /// Half the width and depth of one logical grid unit in world space.
        /// </summary>
        public const float HalfGridUnitSize = GridUnitSize * 0.5f;
    }
}
