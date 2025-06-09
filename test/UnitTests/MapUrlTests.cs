using FluentAssertions;

using ShortUrl.Services;
using ShortUrl.Services.ObjectModel;
using ShortUrl.Services.Repository;

namespace ShortUrl.UnitTests;

public class MapUrlTests
{
    private readonly DataStore _dataStore = new(true);
    private readonly Func<Uri, Uri> _generator = longUrl => new Uri($"https://shor.ty/{Guid.NewGuid()}");
    private readonly string _longUrlStr = "https://example.com/page";

    [Fact]
    public void CreateShortUrl_CreatesNewShortUrl_WhenNoneExists()
    {
        // Arrange
        var mapUrl = new MapUrl(_generator, _dataStore);
        var longUrl = new Uri(_longUrlStr);

        // Act
        var shortUrl = mapUrl.CreateShortUrl(longUrl);

        // Assert
        shortUrl.Should().NotBeNull();
        _dataStore.Data.Should().ContainSingle(d => d.LongUrl == longUrl && d.ShortUrl == shortUrl);
    }

    [Fact]
    public void CreateShortUrl_ReturnsExistingShortUrl_IfNotForceNew()
    {
        // Arrange
        var mapUrl = new MapUrl(_generator, _dataStore);
        var longUrl = new Uri(_longUrlStr);
        var firstShortUrl = mapUrl.CreateShortUrl(longUrl);

        // Act
        var secondShortUrl = mapUrl.CreateShortUrl(longUrl);

        // Assert
        secondShortUrl.Should().Be(firstShortUrl);
        _dataStore.Data.Count(d => d.LongUrl == longUrl).Should().Be(1);
    }

    [Fact]
    public void CreateShortUrl_ForceNew_CreatesNewShortUrl()
    {
        // Arrange
        var mapUrl = new MapUrl(_generator, _dataStore);
        var longUrl = new Uri(_longUrlStr);
        var firstShortUrl = mapUrl.CreateShortUrl(longUrl);

        // Act
        var secondShortUrl = mapUrl.CreateShortUrl(longUrl, forceNew: true);

        // Assert
        secondShortUrl.Should().NotBe(firstShortUrl);
        _dataStore.Data.Count(d => d.LongUrl == longUrl).Should().Be(2);
    }

    [Fact]
    public void CreateShortUrl_WithPreferredShortUrl_UsesPreferred()
    {
        // Arrange
        var mapUrl = new MapUrl(_generator, _dataStore);
        var longUrl = new Uri(_longUrlStr);
        var preferredShortUrl = new Uri("https://shor.ty/custom");

        // Act
        var result = mapUrl.CreateShortUrl(longUrl, preferredShortUrl: preferredShortUrl);

        // Assert
        result.Should().Be(preferredShortUrl);
        _dataStore.Data.Should().Contain(d => d.ShortUrl == preferredShortUrl && d.LongUrl == longUrl);
    }

    [Fact]
    public void CreateShortUrl_WithPreferredShortUrl_ThrowsIfExistsForDifferentLongUrl()
    {
        // Arrange
        var mapUrl = new MapUrl(_generator, _dataStore);
        var longUrl1 = new Uri(_longUrlStr);
        var longUrl2 = new Uri("https://example.com/other");
        var preferredShortUrl = new Uri("https://shor.ty/custom");
        _dataStore.Add(new ShortUrlData(preferredShortUrl, longUrl1));

        // Act
        Action act = () => mapUrl.CreateShortUrl(longUrl2, preferredShortUrl: preferredShortUrl);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("A short URL with the specified preferred short URL already exists for a different long URL.");
    }

    [Fact]
    public void DeleteShortUrl_RemovesShortUrl()
    {
        // Arrange
        var mapUrl = new MapUrl(_generator, _dataStore);
        var longUrl = new Uri(_longUrlStr);
        var shortUrl = mapUrl.CreateShortUrl(longUrl);

        // Act
        var deleted = mapUrl.DeleteShortUrl(shortUrl);

        // Assert
        deleted.Should().BeTrue();
        _dataStore.Data.Should().NotContain(d => d.ShortUrl == shortUrl);
    }

    [Fact]
    public void DeleteShortUrl_ReturnsFalse_IfShortUrlNotFound()
    {
        // Arrange
        var mapUrl = new MapUrl(_generator, _dataStore);
        var shortUrl = new Uri("https://shor.ty/notfound");

        // Act
        var deleted = mapUrl.DeleteShortUrl(shortUrl);

        // Assert
        deleted.Should().BeFalse();
    }
}