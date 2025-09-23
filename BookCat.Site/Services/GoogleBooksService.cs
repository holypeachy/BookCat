using Microsoft.Extensions.Options;

namespace BookCat.Site.Services;

public class GoogleBooksService(HttpClient httpClient, IOptions<GoogleBooksOptions> options)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = options.Value.ApiKey;

}