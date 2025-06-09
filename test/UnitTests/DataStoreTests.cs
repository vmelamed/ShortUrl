using FluentAssertions;

using ShortUrl.Services.ObjectModel;
using ShortUrl.Services.Repository;

namespace ShortUrl.UnitTests;

public class DataStoreTests
{
    private readonly DataStore _dataStore = new(true);

    [Fact]
    public void Add_AddsShortUrlData_WhenNotExists()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/abc");
        var longUrl = new Uri("https://example.com/page");
        var data = new ShortUrlData(shortUrl, longUrl);

        // Act
        _dataStore.Add(data);

        // Assert
        _dataStore.Data.Should().ContainSingle(d => d.ShortUrl == shortUrl && d.LongUrl == longUrl);
    }

    [Fact]
    public void Add_Throws_WhenShortUrlAlreadyExists()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/abc");
        var longUrl = new Uri("https://example.com/page");
        var data = new ShortUrlData(shortUrl, longUrl);
        _dataStore.Add(data);

        // Act
        Action act = () => _dataStore.Add(new ShortUrlData(shortUrl, new Uri("https://other.com/")));

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Short URL '{shortUrl}' already exists in the data store.");
    }

    [Fact]
    public void Delete_RemovesShortUrlData_IfExists()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/abc");
        var longUrl = new Uri("https://example.com/page");
        var data = new ShortUrlData(shortUrl, longUrl);
        _dataStore.Add(data);

        // Act
        var result = _dataStore.Delete(shortUrl);

        // Assert
        result.Should().BeTrue();
        _dataStore.Data.Should().NotContain(d => d.ShortUrl == shortUrl);
    }

    [Fact]
    public void Delete_ReturnsFalse_IfShortUrlNotFound()
    {
        // Arrange
        var shortUrl = new Uri("https://shor.ty/notfound");

        // Act
        var result = _dataStore.Delete(shortUrl);

        // Assert
        result.Should().BeFalse();
    }
}