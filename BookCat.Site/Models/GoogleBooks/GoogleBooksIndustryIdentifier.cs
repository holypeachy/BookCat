namespace BookCat.Site.Models.GoogleBooks;

public class GoogleBooksIndustryIdentifier
{
    public string? Type { get; set; }
    public string? Identifier { get; set; }

    public override string ToString()
    {
        return $"{Type}: {Identifier}";
    }
}