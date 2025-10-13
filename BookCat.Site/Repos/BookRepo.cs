using BookCat.Site.Data;
using BookCat.Site.Models;

namespace BookCat.Site.Repos;

public class BookRepo : IRepo<Book>
{
    private readonly AppDbContext _db;
    private readonly IRepo<BookIdentifier> _bookIdfs;

    public BookRepo(AppDbContext appDbContext, IRepo<BookIdentifier> bookIdentifierRepo)
    {
        _db = appDbContext;
        _bookIdfs = bookIdentifierRepo;
    }
    
    public async Task AddAsync(Book entity)
    {
        Book book = (await _db.Books.AddAsync(entity)).Entity;
        foreach (var item in book.Identifiers)
        {
            await _bookIdfs.AddAsync(item);
        }
        
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _db.Books.Include(b => b.Identifiers).Include( b => b.Reviews).Include(b => b.AddedBy).ToListAsync();
    }

    public async Task<Book> GetByIdAsync(Guid id)
    {
        Book? book = await _db.Books.FindAsync(id) ?? throw new KeyNotFoundException();
        await _db.Entry(book).Reference(b => b.Identifiers).LoadAsync();
        await _db.Entry(book).Reference(b => b.Reviews).LoadAsync();
        await _db.Entry(book).Reference(b => b.AddedBy).LoadAsync();
        return book;
    }

    public async Task UpdateAsync(Book entity)
    {
        throw new NotImplementedException(nameof(BookRepo) + "UpdateAsync, never update books");
    }

    public async Task DeleteAsync(Guid id)
    {
        Book? book = await _db.Books.FindAsync(id) ?? throw new KeyNotFoundException();
        _db.Books.Remove(book);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book entity)
    {
        _db.Books.Remove(entity);

        await _db.SaveChangesAsync();
    }
}