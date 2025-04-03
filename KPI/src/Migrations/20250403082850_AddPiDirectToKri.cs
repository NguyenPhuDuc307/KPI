using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPISolution.Migrations
{
    /// <inheritdoc />
    public partial class AddPiDirectToKri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "KRIId",
                table: "PIs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PIs_KRIId",
                table: "PIs",
                column: "KRIId");

            migrationBuilder.AddForeignKey(
                name: "FK_PIs_KRIs_KRIId",
                table: "PIs",
                column: "KRIId",
                principalTable: "KRIs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PIs_KRIs_KRIId",
                table: "PIs");

            migrationBuilder.DropIndex(
                name: "IX_PIs_KRIId",
                table: "PIs");

            migrationBuilder.DropColumn(
                name: "KRIId",
                table: "PIs");
        }
    }
}
