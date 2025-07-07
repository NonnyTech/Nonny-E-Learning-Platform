using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
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
	public class EnrollmentServiceTest
	{
		private readonly ApplicationDbContext _context;
		private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
		private readonly EnrollmentServices _enrollmentService;
		public EnrollmentServiceTest()
		{
			// Setup InMemory DbContext
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
				.Options;
			_context = new ApplicationDbContext(options);

			var store = new Mock<IUserStore<ApplicationUser>>();
			_userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			_enrollmentService = new EnrollmentServices(_context, _userManagerMock.Object);
		}
		[Fact]
		public async Task CreateOrGetEnrollmentAsync_Should_Create_Enrollment_When_Not_Exists()
		{
			// Arrange
			var courseId = 1;
			var studentId = "student123";
			var studentEmail = "john@example.com";

			var student = new ApplicationUser { Id = studentId, Email = studentEmail };
			_userManagerMock.Setup(x => x.FindByIdAsync(studentId))
							.ReturnsAsync(student);

			await _context.Courses.AddAsync(new Course { CourseId = courseId, Title = "C#" });
			await _context.SaveChangesAsync();

			// Act
			var result = await _enrollmentService.CreateOrGetEnrollmentAsync(courseId, studentId);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("Course enrollment created successfully.", result.Message);
			Assert.True(result.Data > 0);
		}

		[Fact]
		public async Task CreateOrGetEnrollmentAsync_Should_Return_Existing_Enrollment()
		{
			// Arrange
			var courseId = 1;
			var studentId = "student123";
			var existingEnrollment = new Enrollment
			{
				CourseId = courseId,
				StudentId = studentId,
				StudentEmail = "john@example.com",
				IsCompleted = false,
				EnrollmentDate = DateTime.UtcNow
			};

			await _context.Enrollements.AddAsync(existingEnrollment);
			await _context.SaveChangesAsync();

			// Act
			var result = await _enrollmentService.CreateOrGetEnrollmentAsync(courseId, studentId);

			// Assert
			Assert.True(result.Success);
			Assert.Equal(existingEnrollment.EnrollmentId, result.Data);
		}

		[Fact]
		public async Task CreateOrGetEnrollmentAsync_Should_Return_Error_If_Student_Not_Found()
		{
			// Arrange
			var courseId = 1;
			var studentId = "unknown-student";
			_userManagerMock.Setup(x => x.FindByIdAsync(studentId))
							.ReturnsAsync((ApplicationUser)null);

			// Act
			var result = await _enrollmentService.CreateOrGetEnrollmentAsync(courseId, studentId);

			// Assert
			Assert.False(result.Success);
			Assert.Contains("Student not found", result.Errors.First());
		}

		[Fact]
		public async Task GetEnrollmentsByStudentIdAsync_Should_Return_Enrollments()
		{
			// Arrange
			var studentId = "student123";
			var course = new Course { CourseId = 1, Title = "Test Course" };

			var enrollment = new Enrollment
			{
				StudentId = studentId,
				CourseId = course.CourseId,
				StudentEmail = "john@example.com",
				EnrollmentDate = DateTime.UtcNow,
				IsCompleted = false,
				Course = course
			};

			await _context.Courses.AddAsync(course);
			await _context.Enrollements.AddAsync(enrollment);
			await _context.SaveChangesAsync();

			// Act
			var result = await _enrollmentService.GetEnrollmentsByStudentIdAsync(studentId);

			// Assert
			Assert.Single(result);
			Assert.Equal(studentId, result.First().StudentId);
			Assert.Equal("Test Course", result.First().Course.Title);
		}
	}
}
