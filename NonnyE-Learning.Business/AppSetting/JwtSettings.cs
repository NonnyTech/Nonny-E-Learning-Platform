﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.AppSetting
{
	public class JwtSettings
	{
		public string Key { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public int ExpiryMinutes { get; set; }
	}
}
