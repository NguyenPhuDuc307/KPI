using System.Linq.Expressions;

namespace KPISolution.Data.Repositories.Interfaces
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>Collection of all entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets entities matching the specified predicate
        /// </summary>
        /// <param name="predicate">Filter predicate</param>
        /// <returns>Collection of entities matching the predicate</returns>
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets entity by its ID
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>Entity with the specified ID</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets first entity matching the specified predicate
        /// </summary>
        /// <param name="predicate">Filter predicate</param>
        /// <returns>First entity matching the predicate</returns>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds entity to repository
        /// </summary>
        /// <param name="entity">Entity to add</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates entity in repository
        /// </summary>
        /// <param name="entity">Entity to update</param>
        void Update(T entity);

        /// <summary>
        /// Updates entity in repository asynchronously
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Removes entity from repository
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        void Delete(T entity);

        /// <summary>
        /// Removes entity from repository asynchronously
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Checks if entity matching the specified predicate exists
        /// </summary>
        /// <param name="predicate">Filter predicate</param>
        /// <returns>True if entity exists, false otherwise</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Counts entities matching the specified predicate
        /// </summary>
        /// <param name="predicate">Filter predicate</param>
        /// <returns>Number of entities matching the predicate</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}
