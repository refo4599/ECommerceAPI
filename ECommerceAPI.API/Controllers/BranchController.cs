using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Application.DTOs.Branch;
using ECommerceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchesController(IBranchService branchService) : ControllerBase
{
   
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var branches = await branchService.GetAllAsync();
        return Ok(new { success = true, data = branches });
    }

    [HttpGet("default")]
    public async Task<IActionResult> GetDefault()
    {
        var branch = await branchService.GetDefaultAsync();
        if (branch is null)
            return NotFound(new { success = false, message = "مفيش فرع رئيسي" });
        return Ok(new { success = true, data = branch });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var branch = await branchService.GetByIdAsync(id);
        if (branch is null)
            return NotFound(new { success = false, message = "الفرع مش موجود" });
        return Ok(new { success = true, data = branch });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateBranchRequest req)
    {
        var branch = await branchService.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = branch.Id },
            new { success = true, data = branch });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateBranchRequest req)
    {
        var branch = await branchService.UpdateAsync(id, req);
        if (branch is null)
            return NotFound(new { success = false, message = "الفرع مش موجود" });
        return Ok(new { success = true, data = branch });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await branchService.DeleteAsync(id);
        if (!result)
            return NotFound(new { success = false, message = "الفرع مش موجود" });
        return Ok(new { success = true, message = "تم حذف الفرع" });
    }
}