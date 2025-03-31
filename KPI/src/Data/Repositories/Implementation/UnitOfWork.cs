using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.Measurement;
using KPISolution.Models.Entities.Notification;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Entities.Dashboard;

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

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            CriticalSuccessFactors = new Repository<CriticalSuccessFactor>(_context);
            CSFProgresses = new Repository<CSFProgress>(_context);
            CSFKPIs = new Repository<CSFKPI>(_context);
            KRIs = new Repository<KRI>(_context);
            PIs = new Repository<PI>(_context);
            RIs = new Repository<RI>(_context);
            KpiValues = new Repository<KpiValue>(_context);
            KpiMeasurements = new Repository<KpiMeasurement>(_context);
            Targets = new Repository<Target>(_context);
            Thresholds = new Repository<Threshold>(_context);
            Departments = new Repository<Department>(_context);
            BusinessObjectives = new Repository<BusinessObjective>(_context);
            Notifications = new Repository<KPISolution.Models.Entities.Notification.Notification>(_context);

            // Dashboard repositories
            CustomDashboards = new Repository<CustomDashboard>(_context);
            DashboardItems = new Repository<DashboardItem>(_context);
        }

        public IRepository<CriticalSuccessFactor> CriticalSuccessFactors { get; private set; }
        public IRepository<CSFProgress> CSFProgresses { get; private set; }
        public IRepository<CSFKPI> CSFKPIs { get; private set; }
        public IRepository<KRI> KRIs { get; private set; }
        public IRepository<PI> PIs { get; private set; }
        public IRepository<RI> RIs { get; private set; }
        public IRepository<KpiValue> KpiValues { get; private set; }
        public IRepository<KpiMeasurement> KpiMeasurements { get; private set; }
        public IRepository<Target> Targets { get; private set; }
        public IRepository<Threshold> Thresholds { get; private set; }
        public IRepository<Department> Departments { get; private set; }
        public IRepository<BusinessObjective> BusinessObjectives { get; private set; }
        public IRepository<KPISolution.Models.Entities.Notification.Notification> Notifications { get; private set; }

        // Dashboard repositories
        public IRepository<CustomDashboard> CustomDashboards { get; private set; }
        public IRepository<DashboardItem> DashboardItems { get; private set; }

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
            catch
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        /// <inheritdoc/>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null;
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
            if (!_disposed && disposing)
            {
                _context.Dispose();
                _transaction?.Dispose();
            }
            _disposed = true;
        }
    }
}
