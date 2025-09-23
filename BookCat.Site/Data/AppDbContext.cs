using BookCat.Site.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookCat.Site.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Review> Reviews { get; set; }
}