using ECommerceAPI.Application.DTOs.Cart;
using ECommerceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController(ICartService cartService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{branchId}")]
    public async Task<IActionResult> GetCart(int branchId)
    {
        var cart = await cartService.GetCartAsync(UserId, branchId);
        if (cart is null)
            return Ok(new { success = true, data = (object?)null });
        return Ok(new { success = true, data = cart });
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddToCartRequest req)
    {
        try
        {
            var cart = await cartService.AddItemAsync(UserId, req);
            return Ok(new { success = true, data = cart });
        }
        catch (InvalidOperationException ex) when (ex.Message == "DIFFERENT_BRANCH")
        {
            return BadRequest(new { success = false, message = "عندك منتجات من فرع تاني في الكارت" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateItem(int cartItemId, UpdateCartItemRequest req)
    {
        var cart = await cartService.UpdateItemAsync(UserId, cartItemId, req);
        if (cart is null)
            return NotFound(new { success = false, message = "العنصر مش موجود" });
        return Ok(new { success = true, data = cart });
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        var result = await cartService.RemoveItemAsync(UserId, cartItemId);
        if (!result)
            return NotFound(new { success = false, message = "العنصر مش موجود" });
        return Ok(new { success = true, message = "تم حذف العنصر" });
    }

    [HttpDelete("{branchId}/clear")]
    public async Task<IActionResult> ClearCart(int branchId)
    {
        var result = await cartService.ClearCartAsync(UserId, branchId);
        if (!result)
            return NotFound(new { success = false, message = "الكارت مش موجود" });
        return Ok(new { success = true, message = "تم مسح الكارت" });
    }
}