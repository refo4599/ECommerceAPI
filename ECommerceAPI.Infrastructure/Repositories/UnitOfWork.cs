using System;
using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;
using ECommerceAPI.Infrastructure.Data;

namespace ECommerceAPI.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public IUserRepository Users { get; } = new UserRepository(db);
    public IBranchRepository Branches { get; } = new BranchRepository(db);
    public ICategoryRepository Categories { get; } = new CategoryRepository(db);
    public IProductRepository Products { get; } = new ProductRepository(db);
    public IBranchProductRepository BranchProducts { get; } = new BranchProductRepository(db);
    public ICartRepository Carts { get; } = new CartRepository(db);
    public IRepository<CartItem> CartItems { get; } = new GenericRepository<CartItem>(db);
    public IOrderRepository Orders { get; } = new OrderRepository(db);

    public async Task<int> SaveChangesAsync() => await db.SaveChangesAsync();
    public void Dispose() => db.Dispose();
}