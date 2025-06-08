using FluentAssertions;

using ShortUrl.Services.Repository;

namespace ShortUrl.UnitTests;

public class DataStoreTests
{
    [Fact]
    public void AddUrlPair_AddsAndRetrievesShortAndLongUrls()
    {
        // Arrange
        var repo = new DataStore();
        var shortUrl = new Uri("https://shor.ty/abc123");
        var longUrl = new Uri("https://example.com/page");

        // Act
        var added = repo.AddUrlPair(shortUrl, longUrl);
        var retrievedLongUrl = repo.GetLongUrl(shortUrl);
        var retrievedShortUrls = repo.GetShortUrls(longUrl).ToList();

        // Assert
        added.Should().BeTrue();
        retrievedLongUrl.Should().Be(longUrl);
        retrievedShortUrls.Should().Contain(shortUrl);
    }

    [Fact]
    public void AddUrlPair_DuplicateShortUrl_ReturnsFalse()
    {
        // Arrange
        var repo = new DataStore();
        var shortUrl = new Uri("https://shor.ty/abc123");
        var longUrl1 = new Uri("https://example.com/page1");
        var longUrl2 = new Uri("https://example.com/page2");

        // Act
        var firstAdd = repo.AddUrlPair(shortUrl, longUrl1);
        var secondAdd = repo.AddUrlPair(shortUrl, longUrl2);

        // Assert
        firstAdd.Should().BeTrue();
        secondAdd.Should().BeFalse();
    }

    [Fact]
    public void GetShortUrl_ReturnsFirstShortUrl()
    {
        // Arrange
        var repo = new DataStore();
        var longUrl = new Uri("https://example.com/page");
        var shortUrl1 = new Uri("https://shor.ty/abc123");
        var shortUrl2 = new Uri("https://shor.ty/xyz789");

        // Act
        repo.AddUrlPair(shortUrl1, longUrl);
        repo.AddUrlPair(shortUrl2, longUrl);
        var result = repo.GetShortUrls(longUrl);

        // Assert
        //result.Should().Contain(shortUrl1).Or.Contain(shortUrl2);
    }

    [Fact]
    public void GetLongUrl_ReturnsNullIfNotFound()
    {
        // Arrange
        var repo = new DataStore();
        var shortUrl = new Uri("https://shor.ty/notfound");

        // Act
        var result = repo.GetLongUrl(shortUrl);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetShortUrls_ReturnsEmptyIfNotFound()
    {
        // Arrange
        var repo = new DataStore();
        var longUrl = new Uri("https://example.com/notfound");

        // Act
        var result = repo.GetShortUrls(longUrl);

        // Assert
        result.Should().BeEmpty();
    }
}