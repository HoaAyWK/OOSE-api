namespace OpenRequest.DataService.IRepository;

public interface IGenericRepository<TEntity> where TEntity : class
{
    // Get all entities
    Task<IEnumerable<TEntity>> All();

    // Get entity by Id
    Task<TEntity> GetById(Guid id);

    Task<bool> Add(TEntity entity);

    // Update entity or add if it does not exist
    Task<bool> Upsert(Guid id, TEntity entity);

    Task<bool> Delete(Guid id);
}