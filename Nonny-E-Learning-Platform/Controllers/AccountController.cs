using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Data.Helper;
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ExternalLogin(string provider, string returnUrl = null)
		{
			var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
			var properties = _authServices.ConfigureExternalAuthentication(provider, redirectUrl);
			return Challenge(properties, provider);
		}
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
		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[AcceptVerbs("Get", "Post")]
		public async Task<IActionResult> IsEmailInUse(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return Json(true);
			}
			else
			{
				return Json($"Email: {email} is already in use");
			}
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				var response = await _authServices.SignInAsync(model);
				if (response.Success)
				{
					SetSuccessMessage(response.Message);

					// If the returnUrl is local, redirect to it; otherwise, go to Home
					if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}
					// Redirect based on user role
					var userRole = response.Data;
					switch (userRole)
					{
						case "SuperAdmin":
							return RedirectToAction("Index", "SuperAdmin");
						case "Instructor":
							return RedirectToAction("Index", "Home");
						case "Student":
							return RedirectToAction("Index", "Home");
						default:
							return RedirectToAction("Index", "Home");
					}

				}
				else
				{
					SetErrorMessage(response.Message);
				}
			}

			ViewData["ReturnUrl"] = returnUrl; // Maintain returnUrl on validation errors
			return View(model);
		}
		[HttpGet]
        public IActionResult Register() 
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)

        {
            if (ModelState.IsValid) 
            {
                var response = await _authServices.CreateNewStudentAsync(model);
                if (response.Success)
                {
					SetSuccessMessage(response.Message);

                    return RedirectToAction("Login", "Account");
                }
				else
				{
					SetErrorMessage(response.Message);
					return RedirectToAction("Register", "Account");

				}
			}
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var response = await _authServices.SignOutAsync();
            SetSuccessMessage(response.Message);
            return RedirectToAction("Index", "Home");
        }
		public IActionResult ForgetPassword()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var response = await _authServices.ForgetPasswordAsync(model.Email);

			if (response.Success)
			{
				SetSuccessMessage(response.Message);
				return RedirectToAction("Login");
			}
			else
			{
				SetErrorMessage(response.Message);
				return View(model);
			}
		}

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
				SetErrorMessage(response.Message); // or return a view with error message
				return RedirectToAction("ForgotPassword", "Account");

			}

			// Pass the token and userId to the reset password view
			var model = new ResetPasswordModel { UserId = userId, Token = token };
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
		{
			if (!ModelState.IsValid)
			{
				foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
				{
					Console.WriteLine("Model Error: " + error.ErrorMessage);
				}
				return View(model);
			}

			var response = await _authServices.ResetPasswordAsync(model);

			if (response.Success)
			{
				SetSuccessMessage(response.Message);
				return RedirectToAction("Login");
			}

			SetErrorMessage(response.Message);
			return View(model);
		}

		[HttpGet]
		[Route("Account/ConfirmEmail")]
		public async Task<IActionResult> ConfirmEmail(string userId, string token)
		{
			var response = await _authServices.ConfirmEmailAsync(userId, token);

			if (response.Success)
			{
				SetSuccessMessage(response.Message);
			}
			else
			{
				SetErrorMessage(response.Message);
			}

			return RedirectToAction("Login");
		}


	}
}
