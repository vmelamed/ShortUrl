namespace ShortUrl.Services.Sync;

public interface IReaderWriterSync : IDisposable
{
    /// <summary>
    /// Gets the lock.
    /// </summary>
    ReaderWriterLockSlim Lock { get; }

    /// <summary>
    /// Gets a value indicating whether the lock is held and the lock owner can read from the protected resource(s).
    /// </summary>
    bool IsLockHeld { get; }
}
