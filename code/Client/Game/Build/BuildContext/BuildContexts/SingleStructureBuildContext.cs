using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using System;

/// <summary>
/// Build context for a single selected Core structure definition.
/// </summary>
public sealed class SingleStructureBuildContext : BuildContextBase, IStructureBuildIntentContext
{
    #region Initialization

    /// <summary>
    /// Initializes a new single-structure build context.
    /// </summary>
    /// <param name="definition">The structure definition to place.</param>
    /// <param name="rotation">The selected footprint rotation.</param>
    public SingleStructureBuildContext(
        StructureDefinition definition,
        FootprintRotation rotation = FootprintRotation.Deg0)
    {
        ArgumentNullException.ThrowIfNull(definition);

        Definition = definition;
        Rotation = rotation;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the structure definition placed by this context.
    /// </summary>
    public StructureDefinition Definition { get; }

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
    /// <returns><c>true</c> for placement and removal; otherwise, <c>false</c>.</returns>
    public override bool SupportsBuildOperation(BuildOperation buildOperation)
    {
        return buildOperation is BuildOperation.Place or BuildOperation.Remove;
    }

    #endregion

    #region Rotation

    /// <summary>
    /// Sets the footprint rotation used by future placement intents.
    /// </summary>
    /// <param name="rotation">The selected footprint rotation.</param>
    public void SetRotation(FootprintRotation rotation)
    {
        Rotation = rotation;
    }

    /// <summary>
    /// Rotates the selected footprint clockwise for future placement intents.
    /// </summary>
    /// <returns><c>true</c> when the rotation changed.</returns>
    public override bool TryRotateClockwise()
    {
        Rotation = GetNextClockwiseRotation(Rotation);
        return true;
    }

    #endregion

    #region Build Intent

    /// <summary>
    /// Attempts to create a single-structure build intent from the current logical cursor target.
    /// </summary>
    /// <param name="startTarget">The cursor target where the build operation started.</param>
    /// <param name="currentTarget">The current cursor target used to create the build intent.</param>
    /// <param name="buildOperation">The build operation to create.</param>
    /// <param name="buildInteractionMode">The active build interaction mode.</param>
    /// <param name="intent">The created build intent when successful.</param>
    /// <returns><see langword="true"/> when an intent was created; otherwise, <see langword="false"/>.</returns>
    public bool TryCreateBuildIntent(
        CursorTarget startTarget,
        CursorTarget currentTarget,
        BuildOperation buildOperation,
        BuildInteractionMode buildInteractionMode,
        out BuildIntent intent)
    {
        _ = startTarget;
        _ = buildInteractionMode;

        switch (buildOperation)
        {
            case BuildOperation.Place:
                intent = BuildIntent.PlaceStructure(
                    Definition,
                    currentTarget.PlacementCell,
                    Rotation);
                return true;

            case BuildOperation.Remove:
                intent = BuildIntent.RemoveStructureAt(currentTarget.TargetCell);
                return true;

            case BuildOperation.None:
                intent = null!;
                return false;

            default:
                throw new InvalidOperationException($"Unsupported build operation: {buildOperation}");
        }
    }

    #endregion

}
