using System;
using System.Threading.Tasks;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.Measurement;
using KPISolution.Models.Entities.Notification;
using KPISolution.Models.Entities.Objective;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Entities.Dashboard;
using KPISolution.Models.Entities.Base;

namespace KPISolution.Data.Repositories.Interfaces
{
    /// <summary>
    /// Unit of Work interface for managing transactions
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Success Factor and Objective repositories
        IRepository<SuccessFactor> SuccessFactors { get; }

        // CSF repositories
        IRepository<CriticalSuccessFactor> CriticalSuccessFactors { get; }
        IRepository<CSFProgress> CSFProgresses { get; }
        IRepository<CSFKPI> CSFKPIs { get; }

        // Organization repositories
        IRepository<Department> Departments { get; }
        IRepository<BusinessObjective> BusinessObjectives { get; }

        // KPI related repositories
        IRepository<KRI> KRIs { get; }
        IRepository<PI> PIs { get; }
        IRepository<RI> RIs { get; }
        IRepository<Models.Entities.KPI.KPI> KPIs { get; }
        IRepository<KpiValue> KpiValues { get; }
        IRepository<KpiMeasurement> KpiMeasurements { get; }
        IRepository<Target> Targets { get; }
        IRepository<Threshold> Thresholds { get; }

        // Notification repositories
        IRepository<KPISolution.Models.Entities.Notification.Notification> Notifications { get; }

        // Dashboard repositories
        IRepository<CustomDashboard> CustomDashboards { get; }
        IRepository<DashboardItem> DashboardItems { get; }

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
    }
}
