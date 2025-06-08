using ShortUrl.Abstractions;
using ShortUrl.Services.Repository;

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
    /// <param name="preferredShortUrl">An optional preferred short URL to use, if available.</param>
    /// <returns>The generated short URL as a <see cref="Uri"/>.</returns>
    public Uri CreateShortUrl(Uri longUrl, bool forceNew = false, Uri? preferredShortUrl = null)
    {
        if (!forceNew)
        {
            // Check if a short URL already exists for the original URL
            var existingShortUrl = preferredShortUrl is null
                                        ? dataStore.GetShortUrls(longUrl).FirstOrDefault()
                                        : dataStore.GetShortUrls(longUrl).FirstOrDefault(u => u == preferredShortUrl);

            if (existingShortUrl is not null)
                return existingShortUrl;
        }

        if (preferredShortUrl is not null && dataStore.AddUrlPair(preferredShortUrl, longUrl))
            return preferredShortUrl;

        Uri shortUrl = createShortUrl(longUrl);

        dataStore.AddUrlPair(shortUrl, longUrl);
        return shortUrl;
    }

    /// <summary>
    /// Deletes the specified short URL from the dataStore.
    /// </summary>
    /// <param name="shortUrl">The short URL to delete.</param>
    /// <returns>True if the short URL was successfully deleted; otherwise, false.</returns>
    public bool DeleteShortUrl(Uri shortUrl)
        => dataStore.DeleteShortUrl(shortUrl);
}
