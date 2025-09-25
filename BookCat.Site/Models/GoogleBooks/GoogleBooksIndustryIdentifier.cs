namespace BookCat.Site.Models.GoogleBooks;

public class GoogleBooksIndustryIdentifier
{
    public required string Type { get; set; }
    public required string Identifier { get; set; }

    public override string ToString()
    {
        return $"{Type}: {Identifier}";
    }
}