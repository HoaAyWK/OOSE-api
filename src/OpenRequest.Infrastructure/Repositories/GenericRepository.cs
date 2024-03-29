using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRequest.Infrastructure.Data;
using OpenRequest.Core.Interfaces.Repositories;

namespace OpenRequest.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected AppDbContext _context;
    internal DbSet<TEntity> dbSet;
    protected readonly ILogger _logger;

    public GenericRepository(AppDbContext context, ILogger logger)
    {
        _context = context;
        dbSet = context.Set<TEntity>();
        _logger = logger;
    }
    public virtual async Task<IEnumerable<TEntity>> All()
    {
       return await dbSet.AsNoTracking().ToListAsync();
    }
    public virtual async Task<TEntity> GetById(Guid id)
    {
        return await dbSet.FindAsync(id);
    }

    public virtual async Task<bool> Add(TEntity entity)
    {
        await dbSet.AddAsync(entity);
        return true;
    }

    public virtual Task<bool> Delete(Guid id)
    {
        throw new NotImplementedException();
    }


    public virtual Task<bool> Upsert(Guid id, TEntity entity)
    {
        throw new NotImplementedException();
    }
}