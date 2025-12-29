using Godot;

/// <summary>
/// Scene-level camera rig responsible for composing camera nodes
/// and forwarding smoothed movement, rotation, and zoom behavior
/// to the active camera.
/// </summary>
public sealed partial class CameraRig : Node3D
{
    #region NODES

    private Node3D _cameraSocket;
    private Camera3D _camera;

    #endregion

    #region FIELDS

    private Vector2 _moveVelocity;
    private Vector2 _moveDirection;
    private float _rotateVelocity;
    private float _rotateDirection;
    private float _zoomVelocity;
    private float _zoomDirection;
    private float _zoomFactor = 0;

    #endregion

    #region METHODS

    /// <summary>
    /// Resolves camera-related nodes when the scene is ready.
    /// </summary>
    /// <inheritdoc/>
    public override void _Ready()
    {
        _cameraSocket = GetNode<Node3D>("Camera_Socket");
        _camera = GetNode<Camera3D>("Camera_Socket/Camera");
    }

    /// <summary>
    /// Processes camera movement, rotation, and zoom each frame
    /// using velocity-based smoothing.
    /// </summary>
    /// <param name="delta">
    /// Frame time in seconds.
    /// </param>
    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        MoveCamera(delta);
        RotateCamera(delta);
        ZoomCamera(delta);
    }

    /// <summary>
    /// Sets the target movement direction for the camera rig.
    /// </summary>
    /// <param name="direction">
    /// Normalized directional input vector.
    /// </param>
    public void SetMove(Vector2 direction)
    {
        _moveDirection = direction;
    }

    /// <summary>
    /// Sets the target rotation direction for the camera rig.
    /// </summary>
    /// <param name="direction">
    /// Rotation input value.
    /// </param>
    public void SetRotate(float direction)
    {
        _rotateDirection = direction;
    }

    /// <summary>
    /// Sets the target zoom direction for the camera.
    /// </summary>
    /// <param name="direction">
    /// Zoom input value.
    /// </param>
    public void SetZoom(float direction)
    {
        _zoomDirection = direction;
    }

    /// <summary>
    /// Applies smoothed translational movement to the camera rig
    /// based on input direction and acceleration settings.
    /// </summary>
    /// <param name="delta">
    /// Frame time in seconds.
    /// </param>
    private void MoveCamera(double delta)
    {
        // Accelerate smoothly toward target movement direction
        if (_moveDirection != Vector2.Zero)
        {
            _moveVelocity = _moveVelocity.Lerp(_moveDirection * CameraConfigs.CameraMoveSpeed, (float)delta * CameraConfigs.CameraAcceleration);
        }
        else
        {
            _moveVelocity *= Mathf.Exp(-CameraConfigs.CameraDeceleration * (float)delta);
        }

        Vector3 velocityDirection = (-_moveVelocity.Y * Transform.Basis.Z) + (_moveVelocity.X * Transform.Basis.X);
        Position += velocityDirection * (float)delta;
    }

    /// <summary>
    /// Applies smoothed rotational movement to the camera rig
    /// around the vertical axis.
    /// </summary>
    /// <param name="delta">
    /// Frame time in seconds.
    /// </param>
    private void RotateCamera(double delta)
    {
        // Ignore very small rotation input values to prevent jitter.
        if (Mathf.Abs(_rotateDirection) > 0.001f)
        {
            _rotateVelocity = Mathf.Lerp(_rotateVelocity, _rotateDirection * CameraConfigs.CameraRotationSpeed, (float)delta * CameraConfigs.CameraAcceleration);
        }
        else
        {
            _rotateVelocity *= Mathf.Exp(-CameraConfigs.CameraDeceleration * (float)delta);
        }

        Rotation = new Vector3(Rotation.X, Rotation.Y + (_rotateVelocity * (float)delta), Rotation.Z);
        _rotateDirection = 0;
    }

    /// <summary>
    /// Applies smoothed zooming behavior to the camera while
    /// enforcing configured zoom limits.
    /// </summary>
    /// <param name="delta">
    /// Frame time in seconds.
    /// </param>
    private void ZoomCamera(double delta)
    {
        // Smooth acceleration toward the zoom direction
        if (_zoomDirection != 0)
        {
            _zoomVelocity = Mathf.Lerp(_zoomVelocity, _zoomDirection * CameraConfigs.CameraZoomSpeed, (float)delta * CameraConfigs.CameraAcceleration);
        }
        else
        {
            _zoomVelocity *= Mathf.Exp(-CameraConfigs.CameraDeceleration * (float)delta);
        }

        // Prevent the zoom velocity from exceeding the intended speed
        // Prevents infinite small movements
        if (Mathf.Abs(_zoomVelocity) < 0.01f)
        {
            _zoomVelocity = 0;
        }

        float newZoom = _camera.Position.Z + (_zoomVelocity * (float)delta);
        _camera.Position = new Vector3(_camera.Position.X, _camera.Position.Y, Mathf.Clamp(newZoom, CameraConfigs.CameraZoomMin, CameraConfigs.CameraZoomMax));
        _zoomFactor = _camera.Position.Z * 0.1f;
        _zoomDirection = 0;
    }

    #endregion
}

