using CosmosCasino.Core.Map;

/// <summary>
/// Holds the current build intent selected by the player.
/// This class represents intent only and contains no world,
/// cursor, or execution logic.
/// </summary>
public sealed class BuildContext
{
    #region FIELDS

    private FloorType? _selectedFloor;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Whether a build intent is currently active.
    /// </summary>
    public bool HasIntent => _selectedFloor.HasValue;

    /// <summary>
    /// Gets the currently selected floor type, if any.
    /// </summary>
    public FloorType? SelectedFloor => _selectedFloor;

    #endregion

    #region METHODS

    /// <summary>
    /// Sets the active floor type to be used for subsequent build intents.
    /// This method replaces any previously selected build intent and does
    /// not perform validation or initiate build execution.
    /// </summary>
    /// <param name="floorType">
    /// The floor type selected for building.
    /// </param>
    public void SetFloor(FloorType floorType)
    {
        _selectedFloor = floorType;
    }

    /// <summary>
    /// Clears the current build intent.
    /// </summary>
    public void Clear()
    {
        _selectedFloor = null;
    }

    #endregion
}
