using Microsoft.EntityFrameworkCore;
using NonnyE_Learning.Data.Enum;
using NonnyE_Learning.Data.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Data.Models
{
	public class Transaction
	{
		public int TransactionId { get; set; }
		public int EnrollmentId { get; set; }

		public string StudentId { get; set; }

		public string StudentEmail { get; set; }

		public string StudentName { get; set; }

		public decimal Amount { get; set; }

		public string Reference { get; set; } = string.Empty;

		public string? FlutterReference { get; set; }

		public TransactionStatus TransactionStatus { get; set; } 

		public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

		//navigation property to the related service
		public Enrollment Enrollment { get; set; }
	}
}



