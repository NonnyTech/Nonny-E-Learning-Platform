using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Data.Helper;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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

		public AuthServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IEmailServices emailServices)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
			_configuration = configuration;
			_emailServices = emailServices;
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
			ApplicationUser user = new ApplicationUser
            {
                UserName= model.Email,
                Email = model.Email,
                FirstName= model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
				EmailConfirmed = true,
				
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


        public async Task<BaseResponse<string>> SignInAsync(LoginModel model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) 
            
            { 
                return  new BaseResponse<string> 
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
                    Message = "Invalid Login Attempt."

                };


            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "Invalid login attempt."
                };
            }
            else if (result.IsLockedOut)
            {
                return new BaseResponse<string>
                {
                    Success = false,
					Message = "Your account is locked due to multiple failed login attempts. Please try again later or contact support."
				};
            }

            var roles = await _userManager.GetRolesAsync(user);
			if (roles.Contains("SuperAdmin"))
			{
				return new BaseResponse<string>
				{
					Success = true,
					Message = "Login successful.",
					Data = "Index/Home"
				};
			}
			else if (roles.Contains("Student"))
			{
				return new BaseResponse<string>
				{
					Success = true,
					Message = "Login successful.",
					Data = "Index/Home"
				};
			}
			else
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Invalid user or password. No role assigned to the user."
				};
			}

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

			var result = await _userManager.ConfirmEmailAsync(user, token);
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



	}
}


