namespace BookCat.Site.Repos;

public interface IRepo<T> where T : class
{
    Task AddAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetByBookIdAsync(Guid id);
    Task<IEnumerable<T>> GetByUserIdAsync(string id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task DeleteAsync(T entity);
    Task<int> GetCount();
}