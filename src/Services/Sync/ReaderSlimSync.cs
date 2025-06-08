namespace ShortUrl.Services.Sync;

/// <summary>
/// With the help of this class create a synchronized multiple readers/single writer scope by utilizing the <c>using</c> statement.
/// </summary>
public sealed class ReaderSlimSync : IDisposable
{
    readonly ReaderWriterLockSlim _readerWriterLock;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderSlimSync"/> class with the specified <paramref name="readerWriterLock"/> and
    /// waits indefinitely till it acquires the lock in reader mode.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    public ReaderSlimSync(
        ReaderWriterLockSlim readerWriterLock)
    {
        readerWriterLock.EnterReadLock();
        _readerWriterLock = readerWriterLock;
    }

    #region IDisposable pattern implementation
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() => _readerWriterLock.ExitReadLock();
    #endregion
}
