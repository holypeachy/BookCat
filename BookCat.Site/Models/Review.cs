using Microsoft.AspNetCore.Identity;

namespace BookCat.Site.Models;

public class Review
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public required string UserId { get; set; }
    public required int Rating { get; set; }
    public required string Comment { get; set; }
    public required DateTime PostedAt { get; set; }
    public bool AdminDeleted { get; set; } = false;

    public Book Book { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;
}