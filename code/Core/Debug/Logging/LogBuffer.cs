namespace CosmosCasino.Core.Debug.Logging
{
    /// <summary>
    /// Stores recent log entries in a fixed-size circular buffer.
    /// Oldest entries are discarded when capacity is exceeded.
    /// </summary>
    public sealed class LogBuffer
    {
        private readonly LogEntry[] _entries;
        private int _nextIndex;
        private int _count;

        /// <summary>
        /// The fixed maximum number of log entries retained by the buffer.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Monotonic counter incremented on every log write.
        /// Used for change detection by consumers.
        /// </summary>
        public long Version { get; private set; }

        /// <summary>
        /// Number of log entries currently stored.
        /// </summary>
        public int Count => _count;

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
        public LogBuffer(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(capacity, 0, nameof(capacity));

            Capacity = capacity;
            _entries = new LogEntry[capacity];
            _nextIndex = 0;
            _count = 0;
        }

        /// <summary>
        /// Adds a log entry to the buffer.
        /// If the buffer is full, the oldest entry is overwritten.
        /// </summary>
        /// <param name="entry"></param>
        public void Add(in LogEntry entry)
        {
            _entries[_nextIndex] = entry;
            _nextIndex = (_nextIndex + 1) % Capacity;

            if(_count < Capacity)
            {
                _count++;
            }

            Version++;
        }

        /// <summary>
        /// Returns a snapshot of the current log entries
        /// ordered from oldest to newest.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<LogEntry> Snapshot()
        {
            if(_count == 0)
            {
                return Array.Empty<LogEntry>();
            }

            var result = new LogEntry[_count];
            int start = (_nextIndex - _count + Capacity) % Capacity;

            for(int i = 0; i < _count; i++)
            {
                int index = (start + i) % Capacity;
                result[i] = _entries[index];
            }

            return result;
        }

        /// <summary>
        /// Clears all stored log entries.
        /// </summary>
        public void Clear()
        {
            _nextIndex = 0;
            _count = 0;
            Version = 0;
        }
    }
}
