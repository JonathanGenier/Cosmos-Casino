using CosmosCasino.Core.Game.Build;
using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using System;
using System.Collections.Generic;

/// <summary>
/// Build context for generalized Core-owned structure definitions.
/// </summary>
public sealed class StructureBuildContext : BuildContextBase
{
    #region Initialization

    /// <summary>
    /// Initializes a new generalized structure build context.
    /// </summary>
    /// <param name="definition">The structure definition to place.</param>
    /// <param name="buildTool">The Client-side interaction tool used to resolve drag cells.</param>
    /// <param name="rotation">The selected footprint rotation.</param>
    public StructureBuildContext(
        StructureDefinition definition,
        StructureBuildTool buildTool,
        FootprintRotation rotation = FootprintRotation.Deg0)
    {
        ArgumentNullException.ThrowIfNull(definition);

        Definition = definition;
        BuildTool = buildTool;
        Rotation = rotation;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the structure definition placed by this context.
    /// </summary>
    public StructureDefinition Definition { get; }

    /// <summary>
    /// Gets the Client-side tool strategy used to resolve structure drag selections.
    /// </summary>
    public StructureBuildTool BuildTool { get; }

    /// <summary>
    /// Gets the selected footprint rotation.
    /// </summary>
    public FootprintRotation Rotation { get; private set; }

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

    #endregion

    #region Build Intent

    /// <summary>
    /// Attempts to create a generalized structure build intent from logical cursor targets.
    /// </summary>
    /// <param name="startTarget">The cursor target where the build operation started.</param>
    /// <param name="currentTarget">The current cursor target used to create the build intent.</param>
    /// <param name="buildOperation">The build operation to create.</param>
    /// <param name="buildInteractionMode">The active build interaction mode.</param>
    /// <param name="intent">The created build intent when successful.</param>
    /// <returns><see langword="true"/> when an intent was created; otherwise, <see langword="false"/>.</returns>
    public override bool TryCreateBuildIntent(
        CursorTarget startTarget,
        CursorTarget currentTarget,
        BuildOperation buildOperation,
        BuildInteractionMode buildInteractionMode,
        out BuildIntent intent)
    {
        IReadOnlyList<MapCellCoord> cells = StructureDragCellResolver.Resolve(
            BuildTool,
            buildOperation,
            buildInteractionMode,
            startTarget,
            currentTarget);

        if (cells.Count == 0)
        {
            intent = null!;
            return false;
        }

        switch (buildOperation)
        {
            case BuildOperation.Place:
                var placements = new StructurePlacementRequest[cells.Count];

                for (int i = 0; i < cells.Count; i++)
                {
                    placements[i] = new StructurePlacementRequest(
                        Definition,
                        cells[i],
                        Rotation);
                }

                intent = BuildIntent.PlaceStructures(placements);
                return true;

            case BuildOperation.Remove:
                intent = BuildIntent.RemoveStructuresAt(cells);
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
