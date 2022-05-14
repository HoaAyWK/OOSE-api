using OpenRequest.Entities.DbSets;

namespace OpenRequest.DataService.IRepository;

public interface IAssignmentsRepository : IGenericRepository<Assignment>
{
    Task<bool> Submit(Guid id, string filePath);
}
