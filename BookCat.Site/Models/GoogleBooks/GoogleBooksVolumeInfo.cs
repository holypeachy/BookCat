namespace BookCat.Site.Models.GoogleBooks;

public class GoogleBooksVolumeInfo
{
    public required string Title { get; set; }
    public string? Subtitle { get; set; }
    public List<string>? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? PublishedDate { get; set; }
    public string? Description { get; set; }
    public List<GoogleBooksIndustryIdentifier>? IndustryIdentifiers { get; set; }
    public GoogleBooksImageLinks? ImageLinks { get; set; }
}