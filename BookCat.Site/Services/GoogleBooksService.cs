using System.Text.Json;
using System.Text.RegularExpressions;
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
    private const string _apiStringId = "https://www.googleapis.com/books/v1/volumes/";
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<BookDto>> BookSearchName(string name)
    {
        string uri = _apiString + name + "&key=" + _apiKey;

        var response = await _httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Google Books API returned {StatusCode} for {Uri}", response.StatusCode, uri);
            return [];
        }

        return ParseSearchResponse(await response.Content.ReadAsStringAsync());
    }

    public async Task<List<BookDto>> BookSearchIdentifier(string identifier)
    {
        string uri = _apiString + "isbn:" + identifier + "&key=" + _apiKey;

        var response = await _httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Google Books API returned {StatusCode} for {Uri}", response.StatusCode, uri);
            return [];
        }

        return ParseSearchResponse(await response.Content.ReadAsStringAsync());
    }

    public async Task<BookDto> GetBookById(string id)
    {
        string uri = _apiStringId + $"{id}";

        var response = await _httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Google Books API returned {StatusCode} for {Uri}", response.StatusCode, uri);
            throw new Exception();
        }

        var parsed = ParseVolumeResponse(await response.Content.ReadAsStringAsync());
        if (parsed is null)
        {
            _logger.LogWarning("Google Books API couldn't find volume with Id {Id}", id);
            throw new Exception();
        }

        return parsed;
    }

    private List<BookDto> ParseSearchResponse(string response)
    {
        try
        {
            GoogleBooksResponse? booksResponse = JsonSerializer.Deserialize<GoogleBooksResponse>(response, _jsonSerializerOptions);
            
            List<BookDto> bookDtos = [];
            if (booksResponse is null || booksResponse.Items is null) return bookDtos;

            BookDto dto;
            foreach (var item in booksResponse.Items)
            {
                dto = new BookDto
                {
                    GoogleId = item.Id,
                    Title = item.VolumeInfo.Title,
                    Subtitle = item.VolumeInfo.Subtitle,
                    Author = item.VolumeInfo.Authors is not null ? string.Join(", ", item.VolumeInfo.Authors) : null,
                    Description = CleanDescription(item.VolumeInfo.Description),
                    Publisher = item.VolumeInfo.Publisher,
                    PublishedDate = item.VolumeInfo.PublishedDate,
                    CoverUrl = GetBestImage(item.VolumeInfo.ImageLinks),
                    Identifiers = item.VolumeInfo.IndustryIdentifiers
                };
                bookDtos.Add(dto);
            }

            return bookDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception was raised in {Service} at {Time}\n{Exception}", nameof(GoogleBooksService), DateTime.UtcNow, ex);
            return new List<BookDto>();
        }
    }

    private BookDto? ParseVolumeResponse(string response)
    {
        try
        {
            GoogleBooksItem? item = JsonSerializer.Deserialize<GoogleBooksItem>(response, _jsonSerializerOptions);
            var dto = new BookDto
            {
                GoogleId = item.Id,
                Title = item.VolumeInfo.Title,
                Subtitle = item.VolumeInfo.Subtitle,
                Author = item.VolumeInfo.Authors is not null ? string.Join(", ", item.VolumeInfo.Authors) : null,
                Description = CleanDescription(item.VolumeInfo.Description),
                Publisher = item.VolumeInfo.Publisher,
                PublishedDate = item.VolumeInfo.PublishedDate,
                CoverUrl = GetBestImage(item.VolumeInfo.ImageLinks),
                Identifiers = item.VolumeInfo.IndustryIdentifiers
            };

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception was raised in {Service} at {Time}\n{Exception}", nameof(GoogleBooksService), DateTime.UtcNow, ex);
            return null;
        }
    }

    private static string? GetBestImage(GoogleBooksImageLinks? links)
    {
        string? link;
        if (links is null) return null;
        else if (links.ExtraLarge is not null) link = links.ExtraLarge;
        else if (links.Large is not null) link = links.Large;
        else if (links.Medium is not null) link = links.Medium;
        else if (links.Small is not null) link = links.Small;
        else if (links.Thumbnail is not null) link = links.Thumbnail;
        else if (links.SmallThumbnail is not null) link = links.SmallThumbnail;
        else link = null;
        if (link is not null && link.Contains("http://"))
        {
            link = link.Replace("http://", "https://");
        }
        return link;
    }

    public static string? CleanDescription(string? s)
    {
        if (s is null) return null;
        return Regex.Replace(s, @"</?\w*>", " ");
    }

}