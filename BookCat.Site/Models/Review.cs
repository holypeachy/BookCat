using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookCat.Site.Models;

public class Review
{
    public int Id { get; set; }
    public string BookISBN { get; set; }
    public string UserId { get; set; } = null!;
    public int Rating { get; set; }
    public string Comment { get; set; } = null!;
    public DateTime PostedAt { get; set; }
    public bool AdminDeleted { get; set; }

    public Book Book { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;
}