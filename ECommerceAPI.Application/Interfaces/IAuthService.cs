using System;
using System.Collections.Generic;
using System.Text;

using ECommerceAPI.Application.DTOs.Auth;

namespace ECommerceAPI.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}