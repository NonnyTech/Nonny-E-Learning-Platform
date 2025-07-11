using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Models
{
    public class ApplicationUser : IdentityUser <string>
    {
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool IsOtpVerified { get; set; } = false;


	}
}
