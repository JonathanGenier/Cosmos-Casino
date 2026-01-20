using System;

/// <summary>
/// Represents the current physical input state for a single frame.
/// This type is purely factual and contains no gameplay or semantic meaning.
/// </summary>
public sealed class InputState
{
    #region Fields

    private readonly bool[] _buttons = new bool[(int)InputButton.Count];
    private float _scrollDelta;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the accumulated mouse wheel delta for the current frame.
    /// This value is frame-scoped and is reset after processing.
    /// </summary>
    public float ScrollDelta
    {
        get => _scrollDelta;
        set => _scrollDelta = value;
    }

    #endregion

    #region Indexer

    /// <summary>
    /// Gets or sets the pressed state of a specific input button.
    /// </summary>
    /// <param name="button">The input button to query or update.</param>
    /// <returns>true if the button is currently pressed; otherwise, false.</returns>
    public bool this[InputButton button]
    {
        get => _buttons[(int)button];
        set => _buttons[(int)button] = value;
    }

    #endregion

    #region State Management

    /// <summary>
    /// Copies all persistent button state into another input state instance.
    /// Frame-scoped values are intentionally reset on the destination.
    /// </summary>
    /// <param name="destination">The input state that will receive the copied data.</param>
    public void CopyTo(InputState destination)
    {
        Array.Copy(_buttons, destination._buttons, _buttons.Length);
        destination._scrollDelta = 0f;
    }

    /// <summary>
    /// Clears all frame-scoped input values.
    /// This method must be called once per frame after input processing.
    /// </summary>
    public void ResetFrameState()
    {
        _scrollDelta = 0f;
    }

    #endregion
}