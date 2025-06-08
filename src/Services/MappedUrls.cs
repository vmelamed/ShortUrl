using ShortUrl.Abstractions;
using ShortUrl.Services.Repository;

namespace ShortUrl.Services;

public class MappedUrls(DataStore repository) : IMappedUrl
{
    /// <summary>
    /// Gets the original long URL for a given short URL. This is the method that will be called when the browser should be
    /// redirected to the true URL. Every call increases the usage statics for the long URL.
    /// </summary>
    /// <param name="shortUrl">The short URL to look up.</param>
    /// <returns>
    /// The original long URL if found; otherwise, <c>null</c>.
    /// </returns>
    public Uri? GetLongUrl(Uri shortUrl) => repository.GetLongUrl(shortUrl);

    public IEnumerable<Uri> GetShortUrls(Uri longUrl) => repository.GetShortUrls(longUrl);

    public int GetUsage(Uri shortUrl) => repository.GetLongUrlRedirects(shortUrl);
}
