using Moq;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Data.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace Nonny_E_Learning.Test.UnitTesting
{

	public class CertificateServicesTests
	{

		private readonly Mock<IModuleServices> _moduleMock;
		private readonly Mock<ICourseServices> _courseMock;
		private readonly ApplicationDbContext _context;
		private readonly CertificateServices _service;

		public CertificateServicesTests()
		{
			_moduleMock = new Mock<IModuleServices>();
			_courseMock = new Mock<ICourseServices>();

			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
	.UseInMemoryDatabase(databaseName: "TestDb")
	.Options;

			_context = new ApplicationDbContext(options);

			_service = new CertificateServices(_context, _moduleMock.Object, _courseMock.Object);

		}

		[Fact]
		public async Task GenerateCertificateAsync_ReturnsSuccess_WhenAllValid()
		{
			string studentId = "stu123";
			int courseId = 1;

			_moduleMock.Setup(x => x.GetStudentById(studentId)).ReturnsAsync(new ApplicationUser
			{
				FirstName = "John",
				LastName = "Doe"
			});

			_moduleMock.Setup(x => x.HasUserCompletedAllModules(courseId, studentId)).ReturnsAsync(true);

			_courseMock.Setup(x => x.GetCourseById(courseId)).ReturnsAsync(new BaseResponse<Course>
			{
				Success = true,
				Data = new Course { Title = "C# Mastery" }
			});

			_courseMock.Setup(x => x.GetCourseCompletionDate(studentId, courseId)).ReturnsAsync(new BaseResponse<DateTime?>
			{
				Success = true,
				Data = new DateTime(2025, 5, 12)
			});

			var result = await _service.GenerateCertificateAsync(studentId, courseId);

			Assert.True(result.Success);
			Assert.NotNull(result.Data);
			Assert.EndsWith(".pdf", result.Data.FileName);
			Assert.NotEmpty(result.Data.FileBytes);
		}

		[Fact]
		public async Task GenerateCertificateAsync_ReturnsError_WhenStudentNotFound()
		{
			_moduleMock.Setup(x => x.GetStudentById(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

			var result = await _service.GenerateCertificateAsync("invalidId", 1);

			Assert.False(result.Success);
			Assert.Equal("Student not found", result.Message);
		}

		[Fact]
		public async Task GenerateCertificateAsync_ReturnsError_WhenCourseNotFound()
		{
			_moduleMock.Setup(x => x.GetStudentById(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

			_courseMock.Setup(x => x.GetCourseById(It.IsAny<int>())).ReturnsAsync(new BaseResponse<Course>
			{
				Success = false,
				Data = null
			});

			var result = await _service.GenerateCertificateAsync("stu123", 999);

			Assert.False(result.Success);
			Assert.Equal("Course not found", result.Message);
		}

		[Fact]
		public async Task GenerateCertificateAsync_ReturnsError_WhenModulesIncomplete()
		{
			_moduleMock.Setup(x => x.GetStudentById(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

			_courseMock.Setup(x => x.GetCourseById(It.IsAny<int>())).ReturnsAsync(new BaseResponse<Course>
			{
				Success = true,
				Data = new Course()
			});

			_moduleMock.Setup(x => x.HasUserCompletedAllModules(It.IsAny<int>(), It.IsAny<string>()))
					   .ReturnsAsync(false);

			var result = await _service.GenerateCertificateAsync("stu123", 1);

			Assert.False(result.Success);
			Assert.Equal("Certificate cannot be generated. The student has not completed all modules.", result.Message);
		}
	}
}
