using System;


/// <summary>
/// Provides centralized access to application services for the client layer, including input management and node
/// coordination.
/// </summary>
/// <remarks>AppServices acts as a service aggregator, exposing key functionality such as input handling through
/// the InputManager property. It is intended to be used as a single point of access for client-level services,
/// simplifying dependency management and promoting modularity. This class is typically instantiated and managed by the
/// application's node hierarchy.</remarks>
public sealed partial class AppServices : NodeManager
{
    #region Fields

    private InputManager? _inputManager;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the input manager used to process and manage user input events.
    /// </summary>
    public InputManager InputManager
    {
        get => _inputManager ?? throw new InvalidOperationException($"{nameof(InputManager)} accessed before initialization.");
        private set => _inputManager = value;
    }

    #endregion

    #region Godot Process

    /// <summary>
    /// Called when the node enters the scene tree for the first time. Use this method to perform initialization that
    /// requires the node to be part of the active scene.
    /// </summary>
    /// <remarks>This method is automatically invoked by the engine and should not be called directly.
    /// Override this method to set up node references, connect signals, or perform other setup tasks that depend on the
    /// node being added to the scene.</remarks>
    public override void _EnterTree()
    {
        if (_inputManager != null)
        {
            throw new InvalidOperationException($"{nameof(AppServices)} already initialized.");
        }

        InputManager = AddNode<InputManager>();
    }

    #endregion
}