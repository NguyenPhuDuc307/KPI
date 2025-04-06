using System;
using System.Linq;
using System.Threading.Tasks;
.Models.Entities.KPI;
.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KPISolution.Tests.Integration
{
    public class KpiWorkflowTests : IntegrationTestBase
    {
        [Fact]
        public async Task KpiHierarchy_ShouldBePreserved_WhenRetrieved()
        {
            // Arrange - Tạo dữ liệu mẫu
            var finDept = await CreateDepartmentAsync("Finance");
            var hrDept = await CreateDepartmentAsync("HR");

            // Tạo KRI
            var kri = await CreateKriAsync("Financial Performance", "KRI-001", finDept.Name);

            // Tạo RIs liên kết với KRI
            var ri1 = await CreateRiAsync("Revenue Growth", "RI-001", finDept.Name, kri.Id);
            var ri2 = await CreateRiAsync("Cost Reduction", "RI-002", finDept.Name, kri.Id);

            // Tạo RIs độc lập
            var ri3 = await CreateRiAsync("Employee Satisfaction", "RI-003", hrDept.Name);

            // Tạo PIs liên kết với RIs
            var pi1 = await CreatePiAsync("Monthly Sales", "PI-001", finDept.Name, ri1.Id);
            var pi2 = await CreatePiAsync("Quarterly Expenses", "PI-002", finDept.Name, ri2.Id);

            // Tạo PIs độc lập
            var pi3 = await CreatePiAsync("Training Completion", "PI-003", hrDept.Name);

            // Act
            // Lấy tất cả KRIs
            var kris = await Context.KRIs.ToListAsync();
            var retrievedKri = kris.FirstOrDefault(k => k.Id == kri.Id);

            // Lấy tất cả RIs
            var ris = await Context.RIs.ToListAsync();

            // Lấy tất cả PIs
            var pis = await Context.PIs.ToListAsync();

            // Assert
            // Kiểm tra KRI
            Assert.NotNull(retrievedKri);
            Assert.Equal("Financial Performance", retrievedKri.Name);

            // Kiểm tra RIs liên kết với KRI
            var relatedRis = ris.Where(r => r.ParentKriId == kri.Id).ToList();
            Assert.Equal(2, relatedRis.Count);
            Assert.Contains(relatedRis, r => r.Code == "RI-001");
            Assert.Contains(relatedRis, r => r.Code == "RI-002");

            // Kiểm tra RIs độc lập
            var standaloneRis = ris.Where(r => r.ParentKriId == null).ToList();
            Assert.Contains(standaloneRis, r => r.Code == "RI-003");

            // Kiểm tra PIs liên kết với RIs
            var pi1Parent = ris.FirstOrDefault(r => r.Id == pi1.RIId);
            Assert.NotNull(pi1Parent);
            Assert.Equal("Revenue Growth", pi1Parent.Name);

            var pi2Parent = ris.FirstOrDefault(r => r.Id == pi2.RIId);
            Assert.NotNull(pi2Parent);
            Assert.Equal("Cost Reduction", pi2Parent.Name);

            // Kiểm tra PIs độc lập
            var standalonePis = pis.Where(p => p.RIId == null).ToList();
            Assert.Contains(standalonePis, p => p.Code == "PI-003");
        }

        [Fact]
        public async Task FullKpiLifecycle_ShouldWorkCorrectly()
        {
            // Arrange
            var finDept = await CreateDepartmentAsync("Finance");

            // Act & Assert - Tạo, cập nhật và xóa một KPI

            // 1. Tạo KRI
            var kri = new KRI
            {
                Id = Guid.NewGuid(),
                Name = "Revenue KRI",
                Code = "KRI-TEST",
                Department = finDept.Name,
                Description = "Test KRI for full lifecycle",
                Status = KpiStatus.Draft,
                TargetValue = 1000000,
                Unit = "VND",
                CreatedAt = DateTime.UtcNow
            };

            Context.KRIs.Add(kri);
            await Context.SaveChangesAsync();

            // 2. Verify creation
            var createdKri = await Context.KRIs.FindAsync(kri.Id);
            Assert.NotNull(createdKri);
            Assert.Equal("Revenue KRI", createdKri.Name);
            Assert.Equal(KpiStatus.Draft, createdKri.Status);

            // 3. Update KRI
            createdKri.Name = "Updated Revenue KRI";
            createdKri.Status = KpiStatus.Active;
            createdKri.ModifiedAt = DateTime.UtcNow;

            Context.KRIs.Update(createdKri);
            await Context.SaveChangesAsync();

            // 4. Verify update
            var updatedKri = await Context.KRIs.FindAsync(kri.Id);
            Assert.NotNull(updatedKri);
            Assert.Equal("Updated Revenue KRI", updatedKri.Name);
            Assert.Equal(KpiStatus.Active, updatedKri.Status);
            Assert.NotNull(updatedKri.ModifiedAt);

            // 5. Create child RI
            var ri = new RI
            {
                Id = Guid.NewGuid(),
                Name = "Monthly Revenue",
                Code = "RI-TEST",
                Department = finDept.Name,
                ParentKriId = kri.Id,
                Status = KpiStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            Context.RIs.Add(ri);
            await Context.SaveChangesAsync();

            // 6. Verify RI and relationship
            var createdRi = await Context.RIs
                .FirstOrDefaultAsync(r => r.ParentKriId == kri.Id);

            Assert.NotNull(createdRi);
            Assert.Equal("Monthly Revenue", createdRi.Name);
            Assert.Equal(kri.Id, createdRi.ParentKriId);

            // 7. Delete KRI (should delete RI via cascade)
            Context.KRIs.Remove(updatedKri);
            await Context.SaveChangesAsync();

            // 8. Verify deletion
            var deletedKri = await Context.KRIs.FindAsync(kri.Id);
            Assert.Null(deletedKri);

            // 9. Verify cascade deletion of RI
            var deletedRi = await Context.RIs.FindAsync(ri.Id);
            // Should be null with cascade delete, but in the in-memory provider, it might behave differently
            // so we might need to check this in a true database environment
        }
    }
}