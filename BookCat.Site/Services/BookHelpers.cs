namespace BookCat.Site.Services;

public static class BookHelpers
{
    public static string? ExtractISBN(string input)
    {
        if (ISBN.TryParse(input, out ISBN? isbn))
        {
            return isbn.ToString();
        }
        return null;
    }
}