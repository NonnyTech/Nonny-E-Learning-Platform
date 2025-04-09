using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.ViewModel
{
	public class ContactUsModel
	{
		[Required]
		public string? Name { get; set; }

		[Required]
		[EmailAddress]
		public string? Email { get; set; }

		[Required]
		[Phone]
		public string? Phone { get; set; }

		[Required]
		public string? Message { get; set; }

		public string Subject { get; set; } = "New Inquiry from Contact Form";
	}
}
