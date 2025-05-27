namespace KPISolution.Data.Repositories.Interfaces
{
    /// <summary>
    /// Unit of Work interface for managing transactions
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Regular repositories
        IRepository<BaseEntity> BaseEntities { get; }
        IRepository<Department> Departments { get; }
        IRepository<SuccessFactor> SuccessFactors { get; }
        IRepository<ResultIndicator> ResultIndicators { get; }
        IRepository<PerformanceIndicator> PerformanceIndicators { get; }
        IRepository<Objective> Objectives { get; }
        IRepository<Measurement> Measurements { get; }

        // Added shortcuts for compatibility with older code
        IRepository<ResultIndicator> KRIs => this.ResultIndicators;
        IRepository<PerformanceIndicator> PIs => this.PerformanceIndicators;

        // Notification repositories
        IRepository<Notification> Notifications { get; }

        // Dashboard repositories
        IRepository<CustomDashboard> CustomDashboards { get; }
        IRepository<DashboardItem> DashboardItems { get; }

        // Progress repositories
        IRepository<ProgressUpdate> ProgressUpdates { get; }

        /// <summary>
        /// Gets repository for the specified entity type
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>Repository for the entity type</returns>
        IRepository<T> Repository<T>() where T : BaseEntity;

        /// <summary>
        /// Saves all changes made in this context to the database
        /// </summary>
        /// <returns>Number of state entries written to the database</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Completes the unit of work by saving all changes to the database
        /// </summary>
        /// <returns>Number of state entries written to the database</returns>
        Task<int> CompleteAsync();

        /// <summary>
        /// Begins a transaction
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the transaction
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the transaction
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Saves all changes made in this context to the database
        /// </summary>
        /// <returns>Number of state entries written to the database</returns>
        int SaveChanges();
    }
}
