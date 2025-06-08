using FluentAssertions;

using NSubstitute;

using ShortUrl.Services;
using ShortUrl.Services.Repository;

namespace ShortUrl.UnitTests;

public class MapUrlTests
{
    [Fact]
    public void CreateShortUrl_ReturnsExistingShortUrl_IfNotForceNew()
    {
        // Arrange
        var longUrl = new Uri("https://example.com/page");
        var existingShortUrl = new Uri("https://shor.ty/abc123");

        var dataStore = Substitute.For<DataStore>();
        dataStore.GetShortUrls(longUrl).Returns([existingShortUrl]);

        var mapUrl = new MapUrl(_ => throw new Exception("Should not be called"), dataStore);

        // Act
        var result = mapUrl.CreateShortUrl(longUrl, forceNew: false);

        // Assert
        result.Should().Be(existingShortUrl);
        dataStore.Received(1).GetShortUrls(longUrl);
    }

    [Fact]
    public void CreateShortUrl_ReturnsPreferredShortUrl_IfAvailable()
    {
        // Arrange
        var longUrl = new Uri("https://example.com/page");
        var preferredShortUrl = new Uri("https://shor.ty/preferred");

        var dataStore = Substitute.For<DataStore>();
        dataStore.GetShortUrls(longUrl).Returns([]);
        dataStore.AddUrlPair(preferredShortUrl, longUrl).Returns(true);

        var mapUrl = new MapUrl(_ => throw new Exception("Should not be called"), dataStore);

        // Act
        var result = mapUrl.CreateShortUrl(longUrl, forceNew: false, preferredShortUrl: preferredShortUrl);

        // Assert
        result.Should().Be(preferredShortUrl);
        dataStore.Received(1).AddUrlPair(preferredShortUrl, longUrl);
    }

    [Fact]
    public void CreateShortUrl_CreatesNewShortUrl_IfForceNewOrPreferredNotAvailable()
    {
        // Arrange
        var longUrl = new Uri("https://example.com/page");
        var generatedShortUrl = new Uri("https://shor.ty/generated");

        var dataStore = Substitute.For<DataStore>();
        dataStore.GetShortUrls(longUrl).Returns([]);
        dataStore.AddUrlPair(generatedShortUrl, longUrl).Returns(true);

        var mapUrl = new MapUrl(_ => generatedShortUrl, dataStore);

        // Act
        var result = mapUrl.CreateShortUrl(longUrl, forceNew: true);

        // Assert
        result.Should().Be(generatedShortUrl);
        dataStore.Received(1).AddUrlPair(generatedShortUrl, longUrl);
    }

    [Fact]
    public void CreateShortUrl_ReturnsExistingPreferredShortUrl_IfNotForceNew()
    {
        // Arrange
        var longUrl = new Uri("https://example.com/page");
        var preferredShortUrl = new Uri("https://shor.ty/preferred");

        var dataStore = Substitute.For<DataStore>();
        dataStore.GetShortUrls(longUrl).Returns([preferredShortUrl]);

        var mapUrl = new MapUrl(_ => throw new Exception("Should not be called"), dataStore);

        // Act
        var result = mapUrl.CreateShortUrl(longUrl, forceNew: false, preferredShortUrl: preferredShortUrl);

        // Assert
        result.Should().Be(preferredShortUrl);
    }

    [Fact]
    public void DeleteShortUrl_DelegatesToDataStore()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/abc123");
        var dataStore = Substitute.For<DataStore>();
        dataStore.DeleteShortUrl(shortUrl).Returns(true);

        var mapUrl = new MapUrl(_ => throw new Exception("Should not be called"), dataStore);

        // Act
        var result = mapUrl.DeleteShortUrl(shortUrl);

        // Assert
        result.Should().BeTrue();
        dataStore.Received(1).DeleteShortUrl(shortUrl);
    }
}