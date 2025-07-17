using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using System.Security.Claims;

namespace Nonny_E_Learning_Platform.Controllers
{
	public class PaymentController : BaseController
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
		[HttpGet]
        public async Task<IActionResult> InitiatePayment(int courseId, int enrollmentId, decimal amount)
        {
        var studentId = GetStudentId();
        if (studentId == null)
        {
        var returnUrl = Url.Action("InitiatePayment", "Payment", new { courseId, enrollmentId, amount });
        SetErrorMessage("Please login to use this service.");
        return RedirectToAction("Login", "Account", new { returnUrl });
        }
         var courseResponse = await _courseServices.GetCourseById(courseId);
     if (!courseResponse.Success || courseResponse.Data == null)
        {
        SetErrorMessage(courseResponse.Message);
        return RedirectToAction("Index", "Home");
        }
        var transactionResponse = await _transactionServices.CreateTransaction(enrollmentId, studentId, amount);
        if (!transactionResponse.Success)
        {
        SetErrorMessage("Failed to initiate payment.");
        return RedirectToAction("Index", "Home");
         }
         return Redirect(transactionResponse.Data); // Redirect to Flutterwave Payment Link
       }

        private string GetStudentId()
        {
             return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }


		public async Task<IActionResult> VerifyPayment()
        {
        var flutterTransactionId = Request.Query["transaction_id"];
        var transactionRef = Request.Query["tx_ref"];
        if (string.IsNullOrEmpty(flutterTransactionId) || string.IsNullOrEmpty(transactionRef))
        {
        SetErrorMessage("Payment verification failed. Invalid transaction details.");
        return RedirectToAction("Index", "Home");
        }
        var transaction = await _transactionServices.GetTransactionByReferenceAsync(transactionRef);
        if (transaction == null)
        {
        SetErrorMessage("Payment verification failed. Transaction not found.");
        return RedirectToAction("Index", "Home");
        }
        var paymentStatus = await _flutterwaveServices.VerifyFlutterwavePayment(flutterTransactionId);
        await _transactionServices.UpdateTransactionStatusAsync(transaction.Reference, flutterTransactionId, paymentStatus);
        if (!string.Equals(paymentStatus, "successful", StringComparison.OrdinalIgnoreCase))
        {
        SetErrorMessage("Payment failed.");
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
    // If SendPaymentConfirmationEmail is async, await it. Otherwise, leave as is.
        _emailServices.SendPaymentConfirmationEmail(emailModel);
        SetSuccessMessage("Payment was successful, and a confirmation email has been sent.");
			return RedirectToAction("MyCourses", "Courses");
		}


		[HttpGet]
        public async Task<IActionResult> InitiatePricingPlanPayment(int planId)
        {
         var studentId = GetStudentId();
         if (studentId == null)
        {   
        var returnUrl = Url.Action("InitiatePricingPlanPayment", "Payment", new { planId });
        SetErrorMessage("Please login to continue.");
        return RedirectToAction("Login", "Account", new { returnUrl });
         }
        var response = await _transactionServices.CreatePricingPlanTransaction(planId, studentId);
        if (!response.Success)
        {
        SetErrorMessage(response.Message);
        return RedirectToAction("Index", "Home");
         }
        return Redirect(response.Data);
         }

	}
    }
