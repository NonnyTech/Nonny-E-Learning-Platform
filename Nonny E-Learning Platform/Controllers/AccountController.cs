using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;

namespace Nonny_E_Learning_Platform.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthServices _authServices;

        public AccountController(IAuthServices authServices)
        {
            _authServices = authServices;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model) 

        {
            if (ModelState.IsValid)
            {
                var response = await _authServices.SignInAsync (model);
                if (response.Success)
                {
                    TempData["success"] = "Welcome back";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
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
                    TempData["success"] = "Student created successfully. Please check your email to confirm your account";

                    return RedirectToAction("Login", "Account");

                }
                ModelState.AddModelError("", response.Message);
                if (response.Errors != null)
                {
                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError("Please check your input", error);
                    }
                }
               


            }
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var response = await _authServices.SignOutAsync();
            TempData["success"] = "Hope you enjoyed your course";
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
				return BadRequest("Invalid password reset request.");
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
		[HttpPost]

		public IActionResult EditAccount()
		{
			return View();

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
