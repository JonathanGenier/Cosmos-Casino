using CosmosCasino.Core.Serialization;
using CosmosCasino.Core.Services;
using Godot;

public partial class AppManager : Node
{
    public static AppManager Instance { get; private set; }
    public CoreServices CoreServices { get; private set; }

    private AppState _state;

    public AppState State => _state;

    #region GODOT PROCESSES

    public override void _EnterTree()
    {
        if(Instance != null)
        {
            GD.PrintErr("Multiple instances of AppManager detected. There should only be one instance.");
            QueueFree();
            return;
        }

        Instance = this;
        _state = AppState.Boot;
        IntializeCoreServices();
    }

    #endregion

    public void ChangeState(int newState)
    {
        ChangeState((AppState)newState);
    }

    public void ChangeState(AppState newState)
    {
        if(_state == newState)
        {
            return;
        }

        GD.Print($"Changing AppState from {_state} to {newState}.");
        _state = newState;
        SceneLoader.Load(GetSceneForState(newState));
    }

    private static string GetSceneForState(AppState state)
    {
        return state switch
        {
            AppState.MainMenu => ScenePaths.MainMenu,
            AppState.Loading => ScenePaths.Loading,
            AppState.Game => ScenePaths.Game,
            _ => string.Empty
        };
    }

    private void IntializeCoreServices()
    {
        JsonSaveSerializer serializer = new();
        string savePath = OS.GetUserDataDir();
        CoreServices = new CoreServices(serializer, savePath);
    }
}
