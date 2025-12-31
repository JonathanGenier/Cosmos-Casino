using CosmosCasino.Core.Console.Logging;
using Godot;
using System;

/// <summary>
/// Provides a global, client-side access point for resolving the
/// current world-space cursor position.
/// <para>
/// The cursor service coordinates ray generation and resolution
/// strategies through a composed <see cref="CursorResolver"/> while
/// enforcing high-level input policy such as UI-based cursor blocking.
/// </para>
/// <para>
/// This service is stateless after initialization and exposes a
/// single query-style API intended to be consumed by input, camera,
/// and gameplay systems.
/// </para>
/// </summary>
public static class CursorService
{
    #region FIELDS

    private static CursorResolver _resolver;
    private static Func<bool> _isCursorBlockedByUi;

    #endregion

    #region METHODS

    /// <summary>
    /// Initializes the cursor service and configures all cursor
    /// resolution strategies.
    /// <para>
    /// This method must be called once during client startup before
    /// any cursor queries are performed.
    /// </para>
    /// </summary>
    /// <param name="clientService">
    /// Client service container used to provide UI state queries.
    /// </param>
    /// <param name="buildableCollisionMask">
    /// Physics collision mask identifying geometry that can be
    /// targeted by the cursor.
    /// </param>
    /// <param name="planeHeight">
    /// World-space height of the fallback cursor plane.
    /// </param>
    public static void Initialize(
        ClientServices clientService,
        uint buildableCollisionMask,
        float planeHeight = 0f)
    {
        var rayProvider = new CursorRayProvider();
        var physicsResolver = new CursorPhysicsResolver(buildableCollisionMask);
        var planeResolver = new CursorPlaneResolver(planeHeight);

        _isCursorBlockedByUi = () => clientService.UiManager.IsCursorBlockedByUi;
        _resolver = new CursorResolver(
            rayProvider,
            physicsResolver,
            planeResolver
        );
    }

    /// <summary>
    /// Attempts to retrieve the current world-space cursor position.
    /// <para>
    /// The query fails if the cursor is blocked by UI interaction or
    /// if the cursor service has not been initialized.
    /// </para>
    /// </summary>
    /// <param name="position">
    /// Output parameter that receives the resolved cursor position.
    /// </param>
    /// <returns>
    /// <c>true</c> if a valid cursor position was resolved; otherwise
    /// <c>false</c>.
    /// </returns>
    public static bool TryGetCursorPosition(out Vector3 position)
    {
        position = default;

        if (_resolver == null || _isCursorBlockedByUi == null)
        {
            ConsoleLog.Warning(nameof(CursorService), $"{nameof(CursorService)} not initialized.");
            return false;
        }

        if (_isCursorBlockedByUi())
        {
            return false;
        }

        return _resolver.TryResolve(out position);
    }

    #endregion
}
