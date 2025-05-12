using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services.Interfaces
{
	public interface IModuleServices
	{
		Task<List<Module>> GetModulesByCourseIdAsync(int courseId);
		Task MarkModuleAsCompletedAsync(int moduleId, string studentId);
		Task<Module> GetModuleById(int moduleId);
		Task<ModuleProgress> GetModuleProgressAsync(int moduleId, string studentId);
		Task<bool> HasUserCompletedAllModules(int courseId, string studentId);
		public Task<ApplicationUser> GetStudentById(string studentId);
		Task<List<QuizQuestion>> GetQuizQuestionsByModuleIdAsync(int moduleId);
		Task<QuizQuestion> GetQuizQuestionByIdAsync(int quizQuestionId);

	}
}
