using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class ModuleServicesTests
{
	private readonly ApplicationDbContext _context;
	private readonly ModuleServices _service;

	public ModuleServicesTests()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.EnableSensitiveDataLogging()
			.Options;

		_context = new ApplicationDbContext(options);
		_service = new ModuleServices(_context);
	}

	[Fact]
	public async Task GetModulesByCourseIdAsync_ReturnsOrderedModules()
	{
		int courseId = 1;
		_context.Modules.AddRange(
			new Module { ModuleId = 1, CourseId = courseId, Order = 2, Title = "B", CourseContent = "Content B" },
			new Module { ModuleId = 2, CourseId = courseId, Order = 1, Title = "A", CourseContent = "Content A" }
		);
		await _context.SaveChangesAsync();

		var modules = await _service.GetModulesByCourseIdAsync(courseId);

		Assert.Equal(2, modules.Count);
		Assert.Equal(2, modules[0].ModuleId); // Ordered by Order
	}

	[Fact]
	public async Task MarkModuleAsCompletedAsync_AddsNewProgress_WhenNoneExists()
	{
		var module = new Module { ModuleId = 1, CourseId = 1, Order = 1, Title = "Module", CourseContent = "Content" };
		_context.Modules.Add(module);
		await _context.SaveChangesAsync();

		string studentId = "student123";

		await _service.MarkModuleAsCompletedAsync(module.ModuleId, studentId);

		var progress = await _context.ModuleProgress.FirstOrDefaultAsync();
		Assert.NotNull(progress);
		Assert.True(progress.IsCompleted);
		Assert.Equal(studentId, progress.StudentId);
	}

	[Fact]
	public async Task MarkModuleAsCompletedAsync_UpdatesProgress_WhenAlreadyExists()
	{
		var module = new Module { ModuleId = 1, CourseId = 1, Order = 1, Title = "Test", CourseContent = "Test" };
		_context.Modules.Add(module);
		await _context.SaveChangesAsync();

		var existingProgress = new ModuleProgress
		{
			ModuleId = module.ModuleId,
			StudentId = "student1",
			IsCompleted = false
		};
		_context.ModuleProgress.Add(existingProgress);
		await _context.SaveChangesAsync();

		await _service.MarkModuleAsCompletedAsync(module.ModuleId, "student1");

		var progress = await _context.ModuleProgress.FirstOrDefaultAsync();
		Assert.True(progress.IsCompleted);
	}

	[Fact]
	public async Task GetModuleById_ReturnsCorrectModule()
	{
		var module = new Module { ModuleId = 10, CourseId = 1, Title = "Module 10", CourseContent = "Test", Order = 1 };
		_context.Modules.Add(module);
		await _context.SaveChangesAsync();

		var result = await _service.GetModuleById(10);

		Assert.NotNull(result);
		Assert.Equal("Module 10", result.Title);
	}

	[Fact]
	public async Task GetModuleProgressAsync_ReturnsCorrectProgress()
	{
		var progress = new ModuleProgress
		{
			ModuleId = 1,
			StudentId = "studentX",
			IsCompleted = true
		};
		_context.ModuleProgress.Add(progress);
		await _context.SaveChangesAsync();

		var result = await _service.GetModuleProgressAsync(1, "studentX");

		Assert.NotNull(result);
		Assert.True(result.IsCompleted);
	}

	[Fact]
	public async Task HasUserCompletedAllModules_ReturnsTrue_IfAllCompleted()
	{
		var modules = new[]
		{
			new Module { ModuleId = 1, CourseId = 1, Title = "M1", CourseContent = "C1", Order = 1 },
			new Module { ModuleId = 2, CourseId = 1, Title = "M2", CourseContent = "C2", Order = 2 }
		};
		_context.Modules.AddRange(modules);

		var progressList = new[]
		{
			new ModuleProgress { ModuleId = 1, StudentId = "studentA", IsCompleted = true },
			new ModuleProgress { ModuleId = 2, StudentId = "studentA", IsCompleted = true }
		};
		_context.ModuleProgress.AddRange(progressList);

		await _context.SaveChangesAsync();

		var result = await _service.HasUserCompletedAllModules(1, "studentA");

		Assert.True(result);
	}

	[Fact]
	public async Task GetStudentById_ReturnsCorrectStudent()
	{
		_context.Users.Add(new ApplicationUser { Id = "stu001", FirstName = "John", LastName = "Doe" });
		await _context.SaveChangesAsync();

		var student = await _service.GetStudentById("stu001");

		Assert.NotNull(student);
		Assert.Equal("John", student.FirstName);
	}

	[Fact]
	public async Task GetQuizQuestionsByModuleIdAsync_ReturnsOrderedQuestions()
	{
		var questions = new[]
		{
		new QuizQuestion
		{
			QuizQuestionId = 1,
			ModuleId = 1,
			QuestionText = "Q1",
			Order = 2,
			OptionA = "A1",
			OptionB = "B1",
			OptionC = "C1",
			OptionD = "D1",
			CorrectOption = "A1"
		},
		new QuizQuestion
		{
			QuizQuestionId = 2,
			ModuleId = 1,
			QuestionText = "Q2",
			Order = 1,
			OptionA = "A2",
			OptionB = "B2",
			OptionC = "C2",
			OptionD = "D2",
			CorrectOption = "B2"
		}
	};

		_context.QuizQuestions.AddRange(questions);
		await _context.SaveChangesAsync();

		var result = await _service.GetQuizQuestionsByModuleIdAsync(1);

		Assert.Equal(2, result.Count);
		Assert.Equal("Q2", result[0].QuestionText); // Should be ordered by Order
	}
	[Fact]
	public async Task GetQuizQuestionByIdAsync_ReturnsCorrectQuestion()
	{
		_context.QuizQuestions.Add(new QuizQuestion
		{
			QuizQuestionId = 10,
			ModuleId = 1,
			QuestionText = "Quiz Q",
			OptionA = "Option A",
			OptionB = "Option B",
			OptionC = "Option C",
			OptionD = "Option D",
			CorrectOption = "Option A"
		});

		await _context.SaveChangesAsync();

		var question = await _service.GetQuizQuestionByIdAsync(10);

		Assert.NotNull(question);
		Assert.Equal("Quiz Q", question.QuestionText);
	}
}