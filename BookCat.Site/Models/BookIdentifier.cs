namespace BookCat.Site.Models;

public class BookIdentifier
{
    public Guid Id { get; set; }

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
}