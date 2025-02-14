using System.Linq.Expressions;
using Codex.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Codex.Api.Shared;

public interface IRepositoryBase<T>
{
    IQueryable<T> FindAll();
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected ApplicationDbContext RepositoryContext { get; set; }

    public RepositoryBase(ApplicationDbContext repositoryContext)
    {
        RepositoryContext = repositoryContext;
    }

    public IQueryable<T> FindAll()
    {
        return RepositoryContext.Set<T>().AsNoTracking();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
    {
        return RepositoryContext.Set<T>()
            .Where(expression)
            .AsNoTracking();
    }

    public void Create(T entity)
    {
        RepositoryContext.Set<T>().AddAsync(entity);
    }

    public void Update(T entity)
    {
        RepositoryContext.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        RepositoryContext.Set<T>().Remove(entity);
    }
}