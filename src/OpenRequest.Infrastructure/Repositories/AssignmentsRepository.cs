using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OpenRequest.Core.Entities;
using OpenRequest.Core.Interfaces.Repositories;
using OpenRequest.Infrastructure.Data;

namespace OpenRequest.Infrastructure.Repositories;

public class AssignmentsRepository : GenericRepository<Assignment>, IAssignmentsRepository
{
    public AssignmentsRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<IEnumerable<Assignment>> GetAssignmentAlmostEnd()
    {
        return await dbSet.Where(a => a.EndDate < DateTime.UtcNow.AddDays(1))
            .Include(a => a.Post)
                .ThenInclude(p => p.Freelancer)
            .ToListAsync();
    }

    public async Task<bool> Submit(Guid id, string filePath)
    {
        var existingAssignment = await dbSet.Where(a => a.Id == id)
            .FirstOrDefaultAsync();
        if (existingAssignment == null)
        {
            return false;
        }

        existingAssignment.FilePath = filePath;
        existingAssignment.Status = 0;
        return true;
    }

    public async Task<bool> Update(Guid id, string filePath)
    {
        var existingAssignment = await dbSet.Where(x => x.Id == id)
            .FirstOrDefaultAsync();
        if (existingAssignment == null)
        {
            return false;
        }

        existingAssignment.FilePath = filePath;
        existingAssignment.UpdatedDate = DateTime.UtcNow;
        return true;
    }
}