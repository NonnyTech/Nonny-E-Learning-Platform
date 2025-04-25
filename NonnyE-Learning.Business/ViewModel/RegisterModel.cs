using Microsoft.AspNetCore.Mvc;
using NonnyE_Learning.Business.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.ViewModel
{
    public class RegisterModel
    {
        
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
		[Remote(action: "IsEmailInUse", controller: "Account", ErrorMessage = "Email already in use")]
		[ValidEmailDomain("gmail.com", "yahoo.com", ErrorMessage = "Email must be from gmail.com or yahoo.com")]
		public required string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }


		[Required]
		[MinLength(3)]
		public required string FirstName { get; set; }

		[Required]
		[MinLength(3)]
		public required string LastName { get; set; }


		[Required]
		[Phone] 
        public required string PhoneNumber {  get; set; }
    }
}
