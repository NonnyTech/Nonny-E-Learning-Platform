using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Enum;
using NonnyE_Learning.Data.Models;
using System.Security.Claims;

namespace Nonny_E_Learning_Platform.Controllers
{
	[Authorize]
	public class PaymentController : Controller
	{
		private readonly IFlutterwaveServices _flutterwaveServices;
		private readonly ICourseServices _courseServices;
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ITransactionServices _transactionServices;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IEmailServices _emailServices;

		public PaymentController(IFlutterwaveServices flutterwaveServices, ICourseServices courseServices, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ITransactionServices transactionServices, UserManager<ApplicationUser> userManager, IEmailServices emailServices)
		{
			_flutterwaveServices = flutterwaveServices;
			_courseServices = courseServices;
		     _context = context;
			_httpContextAccessor = httpContextAccessor;
			_transactionServices = transactionServices;
			_userManager = userManager;
			_emailServices = emailServices;
		}
		public async Task<IActionResult> InitiatePayment(int courseId, int enrollmentId, decimal amount)
		{

			if (!User.Identity.IsAuthenticated)
			{
				TempData["error"] = "You have to log in to pay for the course.";
				return Challenge(new AuthenticationProperties
				{
					RedirectUri = Url.Action("CourseDetail", "Courses", new { courseId })
				});
			}

			var studentId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (studentId == null)
			{
				TempData["error"] = "User not logged in.";
				return RedirectToAction("Login", "Account");
			}

			var course = await _courseServices.GetCourseById(courseId);
			if (course == null)
			{
				return NotFound();
			}

			var transactionResponse = await _transactionServices.CreateTransaction(enrollmentId, studentId, amount);

			if (!transactionResponse.Success)
			{
				TempData["error"] = "Failed to initiate payment.";
				return RedirectToAction("Payment", new { enrollmentId });
			}
			return Redirect(transactionResponse.Data); // Redirect to Flutterwave Payment Link

		}

		public async Task<IActionResult> VerifyPayment()
		{
			var flutterTransactionId = Request.Query["transaction_id"];
			var transactionRef = Request.Query["tx_ref"];

			if (string.IsNullOrEmpty(flutterTransactionId) || string.IsNullOrEmpty(transactionRef))
			{
				TempData["error"] = "Payment verification failed. Invalid transaction details.";
				return RedirectToAction("Index", "Home");
			}

			var transaction = await _transactionServices.GetTransactionByReferenceAsync(transactionRef);
			if (transaction == null)
			{
				TempData["error"] = "Payment verification failed. Transaction not found.";
				return RedirectToAction("Index", "Home");
			}
			var paymentStatus = await _flutterwaveServices.VerifyFlutterwavePayment(flutterTransactionId);

			await _transactionServices.UpdateTransactionStatusAsync(transaction.Reference, flutterTransactionId, paymentStatus);

			if (paymentStatus.ToLower() != "successful")
			{
				TempData["error"] = "Payment failed.";
				return RedirectToAction("Index", "Home");
			}
			var user = await _userManager.FindByIdAsync(transaction.StudentId);


			var emailModel = new PaymentConfirmationEmailModel
			{
				Email = user.Email,
				StudentName = user.FirstName,
				TransactionId = transaction.Reference,
				Amount = transaction.Amount.ToString("N2"),
				PaymentDate = transaction.TransactionDate
			};

			_emailServices.SendPaymentConfirmationEmail(emailModel);

			TempData["success"] = "Payment was successful, and a confirmation email has been sent.";
			return RedirectToAction("Index", "Home");

		}

		public async Task<IActionResult> Enroll(int transaction_id)
		{
			var transaction = await _context.Transactions.FindAsync(transaction_id);
			if (transaction == null) return NotFound();

			var enrollment = new Enrollment
			{
				StudentEmail = transaction.StudentEmail,
				CourseId = 1, // Update dynamically
				EnrollmentDate = DateTime.UtcNow
			};

			_context.Enrollements.Add(enrollment);
			await _context.SaveChangesAsync();

			return View("EnrollmentSuccess");
		}
	}
}
