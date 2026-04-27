using System;
using System.Collections.Generic;
using System.Text;

using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;
using ECommerceAPI.Infrastructure.Data;

namespace ECommerceAPI.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public IUserRepository Users { get; } = new UserRepository(db);
    public IBranchRepository Branches { get; } = new BranchRepository(db);
    public IProductRepository Products { get; } = new ProductRepository(db);
    public IBranchProductRepository BranchProducts { get; } = new BranchProductRepository(db);
    public ICartRepository Carts { get; } = new CartRepository(db);
    public IRepository<CartItem> CartItems { get; } = new GenericRepository<CartItem>(db);
    public IOrderRepository Orders { get; } = new OrderRepository(db);
    public IRepository<Category> Categories { get; } = new GenericRepository<Category>(db);

    public async Task<int> SaveChangesAsync() => await db.SaveChangesAsync();
    public void Dispose() => db.Dispose();
}
