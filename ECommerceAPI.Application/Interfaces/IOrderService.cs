using System;
using System.Collections.Generic;
using System.Text;
using ECommerceAPI.Application.Common;
using ECommerceAPI.Application.DTOs.Order;

namespace ECommerceAPI.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateFromCartAsync(int userId, CreateOrderRequest request);
    Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId);
    Task<PaginatedResponse<OrderDto>> GetAllPagedAsync(int page, int pageSize);
    Task<OrderDto?> UpdateStatusAsync(int orderId, UpdateOrderStatusRequest request);
}