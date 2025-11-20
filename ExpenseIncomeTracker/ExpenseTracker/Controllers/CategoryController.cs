using Domain.Models;
using Domain.ServiceInterfaces;
using ExpenseTracker.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "user")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [Route("GetById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _categoryService.GetById(id);
        if (category == null)
            return NotFound("Category not found.");
        return Ok(category);
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAll();
        if (categories == null)
            return NotFound("No categories found in the database.");
        return Ok(categories);
    }

    [HttpGet]
    [Route("GetByType/{categoryTypes}")]
    public async Task<IActionResult> GetByType(CategoryTypes categoryTypes)
    {
        var categories = await _categoryService.GetCategoriesByType(categoryTypes);
        if (categories == null)
            return NotFound("No categories for the given type");
        return Ok(categories);
    }

    [HttpPut]
    [Route("UpdateById/{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody]CategoryDto categoryDto)
    {
        var updated = await _categoryService.Update(id, new Category
        {
            Id = id,
            Name = categoryDto.Name,
            CategoryTypes = categoryDto.CategoryTypes
        });
        if (updated == null)
            return NotFound($"Category with id {id} not found.");
        return Ok(updated);
    }

    [HttpDelete]
    [Route("Delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound($"Category with id {id} not found.");
        }
        return NoContent();
    }

    [HttpPost]
    [Route("AddCategory")]
    public async Task<IActionResult> Post([FromBody] CategoryDto categoryDto)
    {
        if (categoryDto == null)
            return BadRequest("Category data must be provided.");
        var created = await _categoryService.Create(new Category
        {
            Name = categoryDto.Name,
            CategoryTypes = categoryDto.CategoryTypes
        });
        if (created == null)
            return BadRequest("Category not created.");
        return CreatedAtAction(nameof(GetById) , new { Id = created.Id }, created);
    }
}