using CosmosCasino.Core.Services;
using Godot;
using System;

/// <summary>
/// Base class for scene-level controllers attached to the scene tree.
/// Provides access to core and client services once the scene
/// has been initialized.
/// </summary>
public abstract partial class SceneController : Node
{
    #region PROPERTIES

    /// <summary>
    /// Core services shared across client and non-client systems.
    /// Available after initialization.
    /// </summary>>
    protected CoreServices CoreServices { get; private set; }

    /// <summary>
    /// Client-specific services available during scene runtime.
    /// Available after initialization.
    /// </summary>
    protected ClientServices ClientServices { get; private set; }

    #endregion

    #region METHODS

    /// <summary>
    /// Initializes the scene controller with required service collections.
    /// This method must be called before the controller is used.
    /// </summary>
    /// <param name="coreServices">
    /// Core services shared across the application.
    /// </param>
    /// <param name="clientServices">
    /// Client-specific services available during scene runtime.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either service collection is <c>null</c>.
    /// </exception>
    public void Initialize(CoreServices coreServices, ClientServices clientServices)
    {
        CoreServices = coreServices ?? throw new ArgumentNullException(nameof(coreServices));
        ClientServices = clientServices ?? throw new ArgumentNullException(nameof(clientServices));
    }

    #endregion
}
