using Godot;
using System;

/// <summary>
/// Provides a 3D camera rig node that supports smooth movement, rotation, and zooming in a scene. Designed for use as a
/// controllable camera system in 3D environments.
/// </summary>
/// <remarks>The CameraRig node manages a Camera3D and exposes methods to set movement, rotation, and zoom
/// directions, which are then applied with velocity-based smoothing each frame. It is intended to be used as a parent
/// node for a camera in Godot projects, allowing for responsive and natural-feeling camera controls. The rig enforces
/// configurable speed, acceleration, and zoom limits, which can be adjusted via the associated CameraConfigs settings.
/// This class is not thread-safe and should be used from the main scene tree thread.</remarks>
public sealed partial class CameraRig : Node3D
{
    #region Nodes

    private Node3D? _cameraSocket;
    private Camera3D? _camera;

    #endregion

    #region Fields

    private bool _isInitialized = false;
    private Vector2 _moveVelocity;
    private Vector2 _moveDirection;
    private float _rotateVelocity;
    private float _rotateDirection;
    private float _zoomVelocity;
    private float _zoomDirection;
    private float _zoomFactor = 0;

    #endregion

    #region Properties

    private Node3D CameraSocket
    {
        get => _cameraSocket ?? throw new InvalidOperationException("CameraSocket has not been initialized.");
        set => _cameraSocket = value;
    }

    private Camera3D Camera
    {
        get => _camera ?? throw new InvalidOperationException("Camera has not been initialized.");
        set => _camera = value;
    }

    #endregion

    #region Godot Methods

    /// <summary>
    /// Called when the node enters the scene tree. Initializes references to the camera socket and camera nodes.
    /// </summary>
    /// <remarks>This method is invoked automatically by the Godot engine when the node is added to the scene.
    /// Override this method to perform setup tasks that require the node to be part of the scene tree.</remarks>
    public override void _Ready()
    {
        CameraSocket = GetNode<Node3D>("Camera_Socket");
        Camera = GetNode<Camera3D>("Camera_Socket/Camera");
        _isInitialized = true;
    }

    /// <summary>
    /// Updates the camera's position, rotation, and zoom based on the elapsed time since the last frame.
    /// </summary>
    /// <param name="delta">The elapsed time, in seconds, since the previous frame. Used to ensure smooth and frame rate–independent camera
    /// movement.</param>
    public override void _Process(double delta)
    {
        if (!_isInitialized)
        {
            return;
        }

        MoveCamera(delta);
        RotateCamera(delta);
        ZoomCamera(delta);
    }

    #endregion

    #region Camera Methods

    /// <summary>
    /// Sets the movement direction for the camera.
    /// </summary>
    /// <param name="direction">The direction vector representing the desired movement. Each component typically ranges from -1.0 to 1.0, where
    /// (0, 0) indicates no movement.</param>
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
    /// Sets the zoom direction for the current operation.
    /// </summary>
    /// <param name="direction">The direction and magnitude of the zoom. Positive values indicate zooming in; negative values indicate zooming
    /// out.</param>
    public void SetZoom(float direction)
    {
        _zoomDirection = direction;
    }

    /// <summary>
    /// Updates the camera's position based on the current movement direction and velocity, applying acceleration and
    /// deceleration as appropriate.
    /// </summary>
    /// <remarks>This method should be called once per frame to ensure smooth camera movement. The camera
    /// accelerates toward the target direction when movement input is present and decelerates smoothly when input
    /// stops.</remarks>
    /// <param name="delta">The elapsed time, in seconds, since the last update. Must be positive.</param>
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
    /// Applies a rotation to the camera based on the current rotation direction and velocity, updating the camera's
    /// orientation smoothly over time.
    /// </summary>
    /// <remarks>This method smooths camera rotation by interpolating velocity and applying acceleration and
    /// deceleration factors. Small rotation inputs are ignored to prevent jitter. Call this method once per frame to
    /// ensure consistent camera movement.</remarks>
    /// <param name="delta">The elapsed time, in seconds, since the last update. Must be positive.</param>
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
    /// Adjusts the camera's zoom level based on the specified time delta, applying smooth acceleration and
    /// deceleration.
    /// </summary>
    /// <remarks>This method smoothly updates the camera's zoom velocity and position, ensuring the zoom
    /// remains within configured minimum and maximum limits. It should be called regularly, such as once per frame, to
    /// achieve smooth zoom transitions.</remarks>
    /// <param name="delta">The elapsed time, in seconds, since the last update. Must be positive.</param>
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

        float newZoom = Camera.Position.Z + (_zoomVelocity * (float)delta);
        Camera.Position = new Vector3(Camera.Position.X, Camera.Position.Y, Mathf.Clamp(newZoom, CameraConfigs.CameraZoomMin, CameraConfigs.CameraZoomMax));
        _zoomFactor = Camera.Position.Z * 0.1f;
        _zoomDirection = 0;
    }

    #endregion
}