using BookCat.Site.Data;
using BookCat.Site.Models;

namespace BookCat.Site.Repos;

public class ReviewRepo : IRepo<Review>
{
    private readonly AppDbContext _db;

    public ReviewRepo(AppDbContext appDbContext)
    {
        _db = appDbContext;
    }

    public async Task AddAsync(Review entity)
    {
        await _db.Reviews.AddAsync(entity);

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await _db.Reviews.ToListAsync();
    }

    public async Task<Review> GetByIdAsync(Guid id)
    {
        Review? review = await _db.Reviews.FindAsync(id) ?? throw new KeyNotFoundException();
        return review;
    }

    public async Task UpdateAsync(Review entity)
    {
        _db.Reviews.Update(entity);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        Review? review = await _db.Reviews.FindAsync(id) ?? throw new KeyNotFoundException();
        _db.Reviews.Remove(review);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Review entity)
    {
        _db.Reviews.Remove(entity);

        await _db.SaveChangesAsync();
    }
}