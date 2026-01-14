using System;

/// <summary>
/// Provides methods and properties for managing the current build context within an application.
/// </summary>
/// <remarks>The BuildContext class allows for setting, retrieving, and clearing the active build context. It is
/// typically used to maintain contextual information relevant to build operations. This class is sealed and cannot be
/// inherited.</remarks>
public sealed class BuildContext
{
    #region Fields

    private BuildContextBase? _activeContext;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the build context changes.
    /// </summary>
    /// <remarks>Subscribers are notified whenever the associated build context is updated or replaced. The
    /// event handler receives the new build context, or <see langword="null"/> if the context is cleared.</remarks>
    public event Action<BuildContextBase?>? ContextChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the currently active build context, if one is available.
    /// </summary>
    public BuildContextBase? ActiveContext => _activeContext;

    #endregion

    #region Context Management

    /// <summary>
    /// Sets the active build context to the specified context instance.
    /// </summary>
    /// <remarks>If an active build context is already set, it will be replaced by the specified context. A
    /// warning is logged when replacing an existing context.</remarks>
    /// <param name="context">The build context to set as the active context. Cannot be null.</param>
    public void Set(BuildContextBase context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _activeContext = context;
        ContextChanged?.Invoke(_activeContext);
    }

    /// <summary>
    /// Clears the current active context, resetting it to its default state.
    /// </summary>
    public void Clear()
    {
        _activeContext = null;
        ContextChanged?.Invoke(null);
    }

    #endregion
}