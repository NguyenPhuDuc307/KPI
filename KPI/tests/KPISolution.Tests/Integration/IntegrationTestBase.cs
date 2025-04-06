using System;
using System.Threading.Tasks;
.Data;
.Models.Entities.KPI;
.Models.Entities.Organization;
.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KPISolution.Tests.Integration
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly ApplicationDbContext Context;
        protected readonly IServiceProvider ServiceProvider;

        protected IntegrationTestBase()
        {
            var services = new ServiceCollection();

            // Tạo một DB context sử dụng in-memory database cho testing
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase($"KPITestDB_{Guid.NewGuid()}"));

            // Đăng ký các dịch vụ khác nếu cần

            ServiceProvider = services.BuildServiceProvider();
            Context = ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Đảm bảo database được tạo
            Context.Database.EnsureCreated();
        }

        // Helper methods cho việc khởi tạo dữ liệu mẫu

        protected async Task<Department> CreateDepartmentAsync(string name)
        {
            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = $"Description for {name}",
                CreatedAt = DateTime.UtcNow
            };

            Context.Departments.Add(department);
            await Context.SaveChangesAsync();
            return department;
        }

        protected async Task<KRI> CreateKriAsync(string name, string code, string departmentName)
        {
            var kri = new KRI
            {
                Id = Guid.NewGuid(),
                Name = name,
                Code = code,
                Department = departmentName,
                Status = KpiStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            Context.KRIs.Add(kri);
            await Context.SaveChangesAsync();
            return kri;
        }

        protected async Task<RI> CreateRiAsync(string name, string code, string departmentName, Guid? parentKriId = null)
        {
            var ri = new RI
            {
                Id = Guid.NewGuid(),
                Name = name,
                Code = code,
                Department = departmentName,
                Status = KpiStatus.Active,
                ParentKriId = parentKriId,
                CreatedAt = DateTime.UtcNow
            };

            Context.RIs.Add(ri);
            await Context.SaveChangesAsync();
            return ri;
        }

        protected async Task<PI> CreatePiAsync(string name, string code, string departmentName, Guid? riId = null)
        {
            var pi = new PI
            {
                Id = Guid.NewGuid(),
                Name = name,
                Code = code,
                Department = departmentName,
                Status = KpiStatus.Active,
                RIId = riId,
                CreatedAt = DateTime.UtcNow
            };

            Context.PIs.Add(pi);
            await Context.SaveChangesAsync();
            return pi;
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}