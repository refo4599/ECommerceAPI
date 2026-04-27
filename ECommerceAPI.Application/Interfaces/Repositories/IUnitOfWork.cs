using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IBranchRepository Branches { get; }
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    IBranchProductRepository BranchProducts { get; }
    ICartRepository Carts { get; }
    IRepository<CartItem> CartItems { get; }
    IOrderRepository Orders { get; }
    Task<int> SaveChangesAsync();
}