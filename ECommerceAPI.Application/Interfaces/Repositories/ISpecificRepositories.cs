using System;
using System.Collections.Generic;
using System.Text;

using ECommerceAPI.Domain.Entities;

namespace ECommerceAPI.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

public interface IBranchRepository : IRepository<Branch>
{
    Task<Branch?> GetDefaultAsync();
    Task<IEnumerable<Branch>> GetActiveAsync();
    Task ResetDefaultAsync();
}

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    Task<Product?> GetByIdWithCategoryAsync(int id);
}

public interface IBranchProductRepository : IRepository<BranchProduct>
{
    Task<IEnumerable<BranchProduct>> GetByBranchAsync(int branchId);
    Task<BranchProduct?> GetByBranchAndProductAsync(int branchId, int productId);
}

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetCartWithItemsAsync(int userId, int branchId);
}

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserAsync(int userId);
    Task<(IEnumerable<Order> Items, int Total)> GetAllPagedAsync(int page, int pageSize);
    Task<Order?> GetByIdWithItemsAsync(int orderId);
}
