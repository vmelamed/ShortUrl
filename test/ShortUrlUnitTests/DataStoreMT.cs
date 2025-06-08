using System.Collections.Concurrent;

using FluentAssertions;

using ShortUrl.Services.Repository;

namespace ShortUrl.UnitTests;

#pragma warning disable xUnit1031 // Do not use blocking task operations in test method

public class DataStoreThreadingTests
{
    [Fact]
    public void AddUrlPair_IsThreadSafe_ForConcurrentAdds()
    {
        // Arrange
        var dataStore = new DataStore();
        var longUrl = new Uri("https://example.com/page");
        int threadCount = 20;
        var shortUrls = new ConcurrentBag<Uri>();
        var tasks = new Task[threadCount];

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            int idx = i;
            tasks[i] = Task.Run(() =>
            {
                var shortUrl = new Uri($"https://shor.ty/{idx}");
                if (dataStore.AddUrlPair(shortUrl, longUrl))
                    shortUrls.Add(shortUrl);
            });
        }
        Task.WaitAll(tasks);

        // Assert
        var allShortUrls = dataStore.GetShortUrls(longUrl);
        allShortUrls.Should().BeEquivalentTo(shortUrls);
        allShortUrls.Should().HaveCount(threadCount);
    }

    [Fact]
    public void GetLongUrl_IsThreadSafe_ForConcurrentReads()
    {
        // Arrange
        var dataStore = new DataStore();
        var longUrl = new Uri("https://example.com/page");
        var shortUrl = new Uri("https://shor.ty/abc123");
        dataStore.AddUrlPair(shortUrl, longUrl);

        int threadCount = 20;
        var results = new ConcurrentBag<Uri?>();
        var tasks = new Task[threadCount];

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                var result = dataStore.GetLongUrl(shortUrl);
                results.Add(result);
            });
        }
        Task.WaitAll(tasks);

        // Assert
        results.Should().OnlyContain(uri => uri == longUrl);
    }

    [Fact]
    public void AddAndDeleteUrlPair_AreThreadSafe()
    {
        // Arrange
        var dataStore = new DataStore();
        var longUrl = new Uri("https://example.com/page");
        int threadCount = 10;
        var shortUrls = new Uri[threadCount];
        for (int i = 0; i < threadCount; i++)
            shortUrls[i] = new Uri($"https://shor.ty/{i}");

        // Act
        Parallel.For(0, threadCount, i => dataStore.AddUrlPair(shortUrls[i], longUrl));

        Parallel.For(0, threadCount, i => dataStore.DeleteShortUrl(shortUrls[i]));

        // Assert
        dataStore.GetShortUrls(longUrl).Should().BeEmpty();
    }
}