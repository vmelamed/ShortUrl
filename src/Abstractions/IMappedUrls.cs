namespace ShortUrl.Abstractions;

/// <summary>
/// Defines the contract for mapping between short URLs and their corresponding long/original URLs,
/// as well as tracking usage statistics for each mapping. These are the methods that will be called when the browser should be
/// redirected to the true URL.
/// This behavior is typically implemented by the Query side of the CQRS pattern, where the focus is on reading and retrieving
/// data rather than modifying it.
/// </summary>
public interface IMappedUrl
{
    /// <summary>
    /// Gets the original long URL for a given short URL. This is the method that will be called when the browser should be
    /// redirected to the true URL. Every call increases the usage statics for the long URL.
    /// </summary>
    /// <param name="shortUrl">The short URL to look up.</param>
    /// <returns>
    /// The original long URL if found; otherwise, <c>null</c>.
    /// </returns>
    Uri? GetLongUrl(Uri shortUrl);

    /// <summary>
    /// Gets all short URLs that forward to a given original URL.
    /// </summary>
    /// <param name="longUrl">The long URL to look up.</param>
    /// <returns>
    /// A sequence of all short URLs mapped to the long URL.
    /// </returns>
    IEnumerable<Uri> GetShortUrls(Uri longUrl);

    /// <summary>
    /// Gets the number of times a long URL has been used.
    /// </summary>
    /// <param name="shortUrl">The short URL to get the usage of.</param>
    /// <returns>
    /// The number of times the specified short URL has been used.
    /// </returns>
    int GetUsage(Uri shortUrl);
}
