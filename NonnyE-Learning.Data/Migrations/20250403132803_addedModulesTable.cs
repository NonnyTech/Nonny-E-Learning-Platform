using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NonnyE_Learning.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedModulesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    ModuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.ModuleId);
                    table.ForeignKey(
                        name: "FK_Modules_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleProgress",
                columns: table => new
                {
                    ModuleProgressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleProgress", x => x.ModuleProgressId);
                    table.ForeignKey(
                        name: "FK_ModuleProgress_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "ModuleId", "CourseContent", "CourseId", "Order", "Title" },
                values: new object[,]
                {
                    { 1, "We are one", 3, 1, "Introduction to C#" },
                    { 2, "We are one", 3, 2, "Getting ready for C#" },
                    { 3, "We are one", 3, 3, "The World of Variables and Operators" },
                    { 4, "We are one", 3, 4, "Arrays, Strings and Lists" },
                    { 5, "We are one", 3, 5, "Making our Program Interactive" },
                    { 6, "We are one", 3, 6, "Making Choices and Decisions" },
                    { 7, "We are one", 3, 7, "Object-Oriented Programming Part " },
                    { 8, "We are one", 3, 8, "Enum and Struct" },
                    { 9, "We are one", 3, 8, "Enum and Struct" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_ModuleId",
                table: "ModuleProgress",
                column: "ModuleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_CourseId",
                table: "Modules",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleProgress");

            migrationBuilder.DropTable(
                name: "Modules");
        }
    }
}
