/// <summary>
/// Represents the high-level lifecycle states of the application
/// <para>
/// <see cref="AppState"/> is used by <c>AppManager</c> to determine
/// which scene should be active and how the application should
/// transition between major phases such as booting, menus, loading,
/// and gameplay.
/// </para>
/// </summary>
public enum AppState
{
    None,
    Boot,
    MainMenu,
    Loading,
    Game
}