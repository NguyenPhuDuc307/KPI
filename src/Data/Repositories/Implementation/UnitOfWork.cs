using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage;

namespace KPISolution.Data.Repositories.Implementation
{
    /// <summary>
    /// Unit of Work implementation for managing transactions
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;
        private readonly ConcurrentDictionary<Type, object> _repositories = new ConcurrentDictionary<Type, object>();

        // Base entities
        private IRepository<BaseEntity>? _baseEntities;

        // Indicator repositories
        private IRepository<SuccessFactor>? _successFactors;
        private IRepository<ResultIndicator>? _resultIndicators;
        private IRepository<PerformanceIndicator>? _performanceIndicators;
        private IRepository<Measurement>? _measurements;

        // Organization repositories
        private IRepository<Department>? _departments;
        private IRepository<Objective>? _objectives;

        // Notification repositories
        private IRepository<Notification>? _notifications;

        // Dashboard repositories
        private IRepository<CustomDashboard>? _customDashboards;
        private IRepository<DashboardItem>? _dashboardItems;

        // Progress repositories
        private IRepository<ProgressUpdate>? _progressUpdates;

        public UnitOfWork(ApplicationDbContext context)
        {
            this._context = context;
        }

        // Base entities
        public IRepository<BaseEntity> BaseEntities => this._baseEntities ??= new Repository<BaseEntity>(this._context);

        // Indicator repositories
        public IRepository<SuccessFactor> SuccessFactors => this._successFactors ??= new Repository<SuccessFactor>(this._context);
        public IRepository<ResultIndicator> ResultIndicators => this._resultIndicators ??= new Repository<ResultIndicator>(this._context);
        public IRepository<PerformanceIndicator> PerformanceIndicators => this._performanceIndicators ??= new Repository<PerformanceIndicator>(this._context);
        public IRepository<Measurement> Measurements => this._measurements ??= new Repository<Measurement>(this._context);

        // Organization repositories
        public IRepository<Department> Departments => this._departments ??= new Repository<Department>(this._context);
        public IRepository<Objective> Objectives => this._objectives ??= new Repository<Objective>(this._context);

        // Notification repositories
        public IRepository<Notification> Notifications => this._notifications ??= new Repository<Notification>(this._context);

        // Dashboard repositories
        public IRepository<CustomDashboard> CustomDashboards => this._customDashboards ??= new Repository<CustomDashboard>(this._context);
        public IRepository<DashboardItem> DashboardItems => this._dashboardItems ??= new Repository<DashboardItem>(this._context);

        // Progress repositories
        public IRepository<ProgressUpdate> ProgressUpdates => this._progressUpdates ??= new Repository<ProgressUpdate>(this._context);

        /// <inheritdoc/>
        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            return (IRepository<T>)this._repositories.GetOrAdd(typeof(T), type => new Repository<T>(this._context));
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync()
        {
            return await this._context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<int> CompleteAsync()
        {
            return await this._context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public int SaveChanges()
        {
            return this._context.SaveChanges();
        }

        /// <inheritdoc/>
        public async Task BeginTransactionAsync()
        {
            this._transaction = await this._context.Database.BeginTransactionAsync();
        }

        /// <inheritdoc/>
        public async Task CommitTransactionAsync()
        {
            try
            {
                if (this._transaction != null)
                {
                    await this._transaction.CommitAsync();
                }
            }
            finally
            {
                if (this._transaction != null)
                {
                    await this._transaction.DisposeAsync();
                    this._transaction = null;
                }
            }
        }

        /// <inheritdoc/>
        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (this._transaction != null)
                {
                    await this._transaction.RollbackAsync();
                }
            }
            finally
            {
                if (this._transaction != null)
                {
                    await this._transaction.DisposeAsync();
                    this._transaction = null;
                }
            }
        }

        /// <summary>
        /// Disposes the UnitOfWork instance
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the UnitOfWork instance
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._context.Dispose();
                    this._transaction?.Dispose();
                }

                this._disposed = true;
            }
        }
    }
}
