using Microsoft.AspNetCore.Authentication;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Business.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services.Interfaces
{
    public interface IAuthServices
    {
        Task<BaseResponse<string>> SignInAsync(LoginModel model);
        Task <BaseResponse<string>> SignOutAsync();

        Task<BaseResponse<string>> CreateNewStudentAsync(RegisterModel model);
        Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string token);
        Task<BaseResponse<string>> ForgetPasswordAsync(string email);
        Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordModel model);

		Task<BaseResponse<string>> ExternalLoginCallbackAsync(string returnUrl, string remoteError);

        AuthenticationProperties ConfigureExternalAuthentication(string provider, string redirectUrl);









	}
}
