using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.Measurement;
using KPISolution.Models.Entities.Notification;
using KPISolution.Models.Entities.Objective;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Entities.Dashboard;
using KPISolution.Models.Entities.Base;
using System.Collections.Concurrent;

namespace KPISolution.Data.Repositories.Implementation
{
    /// <summary>
    /// Unit of Work implementation for managing transactions
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;
        private readonly ConcurrentDictionary<Type, object> _repositories = new ConcurrentDictionary<Type, object>();

        // Success Factor and Objective repositories
        private IRepository<SuccessFactor>? _successFactors;

        // CSF repositories
        private IRepository<CriticalSuccessFactor>? _criticalSuccessFactors;
        private IRepository<CSFProgress>? _csfProgresses;
        private IRepository<CSFKPI>? _csfKpis;

        // Organization repositories
        private IRepository<Department>? _departments;
        private IRepository<BusinessObjective>? _businessObjectives;

        // KPI related repositories
        private IRepository<KRI>? _kris;
        private IRepository<PI>? _pis;
        private IRepository<RI>? _ris;
        private IRepository<Models.Entities.KPI.KPI>? _kpis;
        private IRepository<KpiValue>? _kpiValues;
        private IRepository<KpiMeasurement>? _kpiMeasurements;
        private IRepository<Target>? _targets;
        private IRepository<Threshold>? _thresholds;

        // Notification repositories
        private IRepository<KPISolution.Models.Entities.Notification.Notification>? _notifications;

        // Dashboard repositories
        private IRepository<CustomDashboard>? _customDashboards;
        private IRepository<DashboardItem>? _dashboardItems;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Success Factor and Objective repositories
        public IRepository<SuccessFactor> SuccessFactors => _successFactors ??= new Repository<SuccessFactor>(_context);

        // CSF repositories
        public IRepository<CriticalSuccessFactor> CriticalSuccessFactors => _criticalSuccessFactors ??= new Repository<CriticalSuccessFactor>(_context);
        public IRepository<CSFProgress> CSFProgresses => _csfProgresses ??= new Repository<CSFProgress>(_context);
        public IRepository<CSFKPI> CSFKPIs => _csfKpis ??= new Repository<CSFKPI>(_context);

        // Organization repositories
        public IRepository<Department> Departments => _departments ??= new Repository<Department>(_context);
        public IRepository<BusinessObjective> BusinessObjectives => _businessObjectives ??= new Repository<BusinessObjective>(_context);

        // KPI related repositories
        public IRepository<KRI> KRIs => _kris ??= new Repository<KRI>(_context);
        public IRepository<PI> PIs => _pis ??= new Repository<PI>(_context);
        public IRepository<RI> RIs => _ris ??= new Repository<RI>(_context);
        public IRepository<Models.Entities.KPI.KPI> KPIs => _kpis ??= new Repository<Models.Entities.KPI.KPI>(_context);
        public IRepository<KpiValue> KpiValues => _kpiValues ??= new Repository<KpiValue>(_context);
        public IRepository<KpiMeasurement> KpiMeasurements => _kpiMeasurements ??= new Repository<KpiMeasurement>(_context);
        public IRepository<Target> Targets => _targets ??= new Repository<Target>(_context);
        public IRepository<Threshold> Thresholds => _thresholds ??= new Repository<Threshold>(_context);

        // Notification repositories
        public IRepository<KPISolution.Models.Entities.Notification.Notification> Notifications => _notifications ??= new Repository<KPISolution.Models.Entities.Notification.Notification>(_context);

        // Dashboard repositories
        public IRepository<CustomDashboard> CustomDashboards => _customDashboards ??= new Repository<CustomDashboard>(_context);
        public IRepository<DashboardItem> DashboardItems => _dashboardItems ??= new Repository<DashboardItem>(_context);

        /// <inheritdoc/>
        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            return (IRepository<T>)_repositories.GetOrAdd(typeof(T), type => new Repository<T>(_context));
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <inheritdoc/>
        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <inheritdoc/>
        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Disposes the UnitOfWork instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the UnitOfWork instance
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                    _transaction?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
