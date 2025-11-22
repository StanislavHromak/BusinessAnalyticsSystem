using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessAnalyticsSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIsGeneratedFromSalesToFinancialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGeneratedFromSales",
                table: "FinancialDatas",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGeneratedFromSales",
                table: "FinancialDatas");
        }
    }
}

