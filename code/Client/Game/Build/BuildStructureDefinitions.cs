using CosmosCasino.Core.Game.Build.Domain;
using CosmosCasino.Core.Game.Map;
using CosmosCasino.Core.Game.Structures;
using System;

/// <summary>
/// Transitional client-owned structure definitions for the existing floor and wall tools.
/// </summary>
internal static class BuildStructureDefinitions
{
    #region Fields

    private static readonly MapCellFootprint SingleCellFootprint = new(new[]
    {
        new MapCellOffset(0, 0, 0)
    });

    #endregion

    #region Properties

    /// <summary>
    /// Gets the transitional floor structure definition identity.
    /// </summary>
    internal static StructureDefinitionId FloorDefinitionId { get; } = new(1);

    /// <summary>
    /// Gets the transitional wall structure definition identity.
    /// </summary>
    internal static StructureDefinitionId WallDefinitionId { get; } = new(2);

    /// <summary>
    /// Gets the transitional one-cell floor structure definition.
    /// </summary>
    internal static StructureDefinition Floor { get; } = new(FloorDefinitionId, SingleCellFootprint);

    /// <summary>
    /// Gets the transitional one-cell wall structure definition.
    /// </summary>
    internal static StructureDefinition Wall { get; } = new(WallDefinitionId, SingleCellFootprint);

    #endregion

    #region Resolution

    /// <summary>
    /// Attempts to map a structure definition identity back to the existing client build kind.
    /// </summary>
    /// <param name="definitionId">The structure definition identity.</param>
    /// <param name="buildKind">The matching build kind, if one is registered.</param>
    /// <returns><c>true</c> when the definition is registered with the current client build tools; otherwise, <c>false</c>.</returns>
    internal static bool TryGetBuildKind(StructureDefinitionId definitionId, out BuildKind buildKind)
    {
        if (definitionId == FloorDefinitionId)
        {
            buildKind = BuildKind.Floor;
            return true;
        }

        if (definitionId == WallDefinitionId)
        {
            buildKind = BuildKind.Wall;
            return true;
        }

        buildKind = default;
        return false;
    }

    /// <summary>
    /// Resolves the visual cell slot used by the existing spawn system for a structure definition.
    /// </summary>
    /// <param name="definitionId">The structure definition identity.</param>
    /// <returns>The client visual cell slot.</returns>
    /// <exception cref="NotSupportedException">Thrown when no visual slot is registered for the definition.</exception>
    internal static CellSlot GetCellSlot(StructureDefinitionId definitionId)
    {
        if (definitionId == FloorDefinitionId)
        {
            return CellSlot.Floor;
        }

        if (definitionId == WallDefinitionId)
        {
            return CellSlot.Wall;
        }

        throw new NotSupportedException($"No client cell slot is registered for structure definition {definitionId}.");
    }

    #endregion
}
