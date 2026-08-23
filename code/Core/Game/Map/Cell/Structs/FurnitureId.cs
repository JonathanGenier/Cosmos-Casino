namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Identifies furniture occupying or reserving a local map cell.
    /// </summary>
    /// <param name="Value">The externally supplied furniture identity value.</param>
    internal readonly record struct FurnitureId(int Value);
}
