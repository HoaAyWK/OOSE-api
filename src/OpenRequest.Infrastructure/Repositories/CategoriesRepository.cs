using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OpenRequest.Core.Entities;
using OpenRequest.Core.Interfaces.Repositories;
using OpenRequest.Infrastructure.Data;

namespace OpenRequest.Infrastructure.Repositories;

public class CategoriesRepository : GenericRepository<Category>, ICategoriesRepository
{
    public CategoriesRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {

    }

    public async Task<Category> GetCategoryById(Guid id)
    {
        var category = await dbSet.Where(c => c.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        
        return category;
    }

    public override async Task<bool> Upsert(Guid id, Category category)
    {
        try
        {
            var existingCategory = await dbSet.Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (existingCategory == null)
            {
                return await Add(category);
            }
            
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Upsert method error.", typeof(CategoriesRepository));
            return false;
        }

    }

    public override async Task<bool> Delete(Guid Id)
    {
        try
        {
            var existingCategory = await dbSet.Where(x => x.Id == Id)
                .FirstOrDefaultAsync();
            
            if (existingCategory != null)
            {
                dbSet.Remove(existingCategory);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Delete method error.", typeof(CategoriesRepository));
            return false;
        }
    }
}