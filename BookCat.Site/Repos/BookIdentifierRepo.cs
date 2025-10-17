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

    public async Task<IEnumerable<BookIdentifier>> GetAllAsync()
    {
        return await _db.BookIdentifiers.Include(bi => bi.Book).ToListAsync();
    }

    public async Task<BookIdentifier> GetByIdAsync(Guid id)
    {
        BookIdentifier? identifier = await _db.BookIdentifiers.FindAsync(id) ?? throw new KeyNotFoundException();
        await _db.Entry(identifier).Reference(bi => bi.Book).LoadAsync();
        return identifier;
    }

    public async Task UpdateAsync(BookIdentifier entity)
    {
        _db.BookIdentifiers.Update(entity);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        BookIdentifier? identifier = await _db.BookIdentifiers.FindAsync(id) ?? throw new KeyNotFoundException();
        _db.BookIdentifiers.Remove(identifier);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(BookIdentifier entity)
    {
        _db.BookIdentifiers.Remove(entity);

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<BookIdentifier>> GetByBookIdAsync(Guid id)
    {
        return _db.BookIdentifiers.Where(bi => bi.BookId == id);
    }

    public Task<IEnumerable<BookIdentifier>> GetByUserIdAsync(string id)
    {
        throw new NotImplementedException();
    }
}