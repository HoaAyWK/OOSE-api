using OpenRequest.Core.Dtos.Categories;
using OpenRequest.Core.Dtos.Common;
using OpenRequest.Core.Entities;

namespace OpenRequest.Core.Interfaces.Services;

public interface ICategoryService
{
    Task<Result<IEnumerable<Category>>> GetAllAsync();
    Task<Result<Category>> GetByIdAsync(Guid id);
    Task<Result<string>> AddAsync(CategoryRequest request);
    Task<Result<string>> UpdateAsync(Guid id, CategoryRequest request);
    Task<Result<string>> DeleteAsync(Guid id);
}