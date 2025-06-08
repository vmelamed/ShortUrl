namespace ShortUrl.Services.Sync;

/// <summary>
/// With the help of this class create a synchronized writer scope by utilizing the <c>using</c> statement:
/// When the object is created, it attempts to acquire the lock in writer mode.
/// When disposed, it releases the lock if it was acquired. Never omit disposing this object. Better yet always use it inside
/// of a <c>using</c> statement.
/// </summary>
public sealed class WriterSync : IReaderWriterSync
{
    /// <summary>
    /// Gets the lock.
    /// </summary>
    public ReaderWriterLockSlim Lock { get; init; }

    /// <summary>
    /// Gets a value indicating whether the lock is held and the lock owner can write to the protected resource(s).
    /// </summary>
    public bool IsLockHeld { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WriterSync"/> class with the specified <paramref name="readerWriterLock"/> and
    /// waits indefinitely till it acquires the lock in writer mode.
    /// </summary>
    /// <param name="readerWriterLock">The reader-writer lock.</param>
    /// <param name="waitMs">How long to wait for the lock to be acquired in ms. If 0 - wait indefinitely.</param>
    public WriterSync(
        ReaderWriterLockSlim readerWriterLock,
        int waitMs = 0)
    {
        Lock = readerWriterLock;
        if (waitMs is 0)
        {
            Lock.EnterWriteLock();
            IsLockHeld = true;
        }
        else
            IsLockHeld = Lock.TryEnterWriteLock(waitMs);
    }

    #region IDisposable pattern implementation
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (IsLockHeld)
        {
            Lock.ExitWriteLock();
            IsLockHeld = false;
        }
        GC.SuppressFinalize(this);
    }
    #endregion
}
