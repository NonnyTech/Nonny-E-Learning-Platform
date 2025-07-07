using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonny_E_Learning.Test.UnitTesting
{
	public class CourseServiceTest
	{
		private async Task<ApplicationDbContext> GetInMemoryDbContextAsync()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			var dbContext = new ApplicationDbContext(options);

			// Seed test data if needed
			await dbContext.SaveChangesAsync();

			return dbContext;
		}

		[Fact]
		public async Task AddCourseAsync_Should_Add_Course()
		{
			// Arrange
			var dbContext = await GetInMemoryDbContextAsync();
			var courseService = new CourseService(dbContext);
			var newCourse = new Course
			{
				Title = "Intro to C#",
				Description = "C# Basics"
			};

			// Act
			var result = await courseService.AddCourseAsync(newCourse);

			// Assert
			Assert.True(result.Success);
			Assert.NotNull(result.Data);
			Assert.Equal("Intro to C#", result.Data.Title);
		}

		[Fact]
		public async Task GetAllCoursesAsync_Should_Return_Empty_When_No_Courses()
		{
			// Arrange
			var dbContext = await GetInMemoryDbContextAsync();
			var courseService = new CourseService(dbContext);

			// Act
			var result = await courseService.GetAllCoursesAsync();

			// Assert
			Assert.True(result.Success);
			Assert.Empty(result.Data);
		}

		[Fact]
		public async Task GetCourseById_Should_Return_Course_When_Exists()
		{
			// Arrange
			var dbContext = await GetInMemoryDbContextAsync();
			var course = new Course { Title = "Course X", Description = "Test" };
			dbContext.Courses.Add(course);
			await dbContext.SaveChangesAsync();

			var service = new CourseService(dbContext);

			// Act
			var result = await service.GetCourseById(course.CourseId);

			// Assert
			Assert.True(result.Success);
			Assert.Equal(course.Title, result.Data.Title);
		}

		[Fact]
		public async Task DeleteCourseAsync_Should_Delete_When_Course_Exists()
		{
			// Arrange
			var dbContext = await GetInMemoryDbContextAsync();
			var course = new Course { Title = "To Delete", Description = "Delete me" };
			dbContext.Courses.Add(course);
			await dbContext.SaveChangesAsync();

			var service = new CourseService(dbContext);

			// Act
			var result = await service.DeleteCourseAsync(course.CourseId);

			// Assert
			Assert.True(result.Success);
			Assert.True(result.Data);
			Assert.Null(await dbContext.Courses.FindAsync(course.CourseId));
		}

		[Fact]
		public async Task UpdateCourseAsync_Should_Update_Course()
		{
			// Arrange
			var dbContext = await GetInMemoryDbContextAsync();
			var course = new Course { Title = "Old Title", Description = "Old" };
			dbContext.Courses.Add(course);
			await dbContext.SaveChangesAsync();

			course.Title = "New Title";

			var service = new CourseService(dbContext);

			// Act
			var result = await service.UpdateCourseAsync(course);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("New Title", result.Data.Title);
		}

		[Fact]
		public async Task GetCourseCompletionDate_Should_Return_Latest_CompletedAt_When_Modules_Are_Completed()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			var studentId = "student123";
			var courseId = 1;

			var completedAt1 = new DateTime(2024, 1, 1);
			var completedAt2 = new DateTime(2024, 3, 15);

			using (var context = new ApplicationDbContext(options))
			{
				context.Courses.Add(new Course { CourseId = courseId, Title = "Test Course" });

				// Add Modules linked to the course
				var module1 = new Module { ModuleId = 1, CourseId = courseId, Title = "Module 1", CourseContent = "Content 1" };
				var module2 = new Module { ModuleId = 2, CourseId = courseId, Title = "Module 2", CourseContent = "Content 2" };
				context.Modules.AddRange(module1, module2);

				// Add student's progress on modules
				context.ModuleProgress.AddRange(
					new ModuleProgress
					{
						ModuleId = module1.ModuleId,
						StudentId = studentId,
						IsCompleted = true,
						CompletedAt = completedAt1
					},
					new ModuleProgress
					{
						ModuleId = module2.ModuleId,
						StudentId = studentId,
						IsCompleted = true,
						CompletedAt = completedAt2
					}
				);

				await context.SaveChangesAsync();
			}

			// Act
			using (var context = new ApplicationDbContext(options))
			{
				var service = new CourseService(context);
				var result = await service.GetCourseCompletionDate(studentId, courseId);

				// Assert
				Assert.True(result.Success);
				Assert.Equal(completedAt2, result.Data); // Should return the latest date
			}
		}

	}
}
