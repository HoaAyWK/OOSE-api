using OpenRequest.Core.Entities;

namespace OpenRequest.Core.Interfaces.Repositories;

public interface IAssignmentsRepository : IGenericRepository<Assignment>
{
    Task<bool> Submit(Guid id, string filePath);

    Task<bool> Update(Guid id, string filePath);

    Task<IEnumerable<Assignment>> GetAssignmentAlmostEnd();
}
