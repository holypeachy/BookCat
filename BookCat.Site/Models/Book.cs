namespace BookCat.Site.Models;
public class Book
{
    public Guid Id { get; set; }
    public required string GoogleId { get; set; }
    public required string Title { get; set; }
    public string? Subtitle { get; set; } = null!;
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public string? PublishedDate { get; set; }
    public string? CoverUrl { get; set; }
    public required DateTime AddedOn { get; set; }
    public string? AddedById { get; set; }

    public ICollection<BookIdentifier> Identifiers { get; set; } = [];
    public List<Review> Reviews { get; set; } = [];
    public AppUser? AddedBy { get; set; }
}