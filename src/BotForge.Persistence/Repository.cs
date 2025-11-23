using Microsoft.EntityFrameworkCore;

namespace BotForge.Persistence;

/// <summary>
/// Provides a generic implementation of the repository pattern using Entity Framework Core.
/// </summary>
/// <typeparam name="TContext">The type of DbContext to use.</typeparam>
/// <typeparam name="TKey">The type of the identifier used to retrieve an entity from this repository.</typeparam>
/// <typeparam name="T">The type of entity this repository works with.</typeparam>
/// <param name="context">The database context instance.</param>
public class Repository<TContext, TKey, T>(TContext context) : IRepository<TKey, T>
    where TContext : DbContext
    where TKey : IEquatable<TKey>
    where T : class
{
    private readonly TContext _context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    /// <summary>
    /// Gets the DbSet for the entity type.
    /// </summary>
    protected DbSet<T> DbSet => _dbSet;

    /// <summary>
    /// Gets the database context instance.
    /// </summary>
    protected TContext Context => _context;

    /// <inheritdoc />
    public virtual IQueryable<T> GetAll() => DbSet;

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(TKey id, CancellationToken ct = default) => await DbSet.FindAsync([id], ct).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task AddAsync(T entity, CancellationToken ct = default) => await DbSet.AddAsync(entity, ct).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task SaveChangesAsync(CancellationToken ct = default) => await Context.SaveChangesAsync(ct).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task TruncateAsync(CancellationToken ct = default) => await DbSet.ExecuteDeleteAsync(ct).ConfigureAwait(false);
}
