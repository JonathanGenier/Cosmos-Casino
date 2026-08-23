namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Identifies a structure occupying or reserving a local map cell.
    /// </summary>
    /// <param name="Value">The Core-assigned structure identity value.</param>
    public readonly record struct StructureId(int Value);
}
