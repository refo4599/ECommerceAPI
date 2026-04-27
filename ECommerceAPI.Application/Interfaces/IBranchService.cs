using System;
using System.Collections.Generic;
using System.Text;

using ECommerceAPI.Application.DTOs.Branch;

namespace ECommerceAPI.Application.Interfaces;

public interface IBranchService
{
    Task<IEnumerable<BranchDto>> GetAllAsync();
    Task<BranchDto?> GetByIdAsync(int id);
    Task<BranchDto?> GetDefaultAsync();
    Task<BranchDto> CreateAsync(CreateBranchRequest request);
    Task<BranchDto?> UpdateAsync(int id, UpdateBranchRequest request);
    Task<bool> DeleteAsync(int id);
}