namespace ShortUrl.Services.Sync;

/// <summary>
/// Class ReaderWriterLockSlimExtensions. Utility class for better management of the lifetime of the scope of <see cref="ReaderWriterLockSlim"/>
/// </summary>
public static class ReaderWriterLockSlimExtensions
{
    /// <summary>
    /// Gets the upgradeable reader slim sync. Merely a shortcut to <c>new UpgradeableReaderSlimLock(readerWriterLock)</c>.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    /// <param name="waitMs">How long to wait for the lock to be acquired in ms. If 0 - wait indefinitely.</param>
    /// <returns><see cref="UpgradeableReaderSlimSync" /> object.</returns>
    public static UpgradeableReaderSync UpgradeableReaderLock(this ReaderWriterLockSlim readerWriterLock, int waitMs = 0)
        => new(readerWriterLock, waitMs);

    /// <summary>
    /// Gets the reader slim sync. Mere of a shortcut to <c>new ReaderSync(readerWriterLock)</c> however shows nicely in intellisense.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    /// <param name="waitMs">How long to wait for the lock to be acquired in ms. If 0 - wait indefinitely.</param>
    /// <returns><see cref="ReaderSync" /> object.</returns>
    public static ReaderSync ReaderLock(this ReaderWriterLockSlim readerWriterLock, int waitMs = 0)
        => new(readerWriterLock, waitMs);

    /// <summary>
    /// Gets the reader slim sync. Mere of a shortcut to <c>new WriterSync(readerWriterLock)</c> however shows nicely in intellisense.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    /// <param name="waitMs">How long to wait for the lock to be acquired in ms. If 0 - wait indefinitely.</param>
    /// <returns><see cref="WriterSync" /> object.</returns>
    public static WriterSync WriterLock(this ReaderWriterLockSlim readerWriterLock, int waitMs = 0)
        => new(readerWriterLock, waitMs);
}
