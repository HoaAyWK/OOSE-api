using AutoMapper;
using OpenRequest.Core.Dtos.Categories;
using OpenRequest.Core.Dtos.Common;
using OpenRequest.Core.Dtos.Errors;
using OpenRequest.Core.Entities;
using OpenRequest.Core.Interfaces.UoW;
using OpenRequest.Core.Interfaces.Services;

namespace OpenRequest.Core.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<Category>>> GetAllAsync()
    {
        var result = new Result<IEnumerable<Category>>();
        result.Content = await _unitOfWork.Categories.All();
        return result;
    }

    public async Task<Result<Category>> GetByIdAsync(Guid id)
    {
        var result = new Result<Category>();
        result.Content = await _unitOfWork.Categories.GetById(id);
        return result;
    }

    public async Task<Result<string>> AddAsync(CategoryRequest request)
    {
        var result = new Result<string>();
        var mappedCategory = _mapper.Map<Category>(request);
        
        var added = await _unitOfWork.Categories.Add(mappedCategory);
        if (added)
        {
            await _unitOfWork.CompleteAsync();
            result.Content = "Added category.";
            return result;
        }

        result.Error = new Error
        {
            Code = 500,
            Message = "Cannot add category."
        };

        return result;
    }

    public async Task<Result<string>> UpdateAsync(Guid id, CategoryRequest request)
    {
        var result = new Result<string>();
        var existingCategory = await _unitOfWork.Categories.GetById(id);
        if (existingCategory == null) 
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "Category does not exist."
            };
        }
        
        var mappedCategory = _mapper.Map<Category>(request);
        if (String.Equals(existingCategory.Name, request.Name))
        {
            var updated = await _unitOfWork.Categories.Upsert(id, mappedCategory);
            if (updated)
            {
                await _unitOfWork.CompleteAsync();
                result.Content = "Update successful.";
                return result;
            }
            result.Error = new Error
            {
                Code = 500,
                Message = "Cannot update category."
            };
            return result;
        }
        else 
        {
            var updated = await _unitOfWork.Categories.Upsert(id, mappedCategory);

            if (updated)
            {
                await _unitOfWork.CompleteAsync();
                result.Content = "Update successful.";
                return result;
            }

            result.Error = new Error
            {
                Code = 500,
                Message = "Cannot update category."
            };

            return result;
        }  
    }

    public async Task<Result<string>> DeleteAsync(Guid id)
    {
        var result = new Result<string>();
        var existingPosts = _unitOfWork.PostCategory.ExistingAnyPost(id);

        if (existingPosts)
        {
            result.Error = new Error
            {
                Code = 400,
                Message = "To delete this category, please delete all category's posts first."
            };
            return result;
        }

        var deleted = await _unitOfWork.Categories.Delete(id);
        if (deleted)
        {
            await _unitOfWork.CompleteAsync();
            result.Content = "Deleted category.";
            return result;
        }
        
        result.Error = new Error
        {
            Code = 500,
            Message = "Cannot delete category."
        };
        return result;
    }
}