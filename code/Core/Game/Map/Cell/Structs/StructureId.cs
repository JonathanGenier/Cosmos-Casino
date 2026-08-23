namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Identifies a structure occupying or reserving a local map cell.
    /// </summary>
    /// <param name="Value">The externally supplied structure identity value.</param>
    internal readonly record struct StructureId(int Value);
}
