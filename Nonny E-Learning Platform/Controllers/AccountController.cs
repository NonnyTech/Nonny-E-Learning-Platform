using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Data.Models;

namespace Nonny_E_Learning_Platform.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthServices _authServices;
		private readonly UserManager<ApplicationUser> _userManager;

		public AccountController(IAuthServices authServices, UserManager<ApplicationUser> userManager)
        {
            _authServices = authServices;
			_userManager = userManager;
		}
		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[AcceptVerbs("Get", "Post")]
		[AllowAnonymous]
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
					TempData["success"] = response.Message;

					// If the returnUrl is local, redirect to it; otherwise, go to Home
					if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}

					return RedirectToAction("Index", "Home");
				}
				else
				{
					TempData["error"] = response.Message;
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
					TempData["success"] = response.Message;

                    return RedirectToAction("Login", "Account");
                }
				else
				{
					TempData["error"] = response.Message;
					return RedirectToAction("Register", "Account");

				}
			}
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var response = await _authServices.SignOutAsync();
            TempData["success"] = response.Message;
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
				TempData["success"] = response.Message;
				return RedirectToAction("Login");
			}
			else
			{
				TempData["error"] = response.Message;
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
				TempData["error"] = response.Message; // or return a view with error message
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
				TempData["success"] = response.Message;
				return RedirectToAction("Login");
			}

			TempData["error"] = response.Message;
			return View(model);
		}

		[HttpGet]
		[Route("Account/ConfirmEmail")]
		public async Task<IActionResult> ConfirmEmail(string userId, string token)
		{
			var response = await _authServices.ConfirmEmailAsync(userId, token);

			if (response.Success)
			{
				TempData["success"] = response.Message;
			}
			else
			{
				TempData["error"] = response.Message;
			}

			return RedirectToAction("Login");
		}


	}
}
