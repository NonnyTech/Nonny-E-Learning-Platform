using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services
{
	public class PricingPlanServices : IPricingPlanServices
	{
		private readonly ApplicationDbContext _context;

		public PricingPlanServices(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<BaseResponse<IEnumerable<PricingPlan>>> GetAllPricingPlan()
		{
			try
			{
				var pricinPlan = await _context.PricingPlans.ToListAsync();
				return new BaseResponse<IEnumerable<PricingPlan>>
				{
					Success = true,
					Data = pricinPlan
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse<IEnumerable<PricingPlan>>
				{
					Success = false,
					Message = $"An error occurred while retrieving courses: {ex.Message}"
				};
			}
		}
	}
}
