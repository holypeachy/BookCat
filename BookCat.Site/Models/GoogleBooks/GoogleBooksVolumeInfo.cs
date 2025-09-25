namespace BookCat.Site.Models.GoogleBooks;

public class GoogleBooksVolumeInfo
{
    public required string Title { get; set; }
    public string? Subtitle { get; set; }
    public required List<string> Authors { get; set; }
    public required string Publisher { get; set; }
    public required string PublishedDate { get; set; }
    public string? Description { get; set; }
    public required List<GoogleBooksIndustryIdentifier> IndustryIdentifiers { get; set; }
    public GoogleBooksImageLinks? ImageLinks { get; set; }
}