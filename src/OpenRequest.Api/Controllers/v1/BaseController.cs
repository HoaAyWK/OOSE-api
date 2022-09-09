using Microsoft.AspNetCore.Mvc;

namespace OpenRequest.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseController : ControllerBase
{  
    public BaseController()
    {
    }
}