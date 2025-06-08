namespace ShortUrl.Abstractions;

/// <summary>
/// Provides methods for creating and deleting short URLs mapped to original URLs. These methods are typically used to manage
/// the lifecycle of short URLs, allowing users to create new short URLs or delete existing ones.
/// This behavior is typically implemented by the Command side of the CQRS pattern, where the focus is on modifying data rather
/// than reading it.
/// </summary>
public interface IMapUrl
{
    /// <summary>
    /// Creates a short URL for the specified original URL.
    /// </summary>
    /// <param name="originalUrl">The original URL to shorten.</param>
    /// <param name="forceNew">If true, forces creation of a new short URL even if one already exists for the original URL.</param>
    /// <param name="preferredShortUrl">An optional preferred short URL to use, if available.</param>
    /// <returns>The generated short URL as a <see cref="Uri"/>.</returns>
    Uri CreateShortUrl(Uri originalUrl, bool forceNew = false, Uri? preferredShortUrl = null);

    /// <summary>
    /// Deletes the specified short URL.
    /// </summary>
    /// <param name="shortUrl">The short URL to delete.</param>
    /// <returns>True if the short URL was successfully deleted; otherwise, false.</returns>
    bool DeleteShortUrl(Uri shortUrl);
}
