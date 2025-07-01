using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Models
{
	public class PricingPlan
	{
		public int PricingPlanId { get; set; }
		public string PlanName { get; set; } 
		public decimal Price { get; set; }
		public string Description { get; set; }
	}
}
