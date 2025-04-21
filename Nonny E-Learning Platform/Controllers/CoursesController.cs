using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Data.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace Nonny_E_Learning_Platform.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseServices _courseServices;
       
        private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IEnrollmentServices _enrollmentServices;

		public CoursesController(ICourseServices courseServices,IWebHostEnvironment webHostEnvironment, IEnrollmentServices enrollmentServices)
        {
            _courseServices = courseServices;
            _webHostEnvironment = webHostEnvironment;
			_enrollmentServices = enrollmentServices;
		}

        public async Task<IActionResult> CourseList()
        {
			var response = await _courseServices.GetAllCoursesAsync();

			if (!response.Success)
			{
				
				TempData["ErrorMessage"] = response.Message; 
				return View("Error");  
			}

			return View(response.Data);
		}
        public IActionResult Quiz()
        {
            return View();
        }
       
        public async Task<IActionResult> CourseDetail(int courseId)
		{
			var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var response = await _courseServices.GetCourseById(courseId);

			if (!response.Success)
			{
				return NotFound(response.Message);
			}

			var course = response.Data;
			var enrollmentId = await _enrollmentServices.CreateOrGetEnrollmentAsync(courseId, studentId);

			if (enrollmentId == null)
			{
				TempData["Error"] = "Failed to create enrollment.";
				return RedirectToAction("Index", "Course");
			}
			var viewModel = new CourseDetailsViewModel
			{
				CourseId = course.CourseId,
				Title = course.Title,
				Instructor = course.Instructor,
				Duration = course.Duration,
				Lectures = course.Lectures,
				Price = course.Price,
				ImageUrl = course.ImageUrl,
				Category = course.Category,
				EnrollmentId = enrollmentId.Data
			};


			return View(viewModel);


		}

		public async Task<IActionResult> MyCourses()
		{
			var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (studentId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var enrollments = await _enrollmentServices.GetEnrollmentsByStudentIdAsync(studentId);
			var courses = enrollments.Select(e => e.Course).ToList();

			return View(courses);
		}
	}
}
