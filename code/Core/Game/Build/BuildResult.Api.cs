using CosmosCasino.Core.Game.Build.Domain;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Immutable result representing the outcome of a build attempt
    /// processed by the core build system.
    /// All accepted operations have already been applied to the
    /// authoritative game state. Individual operations may succeed
    /// or fail independently.
    /// </summary>
    public sealed class BuildResult
    {
        #region Constructor

        private BuildResult(BuildIntent intent, IReadOnlyList<BuildOperationResult> results)
        {
            Intent = intent;
            Results = results;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The initial build intent that has been evaluated.
        /// </summary>
        public BuildIntent Intent { get; }

        /// <summary>
        /// Per-cell outcomes produced by the build operation.
        /// </summary>
        public IReadOnlyList<BuildOperationResult> Results { get; }

        #endregion

        #region Factories

        /// <summary>
        /// Creates a completed build result from an evaluated build intent
        /// and the per-cell operation results produced during execution.
        /// </summary>
        /// <param name="intent">
        /// The build intent that was processed by the build system.
        /// </param>
        /// <param name="results">
        /// The collection of per-cell build operation results generated
        /// while applying the intent.
        /// </param>
        /// <returns>
        /// An immutable build result representing the outcome of the build attempt.
        /// </returns>
        internal static BuildResult Done(BuildIntent intent, IReadOnlyList<BuildOperationResult> results)
        {
            return new BuildResult(intent, results);
        }

        #endregion
    }
}