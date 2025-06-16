using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Data.Models;
using NonnyE_Learning.Business.ViewModel;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Business.DTOs.Base;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NonnyE_Learning.Tests
{
	public class AuthServicesTests
	{
		private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
		private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
		private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly Mock<IEmailServices> _emailServicesMock;

		private readonly AuthServices _authServices;

		public AuthServicesTests()
		{
			_userManagerMock = MockUserManager<ApplicationUser>();
			_roleManagerMock = MockRoleManager();
			_signInManagerMock = MockSignInManager<ApplicationUser>();
			_configurationMock = new Mock<IConfiguration>();
			_emailServicesMock = new Mock<IEmailServices>();

			_configurationMock.Setup(x => x["AppBaseUrl"]).Returns("https://localhost");

			_authServices = new AuthServices(
				_userManagerMock.Object,
				_roleManagerMock.Object,
				_signInManagerMock.Object,
				_configurationMock.Object,
				_emailServicesMock.Object
			);
		}

		[Fact]
		public async Task CreateNewStudentAsync_ReturnsError_WhenUserExists()
		{
			var registerModel = new RegisterModel
			{
				Email = "test@example.com",
				Password = "Password123!",
				ConfirmPassword = "Password123!",
				FirstName = "Test",
				LastName = "User",
				PhoneNumber = "1234567890"
			};

			_userManagerMock.Setup(u => u.FindByNameAsync(registerModel.Email))
				.ReturnsAsync(new ApplicationUser());

			var result = await _authServices.CreateNewStudentAsync(registerModel);

			Assert.False(result.Success);
			Assert.Equal("Email Already exist.", result.Message);
		}

		[Fact]
		public async Task SignInAsync_ReturnsError_WhenUserNotFound()
		{
			var loginModel = new LoginModel { Email = "unknown@example.com", Password = "Password123!" };

			_userManagerMock.Setup(u => u.FindByEmailAsync(loginModel.Email))
				.ReturnsAsync((ApplicationUser)null);

			var result = await _authServices.SignInAsync(loginModel);

			Assert.False(result.Success);
			Assert.Equal("User not found.", result.Message);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ReturnsError_WhenUserNotFound()
		{
			_userManagerMock.Setup(x => x.FindByIdAsync("invalidId"))
				.ReturnsAsync((ApplicationUser)null);

			var result = await _authServices.ConfirmEmailAsync("invalidId", "token");

			Assert.False(result.Success);
			Assert.Equal("User not found.", result.Message);
		}

		[Fact]
		public async Task ForgetPasswordAsync_ReturnsError_WhenUserNotFound()
		{
			var email = "nonexistent@example.com";

			_userManagerMock.Setup(u => u.FindByEmailAsync(email))
				.ReturnsAsync((ApplicationUser)null);

			var result = await _authServices.ForgetPasswordAsync(email);

			Assert.False(result.Success);
			Assert.Equal("No account found with the provided email.", result.Message);
		}

		[Fact]
		public async Task SignOutAsync_ReturnsSuccess()
		{
			var result = await _authServices.SignOutAsync();

			Assert.True(result.Success);
			Assert.Equal("Logout successful.", result.Message);
		}

		// --- Mock Helpers ---
		private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
		{
			var store = new Mock<IUserStore<TUser>>();
			return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
		}

		private static Mock<RoleManager<IdentityRole>> MockRoleManager()
		{
			var store = new Mock<IRoleStore<IdentityRole>>();
			return new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
		}

		private static Mock<SignInManager<TUser>> MockSignInManager<TUser>() where TUser : class
		{
			var userManager = MockUserManager<TUser>();
			var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
			var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
			return new Mock<SignInManager<TUser>>(
				userManager.Object,
				contextAccessor.Object,
				claimsFactory.Object,
				null, null, null, null);
		}
	}
}
