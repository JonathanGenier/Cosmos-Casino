using Godot;
using System;

/// <summary>
/// Manages camera input flow by subscribing to input events and delegating camera movement, rotation, and zoom
/// operations to the camera manager.
/// </summary>
/// <remarks>Use this class to connect input events to camera controls within a game flow. The instance subscribes
/// to relevant input events upon creation and should be disposed when no longer needed to prevent resource leaks and
/// unintended event handling. This class is not thread-safe.</remarks>
public class CameraInputFlow : IGameFlow, IDisposable
{
    #region Fields

    private InputManager _inputManager;
    private CameraManager _cameraManager;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the CameraInputFlow class and subscribes to camera-related input events.
    /// </summary>
    /// <param name="inputManager">The input manager that provides camera movement, rotation, and zoom events. Cannot be null.</param>
    /// <param name="cameraManager">The camera manager that controls camera operations. Cannot be null.</param>
    public CameraInputFlow(InputManager inputManager, CameraManager cameraManager)
    {
        _inputManager = inputManager;
        _cameraManager = cameraManager;

        _inputManager.MoveCamera += OnMoveCamera;
        _inputManager.RotateCamera += OnRotateCamera;
        _inputManager.ZoomCamera += OnZoomCamera;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases all resources used by the current instance and unsubscribes from input events.
    /// </summary>
    /// <remarks>Call this method when the instance is no longer needed to ensure that event handlers are
    /// detached and resources are properly released. After calling this method, the instance should not be
    /// used.</remarks>
    public void Dispose()
    {
        _inputManager.MoveCamera -= OnMoveCamera;
        _inputManager.RotateCamera -= OnRotateCamera;
        _inputManager.ZoomCamera -= OnZoomCamera;
    }

    #endregion

    #region Camera Methods

    /// <summary>
    /// Moves the camera in the specified direction.
    /// </summary>
    /// <param name="dir">The direction and magnitude to move the camera, represented as a 2D vector. Each component specifies movement
    /// along the corresponding axis.</param>
    private void OnMoveCamera(Vector2 dir)
    {
        _cameraManager.Move(dir);
    }

    /// <summary>
    /// Rotates the camera by the specified amount.
    /// </summary>
    /// <param name="v">The amount, in degrees, to rotate the camera. Positive values rotate in one direction; negative values rotate in
    /// the opposite direction.</param>
    private void OnRotateCamera(float v)
    {
        _cameraManager.Rotate(v);
    }

    /// <summary>
    /// Adjusts the camera's zoom level by the specified amount.
    /// </summary>
    /// <param name="v">The amount to change the camera's zoom level. Positive values zoom in; negative values zoom out.</param>
    private void OnZoomCamera(float v)
    {
        _cameraManager.Zoom(v);
    }

    #endregion
}