using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ScrumTrainer.Data;

public class DbModelRepository<T> (ApplicationDbContext dbContext): IModelRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task Delete(T model)
    {
        _dbContext.Remove(model);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ICollection<T>> FindOrDefault(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<T?> Get(params object?[]? keyValues)
    {
        return await _dbContext.FindAsync<T>(keyValues);
    }

    public async Task<T?> Insert(T model)
    {
        var entity = (await _dbContext.AddAsync(model)).Entity;
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> Update(T model)
    {
        var entity = _dbContext.Update(model).Entity;
        await _dbContext.SaveChangesAsync();
        return entity;
    }
}