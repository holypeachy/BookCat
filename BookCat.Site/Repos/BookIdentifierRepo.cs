using BookCat.Site.Data;
using BookCat.Site.Models;

namespace BookCat.Site.Repos;

public class BooksIdentifierRepo : IRepo<BookIdentifier>
{
    private readonly AppDbContext _db;

    public BooksIdentifierRepo(AppDbContext appDbContext)
    {
        _db = appDbContext;
    }

    public async Task AddAsync(BookIdentifier entity)
    {
        await _db.BookIdentifiers.AddAsync(entity);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        BookIdentifier? identifier = await _db.BookIdentifiers.FindAsync(id) ?? throw new KeyNotFoundException();
        _db.BookIdentifiers.Remove(identifier);

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<BookIdentifier>> GetAllAsync()
    {
        return await _db.BookIdentifiers.ToListAsync();
    }

    public async Task<BookIdentifier> GetByIdAsync(Guid id)
    {
        BookIdentifier? identifier = await _db.BookIdentifiers.FindAsync(id) ?? throw new KeyNotFoundException();
        return identifier;
    }

    public async Task UpdateAsync(BookIdentifier entity)
    {
        _db.BookIdentifiers.Update(entity);

        await _db.SaveChangesAsync();
    }
}