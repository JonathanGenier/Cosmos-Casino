using Godot;
using System;

/// <summary>
/// Provides an abstract base class for managing and organizing child nodes within a scene tree. Supports creation,
/// initialization, and structured addition of nodes.
/// </summary>
/// <remarks>NodeManager is intended to be subclassed to facilitate advanced node management scenarios, such as
/// dynamic scene composition or custom initialization workflows. It extends the Node class and offers protected methods
/// to streamline node creation and hierarchy management for derived classes.</remarks>
public abstract partial class NodeManager : Node
{
    #region Create Node Methods

    /// <summary>
    /// Creates a new node of type T, applies the specified initialization action, and returns the initialized node.
    /// </summary>
    /// <typeparam name="T">The type of node to create. Must inherit from Node and have a parameterless constructor.</typeparam>
    /// <param name="init">An action that performs initialization on the newly created node. Cannot be null.</param>
    /// <returns>The initialized node of type T.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the created node implements InitializableNodeManager and is not initialized after the initialization
    /// action is applied.</exception>
    protected T CreateInitializableNode<T>(Action<T> init)
        where T : Node, new()
    {
        var node = CreateNode<T>();
        init(node);

        if (node is InitializableNodeManager m && !m.IsInitialized)
        {
            throw new InvalidOperationException($"{typeof(T).Name} was not initialized.");
        }

        return node;
    }

    /// <summary>
    /// Creates a new instance of the specified node type and assigns its name to the type's name.
    /// </summary>
    /// <typeparam name="T">The type of node to create. Must derive from Node and have a parameterless constructor.</typeparam>
    /// <returns>A new instance of type T with its Name property set to the type's name.</returns>
    protected T CreateNode<T>()
        where T : Node, new()
    {
        var node = new T();
        node.Name = typeof(T).Name;
        return node;
    }

    #endregion

    #region Add Node Methods

    /// <summary>
    /// Adds the specified node as a child of the current node and assigns its name based on its type.
    /// </summary>
    /// <remarks>The node's Name property is set to the type name of T before it is added as a child. This
    /// method is intended for use within derived classes to facilitate node creation and hierarchy
    /// management.</remarks>
    /// <typeparam name="T">The type of node to add. Must inherit from Node.</typeparam>
    /// <param name="node">The node instance to add as a child. Cannot be null.</param>
    /// <returns>The node that was added as a child.</returns>
    protected T AddNode<T>(T node)
        where T : Node
    {
        node.Name = typeof(T).Name;
        AddChild(node);
        return node;
    }

    /// <summary>
    /// Creates a new child node of the specified type, adds it to the current node, and returns the created node.
    /// </summary>
    /// <typeparam name="T">The type of node to create. Must be a subclass of Node and have a parameterless constructor.</typeparam>
    /// <returns>The newly created and added child node of type T.</returns>
    protected T AddNode<T>()
        where T : Node, new()
    {
        var node = CreateNode<T>();
        AddChild(node);
        return node;
    }

    /// <summary>
    /// Creates a new child node of the specified type, applies the provided initialization action, and adds it to the
    /// current node's children.
    /// </summary>
    /// <typeparam name="T">The type of node to create. Must be a subclass of Node with a parameterless constructor.</typeparam>
    /// <param name="init">An action that performs additional initialization on the newly created node before it is added as a child.
    /// Cannot be null.</param>
    /// <returns>The newly created and initialized child node of type T.</returns>
    protected T AddInitializableNode<T>(Action<T> init)
        where T : Node, new()
    {
        var node = CreateNode<T>();
        init(node);
        AddChild(node);
        return node;
    }

    #endregion
}