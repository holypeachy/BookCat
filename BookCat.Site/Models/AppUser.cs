using Microsoft.AspNetCore.Identity;

namespace BookCat.Site.Models;

public class AppUser : IdentityUser
{
    public string UserImageUrl { get; set; } = string.Empty;
}