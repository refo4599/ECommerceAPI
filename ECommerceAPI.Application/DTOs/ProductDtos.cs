using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Application.DTOs.Product;

public record CategoryDto(int Id, string Name, string? ImageUrl);

public record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal BasePrice,
    string? ImageUrl,
    bool IsActive,
    CategoryDto? Category);

public record BranchProductDto(
    int Id,
    string Name,
    string? Description,
    decimal EffectivePrice,
    string? ImageUrl,
    bool IsAvailable,
    int Stock,
    CategoryDto? Category);

public record CreateProductRequest(
    string Name,
    string? Description,
    decimal BasePrice,
    string? ImageUrl,
    int CategoryId);

public record UpdateProductRequest(
    string? Name,
    string? Description,
    decimal? BasePrice,
    string? ImageUrl,
    bool? IsActive,
    int? CategoryId);

public record UpdateStockRequest(
    int ProductId,
    int Stock,
    decimal? PriceOverride,
    bool? IsAvailable);

public record AssignProductRequest(
    int ProductId,
    int Stock = 0,
    decimal? PriceOverride = null);