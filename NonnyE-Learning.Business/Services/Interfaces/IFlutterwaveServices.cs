using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services.Interfaces
{
	public interface IFlutterwaveServices
	{
		Task<string> GenerateFlutterwavePaymentLink(Transaction transaction);
		Task<string> VerifyFlutterwavePayment(string transactionId);
		Task<string> GeneratePricingPlanPaymentLink(Transaction transaction);

	}
}
