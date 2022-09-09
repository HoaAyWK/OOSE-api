using AutoMapper;
using OpenRequest.Core.Interfaces.Services;
using OpenRequest.Core.Entities;
using OpenRequest.Core.Interfaces.UoW;

namespace SioayFreelance.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AssignmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Assignment>> GetAssignmentsAlmostEnd()
    {
        return await _unitOfWork.Assignments.GetAssignmentAlmostEnd();
    }
}