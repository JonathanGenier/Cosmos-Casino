
/// <summary>
/// Centralized collection of scene path constants used by the application.
/// <para>
/// <see cref="ScenePaths"/> provides a single source of truth for all scene
/// resource paths, preventing hard-coded strings from being scattered
/// throughout the client layer.
/// </para>
/// </summary>
public static class ScenePaths
{
    /// <summary>
    /// Scene displayed during application boot and early initialization.
    /// </summary>
    public const string Boot = "res://scenes/boot.tscn";

    /// <summary>
    /// Main menu scene used for primary user navigation.
    /// </summary>
    public const string MainMenu = "res://scenes/main_menu.tscn";

    /// <summary>
    /// Transitional loading scene displayed while preparing a game session.
    /// </summary>
    public const string Loading = "res://scenes/loading.tscn";

    /// <summary>
    /// Main gameplay scene.
    /// </summary>
    public const string Game = "res://scenes/game.tscn";
}
