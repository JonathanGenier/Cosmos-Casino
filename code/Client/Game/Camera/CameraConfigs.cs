/// <summary>
/// Defines static, non-runtime camera tuning values used to control
/// camera movement, rotation, zoom, and pitch constraints.
/// These values are intended for development-time balancing and
/// should remain constant during gameplay.
/// </summary>
public static class CameraConfigs
{
    /// <summary>
    /// Pixel distance from the screen edge within which the mouse
    /// will trigger camera panning.
    /// </summary>
    public const int CameraPanMargin = 16;

    /// <summary>
    /// Minimum downward pitch angle, in degrees, that the camera
    /// is allowed to reach.
    /// </summary>
    public const int CameraPitchMinDegree = 30;

    /// <summary>
    /// Maximum upward pitch angle, in degrees, that the camera
    /// is allowed to reach.
    /// </summary>
    public const int CameraPitchMaxDegree = 90;

    /// <summary>
    /// Base movement speed of the camera when translating
    /// across the world.
    /// </summary>
    public const float CameraMoveSpeed = 100.0f;

    /// <summary>
    /// Speed multiplier applied when rotating the camera.
    /// </summary>
    public const float CameraRotationSpeed = 3f;

    /// <summary>
    /// Speed at which the camera zooms in and out.
    /// </summary>
    public const float CameraZoomSpeed = 600f;

    /// <summary>
    /// Minimum zoom distance allowed, preventing the camera
    /// from getting too close to the scene.
    /// </summary>
    public const float CameraZoomMin = 5f;

    /// <summary>
    /// Maximum zoom distance allowed, preventing the camera
    /// from zooming too far out.
    /// </summary>
    public const float CameraZoomMax = 300f;

    /// <summary>
    /// Rate at which the camera accelerates toward its target
    /// movement speed.
    /// </summary>
    public const float CameraAcceleration = 5.0f;

    /// <summary>
    /// Rate at which the camera decelerates when movement input
    /// is released.
    /// </summary>
    public const float CameraDeceleration = 5.0f;
}
