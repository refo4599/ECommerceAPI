using System;
using System.Collections.Generic;
using System.Text;
using ECommerceAPI.Application.DTOs.Cart;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;

namespace ECommerceAPI.Application.Services;

public class CartService(IUnitOfWork uow) : ICartService
{
    public async Task<CartDto?> GetCartAsync(int userId, int branchId)
    {
        var cart = await uow.Carts.GetCartWithItemsAsync(userId, branchId);
        return cart is null ? null : MapToDto(cart);
    }

    public async Task<CartDto> AddItemAsync(int userId, AddToCartRequest req)
    {
        // ← الـ check المهم: كارت من فرع تاني؟
        var existingCart = await uow.Carts
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (existingCart != null && existingCart.BranchId != req.BranchId)
            throw new InvalidOperationException("DIFFERENT_BRANCH");

        var bp = await uow.BranchProducts
            .GetByBranchAndProductAsync(req.BranchId, req.ProductId)
            ?? throw new InvalidOperationException("المنتج غير متاح في هذا الفرع");

        if (!bp.IsAvailable)
            throw new InvalidOperationException("المنتج غير متاح حاليًا");

        if (bp.Stock < req.Quantity)
            throw new InvalidOperationException($"الكمية المتاحة {bp.Stock} فقط");

        var cart = await uow.Carts.GetCartWithItemsAsync(userId, req.BranchId);
        if (cart is null)
        {
            cart = new Cart { UserId = userId, BranchId = req.BranchId };
            await uow.Carts.AddAsync(cart);
            await uow.SaveChangesAsync();
            cart = await uow.Carts.GetCartWithItemsAsync(userId, req.BranchId);
        }

        var existing = cart!.CartItems.FirstOrDefault(ci => ci.ProductId == req.ProductId);
        if (existing is not null)
            existing.Quantity += req.Quantity;
        else
            await uow.CartItems.AddAsync(new CartItem
            {
                CartId = cart.Id,
                ProductId = req.ProductId,
                Quantity = req.Quantity
            });

        await uow.SaveChangesAsync();

        var updated = await uow.Carts.GetCartWithItemsAsync(userId, req.BranchId);
        return MapToDto(updated!);
    }

    public async Task<CartDto?> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemRequest req)
    {
        var item = await uow.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

        if (item is null) return null;

        var branchId = item.Cart.BranchId;

        if (req.Quantity <= 0)
            uow.CartItems.Remove(item);
        else
            item.Quantity = req.Quantity;

        await uow.SaveChangesAsync();

        var cart = await uow.Carts.GetCartWithItemsAsync(userId, branchId);
        return cart is null ? null : MapToDto(cart);
    }

    public async Task<bool> RemoveItemAsync(int userId, int cartItemId)
    {
        var item = await uow.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

        if (item is null) return false;

        uow.CartItems.Remove(item);
        await uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearCartAsync(int userId, int branchId)
    {
        var cart = await uow.Carts.GetCartWithItemsAsync(userId, branchId);
        if (cart is null) return false;

        foreach (var item in cart.CartItems)
            uow.CartItems.Remove(item);

        await uow.SaveChangesAsync();
        return true;
    }

    private static CartDto MapToDto(Cart c)
    {
        var items = c.CartItems.Select(ci =>
        {
            var price = ci.Product?.BasePrice ?? 0;
            return new CartItemDto(
                ci.Id, ci.ProductId,
                ci.Product?.Name ?? "",
                ci.Product?.ImageUrl,
                price, ci.Quantity,
                price * ci.Quantity);
        }).ToList();

        return new CartDto(
            c.Id, c.BranchId,
            c.Branch?.Name ?? "",
            items,
            items.Sum(i => i.Subtotal));
    }
}