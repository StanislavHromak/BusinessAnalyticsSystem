using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessAnalyticsSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddKpiFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BreakEven",
                table: "FinancialDatas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Profit",
                table: "FinancialDatas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ProfitMargin",
                table: "FinancialDatas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ROI",
                table: "FinancialDatas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreakEven",
                table: "FinancialDatas");

            migrationBuilder.DropColumn(
                name: "Profit",
                table: "FinancialDatas");

            migrationBuilder.DropColumn(
                name: "ProfitMargin",
                table: "FinancialDatas");

            migrationBuilder.DropColumn(
                name: "ROI",
                table: "FinancialDatas");
        }
    }
}
