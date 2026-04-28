using System;
using System.Collections.Generic;
using System.Text;
using ECommerceAPI.Application.DTOs.Product;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;

namespace ECommerceAPI.Application.Services;

public class ProductService(IUnitOfWork uow) : IProductService
{
    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await uow.Products.GetAllWithCategoryAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await uow.Products.GetByIdWithCategoryAsync(id);
        return product is null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<BranchProductDto>> GetByBranchAsync(int branchId)
    {
        var list = await uow.BranchProducts.GetByBranchAsync(branchId);
        return list
            .Where(bp => bp.Product.IsActive && bp.IsAvailable)
            .Select(bp => new BranchProductDto(
                bp.ProductId,
                bp.Product.Name,
                bp.Product.Description,
                bp.PriceOverride ?? bp.Product.BasePrice,
                bp.Product.ImageUrl,
                bp.IsAvailable,
                bp.Stock,
                bp.Product.Category is null ? null :
                    new CategoryDto(
                        bp.Product.Category.Id,
                        bp.Product.Category.Name,
                        bp.Product.Category.ImageUrl)));
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest req)
    {
        var product = new Product
        {
            Name = req.Name,
            Description = req.Description,
            BasePrice = req.BasePrice,
            ImageUrl = req.ImageUrl,
            CategoryId = req.CategoryId
        };

        await uow.Products.AddAsync(product);
        await uow.SaveChangesAsync();

        var created = await uow.Products.GetByIdWithCategoryAsync(product.Id);
        return MapToDto(created!);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductRequest req)
    {
        var product = await uow.Products.GetByIdWithCategoryAsync(id);
        if (product is null) return null;

        if (req.Name is not null) product.Name = req.Name;
        if (req.Description is not null) product.Description = req.Description;
        if (req.BasePrice.HasValue) product.BasePrice = req.BasePrice.Value;
        if (req.ImageUrl is not null) product.ImageUrl = req.ImageUrl;
        if (req.IsActive.HasValue) product.IsActive = req.IsActive.Value;
        if (req.CategoryId.HasValue) product.CategoryId = req.CategoryId.Value;
        product.UpdatedAt = DateTime.UtcNow;

        uow.Products.Update(product);
        await uow.SaveChangesAsync();

        var updated = await uow.Products.GetByIdWithCategoryAsync(id);
        return MapToDto(updated!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await uow.Products.GetByIdAsync(id);
        if (product is null) return false;

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        uow.Products.Update(product);
        await uow.SaveChangesAsync();
        return true;
    }

    public async Task UpdateStockAsync(int branchId, UpdateStockRequest req)
    {
        var bp = await uow.BranchProducts
            .GetByBranchAndProductAsync(branchId, req.ProductId);

        if (bp is null)
        {
            bp = new BranchProduct { BranchId = branchId, ProductId = req.ProductId };
            await uow.BranchProducts.AddAsync(bp);
        }

        bp.Stock = req.Stock;
        bp.UpdatedAt = DateTime.UtcNow;
        if (req.PriceOverride.HasValue) bp.PriceOverride = req.PriceOverride;
        if (req.IsAvailable.HasValue) bp.IsAvailable = req.IsAvailable.Value;

        uow.BranchProducts.Update(bp);
        await uow.SaveChangesAsync();
    }

    public async Task AssignToBranchAsync(int branchId, AssignProductRequest req)
    {
        var exists = await uow.BranchProducts
            .AnyAsync(bp => bp.BranchId == branchId && bp.ProductId == req.ProductId);

        if (!exists)
        {
            await uow.BranchProducts.AddAsync(new BranchProduct
            {
                BranchId = branchId,
                ProductId = req.ProductId,
                Stock = req.Stock,
                PriceOverride = req.PriceOverride
            });
            await uow.SaveChangesAsync();
        }
    }

    private static ProductDto MapToDto(Product p) => new(
        p.Id, p.Name, p.Description,
        p.BasePrice, p.ImageUrl, p.IsActive,
        p.Category is null ? null :
            new CategoryDto(
                p.Category.Id,
                p.Category.Name,
                p.Category.ImageUrl));
}