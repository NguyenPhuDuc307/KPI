using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPISolution.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultIndicators_AspNetUsers_ResponsibleManagerId",
                table: "ResultIndicators");

            migrationBuilder.DropIndex(
                name: "IX_ResultIndicators_ResponsibleManagerId",
                table: "ResultIndicators");

            migrationBuilder.DropColumn(
                name: "MaxAlertThreshold",
                table: "ResultIndicators");

            migrationBuilder.DropColumn(
                name: "MinAlertThreshold",
                table: "ResultIndicators");

            migrationBuilder.DropColumn(
                name: "ResponsibleManagerId",
                table: "ResultIndicators");

            migrationBuilder.RenameColumn(
                name: "Direction",
                table: "ResultIndicators",
                newName: "MeasurementDirection");

            migrationBuilder.RenameColumn(
                name: "ContributionPercentage",
                table: "ResultIndicators",
                newName: "Weight");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "ResultIndicators",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "TimeFrame",
                table: "ResultIndicators",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResultType",
                table: "ResultIndicators",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MeasurementScope",
                table: "ResultIndicators",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataSource",
                table: "ResultIndicators",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "ResultIndicators",
                newName: "ContributionPercentage");

            migrationBuilder.RenameColumn(
                name: "MeasurementDirection",
                table: "ResultIndicators",
                newName: "Direction");

            migrationBuilder.AlterColumn<int>(
                name: "Unit",
                table: "ResultIndicators",
                type: "int",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "TimeFrame",
                table: "ResultIndicators",
                type: "int",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ResultType",
                table: "ResultIndicators",
                type: "int",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MeasurementScope",
                table: "ResultIndicators",
                type: "int",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DataSource",
                table: "ResultIndicators",
                type: "int",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxAlertThreshold",
                table: "ResultIndicators",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinAlertThreshold",
                table: "ResultIndicators",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsibleManagerId",
                table: "ResultIndicators",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultIndicators_ResponsibleManagerId",
                table: "ResultIndicators",
                column: "ResponsibleManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultIndicators_AspNetUsers_ResponsibleManagerId",
                table: "ResultIndicators",
                column: "ResponsibleManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
