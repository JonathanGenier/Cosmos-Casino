using Godot;

/// <summary>
/// Provides world-space cursor rays derived from the active 3D camera
/// and current mouse position.
/// <para>
/// This component is responsible solely for translating screen-space
/// mouse coordinates into a world-space ray suitable for cursor
/// resolution. It performs no collision testing or cursor policy
/// decisions.
/// </para>
/// <para>
/// The provider assumes a camera-driven cursor model and will fail
/// gracefully if no active camera is available.
/// </para>
/// </summary>
public sealed class CursorRayProvider
{
    #region METHODS

    /// <summary>
    /// Attempts to construct a world-space ray originating from the
    /// active camera and passing through the current mouse position.
    /// </summary>
    /// <param name="ray">
    /// Output parameter that receives the constructed world-space ray
    /// if successful.
    /// </param>
    /// <returns>
    /// <c>true</c> if a valid camera and viewport were available and
    /// the ray was constructed successfully; otherwise <c>false</c>.
    /// </returns>
    public bool TryGetRay(out Ray3D ray)
    {
        ray = default;
        var viewport = Engine.GetMainLoop() as SceneTree;

        if (viewport == null)
        {
            return false;
        }

        var camera = viewport.Root.GetViewport().GetCamera3D();

        if (camera == null)
        {
            return false;
        }

        Vector2 mousePosition = camera.GetViewport().GetMousePosition();
        Vector3 origin = camera.ProjectRayOrigin(mousePosition);
        Vector3 direction = camera.ProjectRayNormal(mousePosition);

        ray = new Ray3D(origin, direction);

        return true;
    }

    #endregion
}