namespace BotForge.Persistence;

/// <summary>
/// Defines a generic repository pattern interface for data access operations.
/// </summary>
/// <typeparam name="T">The type of entity this repository works with.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets a queryable collection of all entities of type T.
    /// </summary>
    /// <returns>An IQueryable of all entities.</returns>
    IQueryable<T> GetAll();

    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="ct">An instance of the <see cref="CancellationToken"/> to cancel operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="ct">An instance of the <see cref="CancellationToken"/> to cancel operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="ct">An instance of the <see cref="CancellationToken"/> to cancel operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously saves all changes made in this repository to the database.
    /// </summary>
    /// <param name="ct">An instance of the <see cref="CancellationToken"/> to cancel operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Asynchronously truncates repository table with deleting all items from it.
    /// </summary>
    /// <remarks>
    /// Use this method only when you're absolutely sure what you're doing! This method call will remove ALL items from the table.
    /// </remarks>
    /// <param name="ct">An instance of the <see cref="CancellationToken"/> to cancel operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task TruncateAsync(CancellationToken ct = default);
}

/// <summary>
/// Defines a generic repository pattern interface for data access operations with defined key type.
/// </summary>
/// <typeparam name="TKey">The type of the identifier used to retrieve an entity from this repository.</typeparam>
/// <typeparam name="TEntity">The type of entity this repository works with.</typeparam>
public interface IRepository<TKey, TEntity> : IRepository<TEntity>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Asynchronously retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="ct">An instance of the <see cref="CancellationToken"/> to cancel operation.</param>
    /// <returns>The entity if found; otherwise, <see langword="null"/>.</returns>
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
}
