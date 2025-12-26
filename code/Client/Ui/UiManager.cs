/// <summary>
/// Central coordinator for client-side user interface systems.
/// <see cref="UiManager"/> acts as the composition root for UI controllers,
/// owning their lifecycle and coordinating responses to UI-related input
/// intents without directly handling input polling or UI rendering logic.
/// This class is responsible for instantiating long-lived UI elements
/// (such as debug overlays or menus) and delegating visibility or behavior
/// changes to their respective controllers.
/// </summary>
internal partial class UiManager(ClientBootstrap bootstrap) : ClientManager(bootstrap)
{
    private ClientConsoleManager _clientConsoleManager;

    /// <inheritdoc/>
    public override void _Ready()
    {
        _clientConsoleManager = AddOwnedNode(new ClientConsoleManager(Bootstrap));
    }
}