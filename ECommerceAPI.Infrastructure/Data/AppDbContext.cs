using ECommerceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace ECommerceAPI.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<BranchProduct> BranchProducts => Set<BranchProduct>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasConversion<string>();
        });

        mb.Entity<Product>(e =>
            e.Property(p => p.BasePrice).HasColumnType("decimal(18,2)"));

        mb.Entity<BranchProduct>(e =>
        {
            e.HasIndex(bp => new { bp.BranchId, bp.ProductId }).IsUnique();
            e.Property(bp => bp.PriceOverride).HasColumnType("decimal(18,2)");
            e.HasOne(bp => bp.Branch).WithMany(b => b.BranchProducts)
                .HasForeignKey(bp => bp.BranchId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(bp => bp.Product).WithMany(p => p.BranchProducts)
                .HasForeignKey(bp => bp.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Cart>(e =>
        {
            e.HasIndex(c => new { c.UserId, c.BranchId }).IsUnique();
            e.HasOne(c => c.User).WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Order>(e =>
        {
            e.Property(o => o.Status).HasConversion<string>();
            e.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            e.HasOne(o => o.User).WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<OrderItem>(e =>
            e.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)"));
    }
}