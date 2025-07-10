using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Data.Models;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace Nonny_E_Learning_Platform.Controllers
{
    /// <summary>
    /// Handles course-related actions for students and instructors.
    /// </summary>
    public class CoursesController : BaseController
    {
        private readonly ICourseServices _courseServices;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEnrollmentServices _enrollmentServices;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoursesController(
            ICourseServices courseServices,
            IWebHostEnvironment webHostEnvironment,
            IEnrollmentServices enrollmentServices,
            UserManager<ApplicationUser> userManager)
        {
            _courseServices = courseServices;
            _webHostEnvironment = webHostEnvironment;
            _enrollmentServices = enrollmentServices;
            _userManager = userManager;
        }

        /// <summary>
        /// Lists all available courses.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CourseList()
        {
            var response = await _courseServices.GetAllCoursesAsync();
            if (!response.Success)
            {
                SetErrorMessage(response.Message);
                return View("Error");
            }
            return View(response.Data);
        }

        /// <summary>
        /// Shows the quiz view.
        /// </summary>
        [HttpGet]
        public IActionResult Quiz()
        {
            return View();
        }

        /// <summary>
        /// Shows the details for a specific course and enrolls the student if needed.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CourseDetail(int courseId)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _courseServices.GetCourseById(courseId);
            if (!response.Success)
                return NotFound(response.Message);

            var course = response.Data;
            var enrollmentId = await _enrollmentServices.CreateOrGetEnrollmentAsync(courseId, studentId);
            if (enrollmentId == null)
            {
                SetErrorMessage("Failed to create enrollment.");
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

        /// <summary>
        /// Lists all courses the current student is enrolled in.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyCourses()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (studentId == null)
                return RedirectToAction("Login", "Account");

            var enrollments = await _enrollmentServices.GetEnrollmentsByStudentIdAsync(studentId);
            var courses = enrollments.Select(e => e.Course).ToList();
            return View(courses);
        }

        /// <summary>
        /// Shows the instructor's course management view (for instructors only).
        /// </summary>
        [Authorize(Roles = "Instructor")]
        public IActionResult CourseInstructor()
        {
            return View();
        }

        /// <summary>
        /// Shows all courses taught by the current instructor (for instructors only).
        /// </summary>
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> ViewMyCourse()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                SetErrorMessage("User Not Found");
                return RedirectToAction("Index", "Home");
            }

            var instructorFullName = $"{user.FirstName?.Trim()} {user.LastName?.Trim()}";
            var courses = await _courseServices.GetCoursesByInstructorAsync(user.FirstName, user.LastName);
            if (courses == null || !courses.Any())
                SetErrorMessage($"No courses found for instructor: {instructorFullName}");

            return View("ViewMyCourse", courses);
        }
    }
}

