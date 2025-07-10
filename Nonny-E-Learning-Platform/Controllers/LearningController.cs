using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using System.Reflection.Metadata;
using System.Security.Claims;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;

namespace Nonny_E_Learning_Platform.Controllers
{
	[Authorize]
	public class LearningController : BaseController
{
   
		private readonly ICourseServices _courseServices;
		private readonly IEnrollmentServices _enrollmentServices;
		private readonly IModuleServices _moduleServices;
		public LearningController(ICourseServices courseServices, IEnrollmentServices enrollmentServices, IModuleServices moduleServices)
		{
			_courseServices = courseServices;
			_enrollmentServices = enrollmentServices;
			_moduleServices = moduleServices;

		}
		public async Task<IActionResult> StartLearning(int courseId)
		{
			var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			// Create or get the enrollment for the student
			var enrollment = await _enrollmentServices.CreateOrGetEnrollmentAsync(courseId, studentId);

			if (enrollment == null)
				return RedirectToAction("Index", "Home");

			// Get the course and check if it's successful
			var courseResponse = await _courseServices.GetCourseById(courseId);
			if (!courseResponse.Success)
			{
				// If the course was not found or any error occurred
				return RedirectToAction("Index", "Home");
			}
			var course = courseResponse.Data;

			// Get all modules for the course
			var modules = await _moduleServices.GetModulesByCourseIdAsync(courseId);

			if (modules.Any())
			{
				foreach (var module in modules)
				{
					module.Course = course; // Set the course for each module
					module.ModuleProgress = await _moduleServices.GetModuleProgressAsync(module.ModuleId, studentId);
				}
			}

			// Check if the student has completed all modules
			bool isCourseCompleted = await _moduleServices.HasUserCompletedAllModules(courseId, studentId);

			// Prepare the view model
			var viewModel = new CourseLearningViewModel
			{
				Modules = modules,
				IsCourseCompleted = isCourseCompleted
			};

			return View(viewModel);
		}
		public async Task<IActionResult> CompleteModule(int moduleId)
		{
			var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			await _moduleServices.MarkModuleAsCompletedAsync(moduleId, studentId);

			return RedirectToAction("StartLearning", new { courseId = (await _moduleServices.GetModuleById(moduleId)).CourseId });
		}

		public async Task<IActionResult> TakeQuiz(int moduleId)
		{
			var questions = await _moduleServices.GetQuizQuestionsByModuleIdAsync(moduleId);

			var model = new QuizSubmissionViewModel
			{
				ModuleId = moduleId,
				Answers = questions.Select(q => new QuestionAnswer
				{
					QuizQuestionId = q.QuizQuestionId
				}).ToList()
			};

			ViewBag.Questions = questions;
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> SubmitQuiz(QuizSubmissionViewModel submission)
		{
			int correct = 0;

			foreach (var answer in submission.Answers)
			{
				var question = await _moduleServices.GetQuizQuestionByIdAsync(answer.QuizQuestionId);
				if (question != null && question.CorrectOption.Equals(answer.SelectedOption, StringComparison.OrdinalIgnoreCase))
				{
					correct++;
				}
			}

			int total = submission.Answers.Count;
			double score = (double)correct / total * 100;

			if (score >= 80)
			{
				var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				await _moduleServices.MarkModuleAsCompletedAsync(submission.ModuleId, studentId);
				SetSuccessMessage($"Quiz passed! You scored {score:0}%");
			}
			else
			{
				SetErrorMessage($"You scored {score:0}%. Minimum is 80%. Please retake the quiz.");
				return RedirectToAction("TakeQuiz", new { moduleId = submission.ModuleId });
			}

			var courseId = (await _moduleServices.GetModuleById(submission.ModuleId)).CourseId;
			return RedirectToAction("StartLearning", new { courseId });
		}
		public async Task<IActionResult> TakeTheQuiz()
		{
			return View();
		}

	}
}
