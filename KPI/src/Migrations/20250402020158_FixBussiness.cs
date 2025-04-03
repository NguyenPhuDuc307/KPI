using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPISolution.Migrations
{
    /// <inheritdoc />
    public partial class FixBussiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CriticalSuccessFactorId",
                table: "RIs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsKey",
                table: "RIs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "CriticalSuccessFactorId",
                table: "PIs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsKey",
                table: "PIs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SuccessFactorId",
                table: "CriticalSuccessFactors",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SuccessFactors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BusinessObjectiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgressPercentage = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessFactors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuccessFactors_BusinessObjectives_BusinessObjectiveId",
                        column: x => x.BusinessObjectiveId,
                        principalTable: "BusinessObjectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SuccessFactors_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RIs_CriticalSuccessFactorId",
                table: "RIs",
                column: "CriticalSuccessFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_PIs_CriticalSuccessFactorId",
                table: "PIs",
                column: "CriticalSuccessFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_CriticalSuccessFactors_SuccessFactorId",
                table: "CriticalSuccessFactors",
                column: "SuccessFactorId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessFactors_BusinessObjectiveId",
                table: "SuccessFactors",
                column: "BusinessObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessFactors_DepartmentId",
                table: "SuccessFactors",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CriticalSuccessFactors_SuccessFactors_SuccessFactorId",
                table: "CriticalSuccessFactors",
                column: "SuccessFactorId",
                principalTable: "SuccessFactors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PIs_CriticalSuccessFactors_CriticalSuccessFactorId",
                table: "PIs",
                column: "CriticalSuccessFactorId",
                principalTable: "CriticalSuccessFactors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RIs_CriticalSuccessFactors_CriticalSuccessFactorId",
                table: "RIs",
                column: "CriticalSuccessFactorId",
                principalTable: "CriticalSuccessFactors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CriticalSuccessFactors_SuccessFactors_SuccessFactorId",
                table: "CriticalSuccessFactors");

            migrationBuilder.DropForeignKey(
                name: "FK_PIs_CriticalSuccessFactors_CriticalSuccessFactorId",
                table: "PIs");

            migrationBuilder.DropForeignKey(
                name: "FK_RIs_CriticalSuccessFactors_CriticalSuccessFactorId",
                table: "RIs");

            migrationBuilder.DropTable(
                name: "SuccessFactors");

            migrationBuilder.DropIndex(
                name: "IX_RIs_CriticalSuccessFactorId",
                table: "RIs");

            migrationBuilder.DropIndex(
                name: "IX_PIs_CriticalSuccessFactorId",
                table: "PIs");

            migrationBuilder.DropIndex(
                name: "IX_CriticalSuccessFactors_SuccessFactorId",
                table: "CriticalSuccessFactors");

            migrationBuilder.DropColumn(
                name: "CriticalSuccessFactorId",
                table: "RIs");

            migrationBuilder.DropColumn(
                name: "IsKey",
                table: "RIs");

            migrationBuilder.DropColumn(
                name: "CriticalSuccessFactorId",
                table: "PIs");

            migrationBuilder.DropColumn(
                name: "IsKey",
                table: "PIs");

            migrationBuilder.DropColumn(
                name: "SuccessFactorId",
                table: "CriticalSuccessFactors");
        }
    }
}
