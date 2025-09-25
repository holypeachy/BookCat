using BookCat.Site.Models.GoogleBooks;

namespace BookCat.Site.Models;

public class BookDto
{
    public required string GoogleId { get; set; }
    public required string Title { get; set; }
    public string? Subtitle { get; set; }
    public required string Author { get; set; }
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public required string PublishedDate { get; set; }
    public string? CoverUrl { get; set; }

    public required List<GoogleBooksIndustryIdentifier> Identifiers;

    public override string ToString()
    {
        return $"GoogleID: {GoogleId}, Title: \"{Title}\", Subtitle: \"{Subtitle}\", Author: \"{Author}\", Description: \"{Description}\", Publisher: {Publisher}, PublishedDate: \"{PublishedDate}\", CoverURL: {CoverUrl}, Identifiers: \"{string.Join(" | ", Identifiers)}\"";
    }
}