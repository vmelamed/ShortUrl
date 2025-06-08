using FluentAssertions;

using ShortUrl.Services;

namespace ShortUrl.UnitTests;

public class ShortUrlAlgorithmsTests
{
    [Fact]
    public void CreateShortUrl_GeneratesUniqueShortUrls()
    {
        // Arrange
        var longUrl = new Uri("https://example.com/some/long/path");

        // Act
        var shortUrl1 = ShortUrlAlgorithm.CreateShortUrl(longUrl);
        var shortUrl2 = ShortUrlAlgorithm.CreateShortUrl(longUrl);

        // Assert
        shortUrl1.Should().NotBeNull();
        shortUrl2.Should().NotBeNull();
        shortUrl1.Should().NotBe(shortUrl2);
        shortUrl1.Scheme.Should().Be("https");
        shortUrl1.Host.Should().Be(ShortUrlAlgorithm.ThisHostName);
        shortUrl2.Scheme.Should().Be("https");
        shortUrl2.Host.Should().Be(ShortUrlAlgorithm.ThisHostName);
        shortUrl1.AbsolutePath.Trim('/').Should().NotBeNullOrWhiteSpace();
        shortUrl2.AbsolutePath.Trim('/').Should().NotBeNullOrWhiteSpace();
    }

    const string UnresevedUriChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.-_~";

    [Fact]
    public void CreateShortUrl_PathIsBase62Like()
    {
        // Arrange
        var longUrl = new Uri("https://example.com/");

        // Act
        var shortUrl = ShortUrlAlgorithm.CreateShortUrl(longUrl);

        // Assert
        var path = shortUrl.AbsolutePath.Trim('/');
        path.Should().NotBeNullOrEmpty();
        path.Should().MatchRegex(@$"^[{UnresevedUriChars}]*$");
    }

    [Fact]
    public void CreateShortUrl_AlwaysReturnsUriWithExpectedHost()
    {
        // Arrange
        var longUrl = new Uri("https://another.com/");

        // Act
        var shortUrl = ShortUrlAlgorithm.CreateShortUrl(longUrl);

        // Assert
        shortUrl.Host.Should().Be(ShortUrlAlgorithm.ThisHostName);
    }
}