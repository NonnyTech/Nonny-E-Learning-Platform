using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NonnyE_Learning.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedAOrderColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "QuizQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "QuizQuestions");
        }
    }
}
