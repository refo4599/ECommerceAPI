using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Application.Interfaces;

public interface IStockNotificationService
{
    Task NotifyStockUpdatedAsync(int branchId, int productId, int newStock);
    Task NotifyBranchProductsUpdatedAsync(int branchId);
}