using Godot;
using System.Threading.Tasks;

public partial class BootController : Node
{
    public override void _Ready()
    {
        _ = BootAsync();
    }

    private async Task BootAsync()
    {
        GD.Print("Boot: Loading...");
        
        //LoadConfig();
        //LoadUserPreferences();
        //LoadLocalization();
        //LoadFeatureFlags();
        //CheckBuildVersion();

        //await WarmUpShaders();
        //await PreloadResources();

        //InitializeCaches();

        GD.Print("Boot: Loading complete");

        AppManager.Instance.CallDeferred(nameof(AppManager.ChangeState), (int)AppState.MainMenu);
    }
}
