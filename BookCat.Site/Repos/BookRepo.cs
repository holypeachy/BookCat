using BookCat.Site.Data;
using BookCat.Site.Models;

namespace BookCat.Site.Repos;

public class BookRepo : IRepo<Book>
{
    private readonly AppDbContext _db;
    private readonly IRepo<BookIdentifier> _identifiers;

    public BookRepo(AppDbContext appDbContext, IRepo<BookIdentifier> bookIdentifierRepo)
    {
        _db = appDbContext;
        _identifiers = bookIdentifierRepo;
    }
    
    public async Task AddAsync(Book entity)
    {
        Book book = (await _db.Books.AddAsync(entity)).Entity;
        foreach (var item in book.Identifiers)
        {
            await _identifiers.AddAsync(item);
        }
        
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _db.Books.Include(b => b.Identifiers).Include( b => b.Reviews).Include(b => b.AddedBy).ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        Book? book = await _db.Books.FindAsync(id);
        if (book is null) return null;
        await _db.Entry(book).Collection(b => b.Identifiers).LoadAsync();
        await _db.Entry(book).Reference(b => b.AddedBy).LoadAsync();
        return book;
    }

    public async Task UpdateAsync(Book entity)
    {
        _db.Books.Update(entity);

        await _db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        Book? book = await _db.Books.FindAsync(id);
        if (book is null) return false;
        _db.Books.Remove(book);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAsync(Book entity)
    {
        _db.Books.Remove(entity);

        await _db.SaveChangesAsync();
    }

    public Task<IEnumerable<Book>> GetByBookIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Book>> GetByUserIdAsync(string id)
    {
        return _db.Books.Where(b => b.AddedById == id);
    }

    public async Task<int> GetCountAsync()
    {
        return await _db.Books.CountAsync();
    }
}