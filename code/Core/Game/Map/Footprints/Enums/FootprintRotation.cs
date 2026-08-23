namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Describes quarter-turn rotations for authoritative map-cell footprints around the vertical Y axis.
    /// </summary>
    public enum FootprintRotation
    {
        /// <summary>
        /// Leaves footprint offsets unchanged.
        /// </summary>
        Deg0,

        /// <summary>
        /// Rotates footprint offsets one quarter turn using the documented X/Z transform.
        /// </summary>
        Deg90,

        /// <summary>
        /// Rotates footprint offsets two quarter turns in X/Z space.
        /// </summary>
        Deg180,

        /// <summary>
        /// Rotates footprint offsets three quarter turns using the documented X/Z transform.
        /// </summary>
        Deg270
    }
}
