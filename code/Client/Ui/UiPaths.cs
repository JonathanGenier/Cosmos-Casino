/// <summary>
/// Centralized resource paths for client-side UI scenes.
/// This class provides stable, compile-time-safe references to UI scene
/// locations, avoiding hard-coded string literals scattered across the
/// codebase.
/// Paths defined here are intended to be used by UI composition and
/// coordination systems (such as <see cref="UiManager"/>) rather than
/// by individual UI controllers.
/// </summary>
public static class UiPaths
{
    /// <summary>
    /// Scene path for the debug log console UI.
    /// </summary>
    public const string Console = "res://ui/console/console_ui.tscn";
}
