using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Application.DTOs.Cart;

public record CartDto(
    int Id,
    int BranchId,
    string BranchName,
    List<CartItemDto> Items,
    decimal Total);

public record CartItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string? ImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal);

public record AddToCartRequest(
    int BranchId,
    int ProductId,
    int Quantity);

public record UpdateCartItemRequest(int Quantity)
{


};

public record SwitchBranchRequest(int NewBranchId);

public record SwitchBranchResponse(
    CartDto Cart,
    List<string> RemovedItems);