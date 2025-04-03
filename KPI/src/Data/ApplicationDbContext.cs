using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Entities.Identity;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.Measurement;
using KPISolution.Models.Entities.Objective;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Entities.Dashboard;

namespace KPISolution.Data
{
    /// <summary>
    /// Application database context with Identity support and custom entities
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, KpiRole, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Identity DbSets
        public new DbSet<ApplicationUser> Users => Set<ApplicationUser>();
        public new DbSet<KpiRole> Roles => Set<KpiRole>();

        // Objective and Success Factor DbSets
        public DbSet<SuccessFactor> SuccessFactors => Set<SuccessFactor>();

        // CSF DbSets
        public DbSet<CriticalSuccessFactor> CriticalSuccessFactors => Set<CriticalSuccessFactor>();
        public DbSet<CSFProgress> CSFProgresses => Set<CSFProgress>();
        public DbSet<CSFKPI> CSFKPIs => Set<CSFKPI>();

        // KPI DbSets
        public DbSet<KpiBase> KpiBase => Set<KpiBase>();
        public DbSet<KRI> KRIs => Set<KRI>();
        public DbSet<PI> PIs => Set<PI>();
        public DbSet<RI> RIs => Set<RI>();
        public DbSet<Models.Entities.KPI.KPI> KPIs => Set<Models.Entities.KPI.KPI>();
        public DbSet<KpiMeasurement> KpiMeasurements => Set<KpiMeasurement>();

        // Measurement DbSets
        public DbSet<Models.Entities.KPI.KpiValue> KpiValues => Set<Models.Entities.KPI.KpiValue>();
        public DbSet<Target> Targets => Set<Target>();
        public DbSet<Threshold> Thresholds => Set<Threshold>();
        public DbSet<Models.Entities.Measurement.Notification> MeasurementNotifications => Set<Models.Entities.Measurement.Notification>();

        // Organization DbSets
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<BusinessObjective> BusinessObjectives => Set<BusinessObjective>();

        // Notification DbSets
        public DbSet<Models.Entities.Notification.Notification> Notifications => Set<Models.Entities.Notification.Notification>();

        // Dashboard DbSets
        public DbSet<CustomDashboard> CustomDashboards => Set<CustomDashboard>();
        public DbSet<DashboardItem> DashboardItems => Set<DashboardItem>();

        /// <summary>
        /// Configure the entity relationships
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Department self-referencing relationship
            modelBuilder.Entity<Department>()
                .HasOne(d => d.ParentDepartment)
                .WithMany(d => d.ChildDepartments)
                .HasForeignKey(d => d.ParentDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure BusinessObjective self-referencing relationship
            modelBuilder.Entity<BusinessObjective>()
                .HasOne(b => b.ParentObjective)
                .WithMany(b => b.ChildObjectives)
                .HasForeignKey(b => b.ParentObjectiveId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure BusinessObjective-SuccessFactor relationship
            modelBuilder.Entity<BusinessObjective>()
                .HasMany(b => b.SuccessFactors)
                .WithOne(sf => sf.BusinessObjective)
                .HasForeignKey(sf => sf.BusinessObjectiveId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure SuccessFactor-CriticalSuccessFactor relationship
            modelBuilder.Entity<SuccessFactor>()
                .HasMany(sf => sf.CriticalSuccessFactors)
                .WithOne(csf => csf.SuccessFactor)
                .HasForeignKey(csf => csf.SuccessFactorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure CriticalSuccessFactor-RI relationship
            modelBuilder.Entity<CriticalSuccessFactor>()
                .HasMany(csf => csf.ResultIndicators)
                .WithOne(ri => ri.CriticalSuccessFactor)
                .HasForeignKey(ri => ri.CriticalSuccessFactorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure CriticalSuccessFactor-PI relationship
            modelBuilder.Entity<CriticalSuccessFactor>()
                .HasMany(csf => csf.PerformanceIndicators)
                .WithOne(pi => pi.CriticalSuccessFactor)
                .HasForeignKey(pi => pi.CriticalSuccessFactorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure KPI hierarchy relationships
            modelBuilder.Entity<KRI>().ToTable("KRIs");
            modelBuilder.Entity<RI>().ToTable("RIs");
            modelBuilder.Entity<PI>().ToTable("PIs");
            modelBuilder.Entity<Models.Entities.KPI.KPI>().ToTable("KPIs");

            // Configure KpiValue relationships to avoid multiple cascade paths
            modelBuilder.Entity<Models.Entities.KPI.KpiValue>()
                .HasOne(kv => kv.Kpi)
                .WithMany(k => k.Values)
                .HasForeignKey(kv => kv.KpiId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Target relationships to avoid multiple cascade paths
            modelBuilder.Entity<Target>()
                .HasOne<KRI>()
                .WithMany()
                .HasForeignKey(t => t.KpiId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Target>()
                .HasOne<RI>()
                .WithMany()
                .HasForeignKey(t => t.KpiId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Target>()
                .HasOne<PI>()
                .WithMany()
                .HasForeignKey(t => t.KpiId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Threshold relationships to avoid multiple cascade paths
            modelBuilder.Entity<Threshold>()
                .HasOne<KRI>()
                .WithMany()
                .HasForeignKey(t => t.KpiId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Threshold>()
                .HasOne<RI>()
                .WithMany()
                .HasForeignKey(t => t.KpiId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Threshold>()
                .HasOne<PI>()
                .WithMany()
                .HasForeignKey(t => t.KpiId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure CSF-KPI relationship
            modelBuilder.Entity<CSFKPI>()
                .HasKey(ck => new { ck.CsfId, ck.KpiId });

            modelBuilder.Entity<CSFKPI>()
                .HasOne(ck => ck.CSF)
                .WithMany(c => c.CSFKPIs)
                .HasForeignKey(ck => ck.CsfId);

            modelBuilder.Entity<CSFKPI>()
                .HasOne(ck => ck.KPI)
                .WithMany()
                .HasForeignKey(ck => ck.KpiId)
                .IsRequired();

            // Configure ApplicationUser relationships
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Manager)
                .WithMany(u => u.DirectReports)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure KpiRole relationship with Department
            modelBuilder.Entity<KpiRole>()
                .HasOne(r => r.Department)
                .WithMany()
                .HasForeignKey(r => r.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Dashboard relationships
            modelBuilder.Entity<DashboardItem>()
                .HasOne(i => i.Dashboard)
                .WithMany(d => d.DashboardItems)
                .HasForeignKey(i => i.DashboardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
