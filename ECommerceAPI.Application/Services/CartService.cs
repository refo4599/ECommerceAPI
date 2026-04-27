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

    public async Task<SwitchBranchResponse> SwitchBranchAsync(int userId, SwitchBranchRequest req)
    {
        var oldCart = await uow.Carts
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (oldCart is null || oldCart.BranchId == req.NewBranchId)
        {
            var emptyCart = await uow.Carts
                .GetCartWithItemsAsync(userId, req.NewBranchId);
            return new SwitchBranchResponse(
                emptyCart is null
                    ? new CartDto(0, req.NewBranchId, "", [], 0)
                    : MapToDto(emptyCart),
                []);
        }

        var oldCartFull = await uow.Carts
            .GetCartWithItemsAsync(userId, oldCart.BranchId);

        var removedItems = new List<string>();

        var newCart = await uow.Carts
            .GetCartWithItemsAsync(userId, req.NewBranchId);

        if (newCart is null)
        {
            newCart = new Cart { UserId = userId, BranchId = req.NewBranchId };
            await uow.Carts.AddAsync(newCart);
            await uow.SaveChangesAsync();
            newCart = await uow.Carts
                .GetCartWithItemsAsync(userId, req.NewBranchId);
        }

        foreach (var item in oldCartFull!.CartItems)
        {
            var bp = await uow.BranchProducts
                .GetByBranchAndProductAsync(req.NewBranchId, item.ProductId);

            if (bp is null || !bp.IsAvailable || bp.Stock < item.Quantity)
            {
                removedItems.Add(item.Product?.Name ?? $"Product {item.ProductId}");
                continue;
            }

            var existing = newCart!.CartItems
                .FirstOrDefault(ci => ci.ProductId == item.ProductId);

            if (existing is not null)
                existing.Quantity += item.Quantity;
            else
                await uow.CartItems.AddAsync(new CartItem
                {
                    CartId = newCart!.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
        }

        foreach (var item in oldCartFull.CartItems)
            uow.CartItems.Remove(item);

        uow.Carts.Remove(oldCartFull);
        await uow.SaveChangesAsync();

        var updatedCart = await uow.Carts
            .GetCartWithItemsAsync(userId, req.NewBranchId);

        return new SwitchBranchResponse(MapToDto(updatedCart!), removedItems);
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