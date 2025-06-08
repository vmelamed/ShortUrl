namespace ShortUrl.Services.ObjectModel;

/// <summary>
/// Class LongUrlData. The Data associated with a long URL that is stored in the repository.
/// </summary>
class LongUrlData(Uri shortUrl)
{
    readonly HashSet<Uri> _shortUrls = [ shortUrl ];

    /// <summary>
    /// Adds a new short URL associated with this long URL.
    /// </summary>
    /// <param name="shortUrl">The short URL.</param>
    public void AddShortUrl(Uri shortUrl) => _shortUrls.Add(shortUrl);

    /// <summary>
    /// Gets a snapshot of all short URLs associated with the long URL.
    /// </summary>
    public IEnumerable<Uri> ShortUrls => [.. _shortUrls];

    /// <summary>
    /// Removes the short URL.
    /// </summary>
    /// <param name="shortUrl">The short URL.</param>
    /// <returns>The number of remaining short URLs mapped for this long URL.</returns>
    public int RemoveShortUrl(Uri shortUrl)
    {
        _shortUrls.Remove(shortUrl);
        return _shortUrls.Count;
    }
}
