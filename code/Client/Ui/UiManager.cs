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

    private ClientConsoleManager _clientConsoleManager;
    private Viewport _viewport;

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
            _clientConsoleManager = AddOwnedNode(new ClientConsoleManager(Bootstrap), nameof(ClientConsoleManager));
        }
    }

    #endregion
}