using CosmosCasino.Core.Services;

/// <summary>
/// Coordinates client-side application startup by providing access
/// to initialized core and client service collections during bootstrap.
/// </summary>
public sealed class ClientBootstrap
{
    #region CONSTRUCTORS

    /// <summary>
    /// Initializes a new client bootstrap context with the provided
    /// service collections.
    /// </summary>
    /// <param name="coreServices">
    /// Initialized core service container.
    /// </param>
    /// <param name="clientServices">
    /// Initialized client service container.
    /// </param>
    public ClientBootstrap(CoreServices coreServices, ClientServices clientServices)
    {
        CoreServices = coreServices;
        ClientServices = clientServices;
    }

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Core services required by both client and non-client systems.
    /// </summary>
    public CoreServices CoreServices { get; }

    /// <summary>
    /// Client-specific services used during application startup.
    /// </summary>
    public ClientServices ClientServices { get; }

    #endregion
}
