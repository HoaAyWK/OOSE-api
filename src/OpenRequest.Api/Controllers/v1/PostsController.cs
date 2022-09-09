using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.Core.Dtos.Posts;
using OpenRequest.Core.Interfaces.Services;


namespace OpenRequest.Api.Controllers.v1;


public class PostsController : BaseController
{
    private readonly IPostService _postService;
    public PostsController(IPostService postService) : base()
    {        
        _postService = postService;
    }

    [HttpGet]
    [Route("get-posts")]
    public async Task<IActionResult> GetAllAvailablePostsAsync(int status = 1)
    {
        var result = await _postService.GetPostsAsync(status);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-customer-posts")]
    public async Task<IActionResult> GetCustomerPostsAsync(Guid id, int status = 1)
    {
        var result = await _postService.GetPostsByAuthorAsync(id, status);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-owner-posts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetOwnerPostsAsync(Guid id, int status = 1)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _postService.GetOwnerPostsAsync(token, status);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-post-detail")]
    public async Task<IActionResult> GetPostAsync(Guid id)
    {
        var result = await _postService.GetPostByIdAsync(id);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-full-post-detail")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetFullPostAsync(Guid id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();
        
        var result = await _postService.GetProcessingOrClosedPostAsync(token ,id);

        if (result.IsSuccess)
        {
            return Ok(result);
        }
        else 
        {
            return BadRequest(result);
        }
    }

    [HttpPost]
    [Route("create")]
    [Authorize(Roles = "Customer")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateAsync([FromBody] PostRequest request)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _postService.CreateAsync(token, request);

        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    [HttpPut]
    [Route("update")]
    [Authorize(Roles = "Customer")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] PostRequest request)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _postService.UpdateAsync(token, id, request);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete]
    [Route("delete")]
    [Authorize(Roles = "Admin, Customer")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _postService.DeleteAsync(token, id);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Route("select")]
    [Authorize(Roles = "Freelancer")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SelectAsync(Guid id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _postService.SelectAsync(token, id);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Route("unselect")]
    [Authorize(Roles = "Freelancer")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UnselectAsync(Guid id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _postService.UnselectAsync(token, id);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPut]
    [Route("select-freelancer")]
    [Authorize(Roles = "Customer")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SelectFreelancerAsync(Guid postId, Guid freelancerId)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (token == null)
            return BadRequest();

        var result = await _postService.SelectFreelancerAsync(token, postId, freelancerId);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    [HttpGet]
    [Route("get-posts-almost-end")]
    public async Task<IActionResult> GetPostsAlmostEndAsync()
    {
        var result = await _postService.GetPostsAlmostEnd();
        return Ok(result);
    }
}