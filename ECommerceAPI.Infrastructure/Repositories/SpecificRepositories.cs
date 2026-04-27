using System;
using System.Collections.Generic;
using System.Text;

using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;
using ECommerceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Repositories;

public class UserRepository(AppDbContext db)
    : GenericRepository<User>(db), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email) =>
        await _set.FirstOrDefaultAsync(u => u.Email == email);
}

public class BranchRepository(AppDbContext db)
    : GenericRepository<Branch>(db), IBranchRepository
{
    public async Task<Branch?> GetDefaultAsync() =>
        await _set.FirstOrDefaultAsync(b => b.IsDefault && b.IsActive);

    public async Task<IEnumerable<Branch>> GetActiveAsync() =>
        await _set.Where(b => b.IsActive).ToListAsync();

    public async Task ResetDefaultAsync() =>
        await _set.Where(b => b.IsDefault)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsDefault, false));
}

public class ProductRepository(AppDbContext db)
    : GenericRepository<Product>(db), IProductRepository
{
    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync() =>
        await _set.Include(p => p.Category)
            .Where(p => p.IsActive).ToListAsync();

    public async Task<Product?> GetByIdWithCategoryAsync(int id) =>
        await _set.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
}

public class BranchProductRepository(AppDbContext db)
    : GenericRepository<BranchProduct>(db), IBranchProductRepository
{
    public async Task<IEnumerable<BranchProduct>> GetByBranchAsync(int branchId) =>
        await _set
            .Include(bp => bp.Product).ThenInclude(p => p.Category)
            .Where(bp => bp.BranchId == branchId)
            .ToListAsync();

    public async Task<BranchProduct?> GetByBranchAndProductAsync(int branchId, int productId) =>
        await _set.FirstOrDefaultAsync(bp =>
            bp.BranchId == branchId && bp.ProductId == productId);
}

public class CartRepository(AppDbContext db)
    : GenericRepository<Cart>(db), ICartRepository
{
    public async Task<Cart?> GetCartWithItemsAsync(int userId, int branchId) =>
        await _set
            .Include(c => c.Branch)
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.BranchId == branchId);
}

public class OrderRepository(AppDbContext db)
    : GenericRepository<Order>(db), IOrderRepository
{
    public async Task<IEnumerable<Order>> GetByUserAsync(int userId) =>
        await _set
            .Include(o => o.Branch)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<(IEnumerable<Order> Items, int Total)> GetAllPagedAsync(
        int page, int pageSize)
    {
        var query = _set
            .Include(o => o.Branch)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.CreatedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Order?> GetByIdWithItemsAsync(int orderId) =>
        await _set
            .Include(o => o.Branch)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
}