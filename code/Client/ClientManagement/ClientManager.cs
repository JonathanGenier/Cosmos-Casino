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
    /// Adds a node as a managed child of this manager, assigns its name,
    /// and returns the node for fluent initialization.
    /// </summary>
    /// <typeparam name="T">
    /// Type of node being added.
    /// </typeparam>
    /// <param name="node">
    /// Node instance to add as a child.
    /// </param>
    /// <param name="name">
    /// Name to assign to the node in the scene tree.
    /// </param>
    /// <returns>
    /// The added node instance.
    /// </returns>
    protected T AddOwnedNode<T>(T node, string name)
        where T : Node
    {
        node.Name = name;
        AddChild(node);
        return node;
    }

    #endregion
}