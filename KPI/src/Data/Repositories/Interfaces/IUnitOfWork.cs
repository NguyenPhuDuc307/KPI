using System;
using System.Threading.Tasks;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.Measurement;
using KPISolution.Models.Entities.Notification;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Entities.Dashboard;

namespace KPISolution.Data.Repositories.Interfaces
{
    /// <summary>
    /// Unit of Work interface for managing transactions
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IRepository<CriticalSuccessFactor> CriticalSuccessFactors { get; }

        IRepository<CSFProgress> CSFProgresses { get; }

        IRepository<CSFKPI> CSFKPIs { get; }

        IRepository<Department> Departments { get; }

        IRepository<BusinessObjective> BusinessObjectives { get; }

        IRepository<KRI> KRIs { get; }

        IRepository<PI> PIs { get; }

        IRepository<RI> RIs { get; }

        IRepository<KpiValue> KpiValues { get; }

        IRepository<KpiMeasurement> KpiMeasurements { get; }

        IRepository<Target> Targets { get; }

        IRepository<Threshold> Thresholds { get; }

        IRepository<KPISolution.Models.Entities.Notification.Notification> Notifications { get; }

        // Dashboard repositories
        IRepository<CustomDashboard> CustomDashboards { get; }
        IRepository<DashboardItem> DashboardItems { get; }

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
