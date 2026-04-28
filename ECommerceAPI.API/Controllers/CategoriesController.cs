using ECommerceAPI.Application.DTOs.Category;
using ECommerceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await categoryService.GetAllAsync();
        return Ok(new { success = true, data = categories });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await categoryService.GetByIdAsync(id);
        if (category is null)
            return NotFound(new { success = false, message = "الكاتيجوري مش موجودة" });
        return Ok(new { success = true, data = category });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest req)
    {
        var category = await categoryService.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = category.Id },
            new { success = true, data = category });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest req)
    {
        var category = await categoryService.UpdateAsync(id, req);
        if (category is null)
            return NotFound(new { success = false, message = "الكاتيجوري مش موجودة" });
        return Ok(new { success = true, data = category });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await categoryService.DeleteAsync(id);
        if (!result)
            return NotFound(new { success = false, message = "الكاتيجوري مش موجودة" });
        return Ok(new { success = true, message = "تم حذف الكاتيجوري" });
    }
}