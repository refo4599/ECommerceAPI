using System;
using System.Collections.Generic;
using System.Text;
using ECommerceAPI.Domain.Entities;

namespace ECommerceAPI.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IBranchRepository Branches { get; }
    IProductRepository Products { get; }
    IBranchProductRepository BranchProducts { get; }
    ICartRepository Carts { get; }
    IRepository<CartItem> CartItems { get; }
    IOrderRepository Orders { get; }
    IRepository<Category> Categories { get; }

    Task<int> SaveChangesAsync();
}