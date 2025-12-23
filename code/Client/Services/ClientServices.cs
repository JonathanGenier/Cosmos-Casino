using Godot;

/// <summary>
/// Container for all client-side services and presentation-layer systems.
/// Acts as the composition root for the layer, owning the lifecycle of 
/// Godot-dependent systems such as input, UI, camera control, audio, and 
/// debug tooling.
/// </summary>
public partial class ClientServices : Node
{
    #region PROPERTIES

    /// <summary>
    /// Centralized input manager for the client layer.
    /// Provides a single entry point for input intent detection and dispatch,
    /// while delegating input logic to registered input modules.
    /// </summary>
    public InputManager Input { get; private set; }

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Initializes client-side systems owned by this container.
    /// Child nodes created here are guaranteed to be part of the scene tree
    /// before other client systems begin processing.
    /// </summary>
    public override void _Ready()
    {
        Input = new InputManager();
        AddChild(Input);
    }

    #endregion
}
