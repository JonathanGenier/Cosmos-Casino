
/// <summary>
/// Provides constant paths to core scenes used throughout the application, including boot, main menu, and gameplay
/// scenes.
/// </summary>
/// <remarks>Use these constants to reference scene files when loading or switching scenes programmatically. This
/// helps avoid hard-coded paths and ensures consistency across the codebase.</remarks>
public static class ScenePaths
{
    /// <summary>
    /// Represents the resource path to the boot scene used during application startup.
    /// </summary>
    public const string Boot = "res://scenes/boot/boot_scene.tscn";

    /// <summary>
    /// Represents the resource path to the main menu scene.
    /// </summary>
    public const string MainMenu = "res://scenes/main_menu/main_menu_scene.tscn";

    /// <summary>
    /// Represents the resource path to the main game scene.
    /// </summary>
    public const string Game = "res://scenes/game/game_scene.tscn";
}
