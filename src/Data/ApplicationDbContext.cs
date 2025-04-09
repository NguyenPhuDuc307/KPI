using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace KPISolution.Data
{
    /// <summary>
    /// Application database context with Identity support and custom entities
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IndicatorRole, string>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Identity DbSets
        public new DbSet<ApplicationUser> Users => this.Set<ApplicationUser>();
        public new DbSet<IndicatorRole> Roles => this.Set<IndicatorRole>();

        // Organization DbSets
        public DbSet<Department> Departments => this.Set<Department>();
        public DbSet<Objective> Objectives => this.Set<Objective>();

        // Indicator DbSets
        public DbSet<SuccessFactor> SuccessFactors => this.Set<SuccessFactor>();
        public DbSet<ResultIndicator> ResultIndicators => this.Set<ResultIndicator>();
        public DbSet<PerformanceIndicator> PerformanceIndicators => this.Set<PerformanceIndicator>();
        public DbSet<Measurement> Measurements => this.Set<Measurement>();

        // Notification DbSets
        public DbSet<Notification> Notifications => this.Set<Notification>();

        // Dashboard DbSets
        public DbSet<CustomDashboard> CustomDashboards => this.Set<CustomDashboard>();
        public DbSet<DashboardItem> DashboardItems => this.Set<DashboardItem>();

        // Measurement DbSets
        public DbSet<Target> Targets => this.Set<Target>();
        public DbSet<Threshold> Thresholds => this.Set<Threshold>();

        // Progress
        public DbSet<ProgressUpdate> ProgressUpdates { get; set; } = null!;

        /// <summary>
        /// Configure the entity relationships
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the entity tables
            modelBuilder.Entity<SuccessFactor>().ToTable("SuccessFactors");
            modelBuilder.Entity<ResultIndicator>().ToTable("ResultIndicators");
            modelBuilder.Entity<PerformanceIndicator>().ToTable("PerformanceIndicators");
            modelBuilder.Entity<Measurement>().ToTable("Measurements");
            modelBuilder.Entity<Objective>().ToTable("Objectives");

            // Configure Department self-referencing relationship
            modelBuilder.Entity<Department>()
                .HasOne(d => d.ParentDepartment)
                .WithMany(d => d.ChildDepartments)
                .HasForeignKey(d => d.ParentDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Objective self-referencing relationship
            modelBuilder.Entity<Objective>()
                .HasOne(o => o.Parent)
                .WithMany(o => o.Children)
                .HasForeignKey(o => o.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure SuccessFactor recursive relationship
            modelBuilder.Entity<SuccessFactor>()
                .HasOne(sf => sf.Parent)
                .WithMany(sf => sf.Children)
                .HasForeignKey(sf => sf.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure SuccessFactor-Objective relationship
            modelBuilder.Entity<SuccessFactor>()
                .HasOne(sf => sf.Objective)
                .WithMany(o => o.SuccessFactors)
                .HasForeignKey(sf => sf.ObjectiveId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure SuccessFactor-ResultIndicator relationship
            modelBuilder.Entity<SuccessFactor>()
                .HasMany(sf => sf.ResultIndicators)
                .WithOne(ri => ri.SuccessFactor)
                .HasForeignKey(ri => ri.SuccessFactorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure PerformanceIndicator relationships
            modelBuilder.Entity<PerformanceIndicator>(entity =>
            {
                // Relationship with ResultIndicator (PI can belong to an RI)
                entity.HasOne(pi => pi.ResultIndicator)
                      .WithMany(ri => ri.PerformanceIndicators) // RI has many PIs
                      .HasForeignKey(pi => pi.ResultIndicatorId)
                      .IsRequired(false) // A PI doesn't *have* to belong to an RI
                      .OnDelete(DeleteBehavior.SetNull); // If RI is deleted, set PI's RI link to null

                // Direct relationship with SuccessFactor (PI can belong directly to an SF)
                entity.HasOne(pi => pi.SuccessFactor)
                      .WithMany(sf => sf.PerformanceIndicators) // SF can have many PIs directly
                      .HasForeignKey(pi => pi.SuccessFactorId)
                      .IsRequired(false) // A PI might link via RI instead, so direct link is not required
                      .OnDelete(DeleteBehavior.SetNull); // If SF is deleted, set PI's SF link to null 
            });

            // Configure ResultIndicator-PerformanceIndicator relationship
            modelBuilder.Entity<ResultIndicator>()
                .HasMany(ri => ri.PerformanceIndicators)
                .WithOne(pi => pi.ResultIndicator)
                .HasForeignKey(pi => pi.ResultIndicatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Measurement relationships
            modelBuilder.Entity<Measurement>()
                .HasOne(m => m.SuccessFactor)
                .WithMany(sf => sf.Measurements)
                .HasForeignKey(m => m.SuccessFactorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Measurement>()
                .HasOne(m => m.ResultIndicator)
                .WithMany(ri => ri.Measurements)
                .HasForeignKey(m => m.ResultIndicatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Measurement>()
                .HasOne(m => m.PerformanceIndicator)
                .WithMany(pi => pi.Measurements)
                .HasForeignKey(m => m.PerformanceIndicatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Measurement Type Discriminator
            modelBuilder.Entity<Measurement>()
                .HasDiscriminator<string>("MeasurementType")
                .HasValue<Measurement>("Generic");

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

            // Configure Dashboard relationships
            modelBuilder.Entity<DashboardItem>()
                .HasOne(i => i.Dashboard)
                .WithMany(d => d.DashboardItems)
                .HasForeignKey(i => i.DashboardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Target relationships with indicators
            modelBuilder.Entity<Target>()
                .HasOne(t => t.SuccessFactor)
                .WithMany()
                .HasForeignKey(t => t.SuccessFactorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Target>()
                .HasOne(t => t.ResultIndicator)
                .WithMany()
                .HasForeignKey(t => t.ResultIndicatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Target>()
                .HasOne(t => t.PerformanceIndicator)
                .WithMany()
                .HasForeignKey(t => t.PerformanceIndicatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Threshold relationships with indicators
            modelBuilder.Entity<Threshold>()
                .HasOne(t => t.SuccessFactor)
                .WithMany()
                .HasForeignKey(t => t.SuccessFactorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Threshold>()
                .HasOne(t => t.ResultIndicator)
                .WithMany()
                .HasForeignKey(t => t.ResultIndicatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Threshold>()
                .HasOne(t => t.PerformanceIndicator)
                .WithMany()
                .HasForeignKey(t => t.PerformanceIndicatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ProgressUpdate relationships
            modelBuilder.Entity<ProgressUpdate>().ToTable("ProgressUpdates");
            modelBuilder.Entity<ProgressUpdate>()
                .HasOne(p => p.SuccessFactor)
                .WithMany()
                .HasForeignKey(p => p.SuccessFactorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure IndicatorRole relationships
            modelBuilder.Entity<IndicatorRole>(entity =>
            {
                entity.HasOne(r => r.Department)
                    .WithMany()
                    .HasForeignKey(r => r.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
