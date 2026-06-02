using Microsoft.EntityFrameworkCore;
using StudentProjectManagementSystem.Application.Interfaces.Repositories;
using StudentProjectManagementSystem.Infrastructure.Data;

namespace StudentProjectManagementSystem.Infrastructure.Repositories;

public class Repository<T>(
    AppDbContext dbContext
) : IRepository<T> where T : class
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public IQueryable<T> Query()
    {
        return _dbSet;
    }

    public async Task<T?> FindAsync(params object[] keyValues)
    {
        return await _dbSet.FindAsync(keyValues);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
