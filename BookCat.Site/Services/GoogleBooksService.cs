using System.Text.Json;
using BookCat.Site.Models;
using BookCat.Site.Models.GoogleBooks;
using Microsoft.Extensions.Options;

namespace BookCat.Site.Services;

public class GoogleBooksService(HttpClient httpClient, IOptions<GoogleBooksOptions> options, ILogger<GoogleBooksService> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = options.Value.ApiKey;
    private readonly ILogger<GoogleBooksService> _logger = logger;
    private const string _apiString = "https://www.googleapis.com/books/v1/volumes?q=";
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<BookDto>> BookSearchName(string name)
    {
        string uri = _apiString + name + "&key=" + _apiKey;

        var response = await _httpClient.GetAsync(uri);

        return ParseResponse(await response.Content.ReadAsStringAsync());
    }

    public async Task<List<BookDto>> BookSearchIdentifier(string identifier)
    {
        string uri = _apiString + "isbn:" + identifier + "&key=" + _apiKey;

        var response = await _httpClient.GetAsync(uri);

        return ParseResponse(await response.Content.ReadAsStringAsync());
    }

    private List<BookDto> ParseResponse(string response)
    {
        try
        {
            List<BookDto> bookDtos = [];
            GoogleBooksResponse? booksResponse = JsonSerializer.Deserialize<GoogleBooksResponse>(response, _jsonSerializerOptions);
            
            if (booksResponse is null) return new List<BookDto>();
            if (booksResponse.Items is null) return new List<BookDto>();

            foreach (var item in booksResponse.Items)
            {
                bookDtos.Add(
                    new BookDto
                    {
                        GoogleId = item.Id,
                        Title = item.VolumeInfo.Title,
                        Subtitle = item.VolumeInfo.Subtitle,
                        Author = item.VolumeInfo.Authors is not null ? string.Join(", ", item.VolumeInfo.Authors) : null,
                        Description = item.VolumeInfo.Description,
                        Publisher = item.VolumeInfo.Publisher,
                        PublishedDate = item.VolumeInfo.PublishedDate,
                        CoverUrl = GetBestImage(item.VolumeInfo.ImageLinks),
                        Identifiers = item.VolumeInfo.IndustryIdentifiers
                    }
                );
            }
            foreach (var item in bookDtos)
            {
                _logger.LogInformation("BookDto Parsed:\n{}", item.ToString());
                Console.WriteLine();
            }
            return bookDtos;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Exception was raised in {Service} at {Time}\n{Exception}", nameof(GoogleBooksService), DateTime.UtcNow, ex);
            return new List<BookDto>();
        }
    }

    private static string? GetBestImage(GoogleBooksImageLinks? links)
    {
        if (links is null) return null;
        else if (links.ExtraLarge is not null) return links.ExtraLarge;
        else if (links.Large is not null) return links.Large;
        else if (links.Medium is not null) return links.Medium;
        else if (links.Small is not null) return links.Small;
        else return null;
    }

}