using System.Linq.Expressions;
using System.Reflection;

namespace KPISolution.Data.Repositories.Extensions
{
    /// <summary>
    /// Extension methods for IRepository
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Gets a queryable collection of all active entities
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="repository">Repository instance</param>
        /// <returns>Queryable collection of entities</returns>
        public static IQueryable<T> GetAll<T>(this IRepository<T> repository) where T : BaseEntity
        {
            // Get the DbContext from the repository using reflection
            var dbContext = GetDbContextFromRepository(repository);

            // Get the DbSet
            return dbContext.Set<T>().Where(e => e.IsActive);
        }

        /// <summary>
        /// Gets a queryable collection of entities filtered by predicate
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="repository">Repository instance</param>
        /// <param name="predicate">Filter predicate</param>
        /// <returns>Filtered queryable collection of entities</returns>
        public static IQueryable<T> GetAllWhere<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            return repository.GetAll().Where(predicate);
        }

        /// <summary>
        /// Gets a queryable collection with tracking disabled
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="repository">Repository instance</param>
        /// <returns>Queryable collection with tracking disabled</returns>
        public static IQueryable<T> AsNoTracking<T>(this IRepository<T> repository) where T : BaseEntity
        {
            return repository.GetAll().AsNoTracking();
        }

        /// <summary>
        /// Filters a collection of entities based on predicate
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="repository">Repository instance</param>
        /// <param name="predicate">Filter predicate</param>
        /// <returns>Filtered queryable collection</returns>
        public static IQueryable<T> Where<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            return repository.GetAll().Where(predicate);
        }

        /// <summary>
        /// Includes related entities in the query
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="repository">Repository instance</param>
        /// <param name="navigationPropertyPath">Navigation property expression</param>
        /// <returns>Queryable with included related entities</returns>
        public static IQueryable<T> Include<T, TProperty>(this IRepository<T> repository, Expression<Func<T, TProperty>> navigationPropertyPath) where T : BaseEntity
        {
            return repository.GetAll().Include(navigationPropertyPath);
        }

        /// <summary>
        /// Orders entities by a key selector in ascending order
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <param name="repository">Repository instance</param>
        /// <param name="keySelector">Key selector expression</param>
        /// <returns>Ordered queryable collection</returns>
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IRepository<T> repository, Expression<Func<T, TKey>> keySelector) where T : BaseEntity
        {
            return repository.GetAll().OrderBy(keySelector);
        }

        /// <summary>
        /// Orders entities by a key selector in descending order
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <param name="repository">Repository instance</param>
        /// <param name="keySelector">Key selector expression</param>
        /// <returns>Ordered queryable collection</returns>
        public static IOrderedQueryable<T> OrderByDescending<T, TKey>(this IRepository<T> repository, Expression<Func<T, TKey>> keySelector) where T : BaseEntity
        {
            return repository.GetAll().OrderByDescending(keySelector);
        }

        /// <summary>
        /// Projects each element of a sequence into a new form
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="repository">Repository instance</param>
        /// <param name="selector">Projection function</param>
        /// <returns>Projected queryable collection</returns>
        public static IQueryable<TResult> Select<T, TResult>(this IRepository<T> repository, Expression<Func<T, TResult>> selector) where T : BaseEntity
        {
            return repository.GetAll().Select(selector);
        }

        /// <summary>
        /// Gets DbContext from repository instance using reflection
        /// </summary>
        private static ApplicationDbContext GetDbContextFromRepository<T>(IRepository<T> repository) where T : BaseEntity
        {
            var dbContext = repository.GetType()
                .GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(repository) as ApplicationDbContext;

            if (dbContext == null)
            {
                throw new InvalidOperationException("Could not access the DbContext from the repository");
            }

            return dbContext;
        }

        /// <summary>
        /// Executes ToListAsync on the queryable
        /// </summary>
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> queryable)
        {
            return EntityFrameworkQueryableExtensions.ToListAsync(queryable);
        }

        /// <summary>
        /// Executes FirstOrDefaultAsync on the queryable
        /// </summary>
        public static Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> queryable)
        {
            return EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(queryable);
        }

        /// <summary>
        /// Executes FirstOrDefaultAsync on the queryable with a predicate
        /// </summary>
        public static Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
        {
            return EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(queryable, predicate);
        }

        /// <summary>
        /// Executes AnyAsync on the queryable
        /// </summary>
        public static Task<bool> AnyAsync<T>(this IQueryable<T> queryable)
        {
            return EntityFrameworkQueryableExtensions.AnyAsync(queryable);
        }

        /// <summary>
        /// Executes AnyAsync on the queryable with a predicate
        /// </summary>
        public static Task<bool> AnyAsync<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
        {
            return EntityFrameworkQueryableExtensions.AnyAsync(queryable, predicate);
        }

        /// <summary>
        /// Executes CountAsync on the queryable
        /// </summary>
        public static Task<int> CountAsync<T>(this IQueryable<T> queryable)
        {
            return EntityFrameworkQueryableExtensions.CountAsync(queryable);
        }

        /// <summary>
        /// Executes CountAsync on the queryable with a predicate
        /// </summary>
        public static Task<int> CountAsync<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
        {
            return EntityFrameworkQueryableExtensions.CountAsync(queryable, predicate);
        }

        /// <summary>
        /// Gets the paged result
        /// </summary>
        public static async Task<(List<T> Items, int TotalCount)> ToPagedListAsync<T>(this IQueryable<T> query, int page, int pageSize) where T : BaseEntity
        {
            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// Gets the paged result with projection
        /// </summary>
        public static async Task<(List<TResult> Items, int TotalCount)> ToPagedListAsync<TEntity, TResult>(
            this IRepository<TEntity> repository,
            Expression<Func<TEntity, TResult>> selector,
            int page,
            int pageSize) where TEntity : BaseEntity
        {
            var query = repository.GetAll();
            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(selector).ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// Projects and paginates a queryable collection
        /// </summary>
        public static async Task<(List<TResult> Items, int TotalCount)> ProjectToPagedListAsync<TEntity, TResult>(
            this IRepository<TEntity> repository,
            Expression<Func<TEntity, TResult>> selector,
            int page,
            int pageSize) where TEntity : BaseEntity
        {
            var query = repository.GetAll();
            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(selector).ToListAsync();

            return (items, totalCount);
        }
    }
}
