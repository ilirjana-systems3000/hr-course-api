using System.Linq.Expressions;
using HrCourseApi.Application.Abstractions.Repositories;
using HrCourseApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrCourseApi.Infrastructure.Repositories;

public sealed class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _db;

    public Repository(AppDbContext db) => _db = db;

    public IQueryable<T> Query() => _db.Set<T>().AsQueryable();

    public Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Set<T>().FindAsync(new object[] { id }, ct).AsTask();

    public Task AddAsync(T entity, CancellationToken ct = default) => _db.Set<T>().AddAsync(entity, ct).AsTask();

    public void Update(T entity) => _db.Set<T>().Update(entity);

    public void Remove(T entity) => _db.Set<T>().Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        _db.Set<T>().AnyAsync(predicate, ct);
}
