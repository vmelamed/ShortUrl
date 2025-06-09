using ShortUrl.Services.ObjectModel;

namespace ShortUrl.Services.Repository;

/// <summary>
/// Class DataStore. "Stores" the short and long URLs in memory and associated data, simulating a repository.
/// </summary>
public class DataStore(bool test = false)
{
    static readonly List<ShortUrlData> s_shortUrls = [];

    readonly List<ShortUrlData> _shortUrls = test ? [] : s_shortUrls;

    public ReaderWriterLockSlim Lock { get; } = new(LockRecursionPolicy.SupportsRecursion);

    public virtual IEnumerable<ShortUrlData> Data => _shortUrls.AsReadOnly();

    /// <summary>
    /// Adds the specified short URL data.
    /// </summary>
    /// <param name="shortUrlData">The short URL data.</param>
    /// <exception cref="System.InvalidOperationException">Short URL '{shortUrlData.ShortUrl}' already exists in the data store.</exception>
    public virtual void Add(ShortUrlData shortUrlData)
    {
        if (_shortUrls.Any(d => d.ShortUrl == shortUrlData.ShortUrl))
            throw new InvalidOperationException($"Short URL '{shortUrlData.ShortUrl}' already exists in the data store.");

        _shortUrls.Add(shortUrlData);
    }

    public virtual bool Delete(Uri shortUrl)
    {
        var data = _shortUrls.FirstOrDefault(d => d.ShortUrl == shortUrl);

        if (data is not null)
            _shortUrls.Remove(data);

        return data is not null;
    }
}
