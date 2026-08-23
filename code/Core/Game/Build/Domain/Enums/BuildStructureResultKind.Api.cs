namespace CosmosCasino.Core.Game.Build.Domain
{
    /// <summary>
    /// Describes the structure-level effect produced by a build result.
    /// </summary>
    public enum BuildStructureResultKind
    {
        /// <summary>
        /// The build result describes a newly created structure.
        /// </summary>
        Created,

        /// <summary>
        /// The build result describes a removed structure.
        /// </summary>
        Removed
    }
}
