namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Describes the result of validating a local map cell occupancy reservation.
    /// </summary>
    internal enum CellOccupancyOutcome
    {
        /// <summary>
        /// The reservation can be applied.
        /// </summary>
        Valid,

        /// <summary>
        /// The requested reservation already exists and no mutation is required.
        /// </summary>
        NoOp,

        /// <summary>
        /// The reservation conflicts with existing local occupancy.
        /// </summary>
        Invalid
    }
}
