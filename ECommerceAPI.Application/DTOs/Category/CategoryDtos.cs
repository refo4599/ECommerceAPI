using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Application.DTOs.Category;

public record CategoryDto(int Id, string Name, string NameAr, string? ImageUrl);

public record CreateCategoryRequest(string Name, string NameAr, string? ImageUrl);

public record UpdateCategoryRequest(string? Name, string? NameAr, string? ImageUrl);