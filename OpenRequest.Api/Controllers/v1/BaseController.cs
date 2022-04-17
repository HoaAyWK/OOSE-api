using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.DataService.IConfiguration;

namespace OpenRequest.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseController : ControllerBase
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly UserManager<IdentityUser> _userManger;
    
    public BaseController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManger = userManager;
    }
}