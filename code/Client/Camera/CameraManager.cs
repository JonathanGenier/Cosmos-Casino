using CosmosCasino.Core.Console.Logging;
using Godot;

/// <summary>
/// Client-side manager responsible for coordinating camera input
/// and delegating movement, rotation, and zoom commands to the
/// active <see cref="CameraRig"/>.
/// </summary>
/// <param name="bootstrap">
/// Bootstrap context providing access to core and client services.
/// </param>
public sealed partial class CameraManager(ClientBootstrap bootstrap) : ClientManager(bootstrap)
{
    #region FIELDS

    private CameraRig _cameraRig;
    private bool _cameraEnabled = true;

    #endregion

    #region METHODS

    /// <summary>
    /// Resolves the camera rig and registers camera-related input
    /// handlers during scene initialization.
    /// </summary>
    /// <inheritdoc/>
    public override void _Ready()
    {
        using (ConsoleLog.SystemScope(nameof(CameraManager)))
        {
            _cameraRig = GetParent().GetNode<CameraRig>("Camera_Rig");

            if (_cameraRig == null)
            {
                ConsoleLog.Error(nameof(CameraManager), $"{nameof(CameraRig)} not found in {nameof(GameScene)}.");
                return;
            }

            var input = ClientServices.InputManager;
            input.MoveCamera += OnMoveCamera;
            input.RotateCamera += OnRotateCamera;
            input.ZoomCamera += OnZoomCamera;
        }
    }

    /// <summary>
    /// Unregisters camera-related input handlers when the manager
    /// is removed from the scene tree.
    /// </summary>
    /// <inheritdoc/>
    public override void _ExitTree()
    {
        var input = ClientServices.InputManager;
        input.MoveCamera -= OnMoveCamera;
        input.RotateCamera -= OnRotateCamera;
        input.ZoomCamera -= OnZoomCamera;
    }

    /// <summary>
    /// Enables or disables camera control input. When disabled,
    /// all active camera movement, rotation, and zoom are reset.
    /// </summary>
    /// <param name="enabled">
    /// Whether camera input should be processed.
    /// </param>
    public void SetCameraEnabled(bool enabled)
    {
        _cameraEnabled = enabled;

        if (!enabled)
        {
            _cameraRig.SetMove(Vector2.Zero);
            _cameraRig.SetRotate(0f);
            _cameraRig.SetZoom(0f);
        }
    }

    /// <summary>
    /// Handles directional camera movement input.
    /// </summary>
    /// <param name="direction">
    /// Normalized movement vector describing camera translation intent.
    /// </param>
    private void OnMoveCamera(Vector2 direction)
    {
        if (!_cameraEnabled)
        {
            return;
        }

        _cameraRig.SetMove(direction);
    }

    /// <summary>
    /// Handles camera rotation input.
    /// </summary>
    /// <param name="direction">
    /// Rotation direction or magnitude provided by input.
    /// </param>
    private void OnRotateCamera(float direction)
    {
        if (!_cameraEnabled)
        {
            return;
        }

        _cameraRig.SetRotate(direction);
    }

    /// <summary>
    /// Handles camera zoom input.
    /// </summary>
    /// <param name="direction">
    /// Zoom direction or magnitude provided by input.
    /// </param>
    private void OnZoomCamera(float direction)
    {
        if (!_cameraEnabled)
        {
            return;
        }

        _cameraRig.SetZoom(direction);
    }

    #endregion
}
