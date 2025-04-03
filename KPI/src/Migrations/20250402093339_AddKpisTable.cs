using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPISolution.Migrations
{
    /// <inheritdoc />
    public partial class AddKpisTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KPIs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformanceDomain = table.Column<int>(type: "int", nullable: false),
                    CategoryLevel = table.Column<int>(type: "int", nullable: false),
                    FocusArea = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImprovementTarget = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BenchmarkValue = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    OwnerDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KPIs_Departments_OwnerDepartmentId",
                        column: x => x.OwnerDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KPIs_KpiBase_Id",
                        column: x => x.Id,
                        principalTable: "KpiBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KPIs_OwnerDepartmentId",
                table: "KPIs",
                column: "OwnerDepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KPIs");
        }
    }
}
