using ECommerceAPI.Application.DTOs.Order;
using ECommerceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // الكستمر يعمل أوردر من الكارت
    [HttpPost]
    public async Task<IActionResult> CreateFromCart(CreateOrderRequest req)
    {
        try
        {
            var order = await orderService.CreateFromCartAsync(UserId, req);
            return Ok(new { success = true, data = order });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    // الكستمر يشوف أوردراته
    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var orders = await orderService.GetMyOrdersAsync(UserId);
        return Ok(new { success = true, data = orders });
    }

    // Admin يشوف كل الأوردرات
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await orderService.GetAllPagedAsync(page, pageSize);
        return Ok(new { success = true, data = result });
    }

    // Admin يغير status الأوردر
    [HttpPut("{orderId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int orderId, UpdateOrderStatusRequest req)
    {
        try
        {
            var order = await orderService.UpdateStatusAsync(orderId, req);
            if (order is null)
                return NotFound(new { success = false, message = "الأوردر مش موجود" });
            return Ok(new { success = true, data = order });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}