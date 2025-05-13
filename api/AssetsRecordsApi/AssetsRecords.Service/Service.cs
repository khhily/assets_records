using AssetsRecords.Repository;

namespace AssetsRecords.Service;

public class Service<T> : IService<T> where T : class
{
    protected readonly IRepository<T> _repository;

    public Service(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateAsync(T entity)
    {
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        await _repository.DeleteAsync(entity);
        await _repository.SaveChangesAsync();
    }
}