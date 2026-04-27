using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Application.DTOs.Branch;

public record BranchDto(
    int Id,
    string Name,
    string NameAr,
    string Slug,
    string? ImageUrl,
    bool IsDefault,
    bool IsActive);

public record CreateBranchRequest(
    string Name,
    string NameAr,
    string Slug,
    string? ImageUrl,
    bool IsDefault = false);

public record UpdateBranchRequest(
    string? Name,
    string? NameAr,
    string? ImageUrl,
    bool? IsDefault,
    bool? IsActive);
