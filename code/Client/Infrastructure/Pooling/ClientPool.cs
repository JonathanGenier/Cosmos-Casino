using System;
using System.Collections.Generic;

/// <summary>
/// Provides a thread-unsafe object pool for managing reusable instances of a specified reference type. Enables
/// efficient reuse of objects to reduce allocation overhead and improve performance in scenarios where object creation
/// is expensive.
/// </summary>
/// <remarks>ClientPool is intended for scenarios where objects are frequently created and discarded, and reusing
/// instances can improve performance or resource utilization. The pool is not thread-safe; callers must ensure external
/// synchronization if used concurrently from multiple threads. Objects are reset before being returned to the pool,
/// allowing them to be reused in a clean state.</remarks>
/// <typeparam name="T">The type of objects to pool. Must be a reference type.</typeparam>
public partial class ClientPool<T>
    where T : class
{
    #region Fields

    private readonly Queue<T> _pool = new Queue<T>();
    private readonly Func<T> _factory;
    private readonly Action<T>? _resetAction;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the ClientPool class with a specified initial pool size, object factory, and reset
    /// action.
    /// </summary>
    /// <param name="initialSize">The number of objects to preallocate and add to the pool. Must be non-negative.</param>
    /// <param name="factory">A function used to create new instances of the pooled object type when needed. Cannot be null.</param>
    /// <param name="resetAction">An action to reset the state of an object before it is returned to the pool. Cannot be null.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if initialSize is less than zero.</exception>
    /// <exception cref="ArgumentNullException">Thrown if factory or resetAction is null.</exception>
    public ClientPool(int initialSize, Func<T> factory, Action<T>? resetAction = null)
    {
        if (initialSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialSize), "Initial size must be non-negative.");
        }

        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _resetAction = resetAction;

        for (int i = 0; i < initialSize; i++)
        {
            _pool.Enqueue(_factory());
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of items currently contained in the pool.
    /// </summary>
    public int Count => _pool.Count;

    #endregion

    #region Pooling Methods

    /// <summary>
    /// Retrieves an object from the pool if one is available; otherwise, creates a new instance.
    /// </summary>
    /// <remarks>The returned object may be newly created if the pool does not contain any available
    /// instances. Callers are responsible for returning objects to the pool when they are no longer needed.</remarks>
    /// <returns>An instance of type T from the pool, or a new instance if the pool is empty.</returns>
    public T Fetch()
    {
        if (_pool.Count > 0)
        {
            return _pool.Dequeue();
        }

        return _factory();
    }

    /// <summary>
    /// Returns an object to the pool for future reuse.
    /// </summary>
    /// <remarks>After returning the object, it may be reset to its initial state before being made available
    /// for reuse. Once returned, the object should not be used by the caller unless it is obtained from the pool
    /// again.</remarks>
    /// <param name="poolObject">The object to return to the pool. Cannot be null.</param>
    public void Return(T poolObject)
    {
        ArgumentNullException.ThrowIfNull(poolObject);

        _resetAction?.Invoke(poolObject);
        _pool.Enqueue(poolObject);
    }

    #endregion
}

