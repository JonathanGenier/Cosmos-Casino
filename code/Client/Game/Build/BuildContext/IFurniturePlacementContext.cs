using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Furniture;

/// <summary>
/// Build interaction capability for contexts that create authoritative Furniture placement requests.
/// </summary>
internal interface IFurniturePlacementContext
{
    #region Placement Request

    /// <summary>
    /// Attempts to create a Furniture placement request from the specified cursor targets.
    /// </summary>
    /// <param name="startTarget">The cursor target where the interaction started.</param>
    /// <param name="currentTarget">The current cursor target used to create the placement request.</param>
    /// <param name="buildOperation">The requested build operation.</param>
    /// <param name="buildInteractionMode">The active modifier-derived build interaction mode.</param>
    /// <param name="request">The created Furniture placement request when successful.</param>
    /// <returns><c>true</c> when a placement request was created; otherwise, <c>false</c>.</returns>
    bool TryCreateFurniturePlacementRequest(
        CursorTarget startTarget,
        CursorTarget currentTarget,
        BuildOperation buildOperation,
        BuildInteractionMode buildInteractionMode,
        out FurniturePlacementRequest request);

    #endregion
}
