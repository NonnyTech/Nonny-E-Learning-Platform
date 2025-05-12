using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services
{
	public class ModuleServices : IModuleServices
	{
		private readonly ApplicationDbContext _context;

		public ModuleServices(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Module>> GetModulesByCourseIdAsync(int courseId)
		{
			 return await _context.Modules
					   .Where(m => m.CourseId == courseId)
					   .OrderBy(m => m.Order) // Ensure modules are in order
					   .ToListAsync();
			
		}

		public async Task MarkModuleAsCompletedAsync(int moduleId, string studentId)
		{
			var progress = await _context.ModuleProgress
						.FirstOrDefaultAsync(mp => mp.ModuleId == moduleId && mp.StudentId == studentId);

			if (progress == null)
			{
				progress = new ModuleProgress
				{
					ModuleId = moduleId,
					StudentId = studentId,
					IsCompleted = true,
					CompletedAt = DateTime.UtcNow
				};

				_context.ModuleProgress.Add(progress);
			}
			else
			{
				progress.IsCompleted = true;
				progress.CompletedAt = DateTime.UtcNow;
				_context.ModuleProgress.Update(progress);
			}

			await _context.SaveChangesAsync();
		}

		public async Task<Module> GetModuleById(int moduleId)
		{
			return await _context.Modules.FirstOrDefaultAsync(m => m.ModuleId == moduleId);
		}

		public async Task<ModuleProgress> GetModuleProgressAsync(int moduleId, string studentId)
		{
			return await _context.ModuleProgress
								 .FirstOrDefaultAsync(mp => mp.ModuleId == moduleId && mp.StudentId == studentId);
		}

		public async Task<bool> HasUserCompletedAllModules(int courseId, string studentId)
		{
			var modules = await _context.Modules
				.Where(m => m.CourseId == courseId)
				.Select(m => m.ModuleId)
				.ToListAsync();

			var completedModules = await _context.ModuleProgress
				.Where(mp => modules.Contains(mp.ModuleId) && mp.StudentId == studentId && mp.IsCompleted)
				.Select(mp => mp.ModuleId)
				.ToListAsync();

			return modules.All(m => completedModules.Contains(m));
		}

		public async Task<ApplicationUser> GetStudentById(string studentId)
		{
			return await _context.Users.FirstOrDefaultAsync(a => a.Id == studentId);
		}

		public async Task<List<QuizQuestion>> GetQuizQuestionsByModuleIdAsync(int moduleId)
		{
			return await _context.QuizQuestions
									 .Where(q => q.ModuleId == moduleId)
									 .OrderBy(q => q.Order)
									 .ToListAsync();
		}

		public async Task<QuizQuestion> GetQuizQuestionByIdAsync(int quizQuestionId)
		{
			return await _context.QuizQuestions
									 .FirstOrDefaultAsync(q => q.QuizQuestionId == quizQuestionId);
		}
	}
	}

