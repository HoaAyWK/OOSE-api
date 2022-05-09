using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OpenRequest.DataService.Data;
using OpenRequest.DataService.IRepository;
using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.Repository;

public class AssignmentsRepository : GenericRepository<Assignment>, IAssignmentsRepository
{
    public AssignmentsRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
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
}