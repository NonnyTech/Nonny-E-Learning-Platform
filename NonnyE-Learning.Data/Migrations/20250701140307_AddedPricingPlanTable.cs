using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NonnyE_Learning.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPricingPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PricingPlanId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PricingPlans",
                columns: table => new
                {
                    PricingPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingPlans", x => x.PricingPlanId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PricingPlanId",
                table: "Transactions",
                column: "PricingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_PricingPlans_PricingPlanId",
                table: "Transactions",
                column: "PricingPlanId",
                principalTable: "PricingPlans",
                principalColumn: "PricingPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_PricingPlans_PricingPlanId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "PricingPlans");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PricingPlanId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PricingPlanId",
                table: "Transactions");
        }
    }
}
