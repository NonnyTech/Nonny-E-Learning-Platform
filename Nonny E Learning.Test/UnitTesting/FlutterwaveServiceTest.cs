using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NonnyE_Learning.Business.AppSetting;
using NonnyE_Learning.Business.Services;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nonny_E_Learning.Test.UnitTesting
{
	public class FlutterwaveServiceTest
	{
		private readonly Mock<IOptions<FlutterwaveConfig>> _configMock;
		private readonly Mock<HttpMessageHandler> _httpHandlerMock;
		private readonly FlutterwaveConfig _config;
		private readonly HttpClient _httpClient;
		private readonly FlutterwaveServices _flutterwaveServices;

		public FlutterwaveServiceTest()
		{
			_config = new FlutterwaveConfig
			{
				SecretKey = "sk_test_dummy",
				BaseUrl = "https://api.flutterwave.com"
			};

			_configMock = new Mock<IOptions<FlutterwaveConfig>>();
			_configMock.Setup(c => c.Value).Returns(_config);

			_httpHandlerMock = new Mock<HttpMessageHandler>();
			_httpClient = new HttpClient(_httpHandlerMock.Object);

			_flutterwaveServices = new FlutterwaveServices(_httpClient, _configMock.Object);

		}
		[Fact]
		public async Task GenerateFlutterwavePaymentLink_ReturnsLink_WhenResponseIsSuccessful()
		{
			// Arrange
			var transaction = new Transaction
			{
				Reference = "TX123",
				Amount = 5000,
				Enrollment = new Enrollment
				{
					Student = new ApplicationUser { Email = "test@example.com", PhoneNumber = "08012345678", FirstName = "John" },
					Course = new Course { Title = "C# Basics" }
				}
			};

			var mockResponse = new
			{
				status = "success",
				data = new { link = "https://pay.flutterwave.com/dummy-link" }
			};

			_httpHandlerMock.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync",
					ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
					ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(mockResponse), Encoding.UTF8, "application/json")
				});

			// Act
			var result = await _flutterwaveServices.GenerateFlutterwavePaymentLink(transaction);

			// Assert
			Assert.Equal("https://pay.flutterwave.com/dummy-link", result);
		}

		[Fact]
		public async Task GenerateFlutterwavePaymentLink_ReturnsFailed_WhenResponseIsNotSuccessful()
		{
			var transaction = new Transaction { Reference = "TX123", Amount = 5000, Enrollment = new Enrollment { Student = new ApplicationUser(), Course = new Course() } };

			_httpHandlerMock.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync",
					ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
					ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

			var result = await _flutterwaveServices.GenerateFlutterwavePaymentLink(transaction);
			Assert.Null(result);
		}

		[Fact]
		public async Task VerifyFlutterwavePayment_ReturnsStatus_WhenSuccessful()
		{
			var transactionId = "TX123";
			var mockResponse = new
			{
				status = "success",
				data = new { status = "successful" }
			};

			_httpHandlerMock.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync",
					ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
					ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(mockResponse), Encoding.UTF8, "application/json")
				});

			var result = await _flutterwaveServices.VerifyFlutterwavePayment(transactionId);

			Assert.Equal("successful", result);
		}

		[Fact]
		public async Task VerifyFlutterwavePayment_ReturnsFailed_WhenResponseIsNotSuccessful()
		{
			var transactionId = "TX123";

			_httpHandlerMock.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync",
					ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
					ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

			var result = await _flutterwaveServices.VerifyFlutterwavePayment(transactionId);

			Assert.Equal("failed", result);
		}
	}
}
