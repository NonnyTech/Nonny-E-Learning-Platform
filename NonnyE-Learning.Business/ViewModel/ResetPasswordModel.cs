using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.ViewModel
{
	public class ResetPasswordModel
	{
		[Required]
		public string UserId { get; set; }

		[Required]
		public string Token { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "The password must be at least {2} characters long.", MinimumLength = 6)]
		public string NewPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
