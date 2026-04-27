using ECommerceAPI.Application.DTOs.Product;
using ECommerceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await productService.GetAllAsync();
        return Ok(new { success = true, data = products });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await productService.GetByIdAsync(id);
        if (product is null)
            return NotFound(new { success = false, message = "المنتج مش موجود" });
        return Ok(new { success = true, data = product });
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        var products = await productService.GetByBranchAsync(branchId);
        return Ok(new { success = true, data = products });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProductRequest req)
    {
        var product = await productService.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = product.Id },
            new { success = true, data = product });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateProductRequest req)
    {
        var product = await productService.UpdateAsync(id, req);
        if (product is null)
            return NotFound(new { success = false, message = "المنتج مش موجود" });
        return Ok(new { success = true, data = product });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await productService.DeleteAsync(id);
        if (!result)
            return NotFound(new { success = false, message = "المنتج مش موجود" });
        return Ok(new { success = true, message = "تم حذف المنتج" });
    }

    [HttpPost("branch/{branchId}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignToBranch(int branchId, AssignProductRequest req)
    {
        await productService.AssignToBranchAsync(branchId, req);
        return Ok(new { success = true, message = "تم تعيين المنتج للفرع" });
    }

    [HttpPut("branch/{branchId}/stock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStock(int branchId, UpdateStockRequest req)
    {
        await productService.UpdateStockAsync(branchId, req);
        return Ok(new { success = true, message = "تم تحديث الستوك" });
    }
}