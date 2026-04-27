using System;
using System.Collections.Generic;
using System.Text;
using ECommerceAPI.Application.DTOs.Product;

namespace ECommerceAPI.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<IEnumerable<BranchProductDto>> GetByBranchAsync(int branchId);
    Task<ProductDto> CreateAsync(CreateProductRequest request);
    Task<ProductDto?> UpdateAsync(int id, UpdateProductRequest request);
    Task<bool> DeleteAsync(int id);
    Task UpdateStockAsync(int branchId, UpdateStockRequest request);
    Task AssignToBranchAsync(int branchId, AssignProductRequest request);
}
