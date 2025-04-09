using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPISolution.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddThreshold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AlertThreshold",
                table: "PerformanceIndicators",
                newName: "MinAlertThreshold");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxAlertThreshold",
                table: "PerformanceIndicators",
                type: "decimal(18,4)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxAlertThreshold",
                table: "PerformanceIndicators");

            migrationBuilder.RenameColumn(
                name: "MinAlertThreshold",
                table: "PerformanceIndicators",
                newName: "AlertThreshold");
        }
    }
}
