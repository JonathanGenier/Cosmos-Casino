using CosmosCasino.Core.Console.Logging;
using CosmosCasino.Core.Services;
using Godot;
using System;
using static System.Formats.Asn1.AsnWriter;

/// <summary>
/// Container for all client-side services and presentation-layer systems.
/// Acts as the composition root for the layer, owning the lifecycle of 
/// Godot-dependent systems such as input, UI, camera control, audio, and 
/// debug tooling.
/// </summary>
public sealed partial class ClientServices : Node
{
    #region FIELDS

    private ClientBootstrap _bootstrap;

    #endregion

    #region CONSTRUCTORS

    /// <summary>
    /// Creates the client services container and prepares the bootstrap
    /// context used by client-side managers.
    /// </summary>
    /// <param name="core">
    /// Core services shared across all application layers.
    /// </param>
    public ClientServices(CoreServices core)
    {
        _bootstrap = new(core, this);
    }

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Centralized input manager for the client layer.
    /// Provides a single entry point for input intent detection and dispatch,
    /// while delegating input logic to registered input modules.
    /// </summary>
    public InputManager InputManager { get; private set; }

    /// <summary>
    /// Central coordinator for all client-side UI systems.
    /// Responsible for instantiating, owning, and coordinating UI controllers
    /// such as debug overlays, menus, and HUD elements.
    /// </summary>
    public UiManager UiManager { get; private set; }

    /// <summary>
    /// Scene-scoped camera coordination manager.
    /// <see cref="CameraManager"/> is instantiated only while a game session
    /// is active and is attached directly to the active game scene.
    /// This manager bridges high-level camera input intent to the
    /// scene-level <see cref="CameraRig"/>, without owning camera
    /// infrastructure or scene hierarchy.
    /// </summary>
    public CameraManager CameraManager { get; private set; }

    /// <summary>
    /// Manages primary world interaction gestures and routes them to the
    /// currently active interaction tool (e.g. selection, build).
    /// This manager owns the gesture lifecycle (start, update, end, cancel)
    /// but does not interpret gameplay meaning, issue commands, or manage
    /// global input such as right-click or escape.
    /// </summary>
    public InteractionManager InteractionManager { get; private set; }

    /// <summary>
    /// Shared client-side build context representing the currently selected
    /// build intent.
    /// This context is updated by UI-driven systems and consumed by
    /// interaction handlers to derive concrete build commands.
    /// </summary>
    public BuildContext BuildContext { get; private set; }

    /// <summary>
    /// Provides access to the client-side build manager responsible for
    /// issuing build intents and reacting to build results on the client,
    /// without directly mutating authoritative game state.
    /// </summary>
    public ClientBuildManager ClientBuildManager { get; private set; }

    #endregion

    #region METHODS

    /// <summary>
    /// Initializes client-side systems owned by this container.
    /// Child nodes created here are guaranteed to be part of the scene tree
    /// before other client systems begin processing.
    /// </summary>
    /// <inheritdoc/>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(ClientServices)))
        {
            InputManager = AddService(new InputManager(_bootstrap), nameof(InputManager));
            UiManager = AddService(new UiManager(_bootstrap), nameof(UiManager));
        }
    }

    /// <summary>
    /// Initializes client-side systems that are scoped to an active game session.
    /// This method is called when the application transitions into
    /// <see cref="AppState.Game"/> and the game scene has been instantiated
    /// and added to the scene tree.
    /// Scene-scoped managers created here are attached directly to the
    /// provided scene node and are expected to be destroyed automatically
    /// when the scene is unloaded.
    /// </summary>
    /// <param name="scene">
    /// Root node of the active game scene to which scene-scoped services
    /// should be attached.
    /// </param>
    public void StartGame(Node scene)
    {
        ConsoleLog.System(nameof(ClientServices), "Starting game...");

        if (CameraManager != null)
        {
            ConsoleLog.Warning(nameof(ClientServices), $"{nameof(CameraManager)} already exists. Skipping creation.");
            return;
        }

        CameraManager = AddServiceToScene(scene, new CameraManager(_bootstrap), nameof(CameraManager));
        CursorService.Initialize(this, buildableCollisionMask: CollisionLayers.Buildable, planeHeight: 0f);
        BuildContext = new BuildContext();
        ClientBuildManager = AddServiceToScene(scene, new ClientBuildManager(_bootstrap), nameof(ClientBuildManager));
        InteractionManager = AddServiceToScene(scene, new InteractionManager(_bootstrap), nameof(InteractionManager));
        UiManager.LoadGameUI();
    }

    /// <summary>
    /// Cleans up references to scene-scoped client services when a game
    /// session ends.
    /// This method is invoked when leaving <see cref="AppState.Game"/>.
    /// Scene-owned nodes are expected to be freed automatically as part
    /// of scene teardown; this method only clears internal references
    /// held by <see cref="ClientServices"/>.
    /// </summary>
    public void EndGame()
    {
        CameraManager = null;
    }

    /// <summary>
    /// Attaches a client-side service node to the <see cref="ClientServices"/>
    /// container and returns the attached instance.
    /// Services added through this method are considered application-scoped
    /// and persist for the lifetime of the client layer, surviving scene
    /// transitions.
    /// </summary>
    /// <typeparam name="T">
    /// Concrete type of the service node being added.
    /// </typeparam>
    /// <param name="service">
    /// Instance of the service node to attach to the client services container.
    /// </param>
    /// <param name="name">
    /// Node name to assign to the service within the scene tree.
    /// </param>
    /// <returns>
    /// The same service instance after it has been added as a child node.
    /// </returns>
    private T AddService<T>(T service, string name)
        where T : Node
    {
        service.Name = name;
        AddChild(service);
        return service;
    }

    /// <summary>
    /// Attaches a client-side service node directly to a scene node and
    /// returns the attached instance.
    /// This helper is intended for services whose lifetime is bound to
    /// a specific scene (e.g. game-session managers such as camera or
    /// audio controllers), rather than the application as a whole.
    /// The target scene must already be part of the scene tree at the
    /// time of attachment; otherwise an exception is thrown.
    /// </summary>
    /// <typeparam name="T">
    /// Concrete type of the service node being added.
    /// </typeparam>
    /// <param name="scene">
    /// Scene node that will own the service for its lifetime.
    /// </param>
    /// <param name="service">
    /// Instance of the service node to attach.
    /// </param>
    /// <param name="name">
    /// Node name to assign to the service within the scene.
    /// </param>
    /// <returns>
    /// The same service instance after it has been attached to the scene.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the target scene is not yet part of the scene tree.
    /// </exception>
    private T AddServiceToScene<T>(Node scene, T service, string name)
        where T : Node
    {
        if (!scene.IsInsideTree())
        {
            throw new InvalidOperationException("Scene must be in tree before attaching services.");
        }

        service.Name = name;
        scene.AddChild(service);
        return service;
    }
    #endregion
}
