namespace ShortUrl.Services.ObjectModel;

/// <summary>
/// Class Data. The data associated with a short URL that is stored in the repository.
/// </summary>
/// <param name="longUrl">The long URL.</param>
public class ShortUrlData(Uri shortUrl, Uri longUrl)
{
    int _redirects = 0;

    /// <summary>
    /// Gets the short URL.
    /// </summary>
    public Uri ShortUrl { get; } = shortUrl;

    /// <summary>
    /// Gets the long URL mapped to the short URL.
    /// </summary>
    public Uri LongUrl { get; } = longUrl;

    /// <summary>
    /// Gets the number of redirects .
    /// </summary>
    public int Redirects => _redirects;

    /// <summary>
    /// Redirect to the long URL associated with this short long URL.
    /// </summary>
    /// <returns>
    /// The long URL.
    /// </returns>
    public Uri Redirect()
    {
        Interlocked.Increment(ref _redirects);
        return LongUrl;
    }
}
