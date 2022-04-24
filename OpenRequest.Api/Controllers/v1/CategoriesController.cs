using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.Authentication.Models.DTO.Incoming;
using OpenRequest.DataService.IConfiguration;
using OpenRequest.Entities.DbSets;
using OpenRequest.Entities.DTO.Generic;

namespace OpenRequest.Api.Controllers.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CategoriesController : BaseController
{
    public CategoriesController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper)
        : base(unitOfWork, userManager, mapper)
    {

    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _unitOfWork.Categories.All();
        var result = new PagedResult<Category>();
        result.Content = categories.ToList();
        result.ResultCount = categories.Count();

        return Ok(result);
    }

    [HttpGet]
    [Route("GetCategory", Name = "GetCategory")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var category = await _unitOfWork.Categories.GetById(id);
        var result = new Result<Category>();

        if (category == null)
        {
            result.Error = PopulateError(404, "Not Found", "Not Found");
            return NotFound(result);
        }

        result.Content = category;
        return Ok(result);
    }

    [HttpPost]
    [Route("AddCategory")]
    public async Task<IActionResult> AddCategory([FromBody] CategoryDto categoryDto)
    {     
        var result = new Result<CategoryDto>();
        if (ModelState.IsValid)
        {
            var mappedCategory = _mapper.Map<Category>(categoryDto);
            var isAdded = await _unitOfWork.Categories.Add(mappedCategory);
            if (isAdded)
            {
                await _unitOfWork.CompleteAsync();
                result.Content = categoryDto;
                return CreatedAtRoute("GetCategory", new { id = mappedCategory.Id });
            }

            result.Error = PopulateError(400, "Bad Request", "Invalid payload.");
            return BadRequest(result);
        }
        else 
        {
            result.Error = PopulateError(400, "Bad Request", "Invalid payload.");
            return BadRequest(result);
        }
    }

    [HttpPut]
    [Route("UpdateCategory")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryDto categoryDto)
    {
        var result = new Result<CategoryDto>();
        if (ModelState.IsValid)
        {
            var mappedCategory = _mapper.Map<Category>(categoryDto);
            var isUpdated = await _unitOfWork.Categories.Upsert(mappedCategory);

            if (isUpdated)
            {
                await _unitOfWork.CompleteAsync();
                result.Content = categoryDto;
                return CreatedAtAction("GetCategory", new { Id = id });
            }
            
            result.Error = PopulateError(400, "Bad Request", "Invalid payload.");
            return BadRequest(result);
        }
        else
        {
            result.Error = PopulateError(400, "Bad Request", "Invalid payload.");
            return BadRequest(result);
        }
    }

    [HttpDelete]
    [Route("DeleteCategory")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = new Result<string>();

        var isDeleted = await _unitOfWork.Categories.Delete(id);

        if (!isDeleted)
        {
            result.Error = PopulateError(400, "Bad Request", "Invalid payload.");
            return BadRequest(result);
        }

        result.Content = "Deleted successful.";
        return Ok(result);
    }
}