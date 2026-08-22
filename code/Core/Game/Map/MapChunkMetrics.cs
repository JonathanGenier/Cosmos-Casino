namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Defines authoritative metrics for the new boundary-based map chunk partition.
    /// </summary>
    public static class MapChunkMetrics
    {
        /// <summary>
        /// Gets the number of global map cells along one horizontal axis of a map chunk.
        /// Map chunks partition global X/Z cells by boundary ranges such as <c>0..ChunkSize - 1</c>.
        /// </summary>
        public static int ChunkSize => 15;
    }
}
