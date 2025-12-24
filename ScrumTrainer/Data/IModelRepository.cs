using System.Linq.Expressions;

namespace ScrumTrainer.Data;

public interface IModelRepository<T> where T : class
{
    public Task<T?> Insert(T model);
    public Task<T?> Update(T model);
    public Task<ICollection<T>> FindOrDefault(Expression<Func<T, bool>> predicate);
    public Task<T?> Get(params object?[]? keyValues);
    public Task Delete(T model);
}