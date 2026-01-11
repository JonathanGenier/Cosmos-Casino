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
    /// <summary>
    /// Application startup phase where app systems are initialized.
    /// </summary>
    Boot,

    /// <summary>
    /// Main menu phase where the user can start or configure a game.
    /// </summary>
    MainMenu,

    /// <summary>
    /// Active gameplay state where the game loop is running.
    /// </summary>
    Game
}