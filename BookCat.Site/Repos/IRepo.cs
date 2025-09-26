namespace BookCat.Site.Repos;

public interface IRepo<T> where T : class
{
    Task AddAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}