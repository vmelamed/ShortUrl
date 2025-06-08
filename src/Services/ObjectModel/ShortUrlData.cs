namespace ShortUrl.Services.ObjectModel;

/// <summary>
/// Class ShortUrlData. The data associated with a short URL that is stored in the repository.
/// </summary>
/// <param name="longUrl">The long URL.</param>
class ShortUrlData(Uri longUrl)
{
    int _redirects = 0;

    /// <summary>
    /// Gets the long URL associated with the respective short URL.
    /// </summary>
    public Uri LongUrl => longUrl;

    /// <summary>
    /// Redirect to the long URL associated with this short long URL.
    /// </summary>
    /// <returns>
    /// The long URL.
    /// </returns>
    public Uri Redirect()
    {
        Interlocked.Increment(ref _redirects);
        return longUrl;
    }

    /// <summary>
    /// Gets the number of redirects .
    /// </summary>
    public int Redirects => _redirects;
}
