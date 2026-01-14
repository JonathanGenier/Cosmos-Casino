namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Orchestrates the execution of a build intent by applying the
    /// requested build operation to each target cell and aggregating
    /// the per-cell outcomes into a single build result.
    /// </summary>
    public sealed partial class BuildManager
    {
        #region Build Operation

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
        public BuildResult ApplyBuildOperations(BuildIntent intent)
        {
            var operationResults = new List<BuildOperationResult>(intent.Cells.Count);

            foreach (var coord in intent.Cells)
            {
                var result = intent.Kind switch
                {
                    BuildKind.Floor => ResolveFloor(intent, coord),
                    BuildKind.Wall => ResolveWall(intent, coord),
                    // BuildKind.Furniture => ResolveFurniture(...)
                    _ => throw new NotImplementedException($"{nameof(BuildKind)} not implemented.")
                };

                operationResults.Add(result);
            }

            return BuildResult.Done(intent, operationResults);
        }

        #endregion
    }
}

