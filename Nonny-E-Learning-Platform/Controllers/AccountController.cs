using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Data.Models;
using System.Security.Claims;

namespace Nonny_E_Learning_Platform.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthServices _authServices;
		private readonly UserManager<ApplicationUser> _userManager;

		public AccountController(IAuthServices authServices, UserManager<ApplicationUser> userManager)
        {
            _authServices = authServices;
            _userManager = userManager;
        }

        /// <summary>
        /// Initiates external login with the specified provider.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _authServices.ConfigureExternalAuthentication(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        /// <summary>
        /// Handles the external login callback.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var response = await _authServices.ExternalLoginCallbackAsync(returnUrl, remoteError);
            if (!response.Success)
            {
                SetErrorMessage(response.Message);
                return RedirectToAction(nameof(Login));
            }
            SetSuccessMessage(response.Message);
            return LocalRedirect(response.Data);
        }

        /// <summary>
        /// Shows the login page.
        /// </summary>
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Checks if an email is already in use.
        /// </summary>
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Json(true);
            return Json($"Email: {email} is already in use");
        }

		/// <summary>
		/// Handles login POST.
		/// </summary>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
		{
			if (!ModelState.IsValid)
			{
				ViewData["ReturnUrl"] = returnUrl;
				return View(model);
			}

			var response = await _authServices.SignInAsync(model);

			if (response.Success)
			{
				// ? If OTP is needed
				if (response.Message.Contains("OTP"))
				{
					SetSuccessMessage(response.Message);
					return RedirectToAction("VerifyOtp", new { userId = response.Data });
				}

				SetSuccessMessage(response.Message);

				string userRole = response.Data;

				return userRole switch
				{
					"SuperAdmin" => RedirectToAction("Index", "SuperAdmin"),
					"Instructor" => RedirectToAction("Index", "Home"),
					"Student" => RedirectToAction("Index", "Home"),
					_ => RedirectToAction("Index", "Home")
				};
			}

			SetErrorMessage(response.Message);
			ViewData["ReturnUrl"] = returnUrl;
			return View(model);
		}

		/// <summary>
		/// Shows the registration page.
		/// </summary>
		[HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handles registration POST.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var response = await _authServices.CreateNewStudentAsync(model);
            if (response.Success)
            {
                SetSuccessMessage(response.Message);
                return RedirectToAction("Login", "Account");
            }
            SetErrorMessage(response.Message);
            return RedirectToAction("Register", "Account");
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var response = await _authServices.SignOutAsync();
            SetSuccessMessage(response.Message);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Shows the forget password page.
        /// </summary>
        public IActionResult ForgetPassword()
        {
            return View();
        }

        /// <summary>
        /// Handles forget password POST.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var response = await _authServices.ForgetPasswordAsync(model.Email);
            if (response.Success)
            {
                SetSuccessMessage(response.Message);
                return RedirectToAction("Login");
            }
            SetErrorMessage(response.Message);
            return View(model);
        }

        /// <summary>
        /// Shows the reset password page.
        /// </summary>
        [HttpGet("Account/ResetPassword")]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                var response = new BaseResponse<string>
                {
                    Success = false,
                    Message = "Invalid password reset request."
                };
                SetErrorMessage(response.Message);
                return RedirectToAction("ForgotPassword", "Account");
            }
            var model = new ResetPasswordModel { UserId = userId, Token = token };
            return View(model);
        }

        /// <summary>
        /// Handles reset password POST.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var response = await _authServices.ResetPasswordAsync(model);
            if (response.Success)
            {
                SetSuccessMessage(response.Message);
                return RedirectToAction("Login");
            }
            SetErrorMessage(response.Message);
            return View(model);
        }

        /// <summary>
        /// Confirms the user's email.
        /// </summary>
        [HttpGet]
        [Route("Account/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var response = await _authServices.ConfirmEmailAsync(userId, token);
            if (response.Success)
                SetSuccessMessage(response.Message);
            else
                SetErrorMessage(response.Message);
            return RedirectToAction("Login");
        }

		[HttpGet]
		public IActionResult VerifyOtp(string userId)
		{
			ViewBag.UserId = userId;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> VerifyOtp(OtpVerificationModel model)
		{
			if (!ModelState.IsValid)
			{
				SetErrorMessage("Please enter a valid OTP.");
				ViewBag.UserId = model.UserId;
				return View();
			}

			var response = await _authServices.VerifyOtpAsync(model);
			if (response.Success)
			{
				SetSuccessMessage(response.Message);
				return response.Data switch
				{
					"SuperAdmin" => RedirectToAction("Index", "SuperAdmin"),
					"Instructor" => RedirectToAction("Index", "Home"),
					"Student" => RedirectToAction("Index", "Home"),
					_ => RedirectToAction("Index", "Home")
				};
			}
			SetErrorMessage(response.Message);
			ViewBag.UserId = model.UserId;
			return View();
		}



	}
}
