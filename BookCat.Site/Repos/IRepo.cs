namespace BookCat.Site.Repos;

public interface IRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}