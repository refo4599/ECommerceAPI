using ECommerceAPI.Application.Common;
using ECommerceAPI.Application.DTOs.Order;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;
using ECommerceAPI.Domain.Enums;

namespace ECommerceAPI.Application.Services;

public class OrderService(
    IUnitOfWork uow,
    IStockNotificationService stockNotification) : IOrderService
{
    public async Task<OrderDto> CreateFromCartAsync(int userId, CreateOrderRequest req)
    {
        var cart = await uow.Carts.GetCartWithItemsAsync(userId, req.BranchId)
            ?? throw new InvalidOperationException("الكرت مش موجود");

        if (!cart.CartItems.Any())
            throw new InvalidOperationException("الكرت فاضي");

        var orderItems = new List<OrderItem>();
        var stockUpdates = new List<(int productId, int newStock)>();

        foreach (var ci in cart.CartItems)
        {
            var bp = await uow.BranchProducts
                .GetByBranchAndProductAsync(req.BranchId, ci.ProductId)
                ?? throw new InvalidOperationException(
                    $"{ci.Product?.Name} غير متاح في الفرع");

            if (bp.Stock < ci.Quantity)
                throw new InvalidOperationException(
                    $"الكمية المتاحة من {ci.Product?.Name} هي {bp.Stock}");

            var unitPrice = bp.PriceOverride ?? ci.Product!.BasePrice;

            orderItems.Add(new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = unitPrice
            });

            bp.Stock -= ci.Quantity;
            bp.UpdatedAt = DateTime.UtcNow;
            uow.BranchProducts.Update(bp);

            stockUpdates.Add((ci.ProductId, bp.Stock));
        }

        var order = new Order
        {
            UserId = userId,
            BranchId = req.BranchId,
            TotalAmount = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
            Notes = req.Notes,
            OrderItems = orderItems
        };

        await uow.Orders.AddAsync(order);

        foreach (var item in cart.CartItems)
            uow.CartItems.Remove(item);

        await uow.SaveChangesAsync();

        foreach (var (productId, newStock) in stockUpdates)
        {
            await stockNotification.NotifyStockUpdatedAsync(
                req.BranchId, productId, newStock);
        }

        var created = await uow.Orders.GetByIdWithItemsAsync(order.Id);
        return MapToDto(created!);
    }

    public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId)
    {
        var orders = await uow.Orders.GetByUserAsync(userId);
        return orders.Select(MapToDto);
    }

    public async Task<PaginatedResponse<OrderDto>> GetAllPagedAsync(int page, int pageSize)
    {
        var (items, total) = await uow.Orders.GetAllPagedAsync(page, pageSize);
        return new PaginatedResponse<OrderDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<OrderDto?> UpdateStatusAsync(int orderId, UpdateOrderStatusRequest req)
    {
        var order = await uow.Orders.GetByIdAsync(orderId);
        if (order is null) return null;

        if (!Enum.TryParse<OrderStatus>(req.Status, true, out var status))
            throw new ArgumentException($"Status '{req.Status}' غير صحيح");

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        uow.Orders.Update(order);
        await uow.SaveChangesAsync();

        var updated = await uow.Orders.GetByIdWithItemsAsync(orderId);
        return MapToDto(updated!);
    }

    private static OrderDto MapToDto(Order o) => new(
        o.Id, o.BranchId,
        o.Branch?.Name ?? "",
        o.TotalAmount,
        o.Status.ToString(),
        o.Notes,
        o.CreatedAt,
        o.OrderItems.Select(oi => new OrderItemDto(
            oi.ProductId,
            oi.Product?.Name ?? "",
            oi.Product?.ImageUrl,
            oi.Quantity,
            oi.UnitPrice,
            oi.UnitPrice * oi.Quantity)).ToList());
}