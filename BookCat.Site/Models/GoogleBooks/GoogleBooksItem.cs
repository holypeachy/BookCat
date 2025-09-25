namespace BookCat.Site.Models.GoogleBooks;

public class GoogleBooksItem
{
    public required string Id { get; set; }
    public required GoogleBooksVolumeInfo VolumeInfo{ get; set; }
}