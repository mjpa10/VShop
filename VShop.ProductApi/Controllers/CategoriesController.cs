using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using VShop.ProductApi.DTOs;
using VShop.ProductApi.Roles;
using VShop.ProductApi.Services;

namespace VShop.ProductApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
    {
        var categoriesDto = await _categoryService.GetCategories();

        if (categoriesDto == null)
            return NotFound("Categories Not Found");
        
        return Ok(categoriesDto);
     }

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesProducts()
    {
        var categoriesDto = await _categoryService.GetCategoriesProducts();

        if (categoriesDto == null)
            return NotFound("Categories Not Found");

        return Ok(categoriesDto);
    }

    [HttpGet("id/{id:int}", Name = "GetCategoryId")]
    public async Task<ActionResult<CategoryDTO>> Get(int id)
    {
        var categoryDto = await _categoryService.GetCategoryById(id);

        if (categoryDto == null)
            return NotFound("Category Not Found");

        return Ok(categoryDto);
    }

    [HttpGet("name/{name}", Name = "GetCategoryName")]
    public async Task<ActionResult<CategoryDTO>> Get(string name)
    {
        var categoryDto = await _categoryService.GetCategoryByName(name);

        if (categoryDto == null)
            return NotFound("Category Not Found");

        return Ok(categoryDto);
    }
    [HttpPost]
    
    public async Task<ActionResult> Post([FromBody] CategoryDTO categoryDto)
    {
        if (categoryDto == null)
            return BadRequest("Invalid Data");

        await _categoryService.AddCategory(categoryDto);

        return new CreatedAtRouteResult(
            "GetCategoryId", 
            new { id = categoryDto.CategoryId }, 
            categoryDto);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromBody] CategoryDTO categoryDto)
    {

        if (categoryDto == null)
            return BadRequest();

        if (id != categoryDto.CategoryId)
            return BadRequest();

        await _categoryService.UpdateCategory(categoryDto);

        return Ok(categoryDto);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> Delete(int id)
    {

        var categoryDto = await _categoryService.GetCategoryById(id);

        if (categoryDto == null)
            return NotFound("Category Not Found");

        await _categoryService.DeleteCategory(id);

        return NoContent();
    }
}
