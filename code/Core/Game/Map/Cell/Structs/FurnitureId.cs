namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Identifies one authoritative furniture aggregate occupying or reserving map cells.
    /// </summary>
    /// <param name="Value">The Core-assigned furniture identity value.</param>
    public readonly record struct FurnitureId(int Value);
}
