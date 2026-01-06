namespace CosmosCasino.Core.Build
{
    /// <summary>
    /// Specifies the type of build action to perform for a given
    /// build intent.
    /// </summary>
    public enum BuildOperation
    {
        /// <summary>
        /// Places new content on the target cells or replaces
        /// existing content when allowed by the build rules.
        /// </summary>
        Place,

        /// <summary>
        /// Removes existing content from the target cells when
        /// removal conditions are satisfied.
        /// </summary>
        Remove
    }
}
