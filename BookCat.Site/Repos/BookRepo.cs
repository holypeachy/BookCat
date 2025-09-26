using BookCat.Site.Data;
using BookCat.Site.Models;

namespace BookCat.Site.Repos;

public class BookRepo : IRepo<Book>
{
    private readonly AppDbContext _db;

    public BookRepo(AppDbContext appDbContext)
    {
        _db = appDbContext;
    }
    
    public async Task AddAsync(Book entity)
    {
        await _db.Books.AddAsync(entity);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        Book? book = await _db.Books.FindAsync(id) ?? throw new KeyNotFoundException();
        _db.Books.Remove(book);

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _db.Books.ToListAsync();
    }

    public async Task<Book> GetByIdAsync(Guid id)
    {
        Book? book = await _db.Books.FindAsync(id) ?? throw new KeyNotFoundException();
        return book;
    }

    public async Task UpdateAsync(Book entity)
    {
        _db.Books.Update(entity);

        await _db.SaveChangesAsync();
    }
}