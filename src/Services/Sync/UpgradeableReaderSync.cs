namespace ShortUrl.Services.Sync;

/// <summary>
/// With the help of this class create a synchronized reader upgradeable to writer scope by utilizing the <c>using</c> statement.
/// </summary>
public sealed class UpgradeableReaderSync : IDisposable
{
    readonly ReaderWriterLockSlim _readerWriterLock;

    /// <summary>
    /// Initializes a new instance of the <see cref="WriterSlimSync"/> class with the specified <paramref name="readerWriterLock"/> and
    /// waits indefinitely until it acquires the lock in upgradable reader mode.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    public UpgradeableReaderSync(
        ReaderWriterLockSlim readerWriterLock)
    {
        readerWriterLock.EnterUpgradeableReadLock();
        _readerWriterLock = readerWriterLock;
    }

    #region IDisposable pattern implementation
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() => _readerWriterLock.ExitUpgradeableReadLock();
    #endregion
}
