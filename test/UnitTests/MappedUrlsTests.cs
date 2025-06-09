using FluentAssertions;

using ShortUrl.Services;
using ShortUrl.Services.ObjectModel;
using ShortUrl.Services.Repository;

namespace ShortUrl.UnitTests;

public class MappedUrlsTests
{
    private readonly DataStore _dataStore = new(true);
    private readonly MappedUrls _mappedUrls;

    public MappedUrlsTests() => _mappedUrls = new MappedUrls(_dataStore);

    [Fact]
    public void GetLongUrl_ReturnsLongUrl_AndIncrementsRedirects()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/abc");
        var longUrl = new Uri("https://example.com/page");
        var data = new ShortUrlData(shortUrl, longUrl);
        _dataStore.Add(data);

        // Act
        var result = _mappedUrls.GetLongUrl(shortUrl);

        // Assert
        result.Should().Be(longUrl);
        var stored = _dataStore.Data.First(d => d.ShortUrl == shortUrl);
        stored.Redirects.Should().Be(1);
    }

    [Fact]
    public void GetLongUrl_ReturnsNull_IfShortUrlNotFound()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/notfound");

        // Act
        var result = _mappedUrls.GetLongUrl(shortUrl);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetShortUrls_ReturnsAllShortUrlsForLongUrl()
    {
        // Arrange
        var longUrl = new Uri("https://example.com/page");
        var shortUrl1 = new Uri("https://shor.ty/abc");
        var shortUrl2 = new Uri("https://shor.ty/xyz");
        _dataStore.Add(new ShortUrlData(shortUrl1, longUrl));
        _dataStore.Add(new ShortUrlData(shortUrl2, longUrl));

        // Act
        var result = _mappedUrls.GetShortUrls(longUrl).ToList();

        // Assert
        result.Should().BeEquivalentTo([shortUrl1, shortUrl2]);
    }

    [Fact]
    public void GetShortUrls_ReturnsEmpty_IfNoShortUrlsForLongUrl()
    {
        // Arrange
        var longUrl = new Uri("https://example.com/none");

        // Act
        var result = _mappedUrls.GetShortUrls(longUrl);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetUsage_ReturnsRedirectCount()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/abc");
        var longUrl = new Uri("https://example.com/page");
        var data = new ShortUrlData(shortUrl, longUrl);
        _dataStore.Add(data);

        // Act
        _mappedUrls.GetLongUrl(shortUrl);
        _mappedUrls.GetLongUrl(shortUrl);
        var usage = _mappedUrls.GetUsage(shortUrl);

        // Assert
        usage.Should().Be(2);
    }

    [Fact]
    public void GetUsage_ReturnsZero_IfShortUrlNotFound()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/notfound");

        // Act
        var usage = _mappedUrls.GetUsage(shortUrl);

        // Assert
        usage.Should().Be(0);
    }
}