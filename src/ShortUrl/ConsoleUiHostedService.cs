using Microsoft.Extensions.Hosting;

using ShortUrl.Abstractions;

namespace ShortUrl;

public class ConsoleUiHostedService(IMapUrl mapUrl, IMappedUrl mappedUrl) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Use a scope if you need scoped services
        string? choice = null;
        do
        {
            Console.Write("""

            Menu:
               1. Create a short URL
               2. Delete a short URL
               3. Get long URL from short URL
               4. Get statistics for a short URL
               5. Get all short URLs for a long URL

               [enter] to exit

            Enter your choice:
            """);
            choice = (Console.ReadLine() ?? "").Trim();

            if (choice == "")
            {
                Console.WriteLine("Exiting the application.");
                break;
            }

            if (!"12345".Contains(choice))
            {
                Console.WriteLine("Invalid choice. Please try again.");
                continue;
            }

            switch (choice)
            {
            case "1":
                CreateShortUrl();
                break;
            case "2":
                DeleteShortUrl();
                break;
            case "3":
                RedirectShortUrl();
                break;
            case "4":
                GetUsage();
                break;
            case "5":
                GetShortUrlsForLongUrl();
                break;
            }
        }
        while (!stoppingToken.IsCancellationRequested);

        await Task.CompletedTask;
    }

    void CreateShortUrl()
    {
        Console.Write("Enter the long URL to shorten: ");
        var longUrlInput = Console.ReadLine()?.Trim();

        if (!Uri.TryCreate(longUrlInput, UriKind.Absolute, out var longUrl))
        {
            Console.WriteLine("\nInvalid long URL. Please, try again.\n");
            return;
        }

        Console.Write("Enter preferred short URL or just [enter]: ");
        var shortUrlInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(shortUrlInput) ||
            !Uri.TryCreate(shortUrlInput, UriKind.Absolute, out var shortUrl))
            shortUrl = null;

        Console.Write("If there is already a short URL, do you want to re-use it [Y/n]: ");
        var forceNew = Console.ReadLine()?.Trim().ToLowerInvariant() switch
                    {
                        "y" or "yes" or "" => false,
                        _ => true
                    };

        shortUrl = mapUrl.CreateShortUrl(longUrl, forceNew, shortUrl);

        Console.WriteLine($"\nShort URL created: {shortUrl}\n");
    }

    void DeleteShortUrl()
    {
        Console.Write("Enter the short URL to delete: ");
        var shortUrlInput = Console.ReadLine()?.Trim();

        if (!Uri.TryCreate(shortUrlInput, UriKind.Absolute, out var shortUrlToDelete))
        {
            Console.WriteLine("\nInvalid short URL. Please, try again.\n");
            return;
        }

        var deleted = mapUrl.DeleteShortUrl(shortUrlToDelete);

        if (deleted)
            Console.WriteLine($"\nShort URL deleted: {shortUrlToDelete}\n");
        else
            Console.WriteLine($"\nShort URL not found: {shortUrlToDelete}\n");
    }

    void RedirectShortUrl()
    {
        Console.Write("Enter the short URL to redirect: ");
        var shortUrlInput = Console.ReadLine()?.Trim();

        if (!Uri.TryCreate(shortUrlInput, UriKind.Absolute, out var shortUrlToRedirect))
        {
            Console.WriteLine("\nInvalid short URL format.\n");
            return;
        }

        var longUrl = mappedUrl.GetLongUrl(shortUrlToRedirect);

        if (longUrl is null)
        {
            Console.WriteLine($"\nNo long URL found for {shortUrlToRedirect}\n");
            return;
        }

        Console.WriteLine($"\nRedirecting to: {longUrl}\n");
    }

    void GetUsage()
    {
        Console.Write("Enter the short URL to get statistics for: ");
        var shortUrl = Console.ReadLine()?.Trim();

        if (!Uri.TryCreate(shortUrl, UriKind.Absolute, out var shortUri))
        {
            Console.WriteLine("\nInvalid short URL format.\n");
            return;
        }

        var usageCount = mappedUrl.GetUsage(shortUri);

        Console.WriteLine($"\nUsage count for {shortUri}: {usageCount}\n");
    }

    void GetShortUrlsForLongUrl()
    {
        Console.Write("Enter the long URL to get short URLs for: ");
        var longUrlInput = Console.ReadLine()?.Trim();

        if (!Uri.TryCreate(longUrlInput, UriKind.Absolute, out var longUrl))
        {
            Console.WriteLine("\nInvalid long URL format.\n");
            return;
        }

        var shortUrls = mappedUrl.GetShortUrls(longUrl);

        if (!shortUrls.Any())
        {
            Console.WriteLine($"\nNo short URLs found for {longUrl}\n");
            return;
        }

        Console.WriteLine($"\nShort URLs for {longUrl}:");
        foreach (var url in shortUrls)
            Console.WriteLine($"  - {url}");
    }
}