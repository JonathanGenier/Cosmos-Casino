namespace CosmosCasino.Core.Save
{
    /// <summary>
    /// Represents a component that participates in the save and load pipeline
    /// by writing its own state to and restoring it from a shared
    /// <see cref="GameSaveData"/> instance.
    /// Each implementation is fully responsible for the structure,
    /// validity, and interpretation of its own save data.
    /// </summary>
    internal interface ISaveParticipant
    {
        #region METHODS

        /// <summary>
        /// Writes the current state of the participant into the provided
        /// <see cref="GameSaveData"/> instance.
        /// Implementations should write only their own data and must not
        /// modify or depend on data owned by other participants.
        /// </summary>
        /// <param name="save">
        /// The shared save container to write data into.
        /// </param>
        void WriteTo(GameSaveData save);

        /// <summary>
        /// Restores the participant's state from the provided
        /// <see cref="GameSaveData"/> instance.
        /// Implementations are responsible for validating and interpreting
        /// their own data and should throw if the required data is missing
        /// or invalid.
        /// </summary>
        /// <param name="save">
        /// The shared save container to read data from.
        /// </param>
        void ReadFrom(GameSaveData save);

        #endregion
    }
}
