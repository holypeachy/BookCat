using System.ComponentModel.DataAnnotations;

namespace BookCat.Site.Models;

public class Book
{
    [Key]
    public string ISBN { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string? CoverUrl { get; set; }
    public string? Description { get; set; }
    public DateOnly AddedOn { get; set; }

    public List<Review> Reviews { get; set; } = new();
}