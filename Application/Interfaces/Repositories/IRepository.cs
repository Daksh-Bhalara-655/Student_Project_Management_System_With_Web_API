namespace StudentProjectManagementSystem.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();

    Task<T?> FindAsync(params object[] keyValues);

    Task AddAsync(T entity);

    void Remove(T entity);

    Task SaveChangesAsync();
}
