using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessAnalyticsSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Revenue = table.Column<double>(type: "REAL", nullable: false),
                    Expenses = table.Column<double>(type: "REAL", nullable: false),
                    Investment = table.Column<double>(type: "REAL", nullable: false),
                    FixedCosts = table.Column<double>(type: "REAL", nullable: false),
                    VariableCostsPerUnit = table.Column<double>(type: "REAL", nullable: false),
                    PricePerUnit = table.Column<double>(type: "REAL", nullable: false),
                    UnitsSold = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialDatas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialDatas");
        }
    }
}
