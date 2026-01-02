using CosmosCasino.Core.Console.Logging;
using Godot;

/// <summary>
/// Central coordinator for client-side user interface systems.
/// <see cref="UiManager"/> acts as the composition root for UI controllers,
/// owning their lifecycle and coordinating responses to UI-related input
/// intents without directly handling input polling or UI rendering logic.
/// This class is responsible for instantiating long-lived UI elements
/// (such as debug overlays or menus) and delegating visibility or behavior
/// changes to their respective controllers.
/// </summary>
public sealed partial class UiManager(ClientBootstrap bootstrap) : ClientManager(bootstrap)
{
    #region FIELDS

    private Viewport _viewport;
    private ConsoleUiManager _consoleUiManager;
    private BuildUiManager _buildUiManager;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// Indicates whether the cursor is currently blocked by UI interaction.
    /// <para>
    /// This property evaluates whether any UI control is currently
    /// hovered by the pointer and is used to prevent world-space
    /// cursor and pointer-driven input from reacting while the UI
    /// owns the pointer.
    /// </para>
    /// </summary>
    public bool IsCursorBlockedByUi => _viewport.GuiGetHoveredControl() != null;

    #endregion

    #region METHODS

    /// <summary>
    /// Initializes the UI manager and instantiates long-lived UI
    /// controllers owned by the client.
    /// <para>
    /// This method resolves the active viewport and attaches all
    /// managed UI components to the scene tree.
    /// </para>
    /// </summary>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(UiManager)))
        {
            _viewport = GetViewport();
            _consoleUiManager = AddOwnedNode(new ConsoleUiManager(Bootstrap), nameof(ConsoleUiManager));
        }
    }

    /// <summary>
    /// Instantiates and attaches UI controllers that are scoped to an
    /// active game session.
    /// This method is invoked when the client transitions into gameplay
    /// and is responsible for loading UI systems that depend on an
    /// initialized game world (such as build-mode interfaces).
    /// Scene-scoped UI managers created here are expected to be disposed
    /// automatically when the game session ends.
    /// </summary>
    public void LoadGameUI()
    {
        _buildUiManager = AddOwnedNode(new BuildUiManager(Bootstrap), nameof(BuildUiManager));
    }

    #endregion
}