using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Structures
{
    /// <summary>
    /// Immutable domain read model for one authoritative structure instance.
    /// </summary>
    /// <param name="Id">The authoritative structure identity.</param>
    /// <param name="Definition">The immutable definition shared by this structure type.</param>
    /// <param name="Anchor">The global logical map-cell anchor.</param>
    /// <param name="Rotation">The structure footprint rotation.</param>
    public readonly record struct StructureSnapshot(
        StructureId Id,
        StructureDefinition Definition,
        MapCellCoord Anchor,
        FootprintRotation Rotation);
}
