using CosmosCasino.Core.Game.Map;

namespace CosmosCasino.Core.Game.Furniture
{
    /// <summary>
    /// Immutable domain read model for one authoritative furniture instance.
    /// </summary>
    /// <param name="Id">The authoritative furniture identity.</param>
    /// <param name="Definition">The immutable definition shared by this furniture type.</param>
    /// <param name="Anchor">The global logical map-cell anchor.</param>
    /// <param name="Rotation">The furniture footprint rotation.</param>
    public readonly record struct FurnitureSnapshot(
        FurnitureId Id,
        FurnitureDefinition Definition,
        MapCellCoord Anchor,
        FootprintRotation Rotation);
}
