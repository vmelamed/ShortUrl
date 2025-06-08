using ShortUrl.Services.ObjectModel;
using ShortUrl.Services.Sync;

namespace ShortUrl.Services.Repository;

/// <summary>
/// Class DataStore. "Stores" the short and long URLs in memory and associated data, simulating a repository.
/// </summary>
public class DataStore
{
    readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);
    readonly Dictionary<Uri, ShortUrlData> _shortUrls = [];
    readonly Dictionary<Uri, LongUrlData> _longUrls = [];

    /// <summary>
    /// Adds a pair of URLs to the store.
    /// </summary>
    /// <param name="shortUrl">The short URL.</param>
    /// <param name="longUrl">The long URL.</param>
    /// <returns>
    /// <c>true</c> if the pair was successfully added, <c>false</c> otherwise.
    /// </returns>
    public virtual bool AddUrlPair(Uri shortUrl, Uri longUrl)
    {
        using var _ = _lock.WriterLock();

        if (!_shortUrls.TryAdd(shortUrl, new(longUrl)))
            return false; // The short URL already exists

        if (!_longUrls.TryGetValue(longUrl, out var longUrlData))
            _longUrls[longUrl] = new LongUrlData(shortUrl);
        else
            longUrlData.AddShortUrl(shortUrl);

        return true;
    }

    /// <summary>
    /// Gets the long URL corresponding to a short URL.
    /// </summary>
    /// <param name="shortUrl">The short URL.</param>
    /// <returns>The long URI corresponding to the short URL.</returns>
    public virtual Uri? GetLongUrl(Uri shortUrl)
    {
        using var _ = _lock.ReaderLock();

        if (!_shortUrls.TryGetValue(shortUrl, out var shortUrlData))
            return null;

        return shortUrlData.Redirect();
    }

    /// <summary>
    /// Gets a snapshot of the short URLs redirecting to the long URL parameter.
    /// </summary>
    /// <param name="longUrl">The long URL.</param>
    /// <returns>
    /// A sequence of short URLs that redirect to the specified long URL or an empty sequence if none exist.
    /// </returns>
    public virtual IEnumerable<Uri> GetShortUrls(Uri longUrl)
    {
        using var _ = _lock.ReaderLock();

        return _longUrls.TryGetValue(longUrl, out var longUrlData) ? longUrlData.ShortUrls : [];
    }

    /// <summary>
    /// Gets the count of how many times the short URL was used to redirect to a long URL.
    /// Usage is incremented each time the short URL is accessed.
    /// </summary>
    /// <param name="shortUrl">The short URL.</param>
    /// <returns>
    /// The usage count for the specified short URL, which is the number of redirects to the long URL.
    /// </returns>
    public virtual int GetLongUrlRedirects(Uri shortUrl)
    {
        using var _ = _lock.ReaderLock();

        if (_shortUrls.TryGetValue(shortUrl, out var shortUrlData))
            return shortUrlData.Redirects;

        return 0;
    }

    /// <summary>
    /// Deletes the short URL and all data associated with it from the store.
    /// </summary>
    /// <param name="shortUrl">The short URL.</param>
    /// <returns>
    /// <c>true</c> if deleting the short URL was successful, <c>false</c> otherwise.
    /// </returns>
    public virtual bool DeleteShortUrl(Uri shortUrl)
    {
        using var _ = _lock.WriterLock();

        if (!_shortUrls.TryGetValue(shortUrl, out var shortUrlData))
            return false;

        var longUrl = shortUrlData.LongUrl;

        _shortUrls.Remove(shortUrl);

        if (_longUrls.TryGetValue(longUrl, out var longUrlData) &&
            longUrlData.RemoveShortUrl(shortUrl) == 0)
            _longUrls.Remove(longUrl);

        return true;
    }
}
