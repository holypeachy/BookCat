using Microsoft.AspNetCore.Identity;

namespace BookCat.Site.Models;

public class AppUser : IdentityUser
{
    public string UserImageUrl { get; set; } = string.Empty;
    public required DateTime JoinedDateTime { get; set; }
    public string Bio { get; set; } = string.Empty;
}