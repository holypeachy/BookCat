using System.Text.Json;
using BookCat.Site.Models;
using BookCat.Site.Models.GoogleBooks;
using Microsoft.Extensions.Options;

namespace BookCat.Site.Services;

public class GoogleBooksService(HttpClient httpClient, IOptions<GoogleBooksOptions> options)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = options.Value.ApiKey;
    private const string _apiString = "https://www.googleapis.com/books/v1/volumes?q=";
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<BookDto>> BookSearchName(string name)
    {
        string uri = _apiString + name + "&key=" + _apiKey;

        var response = await _httpClient.GetAsync(uri);

        return ParseResponse(await response.Content.ReadAsStringAsync());
    }

    public async Task<List<BookDto>> BookSearchISBN(string ISBN)
    {
        string uri = _apiString + "isbn:" + ISBN + "&key=" + _apiKey;

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
                        Title = item.VolumeInfo.Title + (item.VolumeInfo.Subtitle is not null ? $": {item.VolumeInfo.Subtitle}" : ""),
                        Subtitle = item.VolumeInfo.Subtitle,
                        Author = string.Join(", ", item.VolumeInfo.Authors),
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
                Console.WriteLine(item);
                Console.WriteLine();
            }
            return bookDtos;
        }
        catch
        {
            return new List<BookDto>();
        }
    }

    private string? GetBestImage(GoogleBooksImageLinks? links)
    {
        if (links is null) return null;
        else if (links.ExtraLarge is not null) return links.ExtraLarge;
        else if (links.Large is not null) return links.Large;
        else if (links.Medium is not null) return links.Medium;
        else if (links.Small is not null) return links.Small;
        else return null;
    }

}