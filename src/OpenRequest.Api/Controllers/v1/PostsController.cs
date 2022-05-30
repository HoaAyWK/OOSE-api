using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.Configuration.Messages;
using OpenRequest.Configuration.Status;
using OpenRequest.DataService.IConfiguration;
using OpenRequest.Entities.DbSets;
using OpenRequest.Entities.DTO.Generic;
using OpenRequest.Entities.DTO.Incoming;

namespace OpenRequest.Api.Controllers.v1;


public class PostsController : BaseController
{
    public PostsController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager,
        IMapper mapper) : base(unitOfWork, userManager, mapper)
    {        
    }

    [HttpGet]
    [Route("All")]
    public async Task<IActionResult> GetPosts()
    {
        var posts = await _unitOfWork.Posts.All();
        var result = new PagedResult<Post>();
        result.Content = posts.ToList();
        result.ResultCount = posts.Count();

        return Ok(result);
    }

    [HttpGet]
    [Route("AllActivePosts")]
    public async Task<IActionResult> GetActivePosts()
    {
        var result = new PagedResult<Post>();
        var activePosts = await _unitOfWork.Posts.GetActivePosts();

        result.Content = activePosts.ToList();
        result.ResultCount = activePosts.Count();

        return Ok(result);
    }

    [HttpGet]
    [Route("GetPost", Name = "GetPost")]
    public async Task<IActionResult> GetPost(Guid id)
    {
        var post = await _unitOfWork.Posts.GetById(id);
        var result = new Result<Post>();

        if (post == null)
        {
            result.Error = PopulateError(404,
                ErrorMessages.Type.NotFound,
                ErrorMessages.Post.NotFound);
            return BadRequest(result);
        }

        result.Content = post;
        return Ok(result);
    }

    [HttpGet]
    [Route("GetHighestPricePosts")]
    public async Task<IActionResult> GetHighestPricePosts(int number)
    {
        var result = new PagedResult<Post>();
        var posts = await _unitOfWork.Posts.GetHighestPricePosts(number);
        result.Content = posts.ToList();
        result.ResultCount = posts.Count();

        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("GetPostRequestsByPostId")]
    public async Task<IActionResult> GetPostRequestsByPostId(Guid postId)
    {
        var result = new PagedResult<PostRequest>();
        var postRequests = await _unitOfWork.PostRequests.GetPostRequestsByPostId(postId);
        result.Content = postRequests.ToList();
        result.ResultCount = postRequests.Count();
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("GetCustomerPosts")]
    public async Task<IActionResult> GetCustomerPosts()
    {
        var loggedInUser = await _userManger.GetUserAsync(HttpContext.User);
        var result = new Result<List<Post>>();

        if (loggedInUser == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Type.BadRequest,
                ErrorMessages.Profile.NotFound);    

            return BadRequest(result); 
        }
        
        var identityId = new Guid(loggedInUser.Id);
        var customer = await _unitOfWork.Users.GetByIdentityId(identityId);
        if (customer == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Type.BadRequest,
                ErrorMessages.Profile.NotFound);    

            return BadRequest(result);
        }

        var posts = await _unitOfWork.Posts.GetCustomerPosts(customer.Id);
        result.Content = posts.ToList();

        return Ok(result);
    }
    
    [HttpGet]
    [Route("GetActivePostsByCustomer")]
    public async Task<IActionResult> GetActivePostsByCustomer(Guid id)
    {
        var result = new PagedResult<Post>();
        var posts = await _unitOfWork.Posts.GetCustomerActivePosts(id);
        result.Content = posts.ToList();
        result.ResultCount = posts.Count();

        return Ok(result);
    }


    [HttpGet]
    [Route("GetPostsByCategory")]
    public async Task<IActionResult> GetPostsByCategory(Guid id)
    {
        var result = new Result<List<Post>>();
        var posts = await _unitOfWork.Posts.GetPostsByCategory(id);
        result.Content = posts;
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Route("SelectPost")]
    public async Task<IActionResult> SelectPost(Guid id)
    {
        var result = new Result<string>();
        var loggedInUser = await _userManger.GetUserAsync(HttpContext.User);
        if (loggedInUser == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Type.BadRequest,
                ErrorMessages.Users.NotFound
            );
            return BadRequest(result);
        }

        var identityId = new Guid(loggedInUser.Id);
        var freelancer = await _unitOfWork.Users.GetByIdentityId(identityId);
        if (freelancer == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Type.BadRequest,
                ErrorMessages.Users.NotFound
            );
            return BadRequest(result);
        }
        var postRequest = new PostRequest { PostId = id, FreelancerId = freelancer.Id, Status = 0 };
        var added = await _unitOfWork.PostRequests.Add(postRequest);
        if (!added)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Type.BadRequest,
                ErrorMessages.Post.ErrorWhenSelect
            );
            return BadRequest(result);
        }
        await _unitOfWork.CompleteAsync();
        result.Content = "Selected";
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    [Route("UnSelectPost")]
    public async Task<IActionResult> UnSelectPost(Guid postId, Guid freelancerId)
    {
        var result = new Result<string>();
        var deleted = await _unitOfWork.PostRequests.Delete(postId, freelancerId);
        if (deleted)
        {
            await _unitOfWork.CompleteAsync();
            result.Content = "Deleted";
            return Ok(result);
        }
        result.Error = PopulateError(400,
            ErrorMessages.Type.UnableToProcess,
            ErrorMessages.PostRequest.CannotDelete
        );
        return BadRequest(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("FinishPost")]
    public async Task<IActionResult> FinishPost([FromBody] FinishAssignmentDto finishAssignmentDto) 
    {
        var result = new Result<string>();
        Guid postId = new Guid(finishAssignmentDto.PostId);
        var post = await _unitOfWork.Posts.GetById(postId);
        if (post == null) 
        {
            result.Error = PopulateError(400,
                ErrorMessages.Type.BadRequest,
                ErrorMessages.Post.NotFound
            );
            return BadRequest(result);
        }
        if (post.Assignment == null) 
        {
            result.Error = PopulateError(400,
                ErrorMessages.Type.BadRequest,
                ErrorMessages.Post.NotFound
            );
            return BadRequest(result);
        }

        var finishedAssignment = await _unitOfWork.Assignments.Submit(post.Assignment.Id, finishAssignmentDto.FilePath);
        if (finishedAssignment) 
        {
            var closedPost = await _unitOfWork.Posts.Close(postId);
            if (!closedPost)
            {
                result.Error = PopulateError(400,
                    ErrorMessages.Type.BadRequest,
                    ErrorMessages.Post.UnableToProcess
                );
            }

            await _unitOfWork.CompleteAsync();
            result.Content = "Finished post";
            return Ok(result); 
        }

        result.Error = PopulateError(400,
            ErrorMessages.Type.BadRequest,
            ErrorMessages.Assignment.UnableToProcess
        );
        return BadRequest(result);
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Route("CreatePost")]
    public async Task<IActionResult> CreatePost([FromBody] PostRequestDto postRequestDto)
    {
        var result = new Result<PostRequestDto>();
        if (ModelState.IsValid)
        {
            var loggedUser = await _userManger.GetUserAsync(HttpContext.User);
            if (loggedUser == null) 
            {
                result.Error = PopulateError(400,
                    ErrorMessages.Type.BadRequest,
                    ErrorMessages.Users.NotFound);
                return BadRequest(result);
            }

            Guid identityId = new Guid(loggedUser.Id);
            var user = await _unitOfWork.Users.GetByIdentityId(identityId);

            if (user == null)
            {
                result.Error = PopulateError(400,
                    ErrorMessages.Type.BadRequest,
                    ErrorMessages.Users.NotFound);
                return BadRequest(result);
            }

            var mappedPost =  _mapper.Map<Post>(postRequestDto);
            mappedPost.AuthorId = user.Id;

            var addedPost = await _unitOfWork.Posts.Add(mappedPost);
            if (!addedPost)
            {
                result.Error = PopulateError(400, 
                    ErrorMessages.Type.BadRequest, 
                    ErrorMessages.Generic.InvalidPayload);
                return BadRequest(result);
            }

            foreach (var categoryId in postRequestDto.Categories)
            {
                PostCategory postCategory = new PostCategory() 
                { 
                    PostId = mappedPost.Id, 
                    CategoryId = categoryId 
                };
                await _unitOfWork.PostCategory.Add(postCategory);
            }

            await _unitOfWork.CompleteAsync();
    
            result.Content = postRequestDto;
            var postFDB = await _unitOfWork.Posts.GetById(mappedPost.Id);
            return Ok(postFDB);
        }
        else 
        {
            result.Error = PopulateError(400, 
                ErrorMessages.Type.BadRequest, 
                ErrorMessages.Generic.InvalidPayload);
            return BadRequest(result);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("UpdatePost")]
    public async Task<IActionResult> UpdatePost(Guid id, PostRequestDto postRequestDto)
    {
        var result = new Result<PostRequestDto>();
        if (ModelState.IsValid)
        {
            var postStatus = await _unitOfWork.Posts.GetStatus(id);

            if (postStatus != PostStatus.Open)
            {
                result.Error = PopulateError(400,
                    ErrorMessages.Type.BadRequest,
                    ErrorMessages.Post.InvalidAction);
                return BadRequest(result);
            }

            var mappedPost = _mapper.Map<Post>(postRequestDto);
            var isUpdated = await _unitOfWork.Posts.Upsert(id, mappedPost);
            if (!isUpdated)
            {
                result.Error = PopulateError(400, 
                    ErrorMessages.Type.BadRequest, 
                    ErrorMessages.Generic.InvalidPayload);
                return BadRequest(result);
            }
            
            var postCategories = await _unitOfWork.PostCategory.GetPostCategoriesByPostId(id);

            foreach (var categoryId in postRequestDto.Categories)
            {
                var postCategory = new PostCategory(){ PostId = id, CategoryId = categoryId };
                
                if (!postCategories.Contains(postCategory))
                {
                    await _unitOfWork.PostCategory.Add(postCategory);
                }
            }

            foreach (var postCategory in postCategories)
            {
                if (!postRequestDto.Categories.Contains(postCategory.CategoryId))
                {
                    await _unitOfWork.PostCategory.Delete(postCategory);
                }
            }

            await _unitOfWork.CompleteAsync();
            result.Content = postRequestDto;
            return Ok(result);
        }
        else 
        {
            result.Error = PopulateError(400, 
                ErrorMessages.Type.BadRequest, 
                ErrorMessages.Generic.InvalidPayload);
            return BadRequest(result);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("ClosePost")]
    public async Task<IActionResult> ClosePost(Guid id)
    {
        var result = new Result<bool>();
        var closedPost = await _unitOfWork.Posts.Close(id);
        
        if (closedPost)
        {
            await _unitOfWork.CompleteAsync();
            result.Content = true;
            return Ok(result); 
        }
        else
        {
            result.Content = false;
            result.Error = PopulateError(404,
                ErrorMessages.Type.NotFound,
                ErrorMessages.Post.NotFound);
            return NotFound(result);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("ProcessPost")]
    // Update the Post status from Open to Processing.
    public async Task<IActionResult> ProcessPost(Guid id, Guid freelancerId)
    {
        var result = new Result<string>();
        var postStatus = await _unitOfWork.Posts.GetStatus(id);

        if (postStatus == PostStatus.Closed)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Type.BadRequest,
                ErrorMessages.Post.InvalidAction);
            return BadRequest(result);
        }
        var post = await _unitOfWork.Posts.GetById(id);
        var processPost = await _unitOfWork.Posts.Process(id, freelancerId);
        
        if (processPost)
        {
            DateTime endDate = DateTime.UtcNow.AddDays(post.Duration);
            var assignment = new Assignment
            {
                EndDate = endDate,
                PostId = id
            };
            var createdAssigment = await _unitOfWork.Assignments.Add(assignment);
            if (!createdAssigment)
            {
                result.Error = PopulateError(400,
                    ErrorMessages.Type.BadRequest,
                    ErrorMessages.Assignment.UnableToProcess
                );
                return BadRequest(result);
            }
            await _unitOfWork.CompleteAsync();
            result.Content = ActionMessages.UpdateSuccess;
            return Ok(result); 
        }
        else
        {
            result.Error = PopulateError(404,
                ErrorMessages.Type.NotFound,
                ErrorMessages.Post.NotFound);
            return NotFound(result);
        }
    }

    [HttpDelete]
    [Route("Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = new Result<string>();
        var deleted = await _unitOfWork.Posts.Delete(id);
        if (deleted)
        {
            result.Content = ActionMessages.DeleteSuccess;
            return Ok(result);
        }
        else
        {
            result.Error = PopulateError(404,
                ErrorMessages.Type.NotFound,
                ErrorMessages.Post.NotFound);
            return BadRequest(result);
        }
    }

    [HttpGet]
    [Route("SearchPosts")]
    public async Task<IActionResult> SearchPosts(string search)
    {
        var result = new Result<List<Post>>();
        var posts = await _unitOfWork.Posts.SearchPosts(search);
        result.Content = posts;

        return Ok(result);
    }
}