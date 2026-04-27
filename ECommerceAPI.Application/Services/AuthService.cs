using ECommerceAPI.Application.DTOs.Auth;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;
using ECommerceAPI.Domain.Enums;

namespace ECommerceAPI.Application.Services;

public class AuthService(IUnitOfWork uow, IJwtService jwt) : IAuthService
{
    private const string AdminEmail = "admin@test.com";

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        if (await uow.Users.AnyAsync(u => u.Email == req.Email))
            throw new InvalidOperationException("البريد الإلكتروني مستخدم بالفعل");

        var user = new User
        {
            Name = req.Name,
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = req.Email == AdminEmail ? UserRole.Admin : UserRole.Customer
        };

        await uow.Users.AddAsync(user);
        await uow.SaveChangesAsync();

        return new AuthResponse(
            user.Id, user.Name, user.Email,
            user.Role.ToString(), jwt.GenerateToken(user));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req)
    {
        var user = await uow.Users.GetByEmailAsync(req.Email)
            ?? throw new UnauthorizedAccessException("بيانات الدخول غلط");

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("بيانات الدخول غلط");

        return new AuthResponse(
            user.Id, user.Name, user.Email,
            user.Role.ToString(), jwt.GenerateToken(user));
    }
}