using FluentAssertions;

using ShortUrl.Services.Sync;

namespace ShortUrl.UnitTests;

#pragma warning disable xUnit1026, xUnit1031, xUnit1044

public class ReaderWriterSyncTests
{
    public class RWSyncTestData : TheoryData<
            Func<ReaderWriterLockSlim, int, IReaderWriterSync>,
            Func<ReaderWriterLockSlim, bool>>
    {
        public RWSyncTestData()
        {
            Add(
                (rwLock, ms) => rwLock.ReaderLock(ms),
                rwLock => rwLock.IsReadLockHeld);

            Add(
                (rwLock, ms) => rwLock.WriterLock(ms),
                rwLock => rwLock.IsWriteLockHeld);

            Add(
                (rwLock, ms) => rwLock.UpgradeableReaderLock(ms),
                rwLock => rwLock.IsUpgradeableReadLockHeld);
        }
    }

    [Theory]
    [ClassData(typeof(RWSyncTestData))]
    public void AcquiresAndReleasesLock(
        Func<ReaderWriterLockSlim, int, IReaderWriterSync> syncFactory,
        Func<ReaderWriterLockSlim, bool> isHeld)
    {
        // Arrange
        using var rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        // Act
        using (var sync = syncFactory(rwLock, 0))
        {
            sync.IsLockHeld.Should().BeTrue();
            isHeld(rwLock).Should().BeTrue();
        }

        // Assert
        isHeld(rwLock).Should().BeFalse();
    }

    [Theory]
    [ClassData(typeof(RWSyncTestData))]
    public void IsLockHeld_IsFalse_IfLockNotHeld(
        Func<ReaderWriterLockSlim, int, IReaderWriterSync> syncFactory,
        Func<ReaderWriterLockSlim, bool> isHeld)
    {
        // Arrange
        using var rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        // Act
        var sync = syncFactory(rwLock, 0);
        sync.Dispose();

        // Assert
        sync.IsLockHeld.Should().BeFalse();
        isHeld(rwLock).Should().BeFalse();
    }

    [Theory]
    [ClassData(typeof(RWSyncTestData))]
    public void WaitMs_AcquiresLockWithinTimeout(
        Func<ReaderWriterLockSlim, int, IReaderWriterSync> syncFactory,
        Func<ReaderWriterLockSlim, bool> isHeld)
    {
        // Arrange
        using var rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        // Act
        using var sync = syncFactory(rwLock, 10);

        // Assert
        sync.IsLockHeld.Should().BeTrue();
        isHeld(rwLock).Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(RWSyncTestData))]
    public void WaitMs_DoesNotAcquireIfWriteLockHeld(
        Func<ReaderWriterLockSlim, int, IReaderWriterSync> syncFactory,
        Func<ReaderWriterLockSlim, bool> _)
    {
        // Arrange
        using var rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        Task.WaitAll(
        [
            Task.Run(
                () =>
                {
                    rwLock.EnterWriteLock();
                    Thread.Sleep(100); // Simulate work
                    rwLock.ExitWriteLock();
                }),

            Task.Run(
                () =>
                {
                    Thread.Sleep(10); // Ensure the write lock is acquired before the sync tries to acquire it

                    // Act
                    using var sync = syncFactory(rwLock, 10);

                    // Assert
                    sync.IsLockHeld.Should().BeFalse();
                })
        ]);

        rwLock.IsReadLockHeld.Should().BeFalse();
        rwLock.IsWriteLockHeld.Should().BeFalse();
        rwLock.IsUpgradeableReadLockHeld.Should().BeFalse();
    }

    [Theory]
    [ClassData(typeof(RWSyncTestData))]
    public void Dispose_MultipleTimes_DoesNotThrow(
        Func<ReaderWriterLockSlim, int, IReaderWriterSync> syncFactory,
        Func<ReaderWriterLockSlim, bool> _)
    {
        // Arrange
        using var rwLock = new ReaderWriterLockSlim();
        var sync = syncFactory(rwLock, 0);

        // Act
        sync.Dispose();
        var act = sync.Dispose;

        // Assert
        act.Should().NotThrow();
    }
}