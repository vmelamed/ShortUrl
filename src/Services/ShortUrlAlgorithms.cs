namespace ShortUrl.Services;

public static partial class ShortUrlAlgorithm
{
    /// <summary>
    /// The host name used for generated short URLs.
    /// </summary>
    public const string ThisHostName = "shor.ty";

    const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.-_~";
    static readonly int CharsLength = Chars.Length;

    static int _next = 1_000_000;

    static string GetStringFromNextNumber(int number)
    {
        var bufferLength = 16;
        Span<char> buffer = stackalloc char[bufferLength];
        var current = buffer.Length;

        do
        {
            buffer[--current] = Chars[number % Chars.Length];
            number /= Chars.Length;
        }
        while (number > 0 && current >= 0);

        return buffer[current..bufferLength].ToString();
    }

    public static Uri CreateShortUrl(Uri _)
        => new UriBuilder {
            Scheme = Uri.UriSchemeHttps,
            Host = ThisHostName,
            Path = GetStringFromNextNumber(Interlocked.Increment(ref _next)),
        }
        .Uri;
}
