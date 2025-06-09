using ShortUrl.Abstractions;
using ShortUrl.Services.ObjectModel;
using ShortUrl.Services.Repository;
using ShortUrl.Services.Sync;

namespace ShortUrl.Services;

/// <summary>
/// Implements the Commands for creating and deleting short URLs.
/// </summary>
/// <param name="createShortUrl">A function to generate a short URL from a long URL.</param>
/// <param name="dataStore">The dataStore used to store and retrieve URL mappings.</param>
public class MapUrl(
    Func<Uri, Uri> createShortUrl,
    DataStore dataStore) : IMapUrl
{
    /// <summary>
    /// Creates a short URL for the specified original URL.
    /// </summary>
    /// <param name="longUrl">The original URL to shorten.</param>
    /// <param name="forceNew">If true, forces creation of a new short URL even if one already exists for the original URL.</param>
    /// <param name="preferredShortUrl">An optional preferred short URL to use, if available. Implies <paramref name="forceNew"/> regardless of its value. </param>
    /// <returns>The generated short URL as a <see cref="Uri"/>.</returns>
    public Uri CreateShortUrl(Uri longUrl, bool forceNew = false, Uri? preferredShortUrl = null)
    {
        ShortUrlData? data = null;

        using var _ = dataStore.Lock.UpgradeableReaderLock();

        if (preferredShortUrl is not null)
        {
            data = dataStore
                        .Data
                        .FirstOrDefault(d => d.ShortUrl == preferredShortUrl)
                        ;

            if (data is not null && data.LongUrl != longUrl)
                throw new InvalidOperationException("A short URL with the specified preferred short URL already exists for a different long URL.");

            // else disregard forceNew, since we are using the preferred short URL
        }
        else
        if (!forceNew)
            // Check if a short URL already exists for the long URL
            data = dataStore
                        .Data
                        .FirstOrDefault(d => d.LongUrl == longUrl)
                        ;

        if (data is null)
        {
            using var __ = dataStore.Lock.WriterLock();

            data = new ShortUrlData(preferredShortUrl ?? createShortUrl(longUrl), longUrl);
            dataStore.Add(data);
        }

        return data.ShortUrl;
    }

    /// <summary>
    /// Deletes the specified short URL from the dataStore.
    /// </summary>
    /// <param name="shortUrl">The short URL to delete.</param>
    /// <returns>True if the short URL was successfully deleted; otherwise, false.</returns>
    public bool DeleteShortUrl(Uri shortUrl)
        => dataStore.Delete(shortUrl);
}
