using System.Linq.Expressions;
using System.Reflection;
using ScrumTrainer.Data;

namespace ScrumTrainerTests;

public class TestModelRepository<T> : IModelRepository<T> where T : class
{
    private readonly List<T>_modelSet = [];
    private int _nextId = 1;

    public List<T> ModelSet{ get => _modelSet; }

    private readonly PropertyInfo _idPropertyInfo = typeof(T).GetProperty("Id") 
                ?? throw new InvalidOperationException($"Type {typeof(T).Name} does not have an 'Id' property");

    public Task Delete(T model)
    {
        _modelSet.Remove(model);
        return Task.CompletedTask;
    }

    public Task<ICollection<T>> FindOrDefault(Expression<Func<T, bool>> predicate)
    {
        return Task.FromResult<ICollection<T>>([.. _modelSet.Where(predicate.Compile())]);
    }

    public Task<T?> Get(params object?[]? keyValues)
    {
        if (keyValues == null || keyValues.Length == 0)
            return Task.FromResult<T?>(default);
    
        var result = _modelSet.FirstOrDefault(item =>
            _idPropertyInfo.GetValue(item)?.Equals(keyValues[0]) ?? false);
        
        return Task.FromResult(result);
    }

    public Task<T?> Insert(T model)
    {
        _idPropertyInfo.SetValue(model, _nextId++);
        _modelSet.Add(model);

        return Task.FromResult<T?>(model);
    }

    public Task<T?> Update(T model)
    {
        var id = _idPropertyInfo.GetValue(model);

        var index = _modelSet.FindIndex(m => _idPropertyInfo.GetValue(m) == id);
        _modelSet[index] = model;

        return Task.FromResult<T?>(model);
    }
}
