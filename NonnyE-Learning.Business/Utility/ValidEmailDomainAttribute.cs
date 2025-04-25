using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Utility
{
	public class ValidEmailDomainAttribute : ValidationAttribute
	{
		private readonly string[] _allowedDomains;

		public ValidEmailDomainAttribute(params string[] allowedDomains)
		{
			_allowedDomains = allowedDomains;
		}

		public override bool IsValid(object? value)
		{
			if (value == null) return false;

			var email = value.ToString();
			var parts = email?.Split('@');

			if (parts == null || parts.Length != 2)
				return false;

			var domain = parts[1].ToLower();

			return _allowedDomains.Any(d => d.ToLower() == domain);
		}
	}
}
