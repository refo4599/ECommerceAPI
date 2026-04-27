using ECommerceAPI.Domain.Entities;

namespace ECommerceAPI.Application.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllActiveAsync();
}