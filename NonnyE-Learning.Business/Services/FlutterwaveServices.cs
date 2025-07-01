using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NonnyE_Learning.Business.AppSetting;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services
{
	public class FlutterwaveServices : IFlutterwaveServices
	{
		private readonly HttpClient _httpClient;
		private readonly FlutterwaveConfig _flutterwaveConfig;

		public FlutterwaveServices(HttpClient httpClient, IOptions<FlutterwaveConfig> flutterwaveConfig)
		{
			_httpClient = httpClient;
			_flutterwaveConfig = flutterwaveConfig.Value;
		}

		public async Task<string> GenerateFlutterwavePaymentLink(Transaction transaction)
		{
			var secretKey = _flutterwaveConfig.SecretKey;
			var baseUrl = _flutterwaveConfig.BaseUrl;
			var url = $"{baseUrl}/v3/payments";
			var returnUrl = $"{_flutterwaveConfig.RedirectUrlBase}/Payment/VerifyPayment";

			var requestBody = new
			{
				tx_ref = transaction.Reference,
				amount = transaction.Amount,
				currency = "NGN",
				redirect_url = returnUrl,
				payment_options = "card, banktransfer",
				customer = new { email = transaction.Enrollment.Student.Email,
					phonenumber = transaction.Enrollment.Student.PhoneNumber,
					name = transaction.Enrollment.Student.FirstName 
				},
				customizations = new { title = "Course Payment",
			    description = $"Payment for {transaction.Enrollment.Course.Title}"
				}
			};

			var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");
			var response = await _httpClient.PostAsync(url, requestContent);

			if (!response.IsSuccessStatusCode)
			{
				return null;
			}
			var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

			if (jsonResponse.TryGetProperty("data", out var data) && data.TryGetProperty("link", out var link))
			{
				return link.GetString();
			}
			return "failed";

		}

		public async Task<string> VerifyFlutterwavePayment(string transactionId)
		{
			var secretKey = _flutterwaveConfig.SecretKey;
			var url = $"{_flutterwaveConfig.BaseUrl}/v3/transactions/{transactionId}/verify";

			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");
			var response = await _httpClient.GetAsync(url);
			if (!response.IsSuccessStatusCode)
			{
				return "failed";
			}

			var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

			if (jsonResponse.TryGetProperty("data", out var data) &&
				data.TryGetProperty("status", out var status))
			{
				return status.GetString();
			}

			return "failed";
		}

		public async Task<string> GeneratePricingPlanPaymentLink(Transaction transaction)
		{
			var secretKey = _flutterwaveConfig.SecretKey;
			var url = $"{_flutterwaveConfig.BaseUrl}/v3/payments";
			var returnUrl = $"{_flutterwaveConfig.RedirectUrlBase}/Payment/VerifyPayment";

			var requestBody = new
			{
				tx_ref = transaction.Reference,
				amount = transaction.Amount,
				currency = "NGN",
				redirect_url = returnUrl,
				payment_options = "card, banktransfer",
				customer = new
				{
					email = transaction.StudentEmail,
					name = transaction.StudentName
				},
				customizations = new
				{
					title = "Training Plan Payment",
					description = $"Payment for {transaction.PricingPlan.PlanName} Plan"
				}
			};

			var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

			_httpClient.DefaultRequestHeaders.Remove("Authorization");
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");

			var response = await _httpClient.PostAsync(url, requestContent);
			if (!response.IsSuccessStatusCode)
				return null;

			var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
			if (jsonResponse.TryGetProperty("data", out var data) && data.TryGetProperty("link", out var link))
				return link.GetString();

			return "failed";
		}
	}
}
