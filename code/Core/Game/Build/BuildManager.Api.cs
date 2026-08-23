using CosmosCasino.Core.Game.Build.Domain;

namespace CosmosCasino.Core.Game.Build
{
    /// <summary>
    /// Orchestrates structure-oriented build intent evaluation and execution.
    /// </summary>
    public sealed partial class BuildManager
    {
        #region Preview Operation

        /// <summary>
        /// Evaluates the specified build intent without mutating authoritative map state.
        /// </summary>
        /// <param name="intent">The build intent to evaluate.</param>
        /// <returns>A structure-level build result produced by the shared planner.</returns>
        public BuildResult Evaluate(BuildIntent intent)
        {
            return BuildResultFromPlan(intent, Plan(intent));
        }

        #endregion

        #region Commit Operation

        /// <summary>
        /// Re-evaluates and applies the specified build intent against current authoritative map state.
        /// </summary>
        /// <param name="intent">The build intent to execute.</param>
        /// <returns>A structure-level build result for the plan that was evaluated immediately before commit.</returns>
        public BuildResult Execute(BuildIntent intent)
        {
            BuildPlan plan = Plan(intent);

            if (plan.Outcome == BuildOperationOutcome.Valid)
            {
                Commit(plan);
            }

            return BuildResultFromPlan(intent, plan);
        }

        #endregion
    }
}
