using ECommerceAPI.Application.Common;
using ECommerceAPI.Application.DTOs.Auth;
using ECommerceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(
        RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "تم التسجيل بنجاح"));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
        LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "أهلاً بيك"));
    }
}