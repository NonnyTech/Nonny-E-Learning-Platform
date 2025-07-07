using Microsoft.AspNetCore.Identity;
using Moq;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Data.Enum;

namespace Nonny_E_Learning.Test.UnitTesting
{
	public class TransactionServicesTest
	{
		private readonly ApplicationDbContext _context;
		private readonly Mock<IFlutterwaveServices> _flutterwaveMock;
		private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
		private readonly TransactionServices _service;

		public TransactionServicesTest()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

			_context = new ApplicationDbContext(options);
			_flutterwaveMock = new Mock<IFlutterwaveServices>();

			var store = new Mock<IUserStore<ApplicationUser>>();
			_userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

			_service = new TransactionServices(_userManagerMock.Object, _context, _flutterwaveMock.Object);


		}
		[Fact]
		public async Task CreateTransaction_Should_Return_Failure_When_Enrollment_Not_Found()
		{
			// Act
			var result = await _service.CreateTransaction(999, "student1", 1000);

			// Assert
			Assert.False(result.Success);
			Assert.Contains("Enrollment not found", result.Message);
		}

		[Fact]
		public async Task CreateTransaction_Should_Create_Transaction_And_Return_PaymentLink()
		{
			// Arrange
			var student = new ApplicationUser { Id = "student1", Email = "student@example.com", FirstName = "John", LastName = "Doe" };
			var course = new Course { CourseId = 1, Title = "C# Course" };
			var enrollment = new Enrollment
			{
				EnrollmentId = 1,
				CourseId = 1,
				Course = course,
				StudentEmail = student.Email,
				StudentId = student.Id,
				Student = student,
				EnrollmentDate = DateTime.UtcNow

			};

			await _context.Users.AddAsync(student);
			await _context.Courses.AddAsync(course);
			await _context.Enrollements.AddAsync(enrollment);
			await _context.SaveChangesAsync();

			_flutterwaveMock.Setup(f => f.GenerateFlutterwavePaymentLink(It.IsAny<Transaction>()))
				.ReturnsAsync("http://payment.link");

			// Act
			var result = await _service.CreateTransaction(1, "student1", 5000);

			// Assert
			Assert.True(result.Success);
			Assert.Equal("http://payment.link", result.Data);
		}

		[Fact]
		public async Task GetTransactionByReferenceAsync_Should_Return_Transaction_With_Details()
		{
			// Arrange
			var student = new ApplicationUser
			{
				Id = "student-id-123",
				FirstName = "John",
				LastName = "Doe",
				Email = "student@email.com"
			};
			var course = new Course
			{
				CourseId = 1,
				Title = "Test Course"
			};
			var enrollment = new Enrollment
			{
				EnrollmentId = 1,
				CourseId = 1,
				StudentId = student.Id,
				Student = student,
				Course = course,
				StudentEmail = student.Email
			};
			var transaction = new Transaction
			{
				Enrollment = enrollment,
				EnrollmentId = enrollment.EnrollmentId,
				Amount = 5000,
				StudentId = student.Id,
				StudentEmail = student.Email,
				StudentName = "John Doe",
				TransactionStatus = TransactionStatus.Pending,
				Reference = "ref12345"
			};

			await _context.Users.AddAsync(student);
			await _context.Courses.AddAsync(course);
			await _context.Enrollements.AddAsync(enrollment);
			await _context.Transactions.AddAsync(transaction);
			await _context.SaveChangesAsync();

			// Act
			var result = await _service.GetTransactionByReferenceAsync("ref12345");

			// Assert
			Assert.NotNull(result);
			Assert.Equal("ref12345", result.Reference);
			Assert.Equal("Test Course", result.Enrollment.Course.Title);
		}

		[Fact]
		public async Task UpdateTransactionStatusAsync_Should_Set_Status_To_Completed()
		{
			// Arrange
			var transaction = new Transaction
			{
				Reference = "TX999",
				TransactionStatus = TransactionStatus.Pending,
				StudentId = "student-id-001",
				StudentEmail = "student@email.com",
				StudentName = "Jane Doe",
				EnrollmentId = 1 
			};
			await _context.Transactions.AddAsync(transaction);
			await _context.SaveChangesAsync();

			// Act
			await _service.UpdateTransactionStatusAsync("TX999", "flutter-ref-001", "successful");

			// Assert
			var updated = await _context.Transactions.FirstOrDefaultAsync(t => t.Reference == "TX999");
			Assert.Equal(TransactionStatus.Completed, updated.TransactionStatus);
			Assert.Equal("flutter-ref-001", updated.FlutterReference);
		}
	}
}
