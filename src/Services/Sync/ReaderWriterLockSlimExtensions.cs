namespace ShortUrl.Services.Sync;

/// <summary>
/// Class ReaderWriterLockSlimExtensions. Utility class for better management of the lifetime of the scope of <see cref="ReaderWriterLockSlim"/>
/// </summary>
public static class ReaderWriterLockSlimExtensions
{
    /// <summary>
    /// Gets the upgradeable reader slim sync. Merely a shortcut to <c>new UpgradeableReaderSlimLock(readerWriterLock)</c>.
    /// </summary>
    /// <param name="readerWriterLock">
    /// The reader writer lock.
    /// </param>
    /// <returns>
    /// <see cref="UpgradeableReaderSlimSync"/> object.
    /// </returns>
    public static UpgradeableReaderSync UpgradeableReaderLock(this ReaderWriterLockSlim readerWriterLock) => new(readerWriterLock);

    /// <summary>
    /// Gets the reader slim sync. Mere of a shortcut to <c>new ReaderSlimSync(readerWriterLock)</c> however shows nicely in intellisense.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    /// <returns>
    /// <see cref="ReaderSlimSync"/> object.
    /// </returns>
    public static ReaderSlimSync ReaderLock(this ReaderWriterLockSlim readerWriterLock) => new(readerWriterLock);

    /// <summary>
    /// Gets the reader slim sync. Mere of a shortcut to <c>new WriterSlimSync(readerWriterLock)</c> however shows nicely in intellisense.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    /// <returns>
    /// <see cref="WriterSlimSync"/> object.
    /// </returns>
    public static WriterSlimSync WriterLock(this ReaderWriterLockSlim readerWriterLock) => new(readerWriterLock);
}
