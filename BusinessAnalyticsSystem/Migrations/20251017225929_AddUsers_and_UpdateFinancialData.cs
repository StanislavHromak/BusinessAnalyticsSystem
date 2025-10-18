using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessAnalyticsSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers_and_UpdateFinancialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VariableCostsPerUnit",
                table: "FinancialDatas",
                newName: "VariableCostPerUnit");

            migrationBuilder.RenameColumn(
                name: "ProfitMargin",
                table: "FinancialDatas",
                newName: "TotalCosts");

            migrationBuilder.RenameColumn(
                name: "Expenses",
                table: "FinancialDatas",
                newName: "ROS");

            migrationBuilder.AddColumn<double>(
                name: "GrossCosts",
                table: "FinancialDatas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MarginPerUnit",
                table: "FinancialDatas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Role = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "GrossCosts",
                table: "FinancialDatas");

            migrationBuilder.DropColumn(
                name: "MarginPerUnit",
                table: "FinancialDatas");

            migrationBuilder.RenameColumn(
                name: "VariableCostPerUnit",
                table: "FinancialDatas",
                newName: "VariableCostsPerUnit");

            migrationBuilder.RenameColumn(
                name: "TotalCosts",
                table: "FinancialDatas",
                newName: "ProfitMargin");

            migrationBuilder.RenameColumn(
                name: "ROS",
                table: "FinancialDatas",
                newName: "Expenses");
        }
    }
}
