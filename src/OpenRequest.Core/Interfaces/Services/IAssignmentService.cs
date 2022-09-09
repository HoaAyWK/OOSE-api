using OpenRequest.Core.Entities;

namespace OpenRequest.Core.Interfaces.Services;

public interface IAssignmentService
{
    Task<IEnumerable<Assignment>> GetAssignmentsAlmostEnd();
}