using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using ECommerceAPI.Application.Interfaces.Repositories;
using ECommerceAPI.Domain.Entities;
using ECommerceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure.Repositories;

public class GenericRepository<T>(AppDbContext db) : IRepository<T>
    where T : BaseEntity
{
    protected readonly DbSet<T> _set = db.Set<T>();

    public async Task<T?> GetByIdAsync(int id) =>
        await _set.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _set.ToListAsync();

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
        await _set.FirstOrDefaultAsync(predicate);

    public async Task AddAsync(T entity) =>
        await _set.AddAsync(entity);

    public void Update(T entity) =>
        _set.Update(entity);

    public void Remove(T entity) =>
        _set.Remove(entity);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) =>
        await _set.AnyAsync(predicate);
}