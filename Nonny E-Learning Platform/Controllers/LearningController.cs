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
	public class LearningController : Controller
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

	}
}
