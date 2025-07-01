using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Business.DTOs.Base;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Data.DbContext;
using NonnyE_Learning.Data.Enum;
using NonnyE_Learning.Data.Extension;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NonnyE_Learning.Business.Services
{
	public class TransactionServices : ITransactionServices
	{
		private readonly ApplicationDbContext _context;
		private readonly IFlutterwaveServices _flutterwave;
		private readonly UserManager<ApplicationUser> _userManager;

		public  TransactionServices(UserManager<ApplicationUser> userManager, ApplicationDbContext context,IFlutterwaveServices flutterwave)
		{
			_context = context;
			_userManager = userManager;
			_flutterwave = flutterwave;
		}
		public async Task<BaseResponse<string>> CreateTransaction(int enrollmentId, string studentId, decimal amount)
		{
			try

			{
				var enrollment = await _context.Enrollements
					.Include(en => en.Student)
					.Include(en => en.Course)
					.FirstOrDefaultAsync(se => se.EnrollmentId == enrollmentId);
				if (enrollment == null)

				{
					return new BaseResponse<string>
					{

						Success = false,
						Message = "Enrollment not found.",
						Errors = new List<string> { "Invalid EnrollmentId." }

					};

				}
				var transaction = new Transaction
				{
					EnrollmentId = enrollmentId,
					Amount = amount,
					StudentId = studentId,
					TransactionStatus = TransactionStatus.Pending,
					Reference = ReferenceNumberGenerator.GenerateReferenceNumber(10),
					StudentEmail = enrollment.Student?.Email,
					StudentName = $"{enrollment.Student?.FirstName} {enrollment.Student?.LastName}"

				};
				_context.Transactions.Add(transaction);
				await _context.SaveChangesAsync();
				var paymentUrl = await _flutterwave.GenerateFlutterwavePaymentLink(transaction);
				return new BaseResponse<string>
				{
					Success = true,
					Message = "Transaction created successfully. Redirect user to Flutterwave.",
					Data = paymentUrl // Return the payment link
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Failed to create transaction.",
					Errors = new List<string> { ex.Message }
				};
			}
		}

		public  async Task<Transaction> GetTransactionByReferenceAsync(string transactionRef)
		{
			if (string.IsNullOrEmpty(transactionRef))
				return null;
			return await _context.Transactions
				.Include(x => x.Enrollment)
				.ThenInclude(xe => xe.Course)
				.Include(x => x.Enrollment)
				.ThenInclude(xe => xe.Student)
				.FirstOrDefaultAsync(t => t.Reference == transactionRef);
		}

		public async Task UpdateTransactionStatusAsync(string transactionRef, string flutterTransactionId, string status)
		{
			var transaction = await _context.Transactions
				.FirstOrDefaultAsync(t => t.Reference == transactionRef);
			if (transaction == null) return;
			transaction.FlutterReference = flutterTransactionId;
			transaction.TransactionStatus= status.ToLower()== "successful" ?
				TransactionStatus.Completed : TransactionStatus.Failed;
			
			_context.Transactions.Update(transaction);
			await _context.SaveChangesAsync();
		}

		public async Task<BaseResponse<string>> CreatePricingPlanTransaction(int planId, string studentId)
		{
			var plan = await _context.PricingPlans.FindAsync(planId);
			if (plan == null)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Enrollment not found.",
					Errors = new List<string> { "Plan not found." }

				};
			}

			var user = await _userManager.FindByIdAsync(studentId);
			if (user == null)
			{
				return new BaseResponse<string>
				{
					Success = false,
					Message = "Enrollment not found.",
					Errors = new List<string> { "User not found." }

				};
			}

			var transaction = new Transaction
			{
				PricingPlanId = planId,
				Amount = plan.Price,
				StudentId = studentId,
				TransactionStatus = TransactionStatus.Pending,
				Reference = ReferenceNumberGenerator.GenerateReferenceNumber(10),
				StudentEmail = user.Email,
				StudentName = $"{user.FirstName} {user.LastName}"
			};

			_context.Transactions.Add(transaction);
			await _context.SaveChangesAsync();

			var paymentUrl = await _flutterwave.GeneratePricingPlanPaymentLink(transaction);
			return new BaseResponse<string>
			{
				Success = true,
				Message = "Transaction created successfully. Redirect user to Flutterwave.",
				Data = paymentUrl // Return the payment link
			};
		}

		public async Task<BaseResponse<IEnumerable<Transaction>>> GetAllTransactionAsync()
		{
			try
			{
				var transactions = await _context.Transactions
					.Include(t => t.PricingPlan)
					.Include(t => t.Enrollment)
						.ThenInclude(e => e.Course)
					.ToListAsync(); 
				return new BaseResponse<IEnumerable<Transaction>>
				{
					Success = true,
					Data = transactions
				};

			}
			catch (Exception ex)
			{

				return new BaseResponse<IEnumerable<Transaction>>
				{
					Success = false,
					Message = $"An error occurred while retrieving Transaction: {ex.Message}"
				};
			}
		}
	}
}
