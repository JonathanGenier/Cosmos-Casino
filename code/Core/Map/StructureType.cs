namespace CosmosCasino.Core.Map
{
    /// <summary>
    /// Identifies the type of structural element occupying a map cell.
    /// Structures represent permanent or semi-permanent constructions
    /// that block movement or define spaces.
    /// </summary>
    internal enum StructureType
    {
        /// <summary>
        /// A solid structure that fully blocks passage.
        /// </summary>
        Wall,

        /// <summary>
        /// A structure that allows controlled passage between cells.
        /// </summary>
        Door
    }
}
