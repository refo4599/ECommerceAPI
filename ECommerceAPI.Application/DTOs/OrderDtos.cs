using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Application.DTOs.Order;

public record OrderDto(
    int Id,
    int BranchId,
    string BranchName,
    decimal TotalAmount,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    List<OrderItemDto> Items);

public record OrderItemDto(
    int ProductId,
    string ProductName,
    string? ImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);

public record CreateOrderRequest(
    int BranchId,
    string? Notes = null);

public record UpdateOrderStatusRequest(string Status);