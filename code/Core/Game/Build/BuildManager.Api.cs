using CosmosCasino.Core.Game.Build.Domain;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Orchestrates the execution of a build intent by applying the
    /// requested build operation to each target cell and aggregating
    /// the per-cell outcomes into a single build result.
    /// </summary>
    public sealed partial class BuildManager
    {
        #region Preview Operation

        /// <summary>
        /// Evaluates the specified build intent and returns the result of applying the requested operation to each
        /// cell.
        /// </summary>
        /// <param name="intent">The build intent that specifies the operation to perform and the collection of target cells. Cannot be null.</param>
        /// <returns>A BuildResult containing the outcome of the operation for each cell in the intent.</returns>
        /// <exception cref="NotImplementedException">Thrown if the build kind specified in the intent is not supported.</exception>
        public BuildResult Evaluate(BuildIntent intent)
        {
            var actionResults = new List<BuildOperationResult>(intent.Cells.Count);

            foreach (var coord in intent.Cells)
            {
                var result = intent.Operation switch
                {
                    BuildOperation.Place => _mapManager.CanPlace(intent.Kind, coord),
                    BuildOperation.Remove => _mapManager.CanRemove(intent.Kind, coord),
                    _ => throw new NotImplementedException($"{nameof(BuildKind)} not implemented.")
                };

                actionResults.Add(result);
            }

            return BuildResult.Done(intent, actionResults);
        }

        #endregion

        #region Commit Operation

        /// <summary>
        /// Applies the specified build intent to all target cells,
        /// delegating execution to the appropriate resolver based on
        /// the build kind and operation.
        /// <para>
        /// Each cell is processed independently; failures on one cell
        /// do not prevent operations on other cells.
        /// </para>
        /// </summary>
        /// <param name="intent">
        /// The build intent describing the operation to perform and
        /// the set of target cells.
        /// </param>
        /// <returns>
        /// A <see cref="BuildResult"/> containing the original intent
        /// and the per-cell outcomes produced during execution.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Thrown when the build kind specified by the intent is not
        /// supported by the build manager.
        /// </exception>
        public BuildResult Execute(BuildIntent intent)
        {
            var actionResults = new List<BuildOperationResult>(intent.Cells.Count);
            var buildOperation = intent.Operation;

            foreach (var coord in intent.Cells)
            {
                var result = intent.Kind switch
                {
                    BuildKind.Floor => ExecuteOperationOnFloor(buildOperation, coord),
                    BuildKind.Wall => ExecuteOperationOnWall(buildOperation, coord),
                    _ => throw new NotImplementedException($"{nameof(BuildKind)} not implemented.")
                };

                actionResults.Add(result);
            }

            return BuildResult.Done(intent, actionResults);
        }

        #endregion
    }
}

