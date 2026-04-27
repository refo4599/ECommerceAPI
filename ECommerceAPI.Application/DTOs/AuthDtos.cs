using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Application.DTOs.Auth;

public record RegisterRequest(
    string Name,
    string Email,
    string Password);

public record LoginRequest(
    string Email,
    string Password);

public record AuthResponse(
    int Id,
    string Name,
    string Email,
    string Role,
    string Token);