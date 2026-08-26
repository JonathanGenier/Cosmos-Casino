using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Furniture;
using CosmosCasino.Core.Game.Map;
using System;

/// <summary>
/// Build context for a single selected Core furniture definition.
/// </summary>
public sealed class FurnitureBuildContext : BuildContextBase, IFurniturePlacementContext
{
    #region Initialization

    /// <summary>
    /// Initializes a new furniture build context.
    /// </summary>
    /// <param name="definition">The furniture definition to place.</param>
    /// <param name="rotation">The selected footprint rotation.</param>
    public FurnitureBuildContext(
        FurnitureDefinition definition,
        FootprintRotation rotation = FootprintRotation.Deg0)
    {
        ArgumentNullException.ThrowIfNull(definition);

        Definition = definition;
        Rotation = rotation;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the furniture definition placed by this context.
    /// </summary>
    public FurnitureDefinition Definition { get; }

    /// <summary>
    /// Gets the selected footprint rotation.
    /// </summary>
    public FootprintRotation Rotation { get; private set; }

    #endregion

    #region Capabilities

    /// <summary>
    /// Determines whether this context supports the specified player-facing build operation.
    /// </summary>
    /// <param name="buildOperation">The requested player-facing build operation.</param>
    /// <returns><c>true</c> for placement; otherwise, <c>false</c>.</returns>
    public override bool SupportsBuildOperation(BuildOperation buildOperation)
    {
        return buildOperation == BuildOperation.Place;
    }

    #endregion

    #region Rotation

    /// <summary>
    /// Sets the footprint rotation used by future placement requests.
    /// </summary>
    /// <param name="rotation">The selected footprint rotation.</param>
    public void SetRotation(FootprintRotation rotation)
    {
        Rotation = rotation;
    }

    /// <summary>
    /// Rotates the selected footprint clockwise for future placement requests.
    /// </summary>
    /// <returns><c>true</c> when the rotation changed.</returns>
    public override bool TryRotateClockwise()
    {
        Rotation = GetNextClockwiseRotation(Rotation);
        return true;
    }

    #endregion

    #region Placement Request

    /// <summary>
    /// Attempts to create a furniture placement request from the current logical cursor target.
    /// </summary>
    /// <param name="startTarget">The cursor target where the interaction started.</param>
    /// <param name="currentTarget">The current cursor target used to create the placement request.</param>
    /// <param name="buildOperation">The requested build operation.</param>
    /// <param name="buildInteractionMode">The active modifier-derived build interaction mode.</param>
    /// <param name="request">The created placement request when successful.</param>
    /// <returns><c>true</c> when a placement request was created; otherwise, <c>false</c>.</returns>
    public bool TryCreateFurniturePlacementRequest(
        CursorTarget startTarget,
        CursorTarget currentTarget,
        BuildOperation buildOperation,
        BuildInteractionMode buildInteractionMode,
        out FurniturePlacementRequest request)
    {
        _ = startTarget;
        _ = buildInteractionMode;

        if (buildOperation != BuildOperation.Place)
        {
            request = null!;
            return false;
        }

        request = new FurniturePlacementRequest(
            Definition,
            currentTarget.PlacementCell,
            Rotation);
        return true;
    }

    #endregion
}
