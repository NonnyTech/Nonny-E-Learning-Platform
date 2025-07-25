using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NonnyE_Learning.Business.AppSetting;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Data.Helper;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using OtpNet;
using static System.Net.WebRequestMethods;


namespace NonnyE_Learning.Business.Services
{
    public  class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IConfiguration _configuration;
		private readonly IEmailServices _emailServices;
		private readonly IUserTokenService _userTokenService;
		private readonly WebSettings _webSettings;

		public AuthServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IEmailServices emailServices, IUserTokenService userTokenService, IOptions<WebSettings> webSettings )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
			_configuration = configuration;
			_emailServices = emailServices;
			_userTokenService = userTokenService;
			_webSettings = webSettings.Value;
		}

        public async Task<BaseResponse<string>> CreateNewStudentAsync(RegisterModel model)
        {

            var studentExist = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == model.Email);
            if (studentExist != null) 
            {
                return new BaseResponse<string>

                {
                    Success = false,
                    Message = "Email Already exist.",
                    
                };

            }
			// Validate password strength
			var passwordValidator = new PasswordValidator<ApplicationUser>();
			var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, null, model.Password);
			if (!passwordValidationResult.Succeeded)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Password does not meet the required strength criteria.",
					Errors = passwordValidationResult.Errors.Select(e => e.Description)
				};
			}

			var customUserId = await GenerateCustomUserIdAsync();

			ApplicationUser user = new ApplicationUser
            {
				Id = customUserId,
				UserName = model.Email,
                Email = model.Email,
                FirstName= model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
				EmailConfirmed = false,
				
            };

            var result = await _userManager.CreateAsync(user,model.Password);
            if (!result.Succeeded)
            { 
               
                return new BaseResponse<string>
                
                {
                    Success =false,
                    Message = "Create Student fails.",
                    Errors = result.Errors.Select(e => e.Description)
                };

           }
			if (!await _roleManager.RoleExistsAsync("Student"))
			{
				await _roleManager.CreateAsync(new IdentityRole("Student"));
			}
			await _userManager.AddToRoleAsync(user, "Student");
			// Generate email confirmation token
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			var confirmationLink = $"{_webSettings.ClientURL}/Account/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";


			//Send the confirmation email
			string emailBody = EmailTemplate.RegistrationConfirmationTemplate().Replace("{{ConfirmationLink}}", confirmationLink);
			 _emailServices.SendConfirmationEmail(user.Email, "Confirm your email", emailBody);
			return new BaseResponse<string>
            {
                Success = true,
                Message = "Create Student successfull. Please check your email to confirm your account",
                Data = user.Id
            };
        }

		private async Task<string> GenerateCustomUserIdAsync()
		{
			// Get the latest user with custom ID pattern
			var users = await _userManager.Users
				.Where(u => u.Id.StartsWith("NonnyPlus"))
				.ToListAsync();

			// Find max number used so far
			var maxNumber = users
				.Select(u => u.Id.Replace("NonnyPlus", ""))
				.Where(id => int.TryParse(id, out _))
				.Select(int.Parse)
				.DefaultIfEmpty(0)
				.Max();

			var nextId = maxNumber + 1;

			return $"NonnyPlus{nextId.ToString("D3")}"; // e.g., NonnyPlus001
		}
		private string GenerateOtp(string email)
		{
			var secret = Encoding.ASCII.GetBytes(email); // You can improve this with salt/key
			var totp = new Totp(secret, step: 300); // OTP valid for 5 minutes
			return totp.ComputeTotp();
		}

		private bool VerifyOtp(string email, string userOtp)
		{
			var secret = Encoding.ASCII.GetBytes(email);
			var totp = new Totp(secret, step: 300);
			return totp.VerifyTotp(userOtp, out long _, VerificationWindow.RfcSpecifiedNetworkDelay);
		}
		public async Task<BaseResponse<string>> SignInAsync(LoginModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
				return new BaseResponse<string> { Success = false, Message = "User not found." };

			if (!await _userManager.IsEmailConfirmedAsync(user))
				return new BaseResponse<string> { Success = false, Message = "Email not confirmed." };

			if (!await _userManager.CheckPasswordAsync(user, model.Password))
				return new BaseResponse<string> { Success = false, Message = "Invalid credentials." };

			if (user.IsOtpVerified)
			{
				await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
				var roles = await _userManager.GetRolesAsync(user);
				var userRole = roles.FirstOrDefault();

				return new BaseResponse<string>
				{
					Success = true,
					Message = "Login successful.",
					Data = userRole
				};
			}
			var otp = GenerateOtp(user.Email);
			string body = EmailTemplate.OtpVerificationTemplate().Replace("{{OtpCode}}", otp);
			_emailServices.SendConfirmationEmail(user.Email, "Your OTP Code", body);

			return new BaseResponse<string>
			{
				Success = true,
				Message = "OTP has been sent to your email. Please use the Otp to Login",
				Data = user.Id
			};
		}

		public async Task<BaseResponse<string>> SignOutAsync()
        {
            await _signInManager.SignOutAsync();
            return new BaseResponse<string>
            {
                Success = true,
                Message = "Logout successful."
            };
        }
		public async Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string token)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Invalid confirmation link."
				};
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "User not found."
				};
			}

			var decodedToken = Uri.UnescapeDataString(token);

			var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
			if (result.Succeeded)
			{
				return new BaseResponse<string>
				{
					Success = true,
					Message = "Your email has been confirmed. You can now log in."
				};
			}

			return new BaseResponse<string>
			{
				Success = false,
				Message = "Email confirmation failed. Please try again."
			};
		}
		public async Task<BaseResponse<string>> ForgetPasswordAsync(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "No account found with the provided email."
				};
			}

			// Generate password reset token
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			// URL encode the token to ensure special characters are safely included
			//var decodedToken = Uri.UnescapeDataString(token);

			var resetLink = $"{_webSettings.ClientURL}/Account/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(token)}";
			string emailBody = EmailTemplate.ForgetPasswordTemplate().Replace("{{ResetLink}}", resetLink);
			try
			{
				// Send the reset password email
				_emailServices.SendForgetPasswordEmail(user.Email, "Reset Your Account", emailBody);

				return new BaseResponse<string>
				{
					Success = true,
					Message = "Password reset link has been sent to your email."
				};
			}
			catch (Exception ex)
			{
				// Log error if email sending fails
				return new BaseResponse<string>
				{
					Success = false,
					Message = $"Failed to send password reset email: {ex.Message}"
				};
			}
		}
		public async Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordModel model)
		{
			if (string.IsNullOrEmpty(model.Token))
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Invalid password reset token."
				};
			}

			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "User not found."
				};
			}

			var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
			if (result.Succeeded)
			{
				return new BaseResponse<string>
				{
					Success = true,
					Message = "Password reset successfully."
				};
			}

			var errors = result.Errors.Select(e => e.Description).ToList();
			return new BaseResponse<string>
			{
				Success = false,
				Message = string.Join(", ", errors)
			};
		}

		public async Task<BaseResponse<string>> ExternalLoginCallbackAsync(string returnUrl, string remoteError)
		{
			returnUrl ??= "/";

			if (remoteError != null)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = $"Error from external provider: {remoteError}"
				};
			}

			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Error loading external login information."
				};
			}

			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
			if (result.Succeeded)
			{
				return new BaseResponse<string>
				{
					Success = true,
					Message = "Login successful.",
					Data = returnUrl
				};
			}

			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
			var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "GoogleUser";
			var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);

			if (email == null)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Email claim not received from external provider."
				};
			}

			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				var customUserId = await GenerateCustomUserIdAsync();

				user = new ApplicationUser
				{
					Id = customUserId,
					UserName = email,
					Email = email,
					FirstName = firstName,
					LastName = lastName,
					EmailConfirmed = true // 👈 mark as confirmed automatically
				};

				var createResult = await _userManager.CreateAsync(user);
				if (!createResult.Succeeded)
				{
					return new BaseResponse<string>
					{
						Success = false,
						Message = "Failed to create user.",
						Errors = createResult.Errors.Select(e => e.Description)
					};
				}

				// No need to send confirmation email
			}

			var addLoginResult = await _userManager.AddLoginAsync(user, info);
			if (!addLoginResult.Succeeded)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Failed to link external login.",
					Errors = addLoginResult.Errors.Select(e => e.Description)
				};
			}

			await _signInManager.SignInAsync(user, false);
			return new BaseResponse<string>
			{
				Success = true,
				Message = "Login successful.",
				Data = returnUrl
			};
		}
		public AuthenticationProperties ConfigureExternalAuthentication(string provider, string redirectUrl)
		{
			return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
		}

		public async Task<BaseResponse<string>> VerifyOtpAsync(OtpVerificationModel model)
		{
			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null)
				return new BaseResponse<string> { Success = false, Message = "User not found." };

			if (!VerifyOtp(user.Email, model.OtpCode))
				return new BaseResponse<string> { Success = false, Message = "Invalid or expired OTP." };

			user.IsOtpVerified = true;
			await _userManager.UpdateAsync(user);
			// ✅ Now sign them in
			await _signInManager.SignInAsync(user, false);

			var roles = await _userManager.GetRolesAsync(user);
			var userRole = roles.FirstOrDefault();
			if (string.IsNullOrEmpty(userRole))
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "No role assigned to the user."
				};
			}

			return new BaseResponse<string>
			{
				Success = true,
				Message = "OTP verification successful. Login complete.",
				Data = userRole
			};
		}
	}
}


