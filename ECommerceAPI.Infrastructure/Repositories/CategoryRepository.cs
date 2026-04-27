using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;
using ECommerceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Repositories;

public class CategoryRepository(AppDbContext db)
    : GenericRepository<Category>(db), ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAllActiveAsync() =>
        await _set.ToListAsync();
}