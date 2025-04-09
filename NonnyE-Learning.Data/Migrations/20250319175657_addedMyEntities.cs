using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NonnyE_Learning.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedMyEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollements_AspNetUsers_UserId",
                table: "Enrollements");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Enrollements",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "IsPaid",
                table: "Enrollements",
                newName: "IsCompleted");

            migrationBuilder.RenameColumn(
                name: "EnrollementId",
                table: "Enrollements",
                newName: "EnrollmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollements_UserId",
                table: "Enrollements",
                newName: "IX_Enrollements_StudentId");

            migrationBuilder.RenameColumn(
                name: "ContentPath",
                table: "Courses",
                newName: "Instructor");

            migrationBuilder.AddColumn<DateTime>(
                name: "EnrollmentDate",
                table: "Enrollements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StudentEmail",
                table: "Enrollements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Lectures",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlutterReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId");
                    table.ForeignKey(
                        name: "FK_Transactions_Enrollements_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollements",
                        principalColumn: "EnrollmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "Category", "Description", "Duration", "ImageUrl", "Instructor", "Lectures", "Price", "Title" },
                values: new object[,]
                {
                    { 1, "Web Development", "Build powerful web applications using ASP.NET Core MVC.", "12 weeks", "/upload/xcourse_01.png.pagespeed.ic.XTOvCuUmZu.png", "Anthony Ikemefuna", 23, 400000m, "ASP.NET Core MVC" },
                    { 2, "Design", "Master graphic design using Adobe Photoshop and Illustrator.", "18 weeks", "/upload/xcourse_02.png.pagespeed.ic.PL7Wu2UcSB.png", "Rita Ikemefuna", 23, 250000m, "Graphics Design with Corel draw." },
                    { 3, "Programming", "Learn the fundamentals of C# programming.", "20 weeks", "/upload/xcourse_03.png.pagespeed.ic.8e1MyY5M7i.png", "Stanley Nonso", 23, 300000m, "C# for Beginners" },
                    { 4, "Backend Development", "Learn how to build scalable Web APIs using ASP.NET Core.", "24 weeks", "/upload/xcourse_04.png.pagespeed.ic.2rIKmUwjA7.png", "Anthony Ikemefuna", 23, 500000m, "Building Web APIs with .NET" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CourseId",
                table: "Transactions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_EnrollmentId",
                table: "Transactions",
                column: "EnrollmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollements_AspNetUsers_StudentId",
                table: "Enrollements",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollements_AspNetUsers_StudentId",
                table: "Enrollements");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "EnrollmentDate",
                table: "Enrollements");

            migrationBuilder.DropColumn(
                name: "StudentEmail",
                table: "Enrollements");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Lectures",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Enrollements",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "Enrollements",
                newName: "IsPaid");

            migrationBuilder.RenameColumn(
                name: "EnrollmentId",
                table: "Enrollements",
                newName: "EnrollementId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollements_StudentId",
                table: "Enrollements",
                newName: "IX_Enrollements_UserId");

            migrationBuilder.RenameColumn(
                name: "Instructor",
                table: "Courses",
                newName: "ContentPath");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollements_AspNetUsers_UserId",
                table: "Enrollements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
