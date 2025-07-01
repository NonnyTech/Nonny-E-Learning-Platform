using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

		public AuthServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IEmailServices emailServices, IUserTokenService userTokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
			_configuration = configuration;
			_emailServices = emailServices;
			_userTokenService = userTokenService;
		}

        public async Task<BaseResponse<string>> CreateNewStudentAsync(RegisterModel model)
        {

            var studentExist = await _userManager.FindByNameAsync(model.Email);
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

			var confirmationLink = $"{_configuration["AppBaseUrl"]}/Account/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";

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
		public async Task<BaseResponse<string>> SignInAsync(LoginModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user == null)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "User not found."
				};
			}

			if (!await _userManager.IsEmailConfirmedAsync(user))
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Email not confirmed. Please check your email for the confirmation link."
				};
			}

			if (!await _userManager.CheckPasswordAsync(user, model.Password))
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Invalid email or password. Please try again."
				};
			}

			var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

			if (result.IsLockedOut)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Your account is locked due to multiple failed login attempts. Please try again later or contact support."
				};
			}

			if (!result.Succeeded)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Invalid email or password. Please try again."
				};
			}

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
				Message = "Login successful.",
				Data =userRole
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

			var resetLink = $"{_configuration["AppBaseUrl"]}/Account/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(token)}";
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
				var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
				if (existingUser != null && !await _userManager.IsEmailConfirmedAsync(existingUser))
				{
					await _signInManager.SignOutAsync();
					return new BaseResponse<string>
					{
						Success = false,
						Message = "You cannot log in at the moment—please confirm your email address."
					};
				}
				return new BaseResponse<string> { Success = true, Message = "Login successful.", Data = returnUrl };
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
			if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "You cannot log in at the moment—please confirm your email address."
				};
			}

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
					EmailConfirmed = false
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

				var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var confirmationLink = $"{_configuration["AppBaseUrl"]}/Account/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";
				var emailBody = EmailTemplate.RegistrationConfirmationTemplate().Replace("{{ConfirmationLink}}", confirmationLink);
				_emailServices.SendConfirmationEmail(user.Email, "Confirm your email", emailBody);
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

			if (!await _userManager.IsEmailConfirmedAsync(user))
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "You cannot log in at the moment—please confirm your email address."
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
	}
}


