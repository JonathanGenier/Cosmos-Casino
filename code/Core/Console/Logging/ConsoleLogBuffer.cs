namespace CosmosCasino.Core.Console.Logging
{
    /// <summary>
    /// Stores recent log entries in a fixed-size circular buffer.
    /// Oldest entries are discarded when capacity is exceeded.
    /// </summary>
    internal sealed class ConsoleLogBuffer
    {
        #region FIELDS

        private readonly ConsoleLogEntry[] _entries;
        private int _nextIndex;
        private int _count;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a fixed-size circular buffer for storing log entries.
        /// </summary>
        /// <param name="capacity">
        /// The maximum number of log entries the buffer can hold.
        /// Must be greater than zero.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="capacity"/> is less than or equal to zero.
        /// </exception>
        internal ConsoleLogBuffer(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(capacity, 0, nameof(capacity));

            Capacity = capacity;
            _entries = new ConsoleLogEntry[capacity];
            _nextIndex = 0;
            _count = 0;
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The fixed maximum number of log entries retained by the buffer.
        /// </summary>
        internal int Capacity { get; }

        /// <summary>
        /// Monotonic counter incremented on every log write.
        /// Used for change detection by consumers.
        /// </summary>
        internal long Version { get; private set; }

        /// <summary>
        /// Number of log entries currently stored.
        /// </summary>
        internal int Count => _count;

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Adds a log entry to the buffer.
        /// If the buffer is full, the oldest entry is overwritten.
        /// </summary>
        /// <param name="entry">
        /// Log entry to add to the buffer.
        /// </param>
        internal void Add(in ConsoleLogEntry entry)
        {
            _entries[_nextIndex] = entry;
            _nextIndex = (_nextIndex + 1) % Capacity;

            if (_count < Capacity)
            {
                _count++;
            }

            Version++;
        }

        /// <summary>
        /// Returns a snapshot of the current log entries ordered
        /// from oldest to newest.
        /// </summary>
        /// <returns>
        /// A read-only collection containing the current log entries
        /// in chronological order.
        /// </returns>
        internal IReadOnlyList<ConsoleLogEntry> Snapshot()
        {
            if (_count == 0)
            {
                return Array.Empty<ConsoleLogEntry>();
            }

            var result = new ConsoleLogEntry[_count];
            int start = (_nextIndex - _count + Capacity) % Capacity;

            for (int i = 0; i < _count; i++)
            {
                int index = (start + i) % Capacity;
                result[i] = _entries[index];
            }

            return result;
        }

        /// <summary>
        /// Clears all stored log entries.
        /// </summary>
        internal void Clear()
        {
            _nextIndex = 0;
            _count = 0;
            Version = 0;
        }

        #endregion
    }
}
