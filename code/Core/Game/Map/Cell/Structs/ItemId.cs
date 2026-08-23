namespace CosmosCasino.Core.Game.Map
{
    /// <summary>
    /// Identifies an item occupying or reserving a local map cell.
    /// </summary>
    /// <param name="Value">The externally supplied item identity value.</param>
    internal readonly record struct ItemId(int Value);
}
