using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;

/// <summary>
/// Build interaction capability for contexts that create authoritative Structure build intents.
/// </summary>
internal interface IStructureBuildIntentContext
{
    #region Build Intent

    /// <summary>
    /// Attempts to create a Structure build intent between the specified cursor targets.
    /// </summary>
    /// <param name="startTarget">The cursor target where the build operation started.</param>
    /// <param name="currentTarget">The current cursor target used to create the build intent.</param>
    /// <param name="buildOperation">The requested build operation.</param>
    /// <param name="buildInteractionMode">The active modifier-derived build interaction mode.</param>
    /// <param name="intent">The created Structure build intent when successful.</param>
    /// <returns><c>true</c> when an intent was created; otherwise, <c>false</c>.</returns>
    bool TryCreateBuildIntent(
        CursorTarget startTarget,
        CursorTarget currentTarget,
        BuildOperation buildOperation,
        BuildInteractionMode buildInteractionMode,
        out BuildIntent intent);

    #endregion
}
