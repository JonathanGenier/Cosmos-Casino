namespace CosmosCasino.Core.Game.Map.Terrain.Generation
{
    /// <summary>
    /// Provides terrain height values at world-space coordinates.
    /// Implementations of this interface are responsible for returning
    /// deterministic height values for a given coordinate pair and may
    /// use procedural generation, cached data, or other backing sources.
    /// </summary>
    internal interface ITerrainHeightProvider
    {
        /// <summary>
        /// Returns the terrain height at the specified world-space coordinates.
        /// The returned value must be deterministic for the same input
        /// coordinates within a given provider configuration.
        /// </summary>
        /// <param name="x">
        /// World-space X coordinate at which to sample the terrain height.
        /// </param>
        /// <param name="y">
        /// World-space Y coordinate at which to sample the terrain height.
        /// </param>
        /// <returns>
        /// The terrain height value at the specified world coordinate.
        /// </returns>
        float GetHeight(int x, int y);
    }
}