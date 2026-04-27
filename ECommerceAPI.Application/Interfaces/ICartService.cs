using System;
using System.Collections.Generic;
using System.Text;

using ECommerceAPI.Application.DTOs.Cart;

namespace ECommerceAPI.Application.Interfaces;

public interface ICartService
{
    Task<CartDto?> GetCartAsync(int userId, int branchId);
    Task<CartDto> AddItemAsync(int userId, AddToCartRequest request);
    Task<CartDto?> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemRequest request);
    Task<bool> RemoveItemAsync(int userId, int cartItemId);
    Task<bool> ClearCartAsync(int userId, int branchId);
}