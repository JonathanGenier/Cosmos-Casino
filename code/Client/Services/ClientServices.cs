using CosmosCasino.Core.Debug.Logging;
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

    /// <summary>
    /// Central coordinator for all client-side UI systems.
    /// Responsible for instantiating, owning, and coordinating UI controllers
    /// such as debug overlays, menus, and HUD elements.
    /// </summary>
    public UiManager Ui { get; private set; }

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Initializes client-side systems owned by this container.
    /// Child nodes created here are guaranteed to be part of the scene tree
    /// before other client systems begin processing.
    /// </summary>
    public override void _Ready()
    {
        DevLog.System("ClientServices", "Setting up...");
        Input = AddService(new InputManager());
        Ui = AddService(new UiManager());
        DevLog.System("ClientServices", "Ready");
    }

    #endregion

    #region PRIVATE METHODS

    /// <summary>
    /// Adds a client-side service node to the scene tree and returns it.
    /// This helper method centralizes service attachment logic, ensuring
    /// that all managed client systems are consistently added as children
    /// of this node and reducing the risk of lifecycle or ownership errors.
    /// </summary>
    /// <typeparam name="T">
    /// Concrete type of the service node being added.
    /// </typeparam>
    /// <param name="node">
    /// Instance of the service node to attach to the scene tree.
    /// </param>
    /// <returns>
    /// The same node instance after it has been added as a child.
    /// </returns>
    private T AddService<T>(T node)
        where T : Node
    {
        AddChild(node);
        return node;
    }

    #endregion
}
