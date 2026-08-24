using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;

/// <summary>
/// Provides a base class for build context objects that encapsulate information about a build operation, including its
/// type and intent.
/// </summary>
/// <remarks>This class is intended to be inherited by types that define specific build contexts for various build
/// operations. Implementations create Core build intents from logical cursor targets resolved by the Client.</remarks>
public abstract class BuildContextBase
{
    #region Abstract Methods

    /// <summary>
    /// Attempts to create a build intent between the specified start and current cursor targets.
    /// </summary>
    /// <param name="startTarget">The cursor target where the build operation started.</param>
    /// <param name="currentTarget">The current cursor target used to create the build intent.</param>
    /// <param name="buildOperation">The type of build operation to perform (e.g., place, remove).</param>
    /// <param name="buildInteractionMode">The interaction mode affecting the build operation.</param>
    /// <param name="intent">When this method returns, contains the resulting build intent if the operation succeeds; otherwise, the default
    /// value.</param>
    /// <returns>true if a build intent was successfully created; otherwise, false.</returns>
    public abstract bool TryCreateBuildIntent(
        CursorTarget startTarget,
        CursorTarget currentTarget,
        BuildOperation buildOperation,
        BuildInteractionMode buildInteractionMode,
        out BuildIntent intent);

    #endregion
}
