using System.Linq.Expressions;

namespace KPISolution.Data.Repositories.Implementation
{
    /// <summary>
    /// Generic repository implementation for CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public class Repository<T> : IRepository<T>
        where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        /// <summary>
        /// Creates new repository instance
        /// </summary>
        /// <param name="context">Database context</param>
        public Repository(ApplicationDbContext context)
        {
            this._context = context;
            this._dbSet = context.Set<T>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await this._dbSet.Where(e => e.IsActive).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await this._dbSet.Where(e => e.IsActive).Where(predicate).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await this._dbSet.FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }

        /// <inheritdoc/>
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await this._dbSet.Where(e => e.IsActive).FirstOrDefaultAsync(predicate);
        }

        /// <inheritdoc/>
        public async Task AddAsync(T entity)
        {
            await this._dbSet.AddAsync(entity);
        }

        /// <inheritdoc/>
        public void Update(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            this._dbSet.Update(entity);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            this._dbSet.Update(entity);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Delete(T entity)
        {
            // Soft delete - just mark as inactive
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            this._dbSet.Update(entity);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(T entity)
        {
            // Soft delete - just mark as inactive
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            this._dbSet.Update(entity);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await this._dbSet.Where(e => e.IsActive).AnyAsync(predicate);
        }

        /// <inheritdoc/>
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await this._dbSet.Where(e => e.IsActive).CountAsync(predicate);
        }
    }
}
