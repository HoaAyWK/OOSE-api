using OpenRequest.Core.Interfaces.UoW;
using OpenRequest.Core.Interfaces.Services;
using OpenRequest.Core.Dtos.Common;
using OpenRequest.Core.Dtos.Posts;
using AutoMapper;
using OpenRequest.Core.Entities;
using OpenRequest.Core.Dtos.Errors;

namespace OpenRequest.Core.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public PostService(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<PostResponse>>> GetPostsAsync(int status = 1)
    {
        var result = new Result<IEnumerable<PostResponse>>();
        var posts = await _unitOfWork.Posts.GetPosts(status);
        var mappedPosts = _mapper.Map<IEnumerable<Post>, IEnumerable<PostResponse>>(posts);
        
        result.Content = mappedPosts;
        return result;
    }

    public async Task<Result<IEnumerable<PostResponse>>> GetPostsByAuthorAsync(Guid id, int status = 1)
    {
        var result = new Result<IEnumerable<PostResponse>>();
        var posts = await _unitOfWork.Posts.GetPostsByAuthor(id, status);
        var mappedPosts = _mapper.Map<IEnumerable<Post>, IEnumerable<PostResponse>>(posts);
        result.Content = mappedPosts;
        return result;
    }

    public async Task<Result<IEnumerable<PostResponse>>> GetOwnerPostsAsync(string token, int status = 1)
    {
        var result = new Result<IEnumerable<PostResponse>>();
        var userId = _tokenService.GetUserId(token);
        var currentUser = await _unitOfWork.Users.GetById(new Guid(userId));

        if (currentUser == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Cannot find user."
            };
            return result;
        }

        var posts = await _unitOfWork.Posts.GetPostsByAuthor(currentUser.Id, status);
        var mappedPost = _mapper.Map<IEnumerable<Post>, IEnumerable<PostResponse>>(posts);
        result.Content = mappedPost;

        return result;
    }

    public async Task<Result<PostDetailResponse>> GetPostByIdAsync(Guid id)
    {
        var result = new Result<PostDetailResponse>();
        var post = await _unitOfWork.Posts.GetById(id);

        if (post == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Cannot does not exist.."
            };

            return result;
        }   

        var mappedPost = _mapper.Map<Post, PostDetailResponse>(post);
        result.Content = mappedPost;

        return result;
    }

    public async Task<Result<PostFullDetailResponse>> GetProcessingOrClosedPostAsync(string token, Guid id)
    {
        var result = new Result<PostFullDetailResponse>();
        var userId = _tokenService.GetUserId(token);
        var currentUser = await _unitOfWork.Users.GetById(new Guid(userId));
        
        if (currentUser == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Cannot find your account."
            };
            
            return result;
        }

        var post = await _unitOfWork.Posts.GetProcessingOrClosedPostAsync(id);
        if (post == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Post does not exist.."
            };

            return result;
        }   

        if (currentUser.Id != post.AuthorId && currentUser.Id != post.FreelancerId)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Permission denied."
            };

            return result;
        }

        var mappedPost = _mapper.Map<Post, PostFullDetailResponse>(post);
        result.Content = mappedPost;

        return result;
    }

    public async Task<Result<PostResponse>> CreateAsync(string token, PostRequest request)
    {
        var result = new Result<PostResponse>();
        var userId = _tokenService.GetUserId(token);
        var currentUser = await _unitOfWork.Users.GetById(new Guid(userId));

        if (currentUser == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "You are not signed in."
            };

            return result;
        }

        var mappedPost = _mapper.Map<Post>(request);
        mappedPost.AuthorId = currentUser.Id;
        
        var created = await _unitOfWork.Posts.Create(mappedPost);
        if (created != null)
        {
            foreach (var category in request.Categories)
            {
                PostCategory postCategory = new PostCategory
                { 
                    PostId = mappedPost.Id, 
                    CategoryId = category 
                };
                await _unitOfWork.PostCategory.Add(postCategory);
            }
            await _unitOfWork.CompleteAsync();
            
            var postAdded =_mapper.Map<PostResponse>(created);
            result.Content = postAdded;

            return result;
        }

        result.Error = new Error
        {
            Code = 500,
            Message = "Cannot create post."
        };
        return result; 
    }

    public async Task<Result<string>> UpdateAsync(string token, Guid id, PostRequest request)
    {
        var result = new Result<string>();
        var userId = _tokenService.GetUserId(token);
        var currentUser = await _unitOfWork.Users.GetById(new Guid(userId));

        if (currentUser == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "You are not signed in."
            };
            
            return result;
        }

        var existingPost = await _unitOfWork.Posts.GetById(id);

        if (existingPost == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Post does not exist."
            };

            return result;
        }

        if (currentUser.Id != existingPost.AuthorId)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Cannot delete other user's post."
            };

            return result;
        }

        if (existingPost.Status != 1) 
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Currently this post is in processing or closed. Cannot update."
            };
            
            return result;
        }

        var mappedPost = _mapper.Map<Post>(request);
        var updated = await _unitOfWork.Posts.Update(id, mappedPost);
        if (updated)
        {
            var postCategories = await _unitOfWork.PostCategory.GetByPostId(id);
            foreach (var categoryId in request.Categories)
            {
                var postCategory = new PostCategory() { PostId = id, CategoryId = categoryId };
                foreach (var postCate in postCategories)
                {
                    if (postCate.CategoryId != categoryId)
                    {
                        await _unitOfWork.PostCategory.Add(postCategory);
                    }
                }
            }

            foreach (var postCategory in postCategories)
            {
                if (!request.Categories.Contains(postCategory.CategoryId))
                {
                    await _unitOfWork.PostCategory.Delete(postCategory);
                }
            }

            await _unitOfWork.CompleteAsync();
            result.Content = "Updated post";
            return result;
        }

        result.Error = new Error
        {
            Code = 500,
            Message = "Cannot update post."
        };
        
        return result;
    }

    public async Task<Result<string>> DeleteAsync(string token, Guid id)
    {
        var result = new Result<string>();
        var userId = _tokenService.GetUserId(token);
        var currentUser = await _unitOfWork.Users.GetById(new Guid(userId));

        if (currentUser == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "You are not signed in."
            };
            
            return result;
        }

        var existingPost = await _unitOfWork.Posts.GetById(id);
        if (existingPost == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Post does not exist."
            };

            return result;
        }

        if (currentUser.Id != existingPost.AuthorId)
        {
            var roles = _tokenService.GetRolesFromToken(token);

            if (roles == null)
            {
                result.Error = new Error
                {
                    Code = 400,
                    Message = "Permission denied."
                };
                
                return result;
            }

            if (!roles.Contains("Admin"))
            {
                result.Error = new Error
                {
                    Code = 400,
                    Message = "Cannot delete other user's post."
                };
                
                return result;
            }
        }

        if (existingPost.Status != 1) 
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Currently this post is in processing or closed. Cannot delete."
            };
            
            return result;
        }

        var deleted = await _unitOfWork.Posts.Delete(id);
        if (deleted)
        {
            await _unitOfWork.CompleteAsync();
            result.Content = "Deleted post.";
            return result;
        }

        result.Error = new Error
        {
            Code = 500,
            Message = "Cannot delete post."
        };

        return result;
    }

    public async Task<Result<string>> SelectAsync(string token, Guid id) 
    {
        var result = new Result<string>();
        var existingPost = await _unitOfWork.Posts.GetById(id);
        if (existingPost == null) 
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Post does not exist."
            };

            return result;
        }

        var roles = _tokenService.GetRolesFromToken(token);

        if (roles == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Permission denied."
            };
            
            return result;
        }

        if (!roles.Contains("Freelancer"))
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Only freelancer can send request to the post."
            };
            
            return result;
        }

        var userId = _tokenService.GetUserId(token);
        var currentUser = await _unitOfWork.Users.GetById(new Guid(userId));

        if (currentUser == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Cannot identity your account."
            };

            return result;
        }

        var freelancerRequest = new FreelancerRequest() { PostId = id, FreelancerId = currentUser.Id };
        var added = await _unitOfWork.FreelancerRequests.Add(freelancerRequest);

        if (added)
        {
            await _unitOfWork.CompleteAsync();
            result.Content = "Selected post.";
            return result;
        }

        result.Error = new Error
        {
            Code = 500,
            Message = "Cannot select post."
        };
        
        return result;
    }

    public async Task<Result<string>> UnselectAsync(string token, Guid id)
    {
        var result = new Result<string>();
        var existingPost = await _unitOfWork.Posts.GetById(id);
        if (existingPost == null)
        {
            result.Error = new Error 
            {
                Code = 400,
                Message = "Cannot find post."
            };

            return result;
        }

        var userId = _tokenService.GetUserId(token);
        var currentUser = await _unitOfWork.Users.GetById(new Guid(userId));

        if (currentUser == null)
        {
            result.Error = new Error
            {
                Code = 500,
                Message = "Cannot find your account."
            };

            return result;
        }

        var roles = _tokenService.GetRolesFromToken(token);

        if (roles == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Permission denied."
            };
            
            return result;
        }

        if (!roles.Contains("Freelancer"))
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Only freelancer can unselect post."
            };

            return result;
        }

        var deleted = await _unitOfWork.FreelancerRequests.Delete(id, currentUser.Id);

        if (deleted)
        {
            await _unitOfWork.CompleteAsync();
            result.Content = "Unselected post.";
            return result;
        }

        result.Error = new Error
        {
            Code = 500,
            Message = "Cannot unselect post."
        };

        return result;
    }

    public async Task<Result<string>> SelectFreelancerAsync(string token, Guid postId, Guid freelancerId)
    {
        var result = new Result<string>();
        var existingPost = await _unitOfWork.Posts.GetProcessingOrClosedPostAsync(postId);
        if (existingPost == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Post does not exist."
            };
            
            return result;
        }

        if (existingPost.Status != 1)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "This post already selected freelancer."
            };

            return result;
        }

        var userId = _tokenService.GetUserId(token);
        var currentUser = await _unitOfWork.Users.GetById(new Guid(userId));

        if (currentUser == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Cannot find your account."
            };

            return result;
        }

        var freelancer = await _unitOfWork.Users.GetById(freelancerId);
        if (freelancer == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Freelancer does not exist."
            };

            return result;
        }

        if (currentUser.Id != existingPost.AuthorId)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Cannot process other user's post."
            };

            return result;
        }

        if (existingPost.FreelancerRequests == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Post does not have any freelancer to select."
            };

            return result;
        }

        foreach (var freelancerRequest in existingPost.FreelancerRequests)
        {
            if (freelancerRequest.PostId == existingPost.Id && freelancerRequest.FreelancerId == freelancerId)
            {
                var processed = await _unitOfWork.Posts.Process(postId, freelancerId);
                if (processed)
                {
                    var assignment = new Assignment
                    {
                        PostId = postId,
                        EndDate = DateTime.UtcNow.AddDays(existingPost.Duration)
                    };

                    var added = await _unitOfWork.Assignments.Add(assignment);
                    if (added)
                    {
                        await _unitOfWork.CompleteAsync();
                        result.Content = "Selected freelancer";
                        return result;
                    }
                    
                    result.Error = new Error
                    {
                        Code = 500,
                        Message = "Cannot select freelancer."
                    };

                    return result;
                }

                result.Error = new Error
                {
                    Code = 500,
                    Message = "Cannot select freelancer."
                };

                return result;
            }
        }

        result.Error = new Error
        {
            Code = 400,
            Message = "Cannot find freelancer in post's requests."
        };

        return result;
    }

    public async Task<Result<string>> SubmitAssignmentAsync(string token, Guid postId, string filePath)
    {
        var result = new Result<string>();
        var userId = _tokenService.GetUserId(token);
        var currentUser = await _unitOfWork.Users.GetById(new Guid(token));

        if (currentUser == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Your account does not exist."
            };

            return result;
        }

        var existingPost = await _unitOfWork.Posts.GetProcessingOrClosedPostAsync(postId);
        if (existingPost == null)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Post does not exist."
            };

            return result;
        }

        if (existingPost.Assignment == null)
        {
            result.Error = new Error
            {
                Code = 500,
                Message = "Something went wrong."
            };

            return result;

        }

        if (existingPost.FreelancerId != currentUser.Id)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "You are not working with this post."
            };

            return result;
        }

        var submitted = await _unitOfWork.Posts.Submit(postId);
        if (submitted)
        {
            var updated = await _unitOfWork.Assignments.Update(existingPost.Assignment.Id, filePath);
            if (updated)
            {
                await _unitOfWork.CompleteAsync();
                result.Content = "Submitted assignment";
                return result;
            }

            result.Error = new Error
            {
                Code = 500,
                Message = "Cannot submit assignment."
            };

            return result;
        }

        result.Error = new Error
        {
            Code = 500,
            Message = "Cannot submit assignment."
        };

        return result;
    }

    public async Task<Result<IEnumerable<PostFullDetailResponse>>> GetPostsAlmostEnd()
    {
        var result = new Result<IEnumerable<PostFullDetailResponse>>();
        var posts =  await _unitOfWork.Posts.GetProcessingPostAlmostEnd();
        var mappedPosts = _mapper.Map<IEnumerable<Post>, IEnumerable<PostFullDetailResponse>>(posts);
        result.Content = mappedPosts;
        return result;
    }
}