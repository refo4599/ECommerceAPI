using System;
using System.Collections.Generic;
using System.Text;

using ECommerceAPI.Application.DTOs.Category;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;

namespace ECommerceAPI.Application.Services;

public class CategoryService(IUnitOfWork uow) : ICategoryService
{
    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await uow.Categories.GetAllActiveAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await uow.Categories.GetByIdAsync(id);
        return category is null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest req)
    {
        var category = new Category
        {
            Name = req.Name,
            Description = req.Description,
            ImageUrl = req.ImageUrl
        };

        await uow.Categories.AddAsync(category);
        await uow.SaveChangesAsync();
        return MapToDto(category);
    }

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryRequest req)
    {
        var category = await uow.Categories.GetByIdAsync(id);
        if (category is null) return null;

        if (req.Name is not null) category.Name = req.Name;
        if (req.Description is not null) category.Description = req.Description;
        if (req.ImageUrl is not null) category.ImageUrl = req.ImageUrl;
        category.UpdatedAt = DateTime.UtcNow;

        uow.Categories.Update(category);
        await uow.SaveChangesAsync();
        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await uow.Categories.GetByIdAsync(id);
        if (category is null) return false;

        uow.Categories.Remove(category);
        await uow.SaveChangesAsync();
        return true;
    }

    private static CategoryDto MapToDto(Category c) =>
        new(c.Id, c.Name, c.Description, c.ImageUrl);
}