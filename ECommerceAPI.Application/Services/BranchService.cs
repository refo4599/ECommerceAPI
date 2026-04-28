using System;
using System.Collections.Generic;
using System.Text;
using ECommerceAPI.Application.DTOs.Branch;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;

namespace ECommerceAPI.Application.Services;

public class BranchService(IUnitOfWork uow) : IBranchService
{
    public async Task<IEnumerable<BranchDto>> GetAllAsync()
    {
        var branches = await uow.Branches.GetActiveAsync();
        return branches.Select(MapToDto);
    }

    public async Task<BranchDto?> GetByIdAsync(int id)
    {
        var branch = await uow.Branches.GetByIdAsync(id);
        return branch is null ? null : MapToDto(branch);
    }

    public async Task<BranchDto?> GetDefaultAsync()
    {
        var branch = await uow.Branches.GetDefaultAsync();
        return branch is null ? null : MapToDto(branch);
    }

    public async Task<BranchDto> CreateAsync(CreateBranchRequest req)
    {
        if (req.IsDefault)
            await uow.Branches.ResetDefaultAsync();

        var branch = new Branch
        {
            Name = req.Name,
            Location = req.Location,
            IsDefault = req.IsDefault
        };

        await uow.Branches.AddAsync(branch);
        await uow.SaveChangesAsync();
        return MapToDto(branch);
    }

    public async Task<BranchDto?> UpdateAsync(int id, UpdateBranchRequest req)
    {
        var branch = await uow.Branches.GetByIdAsync(id);
        if (branch is null) return null;

        if (req.IsDefault == true)
            await uow.Branches.ResetDefaultAsync();

        if (req.Name is not null) branch.Name = req.Name;
        if (req.Location is not null) branch.Location = req.Location;
        if (req.IsDefault.HasValue) branch.IsDefault = req.IsDefault.Value;
        if (req.IsActive.HasValue) branch.IsActive = req.IsActive.Value;
        branch.UpdatedAt = DateTime.UtcNow;

        uow.Branches.Update(branch);
        await uow.SaveChangesAsync();
        return MapToDto(branch);  // ← الصح
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var branch = await uow.Branches.GetByIdAsync(id);
        if (branch is null) return false;

        branch.IsActive = false;
        branch.UpdatedAt = DateTime.UtcNow;
        uow.Branches.Update(branch);
        await uow.SaveChangesAsync();
        return true;
    }

    private static BranchDto MapToDto(Branch b) =>
        new(b.Id, b.Name, b.Location, b.IsDefault, b.IsActive);
}