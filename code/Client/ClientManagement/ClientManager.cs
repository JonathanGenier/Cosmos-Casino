using CosmosCasino.Core.Services;
using Godot;
using System;

/// <summary>
/// Base class for client-side managers attached to the scene tree.
/// Provides access to core and client service collections during
/// client runtime initialization.
/// </summary>
public abstract partial class ClientManager : Node
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
        Bootstrap = bootstrap;
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

    /// <summary>
    /// Bootstrap context used to initialize this manager.
    /// Exposed for derived managers that need to pass the same
    /// context to child managers or owned nodes.
    /// </summary>
    protected ClientBootstrap Bootstrap { get; }

    #endregion

    #region METHODS

    /// <summary>
    /// Adds a node owned by this manager to the scene tree and
    /// returns the same instance for fluent initialization.
    /// <para>
    /// This helper exists to make ownership explicit and to
    /// standardize how managers attach long-lived child nodes
    /// such as UI controllers or sub-managers.
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// Concrete node type being added.
    /// </typeparam>
    /// <param name="node">
    /// Node instance to attach as a child of this manager.
    /// </param>
    /// <returns>
    /// The same node instance after it has been added to the scene tree.
    /// </returns>
    protected T AddOwnedNode<T>(T node)
        where T : Node
    {
        AddChild(node);
        return node;
    }

    #endregion
}