using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.DataService.IConfiguration;
using OpenRequest.Entities.DTO.Errors;

namespace OpenRequest.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseController : ControllerBase
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly UserManager<IdentityUser> _userManger;
    protected readonly IMapper _mapper;
    
    public BaseController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManger = userManager;
        _mapper = mapper;
    }

    internal Error PopulateError(int code, string type, string message)
    {
        return new Error
        {
            Code = code,
            Type = type,
            Message = message
        };
    }
}