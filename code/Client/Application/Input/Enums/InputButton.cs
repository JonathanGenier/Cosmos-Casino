/// <summary>
/// Specifies the available input buttons and controls for user interaction by keyboard and mouse.
/// </summary>
/// <remarks>The <see cref="InputButton.Count"/> member represents the total number of entries in 
/// the enumeration and should not be removed or reordered, as it is used for iteration and validation purposes.</remarks>
public enum InputButton
{
    #region Modifiers

    /// <summary>
    /// Represents the Shift modifier key.
    /// </summary>
    Shift,

    /// <summary>
    /// Represents the Ctrl modifier key.
    /// </summary>
    Ctrl,

    /// <summary>
    /// Represents the Alt modifier key.
    /// </summary>
    Alt,

    #endregion

    #region Mouse Input

    /// <summary>
    /// Represents the primary mouse button, typically the left button on most devices.
    /// </summary>
    Primary,

    /// <summary>
    /// Represents the secondary mouse button, typically the right button on most devices.
    /// </summary>
    Secondary,

    /// <summary>
    /// Represnets the middle mouse button, often associated with the scroll wheel click.
    /// </summary>
    Middle,

    #endregion

    #region Keyboard Input

    /// <summary>
    /// Represents the keyboard input action for moving forward.
    /// </summary>
    MoveForward,

    /// <summary>
    /// Represents the keyboard input action for moving backward.
    /// </summary>
    MoveBackward,

    /// <summary>
    /// Represents the keyboard input action for moving left.
    /// </summary>
    MoveLeft,

    /// <summary>
    /// Represents the keyboard input action for moving right.
    /// </summary>
    MoveRight,

    /// <summary>
    /// Represents the keyboard input action for rotating the camera to the left.
    /// </summary>
    RotateLeft,

    /// <summary>
    /// Represents the keyboard input action for rotating the camera to the right.
    /// </summary>
    RotateRight,

    #endregion

    #region Console Input

    /// <summary>
    /// Represents the keyboard input action for toggling the in-application console UI.
    /// </summary>
    ToggleConsole,

    #endregion

    #region Enum Count

    /// <summary>
    /// Represents the total number of entries in the enumeration.
    /// </summary>
    /// <remarks> DO NOT remove OR reorder this entry. This value is intended for 
    /// internal use to determine the size of the enumeration. It should not be used 
    /// as a valid enumeration value in application logic. </remarks>
    Count

    #endregion
}