using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using System.Security.Claims;
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
    var enrollment = await _enrollmentServices.CreateOrGetEnrollmentAsync(courseId, studentId);
    if (enrollment == null)
        return RedirectToAction("Index", "Home");

    var courseResponse = await _courseServices.GetCourseById(courseId);
    if (!courseResponse.Success)
        return RedirectToAction("Index", "Home");
    var course = courseResponse.Data;

    var modules = await _moduleServices.GetModulesByCourseIdAsync(courseId);
    var moduleProgressTasks = modules.Select(async module =>
    {
        module.Course = course;
        module.ModuleProgress = await _moduleServices.GetModuleProgressAsync(module.ModuleId, studentId);
        return module;
    });
    var modulesWithProgress = await Task.WhenAll(moduleProgressTasks);

    bool isCourseCompleted = await _moduleServices.HasUserCompletedAllModules(courseId, studentId);

    var viewModel = new CourseLearningViewModel
    {
        Modules = modulesWithProgress,
        IsCourseCompleted = isCourseCompleted
    };
    return View(viewModel);
}

		public async Task<IActionResult> CompleteModule(int moduleId)
{
    var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    await _moduleServices.MarkModuleAsCompletedAsync(moduleId, studentId);
    var module = await _moduleServices.GetModuleById(moduleId);
    return RedirectToAction("StartLearning", new { courseId = module.CourseId });
}

		public async Task<IActionResult> TakeQuiz(int moduleId)
{
    var questions = await _moduleServices.GetQuizQuestionsByModuleIdAsync(moduleId);
    var model = new QuizSubmissionViewModel
    {
        ModuleId = moduleId,
        Answers = questions.Select(q => new QuestionAnswer { QuizQuestionId = q.QuizQuestionId }).ToList()
    };
    ViewBag.Questions = questions;
    return View(model);
}

		[HttpPost]
public async Task<IActionResult> SubmitQuiz(QuizSubmissionViewModel submission)
{
    int correct = 0;
    var questionTasks = submission.Answers.Select(a => _moduleServices.GetQuizQuestionByIdAsync(a.QuizQuestionId)).ToList();
    var questions = await Task.WhenAll(questionTasks);
    for (int i = 0; i < submission.Answers.Count; i++)
    {
        var answer = submission.Answers[i];
        var question = questions[i];
        if (question != null && question.CorrectOption.Equals(answer.SelectedOption, StringComparison.OrdinalIgnoreCase))
        {
            correct++;
        }
    }
    int total = submission.Answers.Count;
    double score = total > 0 ? (double)correct / total * 100 : 0;
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
    var module = await _moduleServices.GetModuleById(submission.ModuleId);
    return RedirectToAction("StartLearning", new { courseId = module.CourseId });
}

		public async Task<IActionResult> TakeTheQuiz()
		{
			return View();
		}

	}
}
