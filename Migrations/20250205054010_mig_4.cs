using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetApi.Migrations
{
    /// <inheritdoc />
    public partial class mig_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentDebt",
                schema: "BudgetManagement",
                table: "Customers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPayment",
                schema: "BudgetManagement",
                table: "Customers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentDebt",
                schema: "BudgetManagement",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TotalPayment",
                schema: "BudgetManagement",
                table: "Customers");
        }
    }
}
