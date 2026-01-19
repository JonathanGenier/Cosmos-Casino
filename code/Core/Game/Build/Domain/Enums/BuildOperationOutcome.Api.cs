namespace CosmosCasino.Core.Game.Build.Domain
{
    /// <summary>
    /// Specifies the possible outcomes of a build operation.
    /// </summary>
    /// <remarks>Use this enumeration to determine whether a build operation was successful, resulted in no
    /// changes, or was invalid. The values can be used to control subsequent logic based on the result of the build
    /// process.</remarks>
    public enum BuildOperationOutcome
    {
        /// <summary>
        /// Gets a value indicating whether the current state is valid.
        /// </summary>
        Valid,

        /// <summary>
        /// Represents a no-operation (no-op) action or placeholder that performs no work when invoked.
        /// </summary>
        /// <remarks>Use this type to indicate an intentional lack of behavior or as a default
        /// implementation where an operation is required but no action is needed. This can be useful in scenarios such
        /// as testing, stubbing, or providing optional extensibility points.</remarks>
        NoOp,

        /// <summary>
        /// Represents an invalid or uninitialized value.
        /// </summary>
        Invalid,
    }
}
