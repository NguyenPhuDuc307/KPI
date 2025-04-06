using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPISolution.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressUpdateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgressUpdates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuccessFactorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProgressPercentage = table.Column<int>(type: "int", nullable: false),
                    PreviousPercentage = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: true),
                    RiskLevel = table.Column<int>(type: "int", nullable: false),
                    PreviousRiskLevel = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Issues = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Actions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextSteps = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextUpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NeedsAttention = table.Column<bool>(type: "bit", nullable: false),
                    AttentionReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Achievements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsOnTrack = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressUpdates_SuccessFactors_SuccessFactorId",
                        column: x => x.SuccessFactorId,
                        principalTable: "SuccessFactors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgressUpdates_SuccessFactorId",
                table: "ProgressUpdates",
                column: "SuccessFactorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgressUpdates");
        }
    }
}
