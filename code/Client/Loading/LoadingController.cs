using CosmosCasino.Core.Services;
using Godot;
using System;
using System.Threading.Tasks;

public partial class LoadingController : Node
{
    public override void _Ready()
    {
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var coreServices = AppManager.Instance.CoreServices;

        GD.Print("Loading: Loading...");

        // Decide new game vs load game
        StartNewGame(coreServices);

        // Later:
        // core.Game.LoadSave(...)
        // core.Game.InitializeWorld()

        GD.Print("Loading: Loading complete");

        AppManager.Instance.CallDeferred(nameof(AppManager.ChangeState), (int)AppState.Game);
    }

    private void StartNewGame(CoreServices coreServices)
    {
        coreServices.StartNewGame(coreServices.SaveManager);
        GD.Print("Starting new game...");
    }
}
