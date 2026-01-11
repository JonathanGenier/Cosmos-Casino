using Godot;

/// <summary>
/// Provides functionality to generate a world-space ray from the active camera through the current mouse position.
/// </summary>
/// <remarks>Use this class to obtain a ray suitable for object picking or interaction based on the user's cursor
/// position in a 3D scene. The ray is constructed only if a valid camera and viewport are available.</remarks>
public sealed class CursorRayProvider
{
    #region Get

    /// <summary>
    /// Attempts to create a 3D ray from the current mouse position in the main camera's viewport.
    /// </summary>
    /// <remarks>This method requires that a main camera is present in the active scene. If no main camera or
    /// viewport is available, the method returns false and the output parameter is set to its default value.</remarks>
    /// <param name="ray">When this method returns, contains the resulting 3D ray if the operation succeeds; otherwise, contains the
    /// default value.</param>
    /// <returns>true if the ray was successfully created; otherwise, false.</returns>
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