namespace BookCat.Site.Services;

public static class BookHelpers
{
    public static bool IsISBN(string input)
    {
        if (ISBN.TryParse(input, out ISBN? isbn))
        {
            return true;
        }
        return false;
    }
}