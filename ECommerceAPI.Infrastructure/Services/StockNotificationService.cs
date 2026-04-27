using System;
using System.Collections.Generic;
using System.Text;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ECommerceAPI.Infrastructure.Services;

public class StockNotificationService(IHubContext<StockHub> hubContext)
    : IStockNotificationService
{
    public async Task NotifyStockUpdatedAsync(int branchId, int productId, int newStock)
    {
        await hubContext.Clients
            .Group($"branch_{branchId}")
            .SendAsync("StockUpdated", new
            {
                branchId,
                productId,
                newStock
            });
    }

    public async Task NotifyBranchProductsUpdatedAsync(int branchId)
    {
        await hubContext.Clients
            .Group($"branch_{branchId}")
            .SendAsync("BranchProductsUpdated", new { branchId });
    }
}