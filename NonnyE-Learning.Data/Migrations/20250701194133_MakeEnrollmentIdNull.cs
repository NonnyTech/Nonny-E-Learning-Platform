using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NonnyE_Learning.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeEnrollmentIdNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Enrollements_EnrollmentId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "EnrollmentId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Enrollements_EnrollmentId",
                table: "Transactions",
                column: "EnrollmentId",
                principalTable: "Enrollements",
                principalColumn: "EnrollmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Enrollements_EnrollmentId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "EnrollmentId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Enrollements_EnrollmentId",
                table: "Transactions",
                column: "EnrollmentId",
                principalTable: "Enrollements",
                principalColumn: "EnrollmentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
