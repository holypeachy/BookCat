using BookCat.Site.Models.GoogleBooks;

namespace BookCat.Site.Models;
public class BookDto
{
    public required string GoogleId { get; set; }
    public required string Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public string? PublishedDate { get; set; }
    public string? CoverUrl { get; set; }

    public List<GoogleBooksIndustryIdentifier>? Identifiers;

    public override string ToString()
    {
        return $"GoogleID: {GoogleId}, Title: \"{Title}\", Subtitle: \"{Subtitle}\", Author: \"{Author}\", Description: \"{Description}\", Publisher: \"{Publisher}\", PublishedDate: \"{PublishedDate}\", CoverURL: \"{CoverUrl}\", Identifiers: {string.Join(" | ", Identifiers ?? [])}";
    }
}