using CosmosCasino.Core.Services;
using Godot;
using System;

/// <summary>
/// Base class for client-side managers attached to the scene tree.
/// Provides access to core and client service collections during
/// client runtime initialization.
/// </summary>
internal abstract partial class ClientManager : Node
{
    #region CONSTRUCTORS

    /// <summary>
    /// Initializes a client manager with access to required
    /// core and client services.
    /// </summary>
    /// <param name="bootstrap">
    /// Bootstrap context providing initialized service collections.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when required services are missing from the bootstrap context.
    /// </exception>
    protected ClientManager(ClientBootstrap bootstrap)
    {
        CoreServices = bootstrap.CoreServices ?? throw new ArgumentNullException(nameof(bootstrap.CoreServices));
        ClientServices = bootstrap.ClientServices ?? throw new ArgumentNullException(nameof(bootstrap.ClientServices));
    }

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Core services shared across client and non-client systems.
    /// </summary>
    protected CoreServices CoreServices { get; }

    /// <summary>
    /// Client-specific services available during runtime.
    /// </summary>
    protected ClientServices ClientServices { get; }

    #endregion
}