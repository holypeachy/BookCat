using BookCat.Site.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookCat.Site.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<BookIdentifier> BookIdentifiers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(u => u.NormalizedEmail).IsUnique();
        });

        builder.Entity<Book>().HasOne(b => b.AddedBy).WithMany().HasForeignKey(b => b.AddedById).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Book>().HasData(
            new Book
            {
                Id = new Guid("90adc711-8337-468a-a195-8501bac62015"),
                GoogleId = "3qRuzwEACAAJ",
                Title = "Auditing & Assurance Services",
                Subtitle = null,
                Author = "Timothy J. Louwers, Allen D. Blay, Jerry R. Strawser, Jay C. Thibodeau",
                Description = "As auditors, we are trained to investigate beyond appearances to determine the underlying facts-in other words, to look beneath the surface. From the Enron and WorldCom scandals of the early 2000s to the financial crisis of 2007-2008 to present-day issues and challenges related to significant estimation uncertainty, understanding the auditor's responsibility related to fraud, maintaining a clear perspective, probing for details, and understanding the big picture are indispensable to effective auditing. With the availability of greater levels of qualitative and quantitative information (\"Big Data\"), the need for technical skills and challenges facing today's auditor is greater than ever. The Louwers, Bagley, Blay, Strawser, and Thibodeau team has dedicated years of experience in the auditing field to this new edition of Auditing & Assurance Services, supplying the necessary investigative tools for future auditors\"",
                PublishedDate = "2023",
                AddedOn = DateOnly.FromDateTime(new DateTime(2025, 9, 25)),
                AddedById = "f01558ea-6592-4bd4-b938-eabe29da6a89",
            }
        );

        builder.Entity<Review>().HasData(
            new Review
            {
                Id = new Guid("b4556383-7b6c-4e39-afd7-df86664b83c7"),
                BookId = new Guid("90adc711-8337-468a-a195-8501bac62015"),
                UserId = "f01558ea-6592-4bd4-b938-eabe29da6a89",
                Rating = 1,
                Title = "This is my title",
                Comment = "This book fucking sucks",
                PostedAt = new DateTime(2025, 9, 25),
                AdminDeleted = false
            }
        );

        builder.Entity<BookIdentifier>().HasData(
            new BookIdentifier{
                Id = new Guid("146c6c02-ce45-46b0-b811-39ed5cdea789"),
                BookId = new Guid("90adc711-8337-468a-a195-8501bac62015"),
                Type = "ISBN_10",
                Value = "1266796851"
            },
            new BookIdentifier{
                Id = new Guid("accfc097-bb4b-4577-98dd-07b6880ebb0f"),
                BookId = new Guid("90adc711-8337-468a-a195-8501bac62015"),
                Type = "ISBN_13",
                Value = "978-1266796852"
            }
        );
    }
}