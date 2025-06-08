using FluentAssertions;

using NSubstitute;

using ShortUrl.Services;
using ShortUrl.Services.Repository;

namespace ShortUrl.UnitTests;

public class MappedUrlsTests
{
    [Fact]
    public void GetLongUrl_ReturnsLongUrl_WhenShortUrlExists()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/abc123");
        var longUrl = new Uri("https://example.com/page");
        var dataStore = Substitute.For<DataStore>();
        dataStore.GetLongUrl(shortUrl).Returns(longUrl);

        var mappedUrls = new MappedUrls(dataStore);

        // Act
        var result = mappedUrls.GetLongUrl(shortUrl);

        // Assert
        result.Should().Be(longUrl);
        dataStore.Received(1).GetLongUrl(shortUrl);
    }

    [Fact]
    public void GetLongUrl_ReturnsNull_WhenShortUrlDoesNotExist()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/notfound");
        var dataStore = Substitute.For<DataStore>();
        dataStore.GetLongUrl(shortUrl).Returns((Uri?)null);

        var mappedUrls = new MappedUrls(dataStore);

        // Act
        var result = mappedUrls.GetLongUrl(shortUrl);

        // Assert
        result.Should().BeNull();
        dataStore.Received(1).GetLongUrl(shortUrl);
    }

    [Fact]
    public void GetUsage_ReturnsUsageCount()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/abc123");
        var expectedCount = 5;
        var dataStore = Substitute.For<DataStore>();
        dataStore.GetLongUrlRedirects(shortUrl).Returns(expectedCount);

        var mappedUrls = new MappedUrls(dataStore);

        // Act
        var result = mappedUrls.GetUsage(shortUrl);

        // Assert
        result.Should().Be(expectedCount);
        dataStore.Received(1).GetLongUrlRedirects(shortUrl);
    }

    [Fact]
    public void GetShortUrls_ReturnsAllShortUrlsForLongUrl()
    {
        // Arrange
        var longUrl = new Uri("https://example.com/page");
        var shortUrls = new[]
        {
            new Uri("https://shor.ty/abc123"),
            new Uri("https://shor.ty/xyz789")
        };
        var dataStore = Substitute.For<DataStore>();
        dataStore.GetShortUrls(longUrl).Returns(shortUrls);

        var mappedUrls = new MappedUrls(dataStore);

        // Act
        var result = mappedUrls.GetShortUrls(longUrl);

        // Assert
        result.Should().BeEquivalentTo(shortUrls);
        dataStore.Received(1).GetShortUrls(longUrl);
    }
}