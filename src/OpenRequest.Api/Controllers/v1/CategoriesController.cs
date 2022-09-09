using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenRequest.Core.Dtos.Categories;
using OpenRequest.Core.Interfaces.Services;
namespace OpenRequest.Api.Controllers.v1;

public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService) : base()
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _categoryService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetCategoryAsync(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        if (result.Content == null)
        {
            return BadRequest(result);
        }   
        return Ok(result);
    }

    [HttpPost]
    [Route("add")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddAsync([FromBody] CategoryRequest request)
    {
        var result = await _categoryService.AddAsync(request);
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPut]
    [Route("update")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CategoryRequest request)
    {
        var result = await _categoryService.UpdateAsync(id, request);
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete]
    [Route("delete")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}