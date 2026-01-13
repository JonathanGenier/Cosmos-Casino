using System;

/// <summary>
/// Provides a base class for node managers that require explicit initialization and readiness tracking before
/// participating in the Godot scene lifecycle.
/// </summary>
/// <remarks>Derive from this class to implement node managers that must be initialized before entering the scene
/// tree. The class enforces initialization by requiring that initialization occurs before the node is marked as ready.
/// It also provides overridable methods for handling readiness, exit, and per-frame processing events. Attempting to
/// mark the node as initialized or ready more than once will result in an exception.</remarks>
public abstract partial class InitializableNodeManager : NodeManager
{
    #region Fields

    private bool _isInitialized;
    private bool _isReady;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the object has been initialized.
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// Gets a value indicating whether the component is ready for use.
    /// </summary>
    public bool IsReady => _isReady;

    #endregion

    #region Godot Process

    /// <summary>
    /// Called by the engine when the node enters the scene tree. Ensures the node has been initialized before
    /// proceeding.
    /// </summary>
    /// <remarks>This method is part of the Godot node lifecycle and should not be called directly. Override
    /// OnReady to implement custom logic that should run when the node is ready.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the node enters the scene tree without being initialized.</exception>
    public sealed override void _Ready()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException($"{GetType().Name} entered the tree without being Initialized.");
        }

        OnReady();

        _isReady = true;
        ConsoleLog.System(GetType().Name, "Ready");
    }

    /// <summary>
    /// Called when the node is about to be removed from the scene tree. Use this method to perform cleanup or
    /// finalization tasks before the node is freed.
    /// </summary>
    /// <remarks>This method is invoked automatically by the engine when the node is exiting the scene tree.
    /// Override this method to release resources, disconnect signals, or perform other shutdown logic specific to the
    /// node. Do not call this method directly.</remarks>
    public sealed override void _ExitTree()
    {
        OnExit();
    }

    /// <summary>
    /// Called every frame to update the node's state. This method is typically used to implement per-frame logic such
    /// as animations, input handling, or game mechanics.
    /// </summary>
    /// <param name="delta">The elapsed time, in seconds, since the previous frame. Use this value to make updates frame rate independent.</param>
    public sealed override void _Process(double delta)
    {
        OnProcess(delta);
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Marks the current instance as initialized, preventing further initialization.
    /// </summary>
    /// <remarks>This method should be called once during the initialization phase of a derived class to
    /// ensure that initialization logic is not executed multiple times.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the instance has already been initialized.</exception>
    protected void MarkInitialized()
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException("Already initialized.");
        }

        _isInitialized = true;
        ConsoleLog.System(GetType().Name, "Initialized");
    }

    /// <summary>
    /// Marks the current instance as ready, enabling it for subsequent operations that require a ready state.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the instance has already been marked as ready.</exception>
    protected void MarkReady()
    {
        if (_isReady)
        {
            throw new InvalidOperationException("Already ready.");
        }

        _isReady = true;
        ConsoleLog.System(GetType().Name, "Ready");
    }

    #endregion

    #region Virtual Methods

    /// <summary>
    /// Invoked when the object is ready to perform its operations. Override this method to execute custom
    /// initialization logic after the object has been fully prepared.
    /// </summary>
    /// <remarks>This method is called to signal that the object has completed its setup and is ready for use.
    /// Derived classes can override this method to perform additional actions when the object becomes ready. The base
    /// implementation does nothing.</remarks>
    protected virtual void OnReady()
    {
    }

    /// <summary>
    /// Invoked when the component is exiting or being disposed.
    /// </summary>
    /// <remarks>Override this method to perform custom cleanup or resource release logic when the component
    /// is exiting. This method is called by the framework and is intended to be used by derived classes.</remarks>
    protected virtual void OnExit()
    {
    }

    /// <summary>
    /// Performs custom processing logic for the current frame. Called with the elapsed time since the last update.
    /// </summary>
    /// <remarks>Override this method in a derived class to implement frame-based processing. The base
    /// implementation does nothing.</remarks>
    /// <param name="delta">The time, in seconds, that has elapsed since the previous call to this method. Typically used to update
    /// time-dependent logic.</param>
    protected virtual void OnProcess(double delta)
    {
    }

    #endregion
}
